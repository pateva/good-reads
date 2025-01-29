using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodReads.Models
{
    public enum ReadingStatus
    {
        Read,
        WantToRead
    }

    public class BookStatus
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public ReadingStatus Status { get; set; } 

        [Required]
        public long BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
