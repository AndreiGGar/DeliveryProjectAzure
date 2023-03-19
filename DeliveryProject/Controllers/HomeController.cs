using DeliveryProject.Models;
using DeliveryProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeliveryProject.Controllers
{
    public class HomeController : Controller
    {
        private RepositoryDelivery repo;
       
        public HomeController (RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Restaurant> restaurants = this.repo.GetRestaurants();
            ViewData["RESTAURANTS"] = restaurants;
            List<Category> categories = this.repo.GetCategories();
            ViewData["CATEGORIES"] = categories;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}