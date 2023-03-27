using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryProject.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Column("EncryptedPassword")]
        public byte[] EncryptedPassword { get; set; }
        [Column("Salt")]
        public string Salt { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Address")]
        public string? Address { get; set; }
        [Column("Rol")]
        public string Rol { get; set; }
        [Column("Image")]
        public string? Image { get; set; }
        [Column("DateAdd")]
        public DateTime DateAdd { get; set; }
        [Column("DateEdit")]
        public DateTime? DateEdit { get; set; }
    }
}
