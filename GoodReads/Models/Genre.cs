using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class Genre
    {
        [Key]
        public long Id { get; set; }
        public string GenreName { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; }

    }
}
