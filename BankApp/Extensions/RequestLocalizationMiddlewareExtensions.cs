using Microsoft.AspNetCore.Builder;

namespace BankApp.Extensions
{
    public static class RequestLocalizationMiddlewareExtensions
    {
        public static void AddRequestLocalizationMiddleware(this IApplicationBuilder app)
        {
            var supportedCultures = new[] {"en-US", "en-GB"};

            app.UseRequestLocalization(options =>
                {
                    options.AddSupportedCultures(supportedCultures);
                    options.AddSupportedUICultures(supportedCultures);
                    options.SetDefaultCulture(supportedCultures[0]);
                }
            );
        }
    }
}