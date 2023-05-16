using Azure.Security.KeyVault.Secrets;
using DeliveryProjectAzure.Filters;
using DeliveryProjectNuget.Helpers;
using DeliveryProjectNuget.Models;
using System.Configuration;
using System.Net.Http.Headers;

namespace DeliveryProjectAzure.Services
{
    public class ServiceApiDelivery
    {
        private HelperCallApi api;

        public ServiceApiDelivery(HelperCallApi api, IConfiguration configuration, SecretClient secretClient)
        {
            this.api = api;
            /*this.api.Uri = configuration.GetValue<string>("KeyVault:VaultUri");*/
            KeyVaultSecret keyVaultSecret = secretClient.GetSecretAsync("DeliveryApi").Result.Value;
            this.api.Uri = keyVaultSecret.Value;
            this.api.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        #region Auth

        public async Task<string> LoginUserAsync(string username, string password)
        {
            LoginModel model = new LoginModel { UserName = username, Password = password };
            return await this.api.PostApiAsync<string>("/api/Auth/Login", model, "response");
        }

        public async Task<string> RegisterUserAsync(string email, string name, string password, string rol, DateTime dateAdd, string image)
        {
            RegisterModel model = new RegisterModel { Email = email, Name = name, Password = password, Rol = rol, dateAdd = dateAdd, Image = image };
            return await this.api.PostApiAsync<string>("/api/Auth/Register", model);
        }

        public async Task<User> FindUserAsync(string username)
        {
            return await this.api.GetApiAsync<User>("/api/Auth/FindUser/" + username);
        }

        #endregion

        #region No Token

        public async Task<List<Restaurant>> GetRestaurantsAsync()
        {
            return await this.api.GetApiAsync<List<Restaurant>>("/api/NoToken/Restaurants");
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(int id)
        {
            return await this.api.GetApiAsync<Restaurant>("/api/NoToken/RestaurantById/" + id);
        }

        public async Task<List<Restaurant>> GetRestaurantBySearchAsync(string search)
        {
            return await this.api.GetApiAsync<List<Restaurant>>("/api/NoToken/RestaurantsSearch/" + search);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await this.api.GetApiAsync<List<Category>>("/api/NoToken/Categories");
        }

        public async Task<List<Restaurant>> GetRestaurantsByCategoryAsync(int idcategory)
        {
            return await this.api.GetApiAsync<List<Restaurant>>("/api/NoToken/RestaurantsByCategory/" + idcategory);
        }

        public async Task<List<CategoryProduct>> GetRestaurantsCategoriesAsync(int idrestaurant)
        {
            return await this.api.GetApiAsync<List<CategoryProduct>>("/api/NoToken/RestaurantsCategories/" + idrestaurant);
        }

        public async Task<List<Product>> GetRestaurantsCategoriesProductsAsync(int idrestaurant)
        {
            return await this.api.GetApiAsync<List<Product>>("/api/NoToken/RestaurantsCategoriesProducts/" + idrestaurant);
        }
        public async Task<List<Product>> GetProductsCartAsync(List<int> ids)
        {
            List<Product> products = await this.api.GetApiAsync<List<Product>>("/api/NoToken/Products");
            var query = products.Where(product => ids.Contains(product.Id));
            return query.ToList();
        }

        #endregion

        #region Token

        public async Task<string> InsertPurchaseAsync(string token, int userid, int restaurantId, decimal totalprice, string status, bool delivery, string requestdate, string? deliveryaddress, string deliverymethod, string? code, string products, string paymentMethod)
        {
            InsertPurchaseModel model = new InsertPurchaseModel { UserId = userid, RestaurantId = restaurantId, TotalPrice = totalprice, Status = status, Delivery = delivery, RequestDate = requestdate, DeliveryAddress = deliveryaddress, DeliveryMethod = deliverymethod, Code = code, Products = products, PaymentMethod = paymentMethod };
            return await this.api.PostApiTokenAsync<string>("/api/Token/InsertPurchase", model, token);
        }

        public async Task<List<Restaurant>> GetRestaurantsWithWishlistAsync(string token)
        {
            List<Restaurant> restaurants = await this.api.GetApiTokenAsync<List<Restaurant>>("/api/Token/RestaurantsWishlist", token);
            return restaurants;
        }

        public async Task<Boolean> RestaurantExistsInWishlist(string token, int idrestaurant)
        {
            Boolean request = await this.api.GetApiTokenAsync<Boolean>("/api/Token/RestaurantsInWishlist/" + idrestaurant, token);
            return request;
        }

        public async Task<string> AddToWishlistAsync(string token, int restaurantId, string dateAdd)
        {
            return await this.api.PostApiTokenAsync<string>("/api/Token/AddToWishlist/" + restaurantId + "/" + dateAdd, token);
        }

        public async Task<string> DeleteFromWishlistAsync(string token, int restaurantId)
        {
            return await this.api.PostApiTokenAsync<string>("/api/Token/DeleteFromWishlist/" + restaurantId, token);
        }

        public async Task<User> UserProfileAsync(string token)
        {
            User user = await this.api.GetApiTokenAsync<User>("/api/Token/UserProfile", token);
            return user;
        }

        public async Task<List<Purchase>> GetPurchasesByUserIdAsync(string token)
        {
            List<Purchase> purchases = await this.api.GetApiTokenAsync<List<Purchase>>("/api/Token/PurchasesByUser", token);
            return purchases;
        }

        #endregion
    }
}
