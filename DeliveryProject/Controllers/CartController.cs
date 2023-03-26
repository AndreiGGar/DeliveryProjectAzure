using DeliveryProject.Extensions;
using DeliveryProject.Models;
using DeliveryProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProject.Controllers
{
    public class CartController : Controller
    {
        private RepositoryDelivery repo;

        public CartController(RepositoryDelivery repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> GetProducts(int idrestaurant, int? idproduct)
        {
            if (idproduct != null)
            {
                List<int> carrito;
                if (HttpContext.Session.GetObject<List<int>>("CART") == null)
                {
                    carrito = new List<int>();
                }
                else
                {
                    carrito = HttpContext.Session.GetObject<List<int>>("CART");
                }
                if (carrito.Contains(idproduct.Value) == false)
                {
                    carrito.Add(idproduct.Value);
                    HttpContext.Session.SetObject("CART", carrito);
                }
            }
            List<Product> products = await this.repo.GetRestaurantsCategoriesProductsAsync(idrestaurant);
            return View(products);
        }

    }
}
