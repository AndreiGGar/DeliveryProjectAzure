using DeliveryProject.Context;
using DeliveryProject.Models;

namespace DeliveryProject.Repositories
{
    public class RepositoryDelivery : IRepositoryDelivery
    {
        private DataContext context;

        public RepositoryDelivery(DataContext context)
        {
            this.context = context;
        }

        public List<Restaurant> GetRestaurants()
        {
            return this.context.Restaurants.ToList();
        }

        public Restaurant FindRestaurant(int id)
        {
            return this.context.Restaurants.FirstOrDefault(x => x.Id == id);
        }
    }
}
