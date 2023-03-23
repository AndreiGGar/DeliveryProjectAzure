using DeliveryProject.Models;
using DeliveryProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryProject.Controllers
{
    public class RestaurantsController : Controller
    {
        private RepositoryDelivery repo;

        public RestaurantsController(RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public IActionResult Index(int id)
        {

            return View();
        }

        public async Task<IActionResult> View(int? position, int? quantity)
        {

            PaginationRestaurants restaurants = await this.repo.GetRestaurantsAsync(position.Value, quantity.Value);

            return View(restaurants);
        }
    }
}
