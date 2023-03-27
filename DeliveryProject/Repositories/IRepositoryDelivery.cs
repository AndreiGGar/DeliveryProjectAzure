using DeliveryProject.Models;

namespace DeliveryProject.Repositories
{
    public interface IRepositoryDelivery
    {
        Task<List<Restaurant>> GetRestaurantsAsync();
        Task<Restaurant> GetRestaurantByIdAsync(int id);
        Task<List<Restaurant>> GetRestaurantBySearchAsync(string search);
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Restaurant>> GetRestaurantsByCategoryAsync(int? idcategory);
        Task<List<CategoryProduct>> GetRestaurantsCategoriesAsync(int idrestaurant);
        Task<List<Product>> GetRestaurantsCategoriesProductsAsync(int idrestaurant);
        Task<List<Product>> GetProductsCartAsync(List<int> ids);
        int GetMaxIdPurchase();
        void InsertPurchaseProduct(int idpurchase, int idproduct, int quantity);
        void InsertPurchase(int id, int userid, int restaurantId, decimal totalprice, string status, bool delivery, DateTime requestdate, string? deliveryaddress, string deliverymethod, string? code, string products, string paymentMethod);
        int GetMaxIdUser();
        Task RegisterUser(string email, string name, string password, string rol, DateTime dateAdd, string image);
        User LoginUser(string email, string encryptedPassword);
        Task<User> FindUserAsync(string email, string password);
        Task<User> UserProfileAsync(int iduser);
        Task<List<Purchase>> GetPurchasesByUserIdAsync(int userId);
        int GetMaxIdWishlist();
        Task<List<Restaurant>> GetRestaurantsWithWishlistAsync(int userId);
        Task<bool> RestaurantExistsInWishlist(int iduser, int idrestaurant);
        Task AddToWishlist(int iduser, int idrestaurant, string dateAdd);
        Task<Wishlist> GetWishlistItem(int iduser, int idrestaurant);
        Task DeleteFromWishlist(int iduser, int idrestaurant);

    }
}
