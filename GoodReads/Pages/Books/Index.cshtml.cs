using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace GoodReads.Pages.Books
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public IndexModel(GoodReadsContext context)
        {
            _context = context;
        }

        public List<Book> Books { get; set; } = new();
        public List<Author> Authors { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();

        [BindProperty]
        public Book NewBook { get; set; } = new();

        [BindProperty]
        public List<long> SelectedAuthorIds { get; set; } = new();

        [BindProperty]
        public List<long> SelectedGenreIds { get; set; } = new();

        [BindProperty]
        public long SelectedBookId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public long? AuthorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? GenreId { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Authors = await _context.Authors.ToListAsync();
            Genres = await _context.Genres.ToListAsync();

            var booksQuery = _context.Books
                .Include(b => b.AuthorBooks)
                    .ThenInclude(ab => ab.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(b => b.BookStatuses) 
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                booksQuery = booksQuery.Where(b => b.Name.Contains(SearchTerm));

            if (AuthorId.HasValue)
                booksQuery = booksQuery.Where(b => b.AuthorBooks.Any(ab => ab.AuthorId == AuthorId));

            if (GenreId.HasValue)
                booksQuery = booksQuery.Where(b => b.BookGenres.Any(bg => bg.GenreId == GenreId));

            var books = await booksQuery.ToListAsync();

            Books = books.Select(b =>
            {
                var userStatus = b.BookStatuses.FirstOrDefault(bs => bs.UserId == userId);
                b.CurrentUserStatus = userStatus?.Status; 
                return b;
            }).ToList();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("SearchTerm");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check your input.";
                await OnGetAsync();
                return Page();
            }

            var existingBook = await _context.Books
                .Include(b => b.AuthorBooks)
                .FirstOrDefaultAsync(b =>
                    b.Name.ToLower() == NewBook.Name.ToLower() &&
                    b.AuthorBooks.Any(ab => SelectedAuthorIds.Contains(ab.AuthorId))
                );

            if (existingBook != null)
            {
                TempData["ErrorMessage"] = "The book already exists.";
                await OnGetAsync();
                return Page();
            }

            _context.Books.Add(NewBook);
            await _context.SaveChangesAsync();

            foreach (var authorId in SelectedAuthorIds)
            {
                _context.AuthorBooks.Add(new AuthorBook { AuthorId = authorId, BookId = NewBook.Id });
            }

            foreach (var genreId in SelectedGenreIds)
            {
                _context.BookGenres.Add(new BookGenre { GenreId = genreId, BookId = NewBook.Id });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book added successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(string Status)
        {
            if (SelectedBookId == 0)
            {
                TempData["ErrorMessage"] = "Invalid book selection.";
                return RedirectToPage();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingStatus = await _context.BookStatuses
                .FirstOrDefaultAsync(bs => bs.BookId == SelectedBookId && bs.UserId == userId);

            if (existingStatus != null && (Status == "RemoveRead" || Status == "RemoveWantToRead"))
            {
                _context.BookStatuses.Remove(existingStatus);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Book removed from your list.";
                return RedirectToPage();
            }

            if (existingStatus != null)
            {
                existingStatus.Status = Enum.Parse<ReadingStatus>(Status);
            }
            else
            {
                _context.BookStatuses.Add(new BookStatus
                {
                    BookId = SelectedBookId,
                    UserId = userId,
                    Status = Enum.Parse<ReadingStatus>(Status)
                });
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Book status updated successfully!";
            return RedirectToPage();
        }




        public async Task<IActionResult> OnGetAuthorsAsync(long bookId)
        {
            var authors = await _context.AuthorBooks
                .Where(ab => ab.BookId == bookId)
                .Select(ab => new { ab.Author.FirstName, ab.Author.LastName })
                .ToListAsync();

            return new JsonResult(authors, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
