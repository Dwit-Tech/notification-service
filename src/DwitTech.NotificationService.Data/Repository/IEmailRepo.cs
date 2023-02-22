using DwitTech.NotificationService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Data.Repository
{
    public interface IEmailRepo
    {
        void CreateEmail(Email email);
        Email FindEmail(string email);
        void UpdateEmailStatus(Email email, bool status);
        bool SaveChanges();
    }
}
