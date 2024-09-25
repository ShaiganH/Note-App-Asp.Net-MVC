using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNotes.Models
{
    public class Note
    {
        public int NoteId { get; set; }

        [Required]
        [MaxLength(100)]
        [StringLength(100)]
        public string? NoteTitle { get; set; }

        
        public string? NoteDescription { get; set; }


        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; }
    }
}
