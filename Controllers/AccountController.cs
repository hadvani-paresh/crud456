using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CrudProject.Controllers
{
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult VerifyOTP(string email = "")
        {
            ViewBag.Email = email;
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string email = "")
        {
            ViewBag.Email = email;
            return View();
        }

        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
