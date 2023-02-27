using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DwitTech.NotificationService.Data.Tests.Repository
{
    public class EmailRepoTest:IDisposable
    {

        private readonly NotificationDbContext _notificationDbContext;

        public EmailRepoTest()
        {
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _notificationDbContext = new NotificationDbContext(options);
            _notificationDbContext.Database.EnsureCreated();

            _notificationDbContext.SaveChanges();
        }

        

        [Fact]
        public async Task  CreateEmail_ReturnsABooleanValue_WhenCalled ()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new NotificationDbContext(options);

            var emailModel = new Email { 
                            Id = 1, 
                            Body = "me", 
                            From = "c", 
                            To = "x", 
                            Subject = "", 
                            Status = EmailStatus.Pending, 
                            Cc = "", 
                            Bcc = "" 
              };

            IEmailRepo emailRepo = new EmailRepo(dbContext);

            //Act
            var actual = await emailRepo.CreateEmail(emailModel);
            async Task act() => await emailRepo.CreateEmail(emailModel);

            //Assert
            Assert.True(actual);
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        

        public void Dispose()
        {
            _notificationDbContext.Database.EnsureDeleted();
            _notificationDbContext.Dispose();
        }





    }
}
