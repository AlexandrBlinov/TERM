using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Term.CustomAttributes;
using Term.DAL;
using Term.DAL.Resources;

namespace Term.DAL
{
    /// <summary>
    /// Статусы доставки ДПД
    /// </summary>
   public  enum DpdDeliveryStatus
    {
        [MultiCultureDescription(typeof(OrderStatusesTexts), "OnTerminalPickup")]
       OnTerminalPickup,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "OnRoad")]
       OnRoad,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "OnTerminal")]
       OnTerminal,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "Problem")]
       Problem,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "OnTerminalDelivery")]
       OnTerminalDelivery,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "Delivering")]
        Delivering,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "Delivered")]
        Delivered,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "ReturnedFromDelivery")]
        ReturnedFromDelivery,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "Lost")]
        Lost,
       [MultiCultureDescription(typeof(OrderStatusesTexts), "NewOrderByClient")]
        NewOrderByClient,

       [MultiCultureDescription(typeof(OrderStatusesTexts), "NewOrderByDpd")]
        NewOrderByDpd
    }
     
    /// <summary>
    /// Сроки доставки от подразделений до городов (если везем через DPD)
    /// </summary>
    /// 
    [Table("TimesOfDelivery")]
    public class TimeOfDelivery
    {
        // подразделение откуда везем
         [Key, Column(Order = 1)]
        public int DepartmentId { get; set; }

        // город куда везем
         [Key, Column(Order = 2)]
        public string CityId { get; set; }

        // занимает дней (от)
        public int DaysFrom { get; set; }

        // занимает дней (по)
        public int DaysTo { get; set; }

    }


    /// <summary>
    /// Терминалы DPD
    /// </summary>
    public class DpdTerminal
    {
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(4)]
        public string TerminalCode { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string TerminalName { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string CountryCode { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string RegionCode { get; set; }
        
        [MaxLength(Byte.MaxValue)]
        public string CityCode { get; set; }
        
        [MaxLength(Byte.MaxValue)]
        public string CityName { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string Index { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string Street { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string HouseNo { get; set; }

        [MaxLength(10)]
        public string StreetAbbr { get; set; }

        [MaxLength(10)]
        public string Structure { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string Description { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string Schedule { get; set; }

        public override string ToString()
        {
            return String.Format("{0}, {1} {2} {3}", CityName, StreetAbbr, Street, HouseNo);
        }


    }

    /// <summary>
    /// Области 
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Код области 76 - Ярославская
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public  int Id{ get; set; }

        [Required]
        [MaxLength(Byte.MaxValue)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Города
    /// </summary>
     [DataContract(Namespace = "", Name = "city")]
    public class City
    {
        /// <summary>
        /// КЛАДР города
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(11)]
         [DataMember]
        public string Id { get; set; }

        [MaxLength(11)]
        [Required]
        [Index]
        [DataMember]
        public string DpdCode { get; set; }

        /// <summary>
        /// Полное название города
        /// </summary>
        [Required]
        [MaxLength(Byte.MaxValue)]
        [DataMember]
        public string Name { get; set; }

        
        [MaxLength(Byte.MaxValue)]
        [DataMember]
        public string Abbreviation { get; set; }
        
        [ForeignKey("Region")]
        public int RegionId { get; set; }

        public virtual Region Region { get; set; }




    }

   
    /// <summary>
    /// Тарифы доставки до основных городов
    /// </summary>
    public class RateToMainCity
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(11)]
        [ForeignKey("City")]
        
        public string CityId { get; set; }
        public virtual City City { get; set; }


        /// <summary>
        ///  Тарифы Ярославль, Мск, Спб, РНд
        /// </summary>
        public int Rate20 { get; set; }
        public int Rate40 { get; set; }
        public int RatePlus1 { get; set; }

        public int Rate20Door { get; set; }
        public int Rate40Door { get; set; }
        public int RatePlus1Door { get; set; }


        /// <summary>
        ///  Тарифы Екатаринбург
        /// </summary>
        public int Rate20_Ekb { get; set; }
        public int Rate40_Ekb { get; set; }
        public int RatePlus1_Ekb { get; set; }

        public int Rate20Door_Ekb { get; set; }
        public int Rate40Door_Ekb { get; set; }
        public int RatePlus1Door_Ekb { get; set; }
    }


    /// <summary>
    /// Тарифы доставки до дополнительных городов
    /// </summary>
    public class RateToAdditionalCity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(11)]
        [ForeignKey("City")]
        public string CityId { get; set; }
        public virtual City City { get; set; }


        [MaxLength(11)]
        [ForeignKey("MainCity")]
        public string MainCityId { get; set; }
        public virtual City MainCity { get; set; }

        public int Rate { get; set; }
        
    }

    /// <summary>
    /// Транспортные компании
    /// </summary>
    /// 
    public class TransportCompany {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(11)]        
        public string Id { get; set; }

        
        [MaxLength(Byte.MaxValue)]        
        public string Name { get; set; }
        

    }
}
