using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoodReads.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public DetailsModel(GoodReadsContext context)
        {
            _context = context;
        }

        public Book Book { get; set; } = default!;
        public List<Note> Notes { get; set; } = new List<Note>();

        [BindProperty]
        public Note NewNote { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Book = await _context.Books
                .Include(b => b.AuthorBooks)
                    .ThenInclude(ab => ab.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (Book == null)
            {
                return NotFound();
            }

            Notes = await _context.Notes
                .Where(n => n.BookId == id)
                .OrderByDescending(n => n.UserId == userId) // User notes first
                .ThenByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddNoteAsync(long BookId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(Text))
            {
                TempData["ErrorMessage"] = "Note cannot be empty.";
                return RedirectToPage(new { id = BookId });
            }

            var newNote = new Note
            {
                BookId = BookId,
                UserId = userId,
                Text = Text,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Notes.Add(newNote);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = BookId }); // Removed TempData["SuccessMessage"]
        }

        public async Task<IActionResult> OnPostEditNoteAsync(long NoteId, string Text)
        {
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null || note.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToPage();
            }

            note.Text = Text;
            note.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = note.BookId }); // Removed TempData["SuccessMessage"]
        }

        public async Task<IActionResult> OnPostDeleteNoteAsync(long NoteId)
        {
            var note = await _context.Notes.FindAsync(NoteId);
            if (note == null || note.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToPage();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = note.BookId }); // Removed TempData["SuccessMessage"]
        }

    }
}
