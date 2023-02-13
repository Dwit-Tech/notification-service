using AutoMapper;
using DwitTech.NotificationService.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Dtos
{
    public class EmailDto : Profile
    {
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
