using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly IContactForm contactForm;
        public ContactFormController(IContactForm form)
        {
            contactForm = form;
        }
        [HttpGet("submit-contact-form")]
        public async Task<IActionResult> AddFormEntryAsync([FromForm] ContactForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var newEntry = await contactForm.AddFormEntryAsync(form);
            return Ok(newEntry);
        }
    }
}
