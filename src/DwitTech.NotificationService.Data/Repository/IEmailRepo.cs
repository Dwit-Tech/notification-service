using DwitTech.NotificationService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Data.Repository
{
    internal interface IEmailRepo
    {
        IEnumerable<EmailEntity> Entities();
        EmailEntity GetEmailEntityById(int id);
    }
}
