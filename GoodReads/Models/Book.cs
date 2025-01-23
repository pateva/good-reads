using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class Book
    {
        [Key]
        public long Id { get; set; }
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string Name { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
        public ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
        public ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();
        //TODO fix
        public ICollection<Note> Notes { get; set; } 

        public Book()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
