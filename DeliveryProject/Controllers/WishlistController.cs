using DeliveryProjectAzure.Filters;
using DeliveryProjectAzure.Models;
using DeliveryProjectAzure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryProjectAzure.Controllers
{
    [AuthorizeUsers(Policy = "USER")]
    public class WishlistController : Controller
    {
        private RepositoryDelivery repo;

        public WishlistController(RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var iduser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<Restaurant> restaurants = await this.repo.GetRestaurantsWithWishlistAsync(iduser);
            return View(restaurants);
        }

        public async Task<IActionResult> Add(int idrestaurant)
        {
            var iduser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DateTime now = DateTime.UtcNow;
            string time = now.ToString("yyyy-MM-dd HH:mm:ss");

            // Check if the restaurant is already in the user's wishlist
            bool restaurantExists = await this.repo.RestaurantExistsInWishlist(iduser, idrestaurant);

            if (restaurantExists)
            {
                TempData["Message"] = "¡Este restaurante ya está en favoritos!";
                TempData["MessageType"] = "alert-danger";
            }
            else
            {
                await this.repo.AddToWishlist(iduser, idrestaurant, time);
                TempData["Message"] = "¡Restaurante agregado a favoritos!";
                TempData["MessageType"] = "alert-success";
            }

            string previousUrl = Request.Headers["Referer"].ToString();
            return Redirect(previousUrl);
        }

        public async Task<IActionResult> Delete(int idrestaurant)
        {
            var iduser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await this.repo.DeleteFromWishlist(iduser, idrestaurant);
            return RedirectToAction("Index");
        }
    }
}
