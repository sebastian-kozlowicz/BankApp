using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using BankApp.Data;
using BankApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BankApp.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using BankApp.Extensions;
using BankApp.Helpers.Services;
using BankApp.Policies;
using BankApp.Policies.Handlers;
using BankApp.Policies.Requirement;
using Microsoft.AspNetCore.Authorization;
using Nito.AsyncEx;
using BankApp.Helpers.Builders;
using BankApp.Helpers.Factories;
using BankApp.Interfaces.Builders;
using BankApp.Interfaces.Factories;
using BankApp.Interfaces.Services;
using Microsoft.OpenApi.Models;

namespace BankApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankApp", Version = "v1" });
                c.CustomSchemaIds(t => t.ToString());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole<int>>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

            services.AddSingleton<IAuthorizationHandler, UserIdRequirementHandler>();
            services.AddSingleton<IJwtBuilder, JwtBuilder>();
            services.AddScoped<IBankAccountNumberBuilder, BankAccountNumberBuilder>();
            services.AddScoped<IPaymentCardNumberFactory, PaymentCardNumberFactory>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITransferService<InternalTransferService>, InternalTransferService>();
            services.AddScoped<ITransferService<ExternalTransferService>, ExternalTransferService>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            });

            var jwtIssuerOptionsSection = Configuration.GetSection(nameof(JwtIssuerOptions));
            var jwtIssuerOptions = jwtIssuerOptionsSection.Get<JwtIssuerOptions>();
            services.Configure<JwtIssuerOptions>(jwtIssuerOptionsSection);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtIssuerOptions.SymmetricSecurityKey,
                ValidateIssuer = true,
                ValidIssuer = jwtIssuerOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtIssuerOptions.Audience,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.ClaimsIssuer = jwtIssuerOptions.Issuer;
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.SaveToken = false;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyName.UserIdIncludedInJwtToken, policy => policy.Requirements.Add(new UserIdRequirement()));
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankApp V1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization();

            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.AddHealthChecksMiddleware();

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";
                spa.Options.StartupTimeout = new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0);

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            AsyncContext.Run(async () => await DataInitializer.SeedData(userManager, roleManager, context));
        }
    }
}
