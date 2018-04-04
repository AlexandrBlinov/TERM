using System;
using System.ComponentModel.DataAnnotations;

namespace Term.Web.Models
{
    /// <summary>
    /// Водитель
    /// </summary>
    public class Subscriber
    {
        [Key]
        public long SubscriberId { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [MaxLength(150)]
        public string FIO { get; set; }

        [MaxLength(150)]
        public string CarModel { get; set; }

        [MaxLength(50)]
        public string CarNumber { get; set; }

    }

    /// <summary>
    /// Запись о координатах
    /// </summary>
    public class LocationsRecord
    {
        [Key]
        public long Id { get; set; }

        public long SubscriberId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime LocationDate { get; set; }

        public DateTime RequestDate { get; set; }
        
    }
}