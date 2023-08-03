using System;
using System.Threading;
using Confluent.Kafka;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DwitTech.NotificationService.Core.Tests.Services
{
    public class EmailConsumerServiceTests
    {
        [Fact]
        public async Task ConsumeEmailMessage_ShouldCallSendEmail()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("KAFKA_TOPIC", "test-topic")
                })
                .Build();

            var loggerMock = new Mock<ILogger<EmailConsumerService>>();
            var emailServiceMock = new Mock<IEmailService>();
            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            var cancellationToken = new CancellationToken();

            var emailDto = new EmailDto
            {
                FromEmail = "test@gmail.com",
                ToEmail = "jokpo2565@gmail.com",
                Body = "Body of the email",
                Subject = "Welcome Home",
                Cc = "hhh",
                Bcc = "kllll"
            };
            var consumeResult = new ConsumeResult<Ignore, string>
            {
                Message = new Message<Ignore, string>
                {
                    Value = JsonConvert.SerializeObject(emailDto)
                }
            };

            consumerMock.Setup(c => c.Consume(It.IsAny<CancellationToken>()))
                .Returns(consumeResult);

            var emailConsumerService = new EmailConsumerService(config, loggerMock.Object, consumerMock.Object, emailServiceMock.Object);

            emailServiceMock.Setup(c => c.SendEmail(It.IsAny<EmailDto>())).ReturnsAsync(true);

            // Act
            await emailConsumerService.ConsumeEmailMessage(cancellationToken);

            // Assert
            emailServiceMock.Verify(c => c.SendEmail(It.IsAny<EmailDto>()), Times.Once);
        }

        [Fact]
        public async Task ConsumeEmailMessage_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<EmailConsumerService>>();
            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            var emailServiceMock = new Mock<IEmailService>();

            var emailConsumerService = new EmailConsumerService(configMock.Object, loggerMock.Object, consumerMock.Object, emailServiceMock.Object);

            emailServiceMock.Setup(c => c.SendEmail(It.IsAny<EmailDto>())).ReturnsAsync(true);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel(); // Simulate cancellation

            consumerMock.Setup(c => c.Consume(It.IsAny<CancellationToken>()))
                .Throws(new OperationCanceledException(cancellationTokenSource.Token));

            // Act and Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
            {
                return emailConsumerService.ConsumeEmailMessage(cancellationTokenSource.Token);
            });
            emailServiceMock.Verify(e => e.SendEmail(It.IsAny<EmailDto>()), Times.Never);
        }

        [Fact]
        public async Task ConsumeEmailMessage_HandlesExceptions()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<EmailConsumerService>>();
            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            var emailServiceMock = new Mock<IEmailService>();

            var cancellationToken = new CancellationToken();
            consumerMock.Setup(c => c.Consume(It.IsAny<CancellationToken>()))
                .Throws(new Exception("Error occurred while consuming Kafka message"));

            var emailConsumerService = new EmailConsumerService(configMock.Object, loggerMock.Object, consumerMock.Object, emailServiceMock.Object);

            // Act
            Func<Task> act = async () => await emailConsumerService.ConsumeEmailMessage(cancellationToken);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
            emailServiceMock.Verify(e => e.SendEmail(It.IsAny<EmailDto>()), Times.Never);
        }
    }
}