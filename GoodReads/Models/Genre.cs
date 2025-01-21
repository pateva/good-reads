using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class Genre
    {
        [Key]
        public long Id { get; set; }
        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(60, ErrorMessage = "Genre cannot exceed 60 characters.")]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }
        public ICollection<BookGenre> ? BookGenres { get; set; }

    }
}
