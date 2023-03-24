using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryProject.Models
{
    [Table("CategoriesRestaurants")]
    public class CategoryRestaurant
    {
        [Key]
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }
        [Column("CategoryId")]
        public int CategoryId { get; set; }
    }
}
