using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryProject.Models
{
    [Table("OpeningRestaurants")]
    public class OpeningRestaurant
    {
        private DateTime _ClosingTime;
        [Key]
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }
        [Column("WeekDay")]
        public int WeekDay { get; set; }
        [Column("OpeningTime")]
        public DateTime OpeningTime { get; set; }
        [Column("ClosingTime")]
        public DateTime ClosingTime { 
            get { return this._ClosingTime; }
            set {
                this._ClosingTime = value;
            }
        }
        [Column("TwoTimes")]
        public bool TwoTimes { get; set; }
        [Column("OpeningTime2")]
        public DateTime OpeningTime2 { get; set; }
        [Column("ClosingTime2")]
        public DateTime ClosingTime2 { get; set; }
        [NotMapped]
        public TimeOnly OnlyTime { get; set; }
    }
}
