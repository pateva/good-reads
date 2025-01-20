using System.ComponentModel.DataAnnotations.Schema;

namespace GoodReads.Models
{
    public class BookGenre
    {
        public long BookId { get; set; }
        public long GenreId { get; set; }
        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }
        [ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; }
    }
}
