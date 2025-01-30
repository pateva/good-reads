using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GoodReads.Data;
using GoodReads.Models;

namespace GoodReads.Pages.Authors
{
    public class DetailsModel : PageModel
    {
        private readonly GoodReads.Data.GoodReadsContext _context;

        public DetailsModel(GoodReads.Data.GoodReadsContext context)
        {
            _context = context;
        }

        public Author? Author { get; set; } = default!;
        public List<Book> Books { get; set; } = new List<Book>();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Author = await _context.Authors
                .Include(a => a.AuthorBooks)
                .ThenInclude(ab => ab.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Author == null)
            {
                return NotFound();
            }

            Books = Author.AuthorBooks.Select(ab => ab.Book).ToList();
            return Page();
        }
 
    }
}
