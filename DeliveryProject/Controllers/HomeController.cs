﻿using DeliveryProject.Models;
using DeliveryProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeliveryProject.Controllers
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

        public async Task<IActionResult> _PaginationRestaurants(int? pageNumber, int? pageSize, int? category)
        {
            pageNumber = pageNumber ?? 1;
            pageSize = pageSize ?? 3;
            category = category ?? 0;

            var restaurants = await this.repo.GetRestaurantsByCategoryAsync(category.Value);

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