using Identity_Auth.Models;
using Identity_Auth.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Identity_Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;




        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Logout()
        {

            await signInManager.SignOutAsync();
            _httpContextAccessor.HttpContext.Session.Remove("ConnectionDetails");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.Username);
                    await SetAuthenticationProperties(user, model.RememberMe);

                    return RedirectToAction("Help", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Address = model.Address
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SetAuthenticationProperties(user,false);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        private async Task SetAuthenticationProperties(AppUser user, bool rememberMe)
        {
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(1) : (DateTimeOffset?)null
            };

            await signInManager.SignInAsync(user, authProperties);
        }






    }

}



