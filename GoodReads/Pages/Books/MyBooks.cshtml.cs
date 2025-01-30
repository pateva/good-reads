using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public ReadingStatus? Status { get; set; } 

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? AuthorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? GenreId { get; set; }

        public IList<Book> Books { get; set; } = new List<Book>();

        public IList<Author> Authors { get; set; } = new List<Author>();

        public IList<Genre> Genres { get; set; } = new List<Genre>();

        public async Task<IActionResult> OnGetAsync(ReadingStatus? status, string? searchTerm, long? authorId, long? genreId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage();
            }

            Status = status ?? ReadingStatus.Read;
            SearchTerm = searchTerm;
            AuthorId = authorId;
            GenreId = genreId;

            // Fetch authors and genres for filtering
            Authors = await _context.Authors.ToListAsync();
            Genres = await _context.Genres.ToListAsync();

            // Base query: Get books linked to the current user's BookStatus
            var query = _context.BookStatuses
                .Where(bs => bs.UserId == userId && bs.Status == Status) 
                .Include(bs => bs.Book)
                    .ThenInclude(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                .Include(bs => bs.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                .Select(bs => bs.Book) 
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(book =>
                    book.Name.Contains(SearchTerm) ||
                    book.AuthorBooks.Any(ab => ab.Author.FirstName.Contains(SearchTerm) || ab.Author.LastName.Contains(SearchTerm)) ||
                    book.BookGenres.Any(bg => bg.Genre.GenreName.Contains(SearchTerm))
                );
            }

            if (AuthorId.HasValue)
            {
                query = query.Where(book => book.AuthorBooks.Any(ab => ab.AuthorId == AuthorId.Value));
            }

            if (GenreId.HasValue)
            {
                query = query.Where(book => book.BookGenres.Any(bg => bg.GenreId == GenreId.Value));
            }

            Books = await query.ToListAsync();

            return Page();
        }

    }
}
