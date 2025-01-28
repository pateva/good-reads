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

        //TODO fix
        public ICollection<Note> Notes { get; set; }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public User(String firstNmae, String lastName)
        {
            this.LastName = lastName;
            this.FirstName = firstNmae;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
