using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class AuthorBook
    {
        public long AuthorId { get; set; }
        public long BookId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public Author Author { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }
    }
}
