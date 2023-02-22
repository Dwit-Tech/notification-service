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

        public Email FindEmail(string email)
        {
            return _notificationDbContext.Emails.FirstOrDefault(o => o.To == email);
            
        }

        public void CreateEmail(Email email)
        {
            if(email == null)
            {
                throw new ArgumentException(nameof(email));
            }
             _notificationDbContext.Emails.Add(email);
        }

        public void UpdateEmailStatus(Email email, bool status)
        {
            var emailModel = _notificationDbContext.Emails.FirstOrDefault(o => o.To == email.To);

            if(emailModel != null)
            {
                if(!status)
                {
                    emailModel.Status = EmailStatus.Pending;
                    this.SaveChanges();
                }


                if (status)
                {
                    emailModel.Status = EmailStatus.Sent;
                    this.SaveChanges();
                }

            }
        }

        public bool SaveChanges()
        {
            return (_notificationDbContext.SaveChanges() >= 0);
        }

       
    }
}
