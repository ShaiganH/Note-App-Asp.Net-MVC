using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotes.Data;
using MyNotes.Models;
using MyNotes.ViewModels;

namespace MyNotes.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NoteController(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            this._context = _context;
            this._userManager = _userManager;

		}
        public async Task<IActionResult> Index()
        {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
            List<Note> notes = _context.Notes
                .Where(x => x.ApplicationUserId == user.Id)
                .ToList();

            return View(notes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> Create(NoteVM note)
		{
            if (ModelState.IsValid)
            {
                var user =await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                Note newNote = new Note()
                {
                    NoteTitle = note.NoteTitle,
                    NoteDescription = note.NoteDescription,
                    ApplicationUserId = user.Id
                };

                _context.Notes.Add(newNote);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
			return View(note);
		}

        public IActionResult Detail(int id)
        {
            var Note = _context.Notes.FirstOrDefault(x => x.NoteId == id);
            if (Note == null)
            {
                return NotFound();
            }
            var NoteVm = new NoteVM()
            {
                NoteTitle = Note.NoteTitle!,
                NoteDescription = Note.NoteDescription!
            };
            return View(NoteVm);
        }


	}
}
