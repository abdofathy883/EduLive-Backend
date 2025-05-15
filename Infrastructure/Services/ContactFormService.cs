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
        public ContactFormService(IGenericRepo<ContactForm> _repo)
        {
            repo = _repo;
        }
        public async Task<ContactForm> AddFormEntryAsync(ContactForm entry)
        {
            await repo.AddAsync(entry);
            await repo.SaveAllAsync();
            return entry;
        }
    }
}
