using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GoodReads.Data;
using GoodReads.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly GoodReads.Data.GoodReadsContext _context;

        public CreateModel(GoodReads.Data.GoodReadsContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        public List<long> SelectedAuthors { get; set; } = new();

        [BindProperty]
        public List<long> SelectedGenres { get; set; } = new();

        public List<SelectListItem> AuthorList { get; set; } = new();
        public List<SelectListItem> GenreList { get; set; } = new();

        //For more information, see https://aka.ms/RazorPagesCRUD.

        public async Task<IActionResult> OnGetAsync()
        {
            AuthorList = await _context.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.FirstName} {a.LastName}"
                }).ToListAsync();

            GenreList = await _context.Genres
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.GenreName
                }).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdown lists if the form submission fails validation
                await OnGetAsync();
                return Page();
            }

            // Add the new book to the database
            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            // Associate selected authors
            foreach (var authorId in SelectedAuthors)
            {
                _context.AuthorBooks.Add(new AuthorBook
                {
                    AuthorId = authorId,
                    BookId = Book.Id
                });
            }

            // Associate selected genres
            foreach (var genreId in SelectedGenres)
            {
                _context.BookGenres.Add(new BookGenre
                {
                    GenreId = genreId,
                    BookId = Book.Id
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
