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

        public EmailEventConsumer(IConfiguration config, ILogger<EmailEventConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _config = config;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            cancellationTokenSource = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var consumerConfig = scope.ServiceProvider.GetRequiredService<ConsumerConfig>();

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_config["KAFKA_TOPIC"]);

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationTokenSource.Token);
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
            consumer.Close();
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
                return emailDto;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error occurred while deserializing EmailDto from Kafka message");
                throw;
            }
        }
    }
}