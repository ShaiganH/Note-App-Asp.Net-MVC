using System.ComponentModel.DataAnnotations;

namespace MyNotes.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Name is required")]
        [MaxLength(30)]
        [StringLength(30)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
