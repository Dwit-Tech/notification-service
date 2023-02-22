using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
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
        public EmailControllerTest()
        {
            _mockService = new Mock<IEmailService>();
            _controller = new EmailController(_mockService.Object);
        }

        [Fact]
       public void SendEmail_ShouldReturn200StatusIfSuccessFull()
        {

            //Arrange 
            var actual = new EmailDto { From="test1", To="rest", Subject = "Sample Subject", Body = "TestBody",  Bcc ="", Cc=""};
            var expected = true;
            //Act
            var context = new ValidationContext(actual);
            var result = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(actual, context, result, expected);

            //Assert
            Assert.True(isValid);
           

        }
    }

    
}

    

