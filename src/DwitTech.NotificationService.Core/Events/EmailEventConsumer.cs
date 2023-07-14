using Confluent.Kafka;
using DwitTech.NotificationService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailEventConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailEventConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IConsumer<Ignore, string> _consumer;

        public EmailEventConsumer(
            IConfiguration config,
            ILogger<EmailEventConsumer> logger,
            IServiceScopeFactory serviceScopeFactory,
            CancellationTokenSource cancellationTokenSource,
            IConsumer<Ignore, string> consumer)
        {
            _config = config;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _cancellationTokenSource = cancellationTokenSource;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _cancellationTokenSource.Token).Token;
                var emailConsumerService = scope.ServiceProvider.GetRequiredService<IEmailConsumerService>();
                // Subscribe to the configured Kafka topic
                _consumer.Subscribe(_config["KAFKA_TOPIC"]);

                while (!cancellationToken.IsCancellationRequested)
                {
                    await emailConsumerService.ConsumeEmailMessage(cancellationToken);
                }

                _consumer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming Kafka message");
                throw;
            }
        }

        public void StopListening()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}