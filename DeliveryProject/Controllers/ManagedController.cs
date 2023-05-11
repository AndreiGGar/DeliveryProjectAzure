using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DeliveryProjectAzure.Models;
using DeliveryProjectAzure.Repositories;

namespace DeliveryProjectAzure.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryDelivery repo;

        public ManagedController (RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register (string email, string username, string password)

        {
            User user = new User();
            user.Email = email;
            user.Name = username;
            user.Password = password;
            user.Rol = "user";
            user.DateAdd = DateTime.Now;
            user.Image = "0-user.png";
            await this.repo.RegisterUser(user.Email, user.Name, user.Password, user.Rol, user.DateAdd, user.Image);

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            User user = this.repo.LoginUser(email, password);
            if (user == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                Claim claimName = new Claim(ClaimTypes.Name, user.Name);
                Claim claimEmail = new Claim(ClaimTypes.Email, user.Email);
                Claim claimRole = new Claim(ClaimTypes.Role, user.Rol);
                Claim claimId = new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
                Claim claimImage = new Claim("Image", user.Image);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

                identity.AddClaim(claimName);
                identity.AddClaim(claimEmail);
                identity.AddClaim(claimRole);
                identity.AddClaim(claimId);
                identity.AddClaim(claimImage);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddDays(2)
                });

                if (TempData["controller"] != null && TempData["action"] != null)
                {
                    string controller = TempData["controller"].ToString();
                    string action = TempData["action"].ToString();

                    return RedirectToAction(action, controller);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
