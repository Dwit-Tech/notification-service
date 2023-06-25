using Confluent.Kafka;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailEventConsumer
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailEventListener> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource cancellationTokenSource;

        public EmailEventConsumer(IConfiguration config, ILogger<EmailEventListener> logger, IServiceProvider serviceProvider)
        {
            _config = config;
            _logger = logger;
            _serviceProvider = serviceProvider;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartListening()
        {
            using var scope = _serviceProvider.CreateScope();
            var consumerConfig = scope.ServiceProvider.GetRequiredService<ConsumerConfig>();

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_config["KAFKA_TOPIC"]);

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
 
                    var consumeResult = consumer.Consume(cancellationTokenSource.Token);
                    var emailDto = DeserializeEmailDto(consumeResult.Message.Value);
                    var emailService = GetEmailServiceInstance();
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

        private IEmailService GetEmailServiceInstance()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetRequiredService<IEmailService>();
            }
        }
    }
}