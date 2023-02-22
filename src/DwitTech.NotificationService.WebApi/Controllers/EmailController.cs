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
        public IActionResult SendEmail([FromBody] EmailDto MailMessage)
        {

            var createResult = _emailService.CreateEmail(MailMessage);

            if(createResult == null)
            {
                throw new ArgumentNullException("The supplied information appears to be null");
            }

            var response =  _emailService.SendEmail(
                                        MailMessage.From, 
                                        MailMessage.To, 
                                        MailMessage.Subject, 
                                        MailMessage.Body,
                                        MailMessage.Cc, 
                                        MailMessage.Bcc
                                        );

            

            if (response)
            {
                _emailService.UpdateEmailStatus(MailMessage, response);
                return Ok(response);    
            }
            else
            {
                _emailService.UpdateEmailStatus(MailMessage, response);
                return BadRequest(response);
            }
        }
    }
}
