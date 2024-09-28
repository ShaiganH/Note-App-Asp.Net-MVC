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

		public async Task<IActionResult> Completed()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
			List<Note> notes = _context.Notes
				.Where(x => x.ApplicationUserId == user.Id && x.IsCompleted == true)
				.ToList();

			return View(notes);
		}

		public IActionResult Error()
        {
            

            return View("Error");
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
                return RedirectToAction("Error", "note");
            }
            
            return View(Note);
        }

        public async Task<IActionResult> Complete(int? id)
        {
            var user =await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return Unauthorized();
            }
            var note =_context.Notes.FirstOrDefault(x => x.NoteId == id && x.ApplicationUserId == user!.Id);
            if(note is null)
            {
                return RedirectToAction("Error", "note");
            }
            note.IsCompleted = !note.IsCompleted;

            _context.SaveChanges();

            return RedirectToAction("Detail","Note",new { id });



        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return Unauthorized();
            }

            var note = _context.Notes.FirstOrDefault(x => x.NoteId == id && x.ApplicationUserId == user.Id);

            if (note is null)
            {
                return RedirectToAction("Error", "note");
            }

            var noteVm = new NoteVM()
            {
                NoteId = note.NoteId,
                NoteTitle = note.NoteTitle!,
                NoteDescription = note.NoteDescription!
            };

			
			return View(noteVm);
		}

        [HttpPost]
		public async Task<IActionResult> Edit(NoteVM vm)
		{
            if (ModelState.IsValid)
            {
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return Unauthorized();
				}
				var note = _context.Notes.FirstOrDefault(x => x.NoteId == vm.NoteId && x.ApplicationUserId == user.Id);

                if(note == null)
                {
                    return RedirectToAction("Error","note");
                }

				note.NoteTitle = vm.NoteTitle;
				note.NoteDescription = vm.NoteDescription;

				_context.SaveChanges();

				return RedirectToAction("Detail", new { id = note.NoteId });
			}
            return View(vm);
		}

        
     


        public async Task<IActionResult> Delete(int id)
        {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
			var note = _context.Notes.FirstOrDefault(x => x.NoteId == id && x.ApplicationUserId == user.Id);

			if (note == null)
            {
                return RedirectToAction("Error", "note");
            }

            _context.Notes.Remove(note);
            _context.SaveChanges();

            return RedirectToAction("Index", "Note");

        }

	}
}
