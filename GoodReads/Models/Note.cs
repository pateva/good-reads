using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class Note
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public long BookId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Text { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }
    }
}
