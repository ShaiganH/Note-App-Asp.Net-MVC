using System.ComponentModel.DataAnnotations;

namespace MyNotes.ViewModels
{
	public class ForgotPasswordVM
	{
		[Required(ErrorMessage = "Email is Required")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
	}
}
