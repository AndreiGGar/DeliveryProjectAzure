using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryProject.Models
{
    [Table("Wishlist")]
    public class Wishlist
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("UserId")]
        public int UserId { get; set; }
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }
        [Column("DateAdd")]
        public string DateAdd { get; set; }
    }
}