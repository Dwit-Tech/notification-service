using AutoMapper;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.Data.Repository;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailRepo _emailRepo;
        private readonly ILogger<EmailService> _logger;
        private readonly IMapper _mapper;

        public EmailService(IConfiguration config, IEmailRepo emailRepo, ILogger<EmailService> logger, IMapper mapper)
        {
            _config = config;
            _emailRepo = emailRepo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> SendEmail(EmailDto emailDto)
        {
            var emailModel = _mapper.Map<Email>(emailDto);

            await _emailRepo.CreateEmail(emailModel);
            _logger.LogInformation(1, "The email has been inserted into the database");

            var mail = new MimeMessage();
            mail.To.Add(MailboxAddress.Parse(emailDto.ToEmail));
            mail.From.Add(MailboxAddress.Parse(_config["GMAIL_INFO:EMAIL"]));
            mail.Subject = emailDto.Subject;
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDto.Body };

            var client = GenerateSmtpClient();

            try
            {
                await client.SendAsync(mail);
                client.Disconnect(true);
                await _emailRepo.UpdateEmail(emailModel, true);
                _logger.LogInformation(2, "The email status has been updated after successfully sending the email");
                return true;
            }
            catch (Exception ex) when (ex is SocketException)
            {
                await _emailRepo.UpdateEmail(emailModel, false);
                _logger.LogError(3, $"Failed to send the email: {ex.Message}");
                return false;
            }
        }

        private SmtpClient GenerateSmtpClient()
        {
            var smtpClient = new SmtpClient();

            // Retrieve SMTP client configuration values from appsettings.json
            var host = _config["GMAIL_INFO:HOST"];
            var port = Convert.ToInt32(_config["GMAIL_INFO:PASSWORD"]);
            var email = _config["GMAIL_INFO:EMAIL"];
            var password = _config["GMAIL_INFO:APP_PASSWORD"];

            smtpClient.Connect(host, port, SecureSocketOptions.Auto);
            smtpClient.Authenticate(email, password);

            return smtpClient;
        }
    }
}