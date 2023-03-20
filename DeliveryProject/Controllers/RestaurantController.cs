using Microsoft.AspNetCore.Mvc;

namespace DeliveryProject.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Index(int id)
        {

            return View();
        }
    }
}
