using DwitTech.NotificationService.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder SetupMigrations(this IApplicationBuilder app, IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<NotificationDbContext>>();

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
            }

            return app;
        }
    }
}
