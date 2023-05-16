using DeliveryProjectAzure.Filters;
using DeliveryProjectAzure.Services;
using DeliveryProjectNuget.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryProjectAzure.Controllers
{
    public class UserController : Controller
    {
        private ServiceApiDelivery service;

        public UserController(ServiceApiDelivery service)
        {
            this.service = service;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Profile()
        {
            User user = await this.service.UserProfileAsync(HttpContext.Session.GetString("token"));
            return View(user);
        }

        [AuthorizeUsers(Policy = "USER")]
        public async Task<IActionResult> Purchases()
        {
            /*int iduser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);*/
            List<Purchase> purchases = await this.service.GetPurchasesByUserIdAsync(HttpContext.Session.GetString("token"));
            return View(purchases);
        }

        [AuthorizeUsers(Policy = "ADMIN")]
        public async Task<IActionResult> Restaurants()
        {
            List<Restaurant> restaurants = await this.service.GetRestaurantsAsync();
            return View(restaurants);
        }

        [AuthorizeUsers(Policy = "ADMIN")]
        public async Task<IActionResult> Create()
        {
            List<Restaurant> restaurants = await this.service.GetRestaurantsAsync();
            return View(restaurants);
        }

        /*[AuthorizeUsers(Policy = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            List<Restaurant> restaurants = await this.service.GetRestaurantsAsync();
            return View(restaurants);
        }*/
    }
}
