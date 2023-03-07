using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Data.Repository
{

    public class EmailRepo : IEmailRepo
    {
        private readonly NotificationDbContext _notificationDbContext;
       
        public EmailRepo(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }

        public async Task<bool> CreateEmail(Email email)
        {
            if(email is null)
                throw new ArgumentException(nameof(email));
            try
            {
                _notificationDbContext.Emails.Add(email);
                await _notificationDbContext.SaveChangesAsync();
                return true;
            }catch(NullReferenceException ex)
            {
                return false;
            }
        }

        public async Task UpdateEmail(Email email, bool status)
        {
            var emailModel = _notificationDbContext.Emails.FirstOrDefault(o => o.Id == email.Id);

            if(emailModel != null)
            {
                emailModel.Status =  status ?  EmailStatus.Sent : EmailStatus.Pending;
                _notificationDbContext.Update(emailModel);
                await _notificationDbContext.SaveChangesAsync();
                
            }
        }

        
        
    }
}
