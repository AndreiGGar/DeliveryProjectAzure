using DeliveryProject.Extensions;
using DeliveryProject.Models;
using DeliveryProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            List<Product> products = await this.repo.GetRestaurantsCategoriesProductsAsync(id);
            ViewData["PRODUCTS"] = products;
            return View();
        }

        public async Task<IActionResult> AddCart(int? idproduct)
        {
            if (idproduct != null)
            {
                List<int> cart;

                if (HttpContext.Session.GetObject<List<int>>("CART") == null)
                {
                    cart = new List<int>();
                }
                else
                {
                    cart = HttpContext.Session.GetObject<List<int>>("CART");
                }
                cart.Add(idproduct.Value);
                HttpContext.Session.SetObject("CART", cart);
            }
            string previousUrl = Request.Headers["Referer"].ToString();
            return Redirect(previousUrl);
        }

        public async Task<IActionResult> Cart(int? idproduct)
        {
            List<int> cart = HttpContext.Session.GetObject<List<int>>("CART");
            if (cart == null)
            {
                ViewData["MESSAGE"]  = "Actualmente no tienes ningún producto en el carrito";
                return View();
            }
            else
            {
                if (idproduct != null)
                {
                    cart.Remove(idproduct.Value);

                    if (cart.Count == 0)
                    {
                        HttpContext.Session.Remove("CART");
                        ViewData["MESSAGE"] = "Actualmente no tienes ningún producto en el carrito";
                    }
                    else
                    {
                        HttpContext.Session.SetObject("CART", cart);
                    }

                }

                List<Product> products = await this.repo.GetProductsCartAsync(cart);
                return View(products);
            }
        }
        public async Task<IActionResult> Checkout()
        {
            int idpurchase = await this.repo.GetMaxIdPurchaseAsync();
            /*int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);*/
            List<int> cart = HttpContext.Session.GetObject<List<int>>("CART");
            List<Product> products = await this.repo.GetProductsCartAsync(cart);



            /*foreach (Product product in products)
            {
                this.repo.InsertarPedido(idfactura, libro.IdLibro, idusuario);
            }*/
            HttpContext.Session.Remove("CART");
            return RedirectToAction("Purchases", "Success");
        }
    }
}
