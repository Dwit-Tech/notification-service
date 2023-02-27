using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

using Xunit.Sdk;

namespace DwitTech.NotificationService.Core.Tests.Service
{
    public class EmailServiceTest 
    {
        

        [Fact]
        public async Task SendEmail_ShouldReturn_BooleanResult()
        {

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;
           

            var mockDbContext = new Mock<NotificationDbContext>(options);
           
            var configuration = new Mock<IConfiguration>();
            var iLogger = new Mock<ILogger<EmailService>>();
            var emailRepo = new Mock<EmailRepo>(mockDbContext.Object);

            var emailDto = new EmailDto { From = "test@gmail.com", To = "jokpo2565@gmail.com", Body = "test values", Subject = "Going",  Cc ="", Bcc="" };


            IEmailService emailService = new EmailService(configuration.Object, emailRepo.Object, iLogger.Object);
            var res = await emailService.SendEmail(emailDto);
            

            Assert.True(res);
            

        }


        [Fact]
        public async Task SendEmail_Returns_AnExceptionOnFailure()
        {

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;


            var mockDbContext = new Mock<NotificationDbContext>(options);
            var configuration = new Mock<IConfiguration>();
            var iLogger = new Mock<ILogger<EmailService>>();
            var emailRepo = new Mock<EmailRepo>(mockDbContext.Object);

            var emailDto = new EmailDto { From = "test@gmail.com", To = "example@gmail.com", Body = "Body of the email", Subject = "Welcome Home", Cc = "", Bcc = "" };


            IEmailService emailService = new EmailService(configuration.Object, emailRepo.Object, iLogger.Object);
            
            async Task actual() => await emailService.SendEmail(emailDto);

            await Assert.ThrowsAsync<NullReferenceException>(actual);

        }



    }
}
