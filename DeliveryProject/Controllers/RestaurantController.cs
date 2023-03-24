using DeliveryProject.Models;
using DeliveryProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryProject.Controllers
{
    public class RestaurantController : Controller
    {
        private RepositoryDelivery repo;

        public RestaurantController(RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index(int id)
        {
            List<CategoryProduct> categoriesByRestaurant = await this.repo.GetRestaurantsCategoriesAsync(id);
            ViewData["CATEGORIES"] = categoriesByRestaurant;
            List<ProductListViewModel> modelList = await this.repo.GetRestaurantsCategoriesProductsAsync(id);
            return View(modelList);
        }
    }
}
