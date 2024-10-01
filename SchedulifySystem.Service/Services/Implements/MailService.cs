using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SchedulifySystem.Service.BusinessModels.EmailModels;
using SchedulifySystem.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class MailService(IOptions<EmailConfig> emailConfig) : IMailService
    {
        readonly EmailConfig _emailConfig = emailConfig.Value;

        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailConfig.Mail);
            email.To.Add(MailboxAddress.Parse(emailRequest.To));
            email.Subject = emailRequest.Subject;
            var builder = new BodyBuilder();

            //Attachment
            if (emailRequest.AttachmentFilePaths != null)
            {
                byte[] fileBytes;
                foreach (var file in emailRequest.AttachmentFilePaths)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            //Body
            builder.HtmlBody = emailRequest.Content;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailConfig.Mail, _emailConfig.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
