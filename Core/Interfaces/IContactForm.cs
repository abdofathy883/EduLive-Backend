using Core.Models;

namespace Core.Interfaces
{
    public interface IContactForm
    {
        Task<ContactForm> AddFormEntryAsync(ContactForm entry);
    }
}
