using DeliveryProjectAzure.Filters;
using DeliveryProjectNuget.Models;
using DeliveryProjectNuget.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DeliveryProjectAzure.Services;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using System.Configuration;
using Microsoft.ApplicationInsights.DataContracts;

namespace DeliveryProjectAzure.Controllers
{
    public class RestaurantController : Controller
    {
        private ServiceApiDelivery service;
        private ServiceCart serviceCart;
        private TelemetryClient telemetryClient;

        public RestaurantController(ServiceApiDelivery service, ServiceCart serviceCart, TelemetryClient telemetryClient)
        {
            this.service = service;
            this.serviceCart = serviceCart;
            this.telemetryClient = telemetryClient;
        }

        public async Task<IActionResult> Index(int id)
        {
            List<CategoryProduct> categoriesByRestaurant = await this.service.GetRestaurantsCategoriesAsync(id);
            ViewData["CATEGORIES"] = categoriesByRestaurant;
            List<Product> products = await this.service.GetRestaurantsCategoriesProductsAsync(id);
            ViewData["PRODUCTS"] = products;
            Restaurant restaurant = await this.service.GetRestaurantByIdAsync(id);
            ViewData["RESTAURANT"] = restaurant;
            ViewData["SERVICECART"] = serviceCart;
            return View();
        }

        public async Task<IActionResult> Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search) || search.Trim() == "")
            {
                List<Restaurant> restaurants = await this.service.GetRestaurantsAsync();
                return View(restaurants);
            }
            else
            {
                List<Restaurant> restaurants = await this.service.GetRestaurantBySearchAsync(search);
                return View(restaurants);
            }
        }

        public async Task<IActionResult> AddCart(int? idproduct)
        {
            if (idproduct != null)
            {
                string cacheKey = $"cart_{User.Identity.Name}";
                List<int> cart = await this.serviceCart.GetCartAsync(cacheKey);

                if (cart == null)
                {
                    cart = new List<int>();
                }

                cart.Add(idproduct.Value);

                await this.serviceCart.SetCartAsync(cacheKey, cart);
            }

            string previousUrl = Request.Headers["Referer"].ToString();
            return Redirect(previousUrl);
        }

        public async Task<IActionResult> Cart(int? idproduct)
        {
            string cacheKey = $"cart_{User.Identity.Name}";
            List<int> cart = await this.serviceCart.GetCartAsync(cacheKey);

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
                        await this.serviceCart.RemoveCartAsync(cacheKey);
                        ViewData["MESSAGE"] = "Actualmente no tienes ningún producto en el carrito";
                    }
                    else
                    {
                        await this.serviceCart.SetCartAsync(cacheKey, cart);
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
        public async Task<IActionResult> Checkout(int restaurantid, string totalPrice, string deliveryMethod, string deliveryAddress, string paymentMethod, [FromServices] IConfiguration configuration)
        {
            string cacheKey = $"cart_{User.Identity.Name}";
            List<int> cart = await this.serviceCart.GetCartAsync(cacheKey);
            List<Product> products = await this.service.GetProductsCartAsync(cart);

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
            
            this.telemetryClient.TrackEvent("DeliveryProjectPurchasesLogs");
            MetricTelemetry metric = new MetricTelemetry();
            metric.Name = "Purchases";
            metric.Sum = double.Parse(totalPrice);
            this.telemetryClient.TrackMetric(metric);
            string message = "";
            SeverityLevel level;
            if (metric.Sum <= 10)
            {
                level = SeverityLevel.Error;
            }
            else if (metric.Sum <= 15)
            {
                level = SeverityLevel.Critical;
            }
            else if (metric.Sum <= 20)
            {
                level = SeverityLevel.Warning;
            }
            else
            {
                level = SeverityLevel.Information;
            }
            message = purchase.UserId + " " + metric.Sum + "€";
            TraceTelemetry trace = new TraceTelemetry(message, level);
            this.telemetryClient.TrackTrace(trace);

            await this.serviceCart.RemoveCartAsync(cacheKey);
            return RedirectToAction("Success");
        }

        public async Task<IActionResult> Success()
        {
            return View();
        }
    }
}
