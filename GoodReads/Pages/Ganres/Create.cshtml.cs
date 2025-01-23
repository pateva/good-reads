using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GoodReads.Data;
using GoodReads.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Pages.Ganres
{
    public class CreateModel : PageModel
    {
        private readonly GoodReadsContext _context;
        private readonly ILogger<CreateModel> _logger;


        public CreateModel(GoodReadsContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Genre Genre { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check the input.";
                return Page();
            }

            bool genreExists = await _context.Genres.AnyAsync(g => g.GenreName == Genre.GenreName);
            if (genreExists)
            {
                TempData["ErrorMessage"] = "This genre already exists.";
                return Page();
            }

            _context.Genres.Add(Genre);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Genre added successfully.";
            return RedirectToPage("./Create");
        }

    }
}
