using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Pages.Ganres
{
    public class IndexModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public IndexModel(GoodReadsContext context)
        {
            _context = context;
        }

        public List<Genre> Genres { get; set; } = new List<Genre>();

        [BindProperty]
        public Genre Genre { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            Genres = await _context.Genres
                .OrderBy(g => g.GenreName)
                .ToListAsync();

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
            return Page();
        }

        public async Task OnGetAsync()
        {
            Genres = await _context.Genres
                .OrderBy(g => g.GenreName)
                .ToListAsync();
        }
    }
}
