using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Data.Entities
{
    public class EmailEntity
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string From { get; set; } = string.Empty;

        [Required]
        public string To { get; set; } = string.Empty;

        public string Bcc { get; set; } = string.Empty;

        public string Cc { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;
    }
}
