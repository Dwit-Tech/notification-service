using DwitTech.NotificationService.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace DwitTech.NotificationService.Core.Tests.Service
{
    public class EmailServiceTest : IDisposable
    {
        private readonly NotificationDbContext _notificationDbContext;

        public EmailServiceTest()
        {
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _notificationDbContext = new NotificationDbContext(options);
            _notificationDbContext.Database.EnsureCreated();

            _notificationDbContext.SaveChanges();   
        }

        

        [Theory]
        [InlineData("me","jnr83@gmail.com","hi","testing","","", true)]
        public void SendEmail_ShouldReturn_BooleanResult(string From, string To, string Subject, string Body, string Cc, string BCc, bool expected)
        {

        }

        public void Dispose()
        {
            _notificationDbContext.Database.EnsureDeleted();
            _notificationDbContext.Dispose();
        }

    }
}
