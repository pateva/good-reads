using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Pages.BookGenres
{
    public class DetailsModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public DetailsModel(GoodReadsContext context)
        {
            _context = context;
        }

        public Genre? Genre { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch the selected genre and its related books
            Genre = await _context.Genres
            .Include(g => g.BookGenres)
            .ThenInclude(bg => bg.Book)
            .FirstOrDefaultAsync(g => g.Id == id);

            if (Genre == null)
            {
                return NotFound();
            }

            Books = Genre.BookGenres.Select(bg => bg.Book).ToList();
            return Page();
        }
    }
}
