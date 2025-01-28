using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodReads.Models
{
    public class BookStatus
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Status { get; set; } // e.g., "Reading", "Completed", "Want to Read"

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
