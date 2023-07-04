using Confluent.Kafka;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IConsumer<Ignore, string> _consumer; // Add this field


        public EmailEventConsumer(IConfiguration config, ILogger<EmailEventConsumer> logger, IServiceScopeFactory serviceScopeFactory, IConsumer<Ignore, string> consumer)
        {
            _config = config;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            cancellationTokenSource = new CancellationTokenSource();
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            // Subscribe to the configured Kafka topic
            _consumer.Subscribe(_config["KAFKA_TOPIC"]);

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationTokenSource.Token);
                    var emailDto = DeserializeEmailDto(consumeResult.Message.Value);
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    await emailService.SendEmail(emailDto);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Email event consumption was canceled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while consuming Kafka message");
                }
                
            }
            _consumer.Close();
        }

        public void StopListening()
        {
            cancellationTokenSource.Cancel();
        }

        private EmailDto DeserializeEmailDto(string message)
        {
            try
            {
                var emailDto = JsonConvert.DeserializeObject<EmailDto>(message);
#pragma warning disable CS8603 // Possible null reference return.
                return emailDto;
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error occurred while deserializing EmailDto from Kafka message");
                throw;
            }
        }
    }
}