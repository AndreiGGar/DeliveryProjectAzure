using DeliveryProjectAzure.Models;
using DeliveryProjectAzure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;

namespace DeliveryProjectAzure.Controllers
{
    public class HomeController : Controller
    {
        private RepositoryDelivery repo;

        public HomeController(RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Restaurant> restaurants = await this.repo.GetRestaurantsAsync();
            ViewData["RESTAURANTS"] = restaurants;
            List<Category> categories = await this.repo.GetCategoriesAsync();
            ViewData["CATEGORIES"] = categories;
            return View();
        }

        public async Task<IActionResult> _PaginationRestaurants(int? pageNumber, int? pageSize, int? category, string? order, bool? free)
        {
            pageNumber = pageNumber ?? 1;
            pageSize = pageSize ?? 6;
            category = category ?? 0;
            order = order ?? "relevancia";
            free = free ?? false;

            var restaurants = await this.repo.GetRestaurantsByCategoryAsync(category.Value);

            if (free != false)
            {
                restaurants = restaurants.Where(r => r.DeliveryFee == 0).ToList();
            }

            switch (order)
            {
                case "relevancia":
                    restaurants = restaurants.OrderBy(r => r.Id).ToList();
                    break;
                case "novedades":
                    restaurants = restaurants.OrderByDescending(r => r.DateAdd).ToList();
                    break;
                case "tiempoentrega":
                    restaurants = restaurants.OrderBy(r => r.DeliveryMinTime).ToList();
                    break;
                case "menorgastominimo":
                    restaurants = restaurants.OrderBy(r => r.MinimumAmount).ToList();
                    break;
                default:
                    break;
            }

            var RestaurantListViewModel = new RestaurantListViewModel
            {
                Restaurants = restaurants.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList(),
                PaginationRestaurants = new PaginationRestaurants
                {
                    CurrentPage = pageNumber.Value,
                    ItemsPerPage = pageSize.Value,
                    TotalItems = restaurants.Count()
                },
                SelectedCategory = category.Value
            };
            return PartialView("_PaginationRestaurants", RestaurantListViewModel);
        }

        /*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/
    }
}