using AutoMapper;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Sockets;

namespace DwitTech.NotificationService.Core.Tests.Service
{
    public class EmailServiceTest 
    {
        private readonly IConfiguration _configuration;
        public EmailServiceTest()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {

                {"GmailInfo:Email", "johnokpo83@gmail.com"},
                {"GmailInfo:Host", "smtp.gmail.com"},
                {"GmailInfo:Port", "587"},
                {"GmailInfo:AppPassword", "fyvdxxvlsvosecwg"},

            }).Build();
            

        }

        [Fact]
        
        public async Task SendEmail_Returns_BooleanResult()
        {
            var iEmailRepoMock = new Mock<IEmailRepo>();
            var iLogger = new Mock<ILogger<EmailService>>();
            var iEmailService = new Mock<IEmailService>();
            var iMapper = new Mock<IMapper>();
            IEmailService emailService = new EmailService(_configuration, iEmailRepoMock.Object, iLogger.Object, iMapper.Object);

            var emailDto = new EmailDto { From = "test@gmail.com", To = "jokpo2565@gmail.com", Body = "Body of the email", Subject = "Welcome Home", Cc = "hhh", Bcc = "kllll" };

            try
            {
                var act = await emailService.SendEmail(emailDto);
                Assert.True(act);
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }

            
        }


        [Fact]
        public async Task SendEmail_Returns_AnExceptionOnFailure_Due_NetworkConnection()
        {

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;


            var mockDbContext = new Mock<NotificationDbContext>(options);
            var iLogger = new Mock<ILogger<EmailService>>();
            var emailRepo = new Mock<EmailRepo>(mockDbContext.Object);
            var emailDto = new EmailDto { From = "test@gmail.com", To = "example@gmail.com", Body = "Body of the email", Subject = "Welcome Home", Cc = "", Bcc = "" };
            var iMapper = new Mock<IMapper>();

            IEmailService emailService = new EmailService(_configuration, emailRepo.Object, iLogger.Object, iMapper.Object);

            async Task actual() => await emailService.SendEmail(emailDto);

            await Assert.ThrowsAsync<SocketException>(actual);

        }

        


    }
}
