using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DwitTech.NotificationService.WebApi.Controllers
{
    public class EmailController : BaseController
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto MailMessage)
        {
           await _emailService.SendEmail(MailMessage.From,MailMessage.To, MailMessage.Subject, MailMessage.Body,MailMessage.Cc, MailMessage.Bcc);
           return Ok("Email Sent");

        }
    }
}
