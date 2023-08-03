using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;


namespace DwitTech.NotificationService.WebApi.Tests.Controller
{
    public class EmailControllerTest
    {
        private readonly IConfiguration _configuration;
       
        public EmailControllerTest()
        {
            
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {

                {"GmailInfo:Email", "johnokpo83@gmail.com"},
                {"GmailInfo:Host", "smtp.gmail.com"},
                {"GmailInfo:Port", "587"},
                {"GmailInfo:AppPassword", "fyvdxxvlsvosecwg"},

            }).Build();
        }

        [Fact]
       public void SendEmail_ShouldReturn200()
       {

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            var actual = new EmailDto { FromEmail = "test1", ToEmail = "rest", Subject = "Sample Subject", Body = "TestBody", Bcc = "", Cc = "" };

            var _mockService = new Mock<IEmailService>();

            var emailController = new EmailController(_mockService.Object);

            var res = emailController.SendEmail(actual);
            Assert.True(res.IsCompletedSuccessfully);
       }
    }  
}

    

