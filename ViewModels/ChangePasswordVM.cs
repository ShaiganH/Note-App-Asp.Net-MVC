using System.ComponentModel.DataAnnotations;

namespace MyNotes.ViewModels
{
	public class ChangePasswordVM
	{
		public string? UserId { get; set; }

		public string? Token { get; set; }

		[Required(ErrorMessage ="Required")]
		[DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

		[Required(ErrorMessage ="Required")]
		[DataType(DataType.Password)]
		[Compare("NewPassword")]
		[Display(Name ="Confirm password")]
		public string? ConfirmPassword { get; set; }
	}
}
