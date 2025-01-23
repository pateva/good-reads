using GoodReads.Data;
using GoodReads.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Pages.Ganres
{
    public class IndexModel : PageModel
    {
        private readonly GoodReadsContext _context;

        public IndexModel(GoodReadsContext context)
        {
            _context = context;
        }

        public List<Genre> Genres { get; set; } = new List<Genre>();

        public async Task OnGetAsync()
        {
            Genres = await _context.Genres
                .OrderBy(g => g.GenreName)
                .ToListAsync();
        }
    }
}
