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

        public async Task<List<Restaurant>> GetRestaurants()
        {
            return await this.context.Restaurants.ToListAsync();
        }

        public async Task<Restaurant> FindRestaurant(int id)
        {
            return await this.context.Restaurants.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Category>> GetCategories()
        {
            return await this.context.Categories.ToListAsync();
        }
    }
}
