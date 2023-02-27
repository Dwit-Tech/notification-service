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
        
        Task<bool> CreateEmail(Email email);
        Task UpdateEmailStatus(Email email, bool status);
        
    }
}
