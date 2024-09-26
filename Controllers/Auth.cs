using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using MyNotes.Email;
using MyNotes.Models;
using MyNotes.ViewModels;
using System.Security.Claims;

namespace MyNotes.Controllers
{
    public class Auth : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender emailSender;

        public Auth(SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager, IEmailSender emailSender)
        {
            this._signInManager = _signInManager;
            this._userManager = _userManager;
            this.emailSender = emailSender;

        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                var Result = await _signInManager.PasswordSignInAsync(vm.Username!, vm.Password!, vm.RememberMe, false);

                if (Result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (Result.IsNotAllowed)
                {
                    var user = await _userManager.FindByEmailAsync(vm.Username!);
                    if(user == null)
                    {
                        return BadRequest("User not found");
                    }

                    var Token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //Creating Validation Link
                    var Link = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = Token }, Request.Scheme);

                    System.Diagnostics.Debug.WriteLine(Link);
                    await emailSender.SendEmailAsync(user.Email!, "Confirm Email", Link!);




                    ModelState.AddModelError("", "Email isn't verified check your inbox");
                }
                else if (Result.IsLockedOut)
                {
                    ModelState.AddModelError("", "User account is locked.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email/Password");
                }
                return View(vm);
            }
            return View(vm);
        }




        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    Name = vm.Name,
                    Email = vm.Username,
                    UserName = vm.Username
                };
                var Result = await _userManager.CreateAsync(user, vm.Password!);
                if (Result.Succeeded)
                {
                    HttpContext.Session.SetString("Email", user.Email!);

                    //Generating Token for email verification
                    var Token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //Creating Validation Link
                    var Link = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = Token }, Request.Scheme);

                    System.Diagnostics.Debug.WriteLine(Link);
                    await emailSender.SendEmailAsync(user.Email!, "Confirm Email", Link!);


                    var resendLink = Url.Action("ResendEmail", "Auth");

                    // Add link in ViewData success message
                    ViewData["SuccessMessage"] = $"A verification email has been sent to you. Please check your inbox.<a href='{resendLink}'> Resend email</a>.";

                    return View(vm);


                }
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }
            return View(vm);
        }
       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }




        public IActionResult Check()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult EmailConfirmed()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var User = await _userManager.FindByIdAsync(userId);

            if (User == null)
            {
                return NotFound($"User Not Found with ID:'{userId}'.");
            }

            var Result = await _userManager.ConfirmEmailAsync(User, token);


            if (Result.Succeeded)
            {
                await _signInManager.SignInAsync(User, false);
                return RedirectToAction("EmailConfirmed");
            }

            return View("Error");
        }



        
        public async Task<IActionResult> ResendEmail()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email not found. Please register again.");
                return View("Check");  // Return the same view to display the form again
            }

            // Find the user by the email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User Not found");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action("ConfirmEmail", "Auth", new { UserId = user.Id, Token = token }, Request.Scheme);

            await emailSender.SendEmailAsync(user.Email!, "Resend Confirmation", Link!);
			ViewData["SuccessMessage"] = $"Email sent again";

			return RedirectToAction("Register","Auth");
        }




        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email!);
                if (user == null)
                {
                    return BadRequest("User not found");

                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var Link = Url.Action("ChangePassword", "Auth", new { userId = user.Id, Token = token }, Request.Scheme);

                emailSender.SendEmailAsync(user.Email!, "Change Password Request", Link!);

                ViewData["Success"] = "Email has been sent to your address. Check you inbox";

                return View(vm);
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string userId, string Token)
        {
            if (userId == null || Token == null)
            {
                return BadRequest("Something went wrong");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var model = new ChangePasswordVM
            {
                UserId = userId,
                Token = Token
			};

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(vm.UserId!);

                if (user == null)
                {
                    return View(vm);
                }

                var result = await _userManager.ResetPasswordAsync(user, vm.Token!, vm.NewPassword!);

                if (result.Succeeded)
                {
                    ViewData["passwordReset"] = "Password has been reset";
                    return RedirectToAction("Login", "Auth");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }
            return View(vm);
        }
    }
}
