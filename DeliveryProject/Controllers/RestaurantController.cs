using DeliveryProjectAzure.Filters;
using DeliveryProjectNuget.Models;
using DeliveryProjectNuget.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DeliveryProjectAzure.Services;

namespace DeliveryProjectAzure.Controllers
{
    public class RestaurantController : Controller
    {
        private ServiceApiDelivery service;

        public RestaurantController(ServiceApiDelivery service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index(int id)
        {
            List<CategoryProduct> categoriesByRestaurant = await this.service.GetRestaurantsCategoriesAsync(id);
            ViewData["CATEGORIES"] = categoriesByRestaurant;
            List<Product> products = await this.service.GetRestaurantsCategoriesProductsAsync(id);
            ViewData["PRODUCTS"] = products;
            Restaurant restaurant = await this.service.GetRestaurantByIdAsync(id);
            ViewData["RESTAURANT"] = restaurant;
            return View();
        }

        
        public async Task<IActionResult> Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string search)
        {
            if (search == null || search == "" || search == " ")
            {
                List<Restaurant> restaurants = await this.service.GetRestaurantsAsync();
                return View(restaurants);

            } else
            {
                List<Restaurant> restaurants = await this.service.GetRestaurantBySearchAsync(search);
                return View(restaurants);
            }
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
                ViewData["MESSAGE"] = "Actualmente no tienes ningún producto en el carrito";
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

                ViewData["RESTAURANTS"] = await this.service.GetRestaurantsAsync();
                List<Product> products = await this.service.GetProductsCartAsync(cart);
                return View(products);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return RedirectToAction("Cart");
        }

        [AuthorizeUsers(Policy = "USER")]
        [HttpPost]
        public async Task<IActionResult> Checkout(int restaurantid, string totalPrice, string deliveryMethod, string deliveryAddress, string paymentMethod)
        {
            List<int> cart = HttpContext.Session.GetObject<List<int>>("CART");
            List<Product> products = await this.service.GetProductsCartAsync(cart);

            // Create a new purchase object
            Purchase purchase = new Purchase();
            purchase.UserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            purchase.RestaurantId = restaurantid;
            purchase.TotalPrice = decimal.Parse(totalPrice);
            purchase.Status = "Pending";
            if (deliveryMethod == "Delivery")
            {
                purchase.Delivery = true;
                purchase.PaymentMethod = paymentMethod;
            }
            else
            {
                purchase.Delivery = false;
                Random rnd = new Random();
                string code = "";
                for (int i = 0; i < 11; i++)
                {
                    code += rnd.Next(10).ToString();
                }
                purchase.Code = code;
            }
            DateTime requestDate = DateTime.Now;
            string formattedRequestDate = requestDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            purchase.DeliveryMethod = deliveryMethod;
            purchase.DeliveryAddress = deliveryAddress;
            var productsCart = "";
            foreach (Product product in products)
            {
                productsCart += product.Id + ",";
            }
            productsCart = productsCart.TrimEnd(',');
            purchase.Products = productsCart;
            await this.service.InsertPurchaseAsync(HttpContext.Session.GetString("token"), purchase.UserId, purchase.RestaurantId, purchase.TotalPrice, purchase.Status, purchase.Delivery, formattedRequestDate, purchase.DeliveryAddress, purchase.DeliveryMethod, purchase.Code, productsCart, purchase.PaymentMethod);
            // Remove cart from session and redirect to success page
            HttpContext.Session.Remove("CART");
            return RedirectToAction("Success");
        }

        public async Task<IActionResult> Success()
        {
            return View();
        }
    }
}
