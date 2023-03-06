using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.Data.Repository;
using DwitTech.NotificationService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DwitTech.NotificationService.WebApi.Tests.Controller
{
    public class EmailControllerTest
    {

        private readonly Mock<IEmailService> _mockService;
        private readonly EmailController _controller;
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
       public void SendEmail_ShouldReturn20()
        {

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            var mockDbContext = new Mock<NotificationDbContext>(options);
            var emailRepo = new Mock<EmailRepo>(mockDbContext.Object);
            var iLogger = new Mock<ILogger<EmailService>>();
            var iConfig = new Mock<IConfiguration>();

            
            var actual = new EmailDto { From = "test1", To = "rest", Subject = "Sample Subject", Body = "TestBody", Bcc = "", Cc = "" };
            
            var _mockService = new Mock<EmailService>(iConfig.Object, emailRepo.Object, iLogger.Object);

            var emailController = new EmailController(_mockService.Object);

            var res = emailController.SendEmail(actual);
            Assert.True(res.IsCompletedSuccessfully);


        }
    }

    
}

    

