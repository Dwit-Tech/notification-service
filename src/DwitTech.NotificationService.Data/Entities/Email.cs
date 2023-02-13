using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Data.Entities
{
    public class Email : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        public string Bcc { get; set; }

        public string Cc { get; set; }

        [Required]
        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailStatus Status { get; set; }
    }
}
