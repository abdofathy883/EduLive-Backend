using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailService
    {
        //Task SendEmailAsync(string to, string subject, string body);
        //Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
        Task SendEmailWithTemplateAsync(string to, string subject, string templateName, Dictionary<string, string> replacements);
        //Task SendBulkEmailAsync(IEnumerable<string> toList, string subject, string body);
        //Task<bool> ValidateEmailAsync(string email);
    }
}
