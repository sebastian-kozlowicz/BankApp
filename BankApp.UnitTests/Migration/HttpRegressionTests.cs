using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BankApp.Configuration;
using BankApp.Controllers;
using BankApp.Data;
using BankApp.Interfaces.Helpers.Builders.Auth;
using BankApp.Models;
using BankApp.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Migration
{
    [TestClass]
    public class HttpRegressionTests
    {
        // Deliberately not Program.CreateHostBuilder/UseStartup: those read appsettings
        // and seed SQL Server. Use only production service registrations and the API
        // middleware, with synthetic configuration and a private in-memory database.
        private IHost _host;
        private HttpClient _client;
        private const string TestKey = "test-only-signing-key-at-least-32-bytes-long";

        [TestInitialize]
        public async Task Initialize()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=unused.invalid;Database=unused;Integrated Security=true",
                ["JwtIssuerOptions:Issuer"] = "bankapp-tests",
                ["JwtIssuerOptions:Audience"] = "bankapp-tests-client",
                ["JwtIssuerOptions:SecretKey"] = TestKey,
                ["JwtIssuerOptions:ValidFor"] = "00:05:00"
            }).Build();
            var databaseName = Guid.NewGuid().ToString();
            _host = await new HostBuilder().ConfigureWebHost(web => web.UseTestServer()
                .ConfigureServices(services =>
                {
                    new Startup(config).ConfigureServices(services);
                    // Replace the registration itself, including EF's options actions,
                    // rather than layering a second provider onto SQL Server options.
                    services.RemoveAll<ApplicationDbContext>();
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.RemoveAll<Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
                    services.AddControllers().AddApplicationPart(typeof(BranchesController).Assembly);
                })
                .Configure(app =>
                {
                    app.UseMiddleware<ExceptionHandlingMiddleware>();
                    app.UseSwagger();
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                })).StartAsync();
            _client = _host.GetTestClient();
            using var scope = _host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Assert.AreEqual("Microsoft.EntityFrameworkCore.InMemory", db.Database.ProviderName);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            _client?.Dispose();
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }

        [TestMethod]
        public async Task SwaggerKeepsBearerDefinitionAndRequirement()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.AreEqual("bearer", json.RootElement.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer").GetProperty("scheme").GetString());
            Assert.IsTrue(json.RootElement.GetProperty("security")[0].TryGetProperty("Bearer", out _));
            Assert.IsTrue(json.RootElement.GetProperty("paths").TryGetProperty("/api/Branches/{branchId}", out _));
        }

        [TestMethod]
        public async Task CreateBranchKeepsLocationCamelCaseAndReadEndpoint()
        {
            var response = await _client.PostAsync("/api/Branches/CreateWithAddress", new StringContent(
                "{\"branch\":{\"branchCode\":\"007\"},\"address\":{\"country\":\"Poland\",\"city\":\"Poznan\",\"street\":\"Test\",\"houseNumber\":\"1\",\"postalCode\":\"61-001\"}}", Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.AreEqual("007", json.RootElement.GetProperty("branchCode").GetString());
            Assert.AreEqual("Poznan", json.RootElement.GetProperty("branchAddress").GetProperty("city").GetString());
            var read = await _client.GetAsync(response.Headers.Location);
            Assert.AreEqual(HttpStatusCode.OK, read.StatusCode);
        }

        [TestMethod]
        public async Task InvalidModelReturnsBadRequest()
        {
            var response = await _client.PostAsync("/api/Branches/CreateWithAddress", new StringContent("{}", Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task AnonymousProtectedEndpointReturnsUnauthorized()
        {
            var response = await AssignTeller();
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task ForgedTokenReturnsUnauthorized()
        {
            var token = new JwtSecurityToken("bankapp-tests", "bankapp-tests-client",
                new[] { new Claim(CustomClaimTypes.UserId, "123") }, DateTime.UtcNow.AddMinutes(-1),
                DateTime.UtcNow.AddMinutes(5), new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes("forged-test-key-not-the-real-test-key")),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
            Assert.AreEqual(HttpStatusCode.Unauthorized, (await AssignTeller()).StatusCode);
        }

        [TestMethod]
        public async Task TokenWithoutUserIdReturnsForbidden()
        {
            var jwt = _host.Services.GetRequiredService<IJwtBuilder>();
            var token = jwt.GenerateSecurityToken(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test") }));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
            Assert.AreEqual(HttpStatusCode.Forbidden, (await AssignTeller()).StatusCode);
        }

        [TestMethod]
        public async Task ValidTokenReachesControllerInsteadOfAuthenticationFailure()
        {
            var jwt = _host.Services.GetRequiredService<IJwtBuilder>();
            var identity = jwt.GenerateClaimsIdentity(new ApplicationUser { Id = 123, Email = "test@example.invalid" }, Array.Empty<string>());
            var token = jwt.GenerateSecurityToken(identity);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Users.Add(new ApplicationUser { Id = 1, Teller = new Teller { Id = 1 } });
                db.Branches.Add(new Branch { Id = 1, BranchCode = "001" });
                await db.SaveChangesAsync();
            }
            Assert.AreEqual(HttpStatusCode.OK, (await AssignTeller()).StatusCode);
        }

        private Task<HttpResponseMessage> AssignTeller() => _client.PostAsync("/api/Branches/AssignTellerToBranch",
            new StringContent("{\"workerId\":1,\"branchId\":1}", Encoding.UTF8, "application/json"));
    }
}
