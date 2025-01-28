using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class Author
    {
        [Key]
        public long Id { get; set; }
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string FirstName { get; set; }
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();

        public Author()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
