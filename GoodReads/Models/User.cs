using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GoodReads.Models
{
    public class User : IdentityUser
    {   public string FirstName { get; set; }
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
        public ICollection<UserBook> UserBooks { get; set; }
        public ICollection<BookStatus> BookStatuses { get; set; } = new List<BookStatus>();
        public ICollection<Note> Notes { get; set; }

    }
}
