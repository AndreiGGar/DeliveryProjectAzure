using DeliveryProject.Context;
using DeliveryProject.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProject.Repositories
{
    public class RepositoryDelivery : IRepositoryDelivery
    {
        private DataContext context;

        public RepositoryDelivery(DataContext context)
        {
            this.context = context;
        }

        public async Task<List<Restaurant>> GetRestaurantsAsync()
        {
            return await this.context.Restaurants.ToListAsync();
        }

        public async Task<Restaurant> FindRestaurantAsync(int id)
        {
            return await this.context.Restaurants.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await this.context.Categories.ToListAsync();
        }

        public async Task<List<Restaurant>> GetRestaurantsByCategoryAsync(int? idcategory)
        {
            if (idcategory == 0 || idcategory == null)
            {
                return await context.Restaurants.ToListAsync();
            }
            else
            {
                var query = from restaurants in context.Restaurants
                            join categoriesrestaurants in context.CategoriesRestaurants on restaurants.Id equals categoriesrestaurants.RestaurantId
                            join categories in context.Categories on categoriesrestaurants.CategoryId equals categories.Id
                            where categories.Id == idcategory
                            select restaurants;
                return await query.ToListAsync();
            }
        }

        public async Task<List<CategoryProduct>> GetRestaurantsCategoriesAsync(int idrestaurant)
        {
            var query = from restaurants in context.Restaurants
                        join categoriesproducts in context.CategoriesProducts on restaurants.Id equals categoriesproducts.RestaurantId
                        where idrestaurant == restaurants.Id
                        select categoriesproducts;

            return await query.ToListAsync();
        }

        public async Task<List<ProductListViewModel>> GetRestaurantsCategoriesProductsAsync(int idrestaurant)
        {
            var query = from restaurants in context.Restaurants
                        join products in context.Products on restaurants.Id equals products.RestaurantId
                        join categoriesproducts in context.CategoriesProducts on products.Category equals categoriesproducts.Id
                        where idrestaurant == restaurants.Id
                        select new ProductListViewModel { Products = new List<Product> { products }, CategoriesProducts = new List<CategoryProduct> { categoriesproducts } };

            return await query.ToListAsync();
        }
    }
}
