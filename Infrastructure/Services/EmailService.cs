using Core.Interfaces;
using Core.Settings;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService: IEmailService, IEmailSender
    {
        private readonly IOptions<EmailSettings> emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            emailSettings = options;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings.Value.AppEmail
                , emailSettings.Value.AppEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            // Configure the SmtpClient
            using var smtpClient = new System.Net.Mail.SmtpClient(emailSettings.Value.SmtpServer, emailSettings.Value.SmtpPort)
            {
                // Set credentials and enable SSL
                Credentials = new NetworkCredential(emailSettings.Value.AppEmail, emailSettings.Value.AppPassword),
                EnableSsl = true,
            };

            // Send the email
            await smtpClient.SendMailAsync(mailMessage);
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

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(SMTPServer, SMTPPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, emailPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
