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

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        private const int PageSize = 5; 

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Book = await _context.Books
                .Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Book == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notesQuery = _context.Notes
                .Where(n => n.BookId == id)
                .OrderByDescending(n => n.CreatedAt)
                .AsQueryable();

            int totalNotes = await notesQuery.CountAsync();
            ViewData["TotalPages"] = (int)Math.Ceiling(totalNotes / (double)PageSize);

            Notes = await notesQuery
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
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
