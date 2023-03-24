using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DeliveryProject.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Description")]
        public string? Description { get; set; }
        [Column("Price")]
        public decimal Price { get; set; }
        [Column("DiscountPrice")]
        public decimal? DiscountPrice { get; set; }
        [Column("IsDiscounted")]
        public bool IsDiscounted { get; set; }
        [Column("IsActive")]
        public bool IsActive { get; set; }
        [Column("IsAvailable")]
        public bool IsAvailable { get; set; }
        [Column("Category")]
        public int Category { get; set; }
        [Column("DateAdd")]
        public DateTime DateAdd { get; set; }
        [Column("DateEdit")]
        public DateTime? DateEdit { get; set; }
        [Column("CreatedBy")]
        public int CreatedBy { get; set; }
        [Column("EditedBy")]
        public int? EditedBy { get; set; }
        [Column("Image")]
        public string? Image { get; set; }
    }
}
