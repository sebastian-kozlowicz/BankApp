using System.Linq;
using BankApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BankApp.Extensions
{
    public static class HealthChecksMiddlewareExtensions
    {
        public static void AddHealthChecksMiddleware(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            app.UseHealthChecks("/healthcheck/dependencies", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(e => new HealthCheck
                        {
                            Status = e.Value.Status.ToString(),
                            Component = e.Key,
                            Description = e.Value.Description
                        }),
                        Duration = report.TotalDuration
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                }
            });
        }
    }
}
