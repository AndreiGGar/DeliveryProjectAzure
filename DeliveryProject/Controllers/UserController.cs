using DeliveryProjectAzure.Filters;
using DeliveryProjectAzure.Models;
using DeliveryProjectAzure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryProjectAzure.Controllers
{
    public class UserController : Controller
    {
        private RepositoryDelivery repo;

        public UserController(RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Profile()
        {
            User user = await this.repo.UserProfileAsync(int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return View(user);
        }

        [AuthorizeUsers(Policy = "USER")]
        public async Task<IActionResult> Purchases()
        {
            int iduser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<Purchase> purchases = await this.repo.GetPurchasesByUserIdAsync(iduser);
            return View(purchases);
        }

        [AuthorizeUsers(Policy = "ADMIN")]
        public async Task<IActionResult> Restaurants()
        {
            List<Restaurant> restaurants = await this.repo.GetRestaurantsAsync();
            return View(restaurants);
        }

        /*[AuthorizeUsers(Policy = "ADMIN")]
        public async Task<IActionResult> Create()
        {
            List<Restaurant> restaurants = await this.repo.GetRestaurantsAsync();
            return View(restaurants);
        }

        [AuthorizeUsers(Policy = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            List<Restaurant> restaurants = await this.repo.GetRestaurantsAsync();
            return View(restaurants);
        }*/
    }
}
