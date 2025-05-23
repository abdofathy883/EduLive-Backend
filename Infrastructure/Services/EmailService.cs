using Core.Interfaces;
using Core.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService: IEmailService
    {
        private readonly IOptions<EmailSettings> emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            emailSettings = options;
        }
        public async Task SendEmailWithTemplateAsync(string to, string subject, string templateName, Dictionary<string, string> replacements)
        {
            var fromEmail = emailSettings.Value.AppEmail;
            var SMTPServer = emailSettings.Value.SmtpServer;
            var SMTPPort = emailSettings.Value.SmtpPort;
            var emailPassword = emailSettings.Value.AppPassword;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html");
            var htmlbody = await File.ReadAllTextAsync(templatePath);

            foreach (var pair in replacements)
            {
                htmlbody = htmlbody.Replace(pair.Key, pair.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Tahfez Quran", fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlbody
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(SMTPServer, SMTPPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, emailPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
