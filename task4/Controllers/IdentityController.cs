using System.Security.Claims;
using Application.Common.Contracts.Services;
using Application.Models.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace task4.Controllers {

    public class IdentityController : Controller {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService) {
            this._identityService = identityService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserManagement() {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInRequest request) {
            if (!ModelState.IsValid) {
                return View(request);
            }

            var result = await _identityService.SignInAsync(request);
            if (!result.Succeeded) {
                ModelState.AddModelError("", "Authentication failed!");
                _AddModelErrors(result);
                return View(request);
            }

            await _SignInAsync(result);
            return RedirectToAction("");
        }

        private async Task _SignInAsync(AuthenticateResponse response) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Email, response.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, response.Username),
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Authorize]
        [HttpPost]
        public async new Task<IActionResult> SignOut() {
            await HttpContext.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult SignUp() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpRequest request) {
            var result = await _identityService.SignUpAsync(request);
            if (!result.Succeeded) {
                ModelState.AddModelError("", "Registration failed!");
                _AddModelErrors(result);
                return View(request);
            }

            await _SignInAsync(result);
            return RedirectToAction("");
        }

        private void _AddModelErrors(AuthenticateResponse response) {
            foreach(var error in response.Errors ?? Enumerable.Empty<IdentityError>()) {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
