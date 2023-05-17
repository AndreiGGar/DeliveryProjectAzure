using DeliveryProjectAzure.Filters;
using DeliveryProjectNuget.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DeliveryProjectAzure.Services;

namespace DeliveryProjectAzure.Controllers
{
    [AuthorizeUsers(Policy = "USER")]
    public class WishlistController : Controller
    {
        private ServiceApiDelivery service;

        public WishlistController(ServiceApiDelivery service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            /*var iduser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);*/
            List<Restaurant> restaurants = await this.service.GetRestaurantsWithWishlistAsync(HttpContext.Session.GetString("token"));
            return View(restaurants);
        }

        public async Task<IActionResult> Add(int idrestaurant)
        {
            DateTime now = DateTime.UtcNow;
            string time = now.ToString("yyyy-MM-dd HH:mm:ss");

            bool restaurantExists = await this.service.RestaurantExistsInWishlist(HttpContext.Session.GetString("token"), idrestaurant);

            if (restaurantExists)
            {
                TempData["Message"] = "¡Este restaurante ya está en favoritos!";
                TempData["MessageType"] = "alert-danger";
            }
            else
            {
                await this.service.AddToWishlistAsync(HttpContext.Session.GetString("token"), idrestaurant, time);
                TempData["Message"] = "¡Restaurante agregado a favoritos!";
                TempData["MessageType"] = "alert-success";
            }

            string previousUrl = Request.Headers["Referer"].ToString();
            return Redirect(previousUrl);
        }

        public async Task<IActionResult> Delete(int idrestaurant)
        {
            await this.service.DeleteFromWishlistAsync(HttpContext.Session.GetString("token"), idrestaurant);
            return RedirectToAction("Index");
        }
    }
}
