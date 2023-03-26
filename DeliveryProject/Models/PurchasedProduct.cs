using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryProject.Models
{
    [Table("PurchasedProducts")]
    public class PurchasedProduct
    {
        [Key]
        [Column("PurchaseId")]
        public int PurchaseId { get; set; }
        [Column("ProductId")]
        public int ProductId { get; set; }
        [Column("Quantity")]
        public int Quantity { get; set; }
    }
}
