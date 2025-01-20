using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoodReads.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (!User.Identity?.IsAuthenticated ?? true) // Check if the user is not authenticated
            {
                return RedirectToPage("/Account/Login");
            }

            return Page(); // Render the index page for authenticated users
        }
    }
}
