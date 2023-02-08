using DwitTech.NotificationService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Data.Repository
{
    public class EmailRepo : IEmailRepo
    {
        public IEnumerable<EmailEntity> Entities()
        {
            var entities = new List<EmailEntity> 
            { 
            };
            return entities;
        }

        public EmailEntity GetEmailEntityById(int id)
        {
            return new EmailEntity 
            {
            };
        }
    }
}
