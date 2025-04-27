using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Infrastructure.Services
{
    public class MailingService(MailSettings mailSettings) : IMailingService , IEmailSender
    {
        private readonly MailSettings _mailSettings = mailSettings;
        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject,
            };
            email.To.Add(MailboxAddress.Parse(mailTo));
            var bulider = new BodyBuilder();
            if (attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();

                        bulider.Attachments.Add(file.FileName, fileBytes,ContentType.Parse(file.ContentType));
                    }
                }
            }
            
            bulider.HtmlBody = body;
            
            email.Body=bulider.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.DispalyName,_mailSettings.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailAsync(email, subject, htmlMessage, null);
        }
    }
}
