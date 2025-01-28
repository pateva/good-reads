using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Pages.Books
{
    public class MyBooksModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public MyBooksModel(GoodReadsContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; } 

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? AuthorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? GenreId { get; set; }

        public IList<Book> Books { get; set; } = new List<Book>();

        public IList<Author> Authors { get; set; } = new List<Author>();

        public IList<Genre> Genres { get; set; } = new List<Genre>();

        public async Task<IActionResult> OnGetAsync(string? status, string? searchTerm, long? authorId, long? genreId)
        {
            Status = status ?? "Read";
            SearchTerm = searchTerm;
            AuthorId = authorId;
            GenreId = genreId;

            // Fetch authors and genres for filtering
            Authors = await _context.Authors.ToListAsync();
            Genres = await _context.Genres.ToListAsync();

            // Base query to get books by status
            var query = _context.BookStatuses
                .Include(bs => bs.Book)
                    .ThenInclude(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                .Include(bs => bs.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                .Where(bs => bs.UserId == User.Identity.Name && bs.Status == Status)
                .Select(bs => bs.Book);

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(book =>
                    book.Name.Contains(SearchTerm) ||
                    book.AuthorBooks.Any(ab => ab.Author.FirstName.Contains(SearchTerm) || ab.Author.LastName.Contains(SearchTerm)) ||
                    book.BookGenres.Any(bg => bg.Genre.GenreName.Contains(SearchTerm))
                );
            }

            // Apply author filter
            if (AuthorId.HasValue)
            {
                query = query.Where(book => book.AuthorBooks.Any(ab => ab.AuthorId == AuthorId.Value));
            }

            // Apply genre filter
            if (GenreId.HasValue)
            {
                query = query.Where(book => book.BookGenres.Any(bg => bg.GenreId == GenreId.Value));
            }

            Books = await query.ToListAsync();
            return Page();
        }
    }
}
