using System;
using System.IO;
using System.Reflection;
using BankApp.ActionFilters;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Extensions;
using BankApp.Helpers.Builders.Auth;
using BankApp.Helpers.Builders.Logging;
using BankApp.Helpers.Builders.Number;
using BankApp.Helpers.Factories;
using BankApp.Helpers.Services;
using BankApp.Interfaces.Helpers.Builders.Auth;
using BankApp.Interfaces.Helpers.Builders.Logging;
using BankApp.Interfaces.Helpers.Builders.Number;
using BankApp.Interfaces.Helpers.Factories;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Middlewares;
using BankApp.Models;
using BankApp.Policies;
using BankApp.Policies.Handlers;
using BankApp.Policies.Requirement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nito.AsyncEx;

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

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

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
            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<ITellerService, TellerService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ITransferService<InternalTransferService>, InternalTransferService>();
            services.AddScoped<ITransferService<ExternalTransferService>, ExternalTransferService>();
            services.AddScoped<ILogSanitizedBuilder, LogSanitizedBuilder>();
            services.AddScoped<ISensitiveDataPropertyNamesBuilder, SensitiveDataPropertyNamesBuilder>();
            services.AddScoped<IRequestResponseLoggingBuilder, RequestResponseLoggingBuilder>();
            services.AddScoped<RequestResponseLoggingMiddleware>();
            services.AddScoped<RequestResponseLoggingFilter>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            });

            services.Configure<LogSanitizationOptions>(Configuration.GetSection(nameof(LogSanitizationOptions)));

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.ClaimsIssuer = jwtIssuerOptions.Issuer;
                options.TokenValidationParameters = tokenValidationParameters;
                options.SaveToken = false;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyName.UserIdIncludedInJwtToken,
                    policy => policy.Requirements.Add(new UserIdRequirement()));
            });

            services.AddControllers(c =>
            {
                c.Filters.AddService(typeof(RequestResponseLoggingFilter));
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context)
        {
            // Replaced by RequestResponseLoggingFilter
            //app.UseMiddleware<RequestResponseLoggingMiddleware>(); 

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

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.AddRequestLocalizationMiddleware();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.AddHealthChecksMiddleware();

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    spa.UseAngularCliServer("start");
            });

            AsyncContext.Run(async () => await DataInitializer.SeedData(userManager, roleManager, context));
        }
    }
}