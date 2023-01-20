using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Mail;
using MailKit.Net.Smtp;

namespace BusinessLogic.Services
{
    public class Mail : IMail
    {
        private readonly MailSettings _mailSettings;
        public Mail(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task<bool> SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();

                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                {
                    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    var email = new MimeMessage();
                    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                    email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                    email.Subject = mailRequest.Subject;
                    var builder = new BodyBuilder();

                    builder.HtmlBody = mailRequest.Body;
                    email.Body = builder.ToMessageBody();

                    using var smtp = new SmtpClient();
                    {
                        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                        await smtp.SendAsync(email);
                        smtp.Disconnect(true);
                    }

                    return true;
                }
                catch (Exception ex2)
                {

                    return false;
                }
            }
        }
    }
}
