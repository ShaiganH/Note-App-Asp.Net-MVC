using System.ComponentModel.DataAnnotations;

namespace MyNotes.ViewModels
{
	public class NoteVM
	{
		[Required(ErrorMessage ="Title is required")]
		[MaxLength(100,ErrorMessage ="Max length 100")]
		public string NoteTitle { get; set; }

		[Required(ErrorMessage = "Description is required")]
		[DataType(DataType.MultilineText)]
		public string NoteDescription { get; set; }
	}
}
