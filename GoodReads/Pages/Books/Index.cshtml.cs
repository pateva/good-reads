using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoodReads.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly GoodReads.Data.GoodReadsContext _context;

        public IndexModel(GoodReads.Data.GoodReadsContext context)
        {
            _context = context;
        }

        public IList<Book> Book { get;set; } = new List<Book>();
        public List<SelectListItem> AuthorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GenreList { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch books with related data (authors and genres)
            Book = await _context.Books
                .Include(b => b.AuthorBooks)
                    .ThenInclude(ab => ab.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .ToListAsync();

            // Fetch authors for dropdowns
            AuthorList = await _context.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.FirstName} {a.LastName}"
                }).ToListAsync();

            // Fetch genres for dropdowns
            GenreList = await _context.Genres
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.GenreName
                }).ToListAsync();

            return Page();
        }

    }
}
