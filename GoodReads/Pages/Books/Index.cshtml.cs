using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodReads.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public IndexModel(GoodReadsContext context)
        {
            _context = context;
        }

        public List<Book> Books { get; set; } = new List<Book>();
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Genre> Genres { get; set; } = new List<Genre>();

        [BindProperty]
        public Book NewBook { get; set; } = new Book();

        [BindProperty]
        public List<long> SelectedAuthorIds { get; set; } = new List<long>();

        [BindProperty]
        public List<long> SelectedGenreIds { get; set; } = new List<long>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public long? AuthorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? GenreId { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch authors and genres for the filters
            Authors = await _context.Authors.ToListAsync();
            Genres = await _context.Genres.ToListAsync();

            // Fetch books
            var booksQuery = _context.Books
                .Include(b => b.AuthorBooks)
                    .ThenInclude(ab => ab.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                booksQuery = booksQuery.Where(b => b.Name.Contains(SearchTerm));
            }

            if (AuthorId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.AuthorBooks.Any(ab => ab.AuthorId == AuthorId));
            }

            if (GenreId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.BookGenres.Any(bg => bg.GenreId == GenreId));
            }

            Books = await booksQuery.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove irrelevant validations like SearchTerm during POST
            ModelState.Remove("SearchTerm");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check your input.";
                await OnGetAsync(); // Reload data for the modal
                return Page();
            }

            // Validate if a book with the same name and authors already exists
            var existingBook = await _context.Books
                .Include(b => b.AuthorBooks)
                .FirstOrDefaultAsync(b =>
                    b.Name.ToLower() == NewBook.Name.ToLower() &&
                    b.AuthorBooks.Any(ab => SelectedAuthorIds.Contains(ab.AuthorId))
                );

            if (existingBook != null)
            {
                TempData["ErrorMessage"] = $"The book '{NewBook.Name}' with the same authors already exists.";
                await OnGetAsync(); // Reload data for the modal
                return Page();
            }

            // Add the new book
            _context.Books.Add(NewBook);
            await _context.SaveChangesAsync();

            // Add author relationships
            foreach (var authorId in SelectedAuthorIds)
            {
                _context.AuthorBooks.Add(new AuthorBook
                {
                    AuthorId = authorId,
                    BookId = NewBook.Id
                });
            }

            // Add genre relationships
            foreach (var genreId in SelectedGenreIds)
            {
                _context.BookGenres.Add(new BookGenre
                {
                    GenreId = genreId,
                    BookId = NewBook.Id
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book added successfully.";
            return RedirectToPage();
        }


    }
}
