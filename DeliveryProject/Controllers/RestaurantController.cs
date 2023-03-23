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

        public IActionResult Index(int id)
        {
            return View();
        }
    }
}
