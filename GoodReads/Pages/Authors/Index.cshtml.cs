using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GoodReads.Data;
using GoodReads.Models;
using Microsoft.Extensions.Hosting;

namespace GoodReads.Pages.Authors
{
    public class IndexModel : PageModel
    {
        private readonly GoodReads.Data.GoodReadsContext _context;
        private readonly IWebHostEnvironment _environment;


        public IndexModel(GoodReads.Data.GoodReadsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IList<Author> Authors { get; set; } = new List<Author>();
        [BindProperty]
        public Author Author { get; set; } = new Author();

        public async Task OnGetAsync()
        {
            Authors = await _context.Authors.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Authors = await _context.Authors.ToListAsync();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check your input.";
                return Page();
            }

            // Use file name to reference the image
            Author.CreatedAt = DateTime.UtcNow;
            Author.UpdatedAt = DateTime.UtcNow;

            _context.Authors.Add(Author);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author added successfully.";
            return RedirectToPage();
        }

    }
}
