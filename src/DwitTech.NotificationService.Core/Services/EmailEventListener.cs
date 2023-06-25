using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailEventListener : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailEventConsumer _emailEventConsumer;

        public EmailEventListener(IServiceProvider serviceProvider, EmailEventConsumer emailEventConsumer)
        {
            _serviceProvider = serviceProvider;
            _emailEventConsumer = emailEventConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailEventConsumer = scope.ServiceProvider.GetRequiredService<EmailEventConsumer>();
            await emailEventConsumer.StartListening();

            await Task.CompletedTask;
        }
    }
}