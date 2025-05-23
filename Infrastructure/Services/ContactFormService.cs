using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ContactFormService : IContactForm
    {
        private readonly IGenericRepo<ContactForm> repo;
        private readonly IEmailService emailService;
        public ContactFormService(IGenericRepo<ContactForm> _repo, IEmailService emailService)
        {
            repo = _repo;
            this.emailService = emailService;
        }
        public async Task<ContactForm> AddFormEntryAsync(ContactForm entry)
        {
            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            var replacement = new Dictionary<string, string>
            {
                { "Name", entry.Name },
            };
            await repo.AddAsync(entry);
            await repo.SaveAllAsync();
            await emailService.SendEmailWithTemplateAsync(entry.Email, "We Recived Your Message", "ContactFormConfirmation", replacement);
            return entry;
        }
    }
}
