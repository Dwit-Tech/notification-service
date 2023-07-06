using Confluent.Kafka;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailConsumerService : IEmailConsumerService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailConsumerService> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IEmailService _emailService;

        public EmailConsumerService(
            IConfiguration config,
            ILogger<EmailConsumerService> logger,
            IConsumer<Ignore, string> consumer,
            IEmailService emailService)
        {
            _config = config;
            _logger = logger;
            _consumer = consumer;
            _emailService = emailService;
        }

        public async Task ConsumeEmailMessage(CancellationToken cancellationToken)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                var emailDto = DeserializeEmailDto(consumeResult.Message.Value);
                await _emailService.SendEmail(emailDto);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Email event consumption was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming Kafka message");
                throw;
            }
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
