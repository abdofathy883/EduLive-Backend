using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eLearning_Admin.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return RedirectToPage("/Account/Login");
            //}

            //return Page();
        }
    }
}
