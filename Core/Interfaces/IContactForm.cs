using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IContactForm
    {
        Task<ContactForm> AddFormEntryAsync(ContactForm entry);
    }
}
