using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DwitTech.NotificationService.WebApi.Controllers
{
    public class EmailController : BaseController
    {
        private readonly EmailService _emailService;
        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SendEmail([FromBody] EmailDto MailMessage)
        {

           var response =  _emailService.SendEmail(
                                        MailMessage.From, 
                                        MailMessage.To, 
                                        MailMessage.Subject, 
                                        MailMessage.Body,
                                        MailMessage.Cc, 
                                        MailMessage.Bcc
                                        );
        }
    }
}
