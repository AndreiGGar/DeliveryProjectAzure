namespace DeliveryProject.Models
{
    public class RestaurantListViewModel
    {
        public List<Restaurant> Restaurants { get; set; }
        public PaginationRestaurants PaginationRestaurants { get; set; }
        public int SelectedCategory { get; set; }
    }
}
