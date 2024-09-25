using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyNotes.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }


        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
