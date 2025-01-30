using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoodReads.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GoodReadsContext _context;

        public IndexModel(ILogger<IndexModel> logger, GoodReadsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<Book> MyBooks { get; set; } = new List<Book>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                MyBooks = new List<Book>(); // Ensure no error for non-logged-in users
                return;
            }

            // Fetch books where the user has set a status
            var booksQuery = _context.BookStatuses
                .Where(bs => bs.UserId == userId)  // Only get books related to the user
                .Include(bs => bs.Book)
                    .ThenInclude(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                .Include(bs => bs.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                .Select(bs => new
                {
                    bs.Book,
                    bs.Status
                })
                .ToListAsync();

            var booksWithStatus = await booksQuery;

            MyBooks = booksWithStatus.Select(b =>
            {
                b.Book.CurrentUserStatus = b.Status; 
                return b.Book;
            }).ToList();

        }

    }
}
