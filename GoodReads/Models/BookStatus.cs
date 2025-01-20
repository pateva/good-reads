using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class BookStatus
    {
        [Key]
        public long Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
