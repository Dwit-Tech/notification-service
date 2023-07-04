using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DwitTech.NotificationService.Core.Tests.Services
{
    public class EmailEventConsumerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldConsumeAndSendEmail()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("KAFKA_TOPIC", "test-topic"),
                    new KeyValuePair<string, string>("KAFKA_CONSUMER_GROUP_ID", "test-group-id")
                })
                .Build();

            var loggerMock = new Mock<ILogger<EmailEventConsumer>>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var emailServiceMock = new Mock<IEmailService>();
            var consumerMock = new Mock<IConsumer<Ignore, string>>();

            var emailDto = new EmailDto 
            {
                From = "test@gmail.com",
                To = "jokpo2565@gmail.com",
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

            serviceScopeFactoryMock.Setup(f => f.CreateScope())
                .Returns(Mock.Of<IServiceScope>());

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IEmailService)))
                .Returns(emailServiceMock.Object);
            serviceProviderMock.Setup(p => p.GetService(typeof(ConsumerConfig)))
                .Returns(new ConsumerConfig());

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(s => s.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            serviceScopeFactoryMock.Setup(f => f.CreateScope())
                .Returns(serviceScopeMock.Object);

            var emailEventConsumer = new EmailEventConsumer(config, loggerMock.Object, serviceScopeFactoryMock.Object, consumerMock.Object);

            // Act
            await emailEventConsumer.StartAsync(CancellationToken.None);
            //await emailEventConsumer.StopAsync(CancellationToken.None)

            // Assert
            emailServiceMock.Verify(e => e.SendEmail(emailDto), Times.Once);
            consumerMock.Verify(c => c.Close(), Times.Once);
        }
    }
}