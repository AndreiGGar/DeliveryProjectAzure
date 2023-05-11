using DeliveryProjectAzure.Context;
using DeliveryProjectAzure.Helpers;
using DeliveryProjectAzure.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProjectAzure.Repositories
{
    public class RepositoryDelivery : IRepositoryDelivery
    {
        private DataContext context;

        public RepositoryDelivery(DataContext context)
        {
            this.context = context;
        }

        #region Restaurants | Categories | Products
        public async Task<List<Restaurant>> GetRestaurantsAsync()
        {
            return await this.context.Restaurants.ToListAsync();
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(int id)
        {
            return await this.context.Restaurants.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Restaurant>> GetRestaurantBySearchAsync(string search)
        {
            return await this.context.Restaurants.Where(z => z.Name.Contains(search)).ToListAsync();
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

        public async Task<List<Product>> GetRestaurantsCategoriesProductsAsync(int idrestaurant)
        {
            /*var query = from restaurants in context.Restaurants
                        join products in context.Products on restaurants.Id equals products.RestaurantId
                        join categoriesproducts in context.CategoriesProducts on products.Category equals categoriesproducts.Id
                        where idrestaurant == restaurants.Id
                        select new ProductListViewModel { Products = new List<Product> { products }, CategoriesProducts = new List<CategoryProduct> { categoriesproducts } };*/
            var query = from restaurants in context.Restaurants
                        join products in context.Products on restaurants.Id equals products.RestaurantId
                        join categoriesproducts in context.CategoriesProducts on products.Category equals categoriesproducts.Id
                        where idrestaurant == restaurants.Id
                        select products;

            return await query.ToListAsync();
        }
        #endregion

        #region Purchases and cart
        public async Task<List<Product>> GetProductsCartAsync(List<int> ids)
        {
            var query = from products in this.context.Products
                        where ids.Contains(products.Id)
                        select products;
            return await query.ToListAsync();
        }

        public int GetMaxIdPurchase()
        {
            if (this.context.Purchases.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Purchases.Max(z => z.Id) + 1;
            }
        }

        public void InsertPurchaseProduct(int idpurchase, int idproduct, int quantity)
        {
            PurchasedProduct purchasedProduct = new PurchasedProduct();
            purchasedProduct.PurchaseId = idpurchase;
            purchasedProduct.ProductId = idproduct;
            purchasedProduct.Quantity = quantity;
            this.context.PurchasedProducts.Add(purchasedProduct);
            this.context.SaveChanges();
        }

        public void InsertPurchase(int id, int userid, int restaurantId, decimal totalprice, string status, bool delivery, DateTime requestdate, string? deliveryaddress, string deliverymethod, string? code, string products, string paymentMethod)
        {
            Purchase purchase = new Purchase();
            purchase.Id = id;
            purchase.UserId = userid;
            purchase.RestaurantId = restaurantId;
            purchase.TotalPrice = totalprice;
            purchase.Status = status;
            purchase.Delivery = delivery;
            purchase.RequestDate = requestdate;
            if (deliveryaddress != null)
            {
                purchase.DeliveryAddress = deliveryaddress;
            };
            purchase.DeliveryMethod = deliverymethod;
            if (code != null)
            {
                purchase.Code = code;
            }
            purchase.Products = products;
            if (paymentMethod != null)
            {
                purchase.PaymentMethod = paymentMethod;
            }
            this.context.Purchases.Add(purchase);
            this.context.SaveChanges();
        }
        #endregion

        #region Users
        public int GetMaxIdUser()
        {
            if (this.context.Users.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Users.Max(x => x.Id) + 1;
            }
        }

        public async Task RegisterUser(string email, string name, string password, string rol, DateTime dateAdd, string image)
        {
            User user = new User();
            user.Id = this.GetMaxIdUser();
            user.Email = email;
            user.Name = name;
            user.Password = password;
            user.Salt = HelperCryptography.GenerateSalt();
            user.EncryptedPassword = HelperCryptography.EncryptPassword(password, user.Salt);
            user.Rol = rol;
            user.DateAdd = dateAdd;
            user.Image = image;

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();
        }

        public User LoginUser(string email, string encryptedPassword)

        {
            User user = this.context.Users.FirstOrDefault(z => z.Email == email);

            if (user == null)
            {
                return null;
            }
            else
            {
                byte[] passUser = user.EncryptedPassword;
                string salt = user.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(encryptedPassword, salt);
                bool response = HelperCryptography.CompareArrays(passUser, temp);

                if (response == true)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<User> FindUserAsync(string email, string password)
        {
            var query = from data in this.context.Users
                        where data.Email == email && data.Password == password
                        select data;

            return query.FirstOrDefault();
        }

        public async Task<User> UserProfileAsync(int iduser)
        {
            return this.context.Users.FirstOrDefault(z => z.Id == iduser);
        }
        public async Task<List<Purchase>> GetPurchasesByUserIdAsync(int userId)
        {
            var purchases = await this.context.Purchases
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return purchases;
        }
        #endregion

        #region Wishlist
        public int GetMaxIdWishlist()
        {
            if (this.context.Wishlist.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Wishlist.Max(x => x.Id) + 1;
            }
        }

        public async Task<List<Restaurant>> GetRestaurantsWithWishlistAsync(int userId)
        {
            var wishlist = await this.context.Wishlist
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var restaurantIds = wishlist.Select(w => w.RestaurantId).ToList();

            var restaurants = await this.context.Restaurants
                .Where(r => restaurantIds.Contains(r.Id))
                .ToListAsync();

            return restaurants;
        }

        public async Task<bool> RestaurantExistsInWishlist(int iduser, int idrestaurant)
        {
            Wishlist wishlist = await this.context.Wishlist.FirstOrDefaultAsync(w => w.UserId == iduser && w.RestaurantId == idrestaurant);
            return wishlist != null;
        }

        public async Task AddToWishlist(int iduser, int idrestaurant, string dateAdd)
        {
            Wishlist wishlist = new Wishlist();
            wishlist.Id = GetMaxIdWishlist();
            wishlist.UserId = iduser;
            wishlist.RestaurantId = idrestaurant;
            wishlist.DateAdd = dateAdd;

            this.context.Wishlist.Add(wishlist);
            await this.context.SaveChangesAsync();
        }

        public async Task<Wishlist> GetWishlistItem(int iduser, int idrestaurant)
        {
            return await context.Wishlist.FirstOrDefaultAsync(w => w.UserId == iduser && w.RestaurantId == idrestaurant);
        }

        public async Task DeleteFromWishlist(int iduser, int idrestaurant)
        {
            var wishlistItem = await GetWishlistItem(iduser, idrestaurant);

            if (wishlistItem != null)
            {
                context.Wishlist.Remove(wishlistItem);
                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
