using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Term.CustomAttributes;
using System.Linq;
using System.Configuration;
using Term.DAL.Resources;
using System.Runtime.Serialization;
using System.Xml.Serialization;

//using System.ComponentModel.DataAnnotations.Schema;


namespace Term.DAL
{
    public enum ProductType
    {
        Other = 0,
        Tyre = 1,
        Disk = 2,
        Akb = 3,
        Acc = 4,
        CargoDisk = 5,
    }

    public enum WheelType
    {
        Alloy = 0,
        Steel,
        Forged
    }

    

    /// 
    /// <summary>
    /// class to store Managers entities
    /// </summary>
    public class Manager
    {
         [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid GuidIn1S { get; set; }

        [MaxLength(Byte.MaxValue)]
        [Display(Name = "FIO", ResourceType = typeof(ResourceManagerInfo))]
        public string Fio { get; set; }

        [MaxLength(Byte.MaxValue)]
        [Display(Name = "Email", ResourceType = typeof(ResourceManagerInfo))]
        public string EMail { get; set; }

        [MaxLength(Byte.MaxValue)]
        [Display(Name = "MobilePhoneNumber", ResourceType = typeof(ResourceManagerInfo))]
        public string MobilePhoneNumber { get; set; }

        [MaxLength(Byte.MaxValue)]
        [Display(Name = "InternalPhoneNumber", ResourceType = typeof(ResourceManagerInfo))]
        public string InternalPhoneNumber { get; set; }

        [MaxLength(100)]
        [Display(Name = "Skype", ResourceType = typeof(ResourceManagerInfo))]
        public string Skype { get; set; }

        [MaxLength(100)]
        [Display(Name = "ICQ", ResourceType = typeof(ResourceManagerInfo))]
        public string ICQ { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string PersonalPhoto { get; set; }


    }


    /// <summary>
    /// class to store managers of partners
    /// </summary>
    public class ManagersOfPartner {
        [Key, Column(Order = 1)]
        [ForeignKey("Partner")]
        public string  PartnerId { get; set; }

        [Key, Column(Order = 2)]
        [ForeignKey("Manager")]
        public Guid ManagerId { get; set; }

        public Partner Partner { get; set; }
        
        public Manager Manager { get; set; }
    }

    /// <summary>
    /// class used to store collection of primitive serialized objects like constants
    /// </summary>
    public class StoredKeyValueItem {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(50)]
     public string  Key { get; set; }

    [MaxLength(10)]
    public string Factory { get; set; }

    [MaxLength(Int16.MaxValue)]
    public string Value { get; set; }

    
    }

    

    /// <summary>
    /// Цвета для 
    /// </summary>
    public class DiskColour
    {
        /*[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } */
        [Key]
        [MaxLength(50)]
        public string ColourName { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string DescriptionEn { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string DescriptionRu { get; set; }
    }

    /// <summary>
    /// Языки интерфейса
    /// </summary>
    public class Languages
    {
        [Key]
        [MaxLength(50)]
        public string LanguageName { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string LanguageDescription { get; set; }

    }

# region Product
    // Таблица Производители
    public class Producer 
    {

        //public string ProducerUId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProducerId { get; set; }

        //Name. The Name of the producer
        [DisplayName("Производитель")]
        [Required(ErrorMessage = "Требуется ввести имя производителя")]
        [StringLength(100)]

        public string Name { get; set; }

        // tyre , disk , acc , akb, other
    /*    [Index]
        [StringLength(20)]
        public string ProductType { get; set; } */
        [DefaultValue(ProductType.Other)]
         public ProductType ProductType { get; set; }

        [DisplayName("Активность")]
        public bool Active { get; set; }


        //Description.
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public override string ToString()
        {
            return ProducerId.ToString().PadLeft(5, '0');
        }
       
    }

    // Таблица Модели
    public class Model 
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ModelId { get; set; }

        public int? ProducerId { get; set; }

        //public virtual Producer Producer { get; set; }

        [DisplayName("Модель")]
        public string Name { get; set; }

        [Index("IX_Season")]
        [DisplayName("Сезон")]
        [StringLength(20)]
        public string Season { get; set; }

        /* public string PictureUrl { get; set; } */

        [DisplayName("Описание модели")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public override string ToString()
        {
            return ModelId.ToString().PadLeft(5, '0');
        }
    }


    public class Department 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DepartmentId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public override string ToString()
        {
            return DepartmentId.ToString().PadLeft(5, '0');
        }

    }
    // типоразмеры

    public class Tiporazmer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TiporazmerId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Diametr { get; set; }

        [StringLength(10)]
        public string Width { get; set; }

        [StringLength(10)]
        public string Height { get; set; }

        [StringLength(10)]
        public string PCD { get; set; }

        [StringLength(10)]
        public string ET { get; set; }

        [DisplayName("DIA")]
        [StringLength(10)]
        public string DIA { get; set; }

        // [DisplayName("Число отверстий")]
        [StringLength(10)]
        public string Holes { get; set; }

        [StringLength(20)]
        public string ProductType { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }

    // Таблица Товары
    public class Product 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName(@"Идентификатор товара")]
        public int ProductId { get; set; }

        [StringLength(100)]
        //[DisplayName("Наименование товара")]
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Index]
        public string Name { get; set; }

        //[DisplayName("Вид товара")]
        [Display(Name = "TypeProduct", ResourceType = typeof(Resource))]
        /*[StringLength(20)]
        [Index]
        public string ProductType { get; set; } */

        [Index]
        public ProductType ProductType { get; set; }

        //[DisplayName("Производитель")]
        [Display(Name = "Brand", ResourceType = typeof(Resource))]
        [Index]
        public int? ProducerId { get; set; }

        //[DisplayName("Производитель")]
        [Display(Name = "Brand", ResourceType = typeof(Resource))]
        public virtual Producer Producer { get; set; }

        //[DisplayName("Модель")]
        [Display(Name = "Model", ResourceType = typeof(Resource))]
        public int? ModelId { get; set; }

        public virtual Model Model { get; set; }


        [StringLength(25)]
        //[DisplayName("Артикул")]
        [Display(Name = "ManufacturerCode", ResourceType = typeof(Resource))]
        public string Article { get; set; }

        [StringLength(50)]
        //[DisplayName("Артикул")]
        [Display(Name = "ManufacturerCountry", ResourceType = typeof(Resource))]
        public string CountryProducer { get; set; }

        //[DisplayName("Типоразмер")]
        [Display(Name = "Size", ResourceType = typeof(Resource))]
        public int? TiporazmerId { get; set; }

        public virtual Tiporazmer Tiporazmer { get; set; }

        [DisplayName("Цена")]
        public decimal? Price { get; set; }

        public bool IsFolder { get; set; }

        public int? ParentId { get; set; }

        [DefaultValue(0)]
        public decimal Weight { get; set; }

        [DefaultValue(0)]
        public decimal Volume { get; set; }

        [DefaultValue(0)]
        public WheelType WheelType { get; set; }

        [MaxLength(10)]
        public string Factory { get; set; }

        public override string ToString()
        {
            return ProductId.ToString().PadLeft(7, '0');
        }



        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProductTypeToView
        {
            get
            {
                return this.ProductType.ToString();

            }
        }

         [MaxLength(Byte.MaxValue)]
        public string PathToRemotePicture { get; set; }

        // Штрихкоды EAN-13 для 
        [MaxLength(13)]
         public string BarCode { get; set; }
    }

    public class ProductForSale
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }
    }


    /// <summary>
    /// Всевозможные свойства товаров
    /// </summary>
    public class ProductPropertyDescription
    {
        [MaxLength(50)]
        [Key]
        public string Name { get; set; }

        [MaxLength(50)]
        public string NameToRus { get; set; }
        public virtual ProductType? ProductType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual string NameToDisplay {get {
               return  ProductPropertiesTexts.ResourceManager.GetString(Name)?? NameToRus;
            }

}
    }

    /// <summary>
    /// Значения свойств товаров 
    /// </summary>
    public class ProductPropertyValue
    {
        [DisplayName("Идентификатор товара")]
        [Key, Column(Order = 1)]
        public int ProductId { get; set; }

        [ForeignKey("ProductPropertyDescription")]
        [MaxLength(50)]
        [Key, Column(Order = 2)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Value { get; set; }

        
        public virtual ProductPropertyDescription ProductPropertyDescription { get; set; }


        /*
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string NameToRus
        {
            get
            {
                string strValue;
                string notFoundProductType = "прочее";
                var typesDict = new Dictionary<string, string>
                {
                    {"brand", Resource.BrandAkb},
                    {"inrush_current", Resource.CrankCurrent},
                    {"polarity", Resource.Polarity},
                    {"volume", Resource.Capacity},
                    {"sizes", Resource.Dimensions},
                    {"fixing", Resource.Fixing},
                    {"cap", Resource.Cap},
                    {"maxload", Resource.MaxLoad},
                };
                return typesDict.TryGetValue(Name, out strValue) ? strValue : notFoundProductType;

            }
        } */
    }


#endregion Product
    /// <summary>
    /// Цены товаров
    /// </summary>
    public class PriceOfProduct
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Идентификатор товара")]
        public int ProductId { get; set; }


        [DisplayName("Закупочная цена")]
        public decimal? Price { get; set; }

        [DisplayName("Рекоммендованная цена опт")]
        public decimal? Price1 { get; set; }

        [DisplayName("Рекоммендованная цена розница")]
        public decimal? Price2 { get; set; }

        [DisplayName("Базовая цена")]
        public decimal? PriceBase { get; set; }

        [DisplayName("Крупный опт1")]
        public decimal? PriceOpt1 { get; set; }

    }


    #region Partner
    // Таблица Партнеры
    [DataContract(Namespace = "")]
    public class Partner
    {
        private const string ru_RU = "ru-RU";

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(7)]
        [DataMember(Order = 1)]
        public string PartnerId { get; set; }

         [DataMember(Order = 2)]
        public string Name { get; set; }
        
        [DataMember(Order = 3)]
        public string FullName { get; set; }

        [DisplayName("ФИО")]
        [StringLength(100)]
        [DataMember(Order = 4)]
        public string ContactFIO { get; set; }

        [DataMember(Order = 5)]
        public string Address { get; set; }

        [DataMember(Order = 6)]
        public string PhoneNumber { get; set; }

         [DataMember(Order = 7)]
        public string INN { get; set; }

         [DataMember(Order = 8)]
        public string KPP { get; set; }

        [StringLength(10)]
        [DataMember(Order = 9)]
        public string Culture { get; set; }

        [DataMember(Order = 10)]
        [DefaultValue(0)]
        
        public int CustomDutyVal { get; set; }

        [DataMember(Order = 11)]
        [DefaultValue(0)]
        public int VatVal { get; set; }

     //   public string Sale { get; set; }

       
        // Показывать распродажи в дисках
        [DefaultValue(false)]
        public bool IsSale { get; set; }

        [NotMapped]
        public bool IsForeign { get {
            return (Culture != null && Culture != ru_RU);
        } }

        [ForeignKey("MainManager")]
        public Guid? ManagerId { get; set; }
        
        /// <summary>
        ///  Основной менеджер контрагента
        /// </summary>
        public virtual Manager MainManager { get; set; }

        /// <summary>
        /// Признак, есть ли транспортный договор с DPD
        /// </summary>
        [DefaultValue(false)]
        public bool HasDpdContract { get; set; }

        // Дата договора продаж
        public DateTime? ContractDate { get; set; }

        // Номер договора продаж
        [MaxLength(50)]
        public string ContractNumber { get; set; }

        // Процент наценки DPD
        public byte? DeliveryProfitPercent { get; set; }

        // Основной договор - предоплатный
        public bool PrePay { get; set; }
                
        public bool HasOwnRest { get; set; }
        
        // есть договоры *
        public bool HasStar { get; set; }

        // есть предоплатные договоры
        public bool HasPrepay { get; set; }

        // способ доставки 1- самовывоз, 0- доставка наша
        public int  WayOfDelivery { get; set; }

        // реквизит нужен для подсчета цены исходя из предоплатной цены (в настройках клиента)
        // считать цену исходя из предоплатной цены (только если PrePay=0 и HasPrepay=1)
        // данный реквизит не синхронизируется
        public bool UsePrepayPrices { get; set; }

        /// <summary>
        /// Число дней для заявок на возврат
        /// </summary>
        public int? NumberOfDaysForReturn { get; set; }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, тел:{3} ", this.INN ?? "", this.FullName ?? "", this.Address ?? "", this.PhoneNumber);

        }

        
    }

    /// <summary>
    /// Описание свойств партнера
    /// </summary>
    public class PartnerPropertyDescription
    {
        [MaxLength(50)]
        [Key]
        public string Name { get; set; }
               
    }

    /// <summary>
    /// Значение свойств партнера
    /// </summary>
    public class PartnerPropertyValue
    {

        [Key, Column(Order = 1)]
        [MaxLength(7)]
        public string PartnerId { get; set; }

        
        [ForeignKey("PartnerPropertyDescription")]
        [MaxLength(50)]
        [Key, Column(Order = 2)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Value { get; set; }


        public virtual PartnerPropertyDescription PartnerPropertyDescription { get; set; }

    }

    /// <summary>
    /// Адрес партнера который берется из 1С
    /// </summary>
    public class AddressOfPartner {
        [ForeignKey("Partner")]
        [Key, Column(Order = 1)]
        [MaxLength(7)]
        public string PartnerId { get; set; }

        
        [MaxLength(5)]
        [Key, Column(Order = 2)]
        public string AddressId { get; set; }

        [ForeignKey("PartnerPoint")]
        public int? PointId { get; set; }


        [MaxLength(Byte.MaxValue)]        
        public string Address { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual PartnerPoint PartnerPoint { get; set; }

    }

    /// <summary>
    /// Производители которых не показываем партнерам
    /// </summary>
    public class ProducerNotDisplayedFromPartner
    {
        [ForeignKey("Producer")]
        [Key, Column(Order = 1)]
        
        public int ProducerId { get; set; }


        [ForeignKey("Partner")]
        [MaxLength(7)]
        [Key, Column(Order = 2)]
        public string PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual Producer Producer { get; set; }
    }

#endregion Partner

    /// <summary>
    ///  Сторонние поставщики (внешний ключ -> Partner)
    /// </summary>

    [DataContract(Namespace = "")]
    public class Supplier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DataMember]
        public int Id { get; set; }
        
        [MaxLength(7)]
        public string PartnerId { get; set; }

        // Активен
        [DataMember]
        public bool Active { get; set; }

        // Число дней доставки
        [DataMember]
        public int Days { get; set; }

        [DataMember]
        public int DepartmentId { get; set; }
        
        // лимит товара
        public int ProductLimit { get; set; }

        [MaxLength(255)]
        [DataMember]
        public string Name { get; set; }

        public bool PrePay { get; set; }


    }

    // Таблица цен
     [DataContract(Namespace = "")]
    public class PriceOfPartner
    {
         [DataMember]
        [Key, Column(Order = 2)]
        public int ProductId { get; set; }

         [DataMember]
        [StringLength(7)]
        [Key, Column(Order = 1)]
        public string PartnerId { get; set; }

         [DataMember]
        public decimal Price { get; set; }


        public decimal Price2 { get; set; }




    }



    /// <summary>
    /// Общие остатки собственных товаров
    /// </summary>
    public class RestOfProduct
    {

        [Key, Column(Order = 1)]
        public int ProductId { get; set; }

        [Key, Column(Order = 2)]
        public int DepartmentId { get; set; }

        public int Rest { get; set; }

        [StringLength(7)]
        public string SupplierId { get; set; }

    }

   
    /// <summary>
    /// Остатки товаров поставщиков
    /// </summary>
    /// 

    [DataContract(Namespace = "",IsReference = false)]
    public class RestOfSupplier
    {
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Supplier")]
        [DataMember]
        public int SupplierId { get; set; }
        
        [IgnoreDataMember]
        public virtual Supplier Supplier { get; set; }

        [DataMember]
        [Key, Column(Order = 2)]
        public int ProductId { get; set; }

        [DataMember]
        public int Rest { get; set; }
        
    }



    /// <summary>
    /// Персональные резервы товаров покупателей 
    /// (добавляются к общим остаткам собств. товаров)
    /// </summary>
    public class RestOfPartner
    {
        [DataMember]
        [Key, Column(Order = 2)]
        public int ProductId { get; set; }

        [DataMember]
        [StringLength(7)]
        [Key, Column(Order = 1)]
        public string PartnerId { get; set; }

        [DataMember]
        public int Rest { get; set; }

    }



    /// <summary>
    /// Корзина для партнера и клиента
    /// </summary>
    public class Cart
    {
        [Key]
        public int RecordId { get; set; }

        [Index]
        [StringLength(50)]
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }

        public int DepartmentId { get; set; }
        public int DaysToDepartment { get; set; }
        public decimal Price { get; set; }

        [DefaultValue(0)]
        public decimal PriceOfPoint { get; set; }

        [DefaultValue(0)]
        public decimal PriceOfClient { get; set; }
        public System.DateTime DateCreated { get; set; }
        public virtual Product Product { get; set; }

        [NotMapped]
        public string DepartmentName { get {return DepartmentId.ToString().PadLeft(5, '0');} }

        public int SupplierId { get; set; }

        public bool PriceIsPrepay { get; set; }

        //  { Removed from foregn key control by Lapenkov 2016-06-23
        //   public virtual Department Department { get; set; }
        // }

    }


   


    // статусы заказов
    public enum OrderStatuses
    {
        //[EnumDescription("New")]
        // Новый (не проведен)
         [MultiCultureDescription(typeof(OrderStatusesTexts), "New")]
        New = 1,

        //[EnumDescription("Confirmed")]
         // Подтвержден 
        [MultiCultureDescription(typeof(OrderStatusesTexts), "Confirmed")]
        Confirmed = 2,

        //[EnumDescription("Cancelled")]
        // Отменен 
        [MultiCultureDescription(typeof(OrderStatusesTexts), "Cancelled")]
        Chancelled = 3,
        
        //[EnumDescription("ShippedCompletely")]
        // Отгружен
        [MultiCultureDescription(typeof(OrderStatusesTexts), "ShippedCompletely")]
        ShippedCompletely = 4,

        //[EnumDescription("ShippedPartially")]
        // Отгружен частично
        [MultiCultureDescription(typeof(OrderStatusesTexts), "PartiallySold")]
        PartiallySold = 5,

        //[EnumDescription("DeliveredToTheBranch")]
        // Доставлен в филиал
        [MultiCultureDescription(typeof(OrderStatusesTexts), "DeliveredToTheBranch")]

        DeliveredToTheBranch = 6,

         [MultiCultureDescription(typeof(OrderStatusesTexts), "OnTheWay")]
        //[EnumDescription("OnTheWay")]
        // В пути в филиал
        OnTheWay = 7,

        //[EnumDescription("Loading")]
         [MultiCultureDescription(typeof(OrderStatusesTexts), "Loading")]
         Loading = 8,

        //        [EnumDescription("PartiallyLoaded")]
         [MultiCultureDescription(typeof(OrderStatusesTexts), "PartiallyLoaded")]
         PartiallyLoaded = 9,


        /*
         * C 10 по 14 - на отгрузке
         */
        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingPreparedForShipment")]
        OnShipmentPartially2 = 10,

        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingPreparedForShipment")]
        OnShipmentPartially3 = 11,
        
        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingPreparedForShipment")]
        OnShipmentPartially4 = 12,

        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingPreparedForShipment")]
        OnShipmentPartially5 = 13,

        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingPreparedForShipment")]
        OnShipmentPartially6 = 14,


        [MultiCultureDescription(typeof(OrderStatusesTexts), "Cancelled")]
        OnShipmentCancelled2 = 15,


        // на подтверждении у поставщика - пишем статус = в обработке 
        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingConfirmedBySupplier")]
        //  [EnumDescription("BeingPreparedForShipment")]
        BeingConfirmedBySupplier = 16,

        // подтвержден поставщиком - на отгрузке на склад
        [MultiCultureDescription(typeof(OrderStatusesTexts), "BeingDeliveredToStockFromSupplier")]
        //  [EnumDescription("BeingPreparedForShipment")]
        BeingDeliveredToStockFromSupplier = 17,

        // доставлен клиенту
        [MultiCultureDescription(typeof(OrderStatusesTexts), "DeliveredToClient")]
        //  [EnumDescription("BeingPreparedForShipment")]
        DeliveredToClient = 18,

       
        // доставлен клиенту ( для реализации)
        [MultiCultureDescription(typeof(OrderStatusesTexts), "ShippedForSale")]
        ShippedForSale = 19,


        // отменен поставщиком
        [MultiCultureDescription(typeof(OrderStatusesTexts), "CancelledBySupplier")]
        CancelledBySupplier = 20,
        


    }


    // тип цен
    public enum PriceTypeEnum
    {
        [EnumDescription("Закупочная")]
        Zakup = 1,
        [EnumDescription("Базовая")]
        Base,
        [EnumDescription("Рекоммендованная")]
        Recommend,
        [EnumDescription("Не показывать цену")]
        Dont_Show_Price,
        [EnumDescription("Не показывать производителя")]
        Dont_Show_Producer
    }

    // для кого цены ( для клиента / для точки) 
    public enum PriceListFor
    {
        Point = 1, Client = 2
    }

    /// <summary>
    /// Не используем (для уведмления головного терминала что пришел заказ на точку)
    /// </summary>
    public class NotificationOfOrder
    {
        [Key]
        public Guid Order_guid { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(7)]
        public string PartnerId { get; set; }

        public int PointId { get; set; }
    }


    


    /// <summary>
    /// Заказ покупателя с обязательным указанием точки
    /// </summary>
    public class Order
    {

        private ICollection<OrderDetail> _orderDetails;

        public Order()
        {
            _orderDetails = new List<OrderDetail>();
        }


      
      

        [Key]  
        public Guid GuidIn1S { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(7)]

        public string PartnerId { get; set; }

        [ForeignKey("Point")]
        public int PointId { get; set; }


        [ForeignKey("PartnerPointId")]
        public virtual PartnerPoint Point { get; set; }

        [DefaultValue(5)]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public int DaysToDepartment { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DepartmentIdTo5Simbols
        {
            get
            {

                return (DepartmentId.ToString().PadLeft(5, '0'));

            }
        }


        
        [DisplayName("Статус доставки DPD")]
        public DpdDeliveryStatus? DpdDeliveryStatus { get; set; }

        [DefaultValue(OrderStatuses.New)]
        [DisplayName("Статус заказа")]
        public OrderStatuses OrderStatus { get; set; }

        [DisplayName("Номер в учетной базе")]
        [StringLength(8)]

        public string NumberIn1S { get; set; }

        [DefaultValue(false)]
        public bool IsJoined { get; set; }

        [DisplayName("Сумма")]
        [DefaultValue(0)]
        public decimal Total { get; set; }


        [DisplayName("Сумма точки")]
        [DefaultValue(0)]
        public decimal TotalOfPoint { get; set; }


        [DisplayName("Сумма клиента")]
        [DefaultValue(0)]
        public decimal TotalOfClient { get; set; }

        /// <summary>
        ///  Критерий что доставка транспортной компанией
        /// </summary>
        [DefaultValue(0)]
        public bool IsDeliveryByTk { get; set; }

        /// <summary>
        ///  Сумма доставки 
        /// </summary>
        [DefaultValue(0)]
        public decimal CostOfDelivery { get; set; }

        [DisplayName("Дата")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy")]
        public System.DateTime OrderDate { get; set; }


        [DisplayName("Дата отгрузки")]
        [DataType(DataType.Date)]
        public System.DateTime? DeliveryDate { get; set; }

        [DisplayName("Дата оплаты")]
        [DataType(DataType.Date)]
        public System.DateTime? DateOfPayment { get; set; }

        [StringLength(50)]
        public string ContactFIOOfClient { get; set; }

        [StringLength(50)]
        public string PhoneNumberOfClient { get; set; }


        [DisplayName("Комментарий к заказу")]
        [StringLength(250)]
        public string Comments { get; set; }


        // Адрес клиента  или терминала строкой
        [StringLength(500)]
        public string DeliveryDataString { get; set; }


        // Диапазон дней доставки до клиента строкой
        [StringLength(50)]
        public string RangeDeliveryDays { get; set; }


        [DisplayName("В резерв")]
        public bool isReserve { get; set; }

        [DefaultValue(0)]
        public int SupplierId { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get { return _orderDetails; } set { _orderDetails = value; } }


        public void CalculateTotals()
        {
            Total = OrderDetails.Sum(p => p.Price * p.Count);
            TotalOfClient = OrderDetails.Sum(p => p.PriceOfClient * p.Count);
            TotalOfPoint = OrderDetails.Sum(p => p.PriceOfPoint * p.Count);

        }

        // данный статус устанавливается только в терминале (без обмена с 1С)
        // чтобы понять поставщик отменил или это из 1С выгрузилось
        
        public StatusForOrderItemOfSupplier StatusOfSupplier { get; set; }

        public DateTime? DateProcessedBySupplier { get; set; }


        public override string ToString()
        {
            return String.Format("number:{0} status:{1} partner:{2} total:{3} supplier:{4}", this.NumberIn1S,this.OrderStatus,this.PartnerId, this.Total,this.SupplierId);
            
        }
    }

    /// <summary>
    /// Статус передается когда заказ подтверждается 
    /// или отменяется поставщиком именнно в терминале
    /// </summary>

  public  enum StatusForOrderItemOfSupplier
    {
        Rejected=-1,
        NotProcessed =0,
        Confirmed=1

    }

    public class OrderDetail
    {
        public long OrderDetailId { get; set; }
        
        [ForeignKey("Order")]
        public Guid GuidIn1S { get; set; } 
                
        [DefaultValue(0)]
        public int RowNumber { get; set; }

      //  [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

        [DefaultValue(0)]
        public decimal PriceOfClient        { get; set; }

        [DefaultValue(0)]
        public decimal PriceOfPoint { get; set; }
    
        public virtual Order Order { get; set; }
    
        // первоначальная цена
        [DefaultValue(0)]
        public decimal PriceInitial  {get;set;}

        public StatusForOrderItemOfSupplier StatusOfSupplier { get; set; }

        public DateTime? DateProcessedBySupplier { get; set; }

        public virtual Product Product { get; set; }

    }

  
    [DataContract]
    public class PartnerPoint
    {
        private ICollection<PartnerPriceRule> _partnerPriceRules;

        public PartnerPoint()
        {
            _partnerPriceRules = new List<PartnerPriceRule>();

        }
        [Key]
        [DataMember]
         public int PartnerPointId { get; set; }


    
        [DataMember]
        [NotMapped]
        public virtual string Name
        {
            get
            {
                return String.Concat("Point", PartnerPointId); //PartnerPointId.ToString()

            }
            set
            { /**/ }
            
       } 

        [StringLength(7)]
        [DataMember]
        [ForeignKey("Partner")]
        public string PartnerId { get; set; }

        public virtual ICollection<PartnerPriceRule> PartnerPriceRules { get { return _partnerPriceRules; } set { _partnerPriceRules = value; } }

        [DisplayName("Число дней доставки с центрального склада")]
        [DataMember]
        public int DaysToMainDepartment { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public int? DepartmentId { get; set; }

        [DataMember]
        public int DaysToDepartment { get; set; }

        [DisplayName("Контактное лицо")]
        [StringLength(100)]
        [DataMember]
        public string ContactFIO { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(Resource))]
        [StringLength(100)]
        [DataMember]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Resource))]
        [StringLength(100)]
        [DataMember]
        public string Address { get; set; }

        [DisplayName("Пароль")]
        [StringLength(8)]
        
        public string Password { get; set; }

        [StringLength(8)]
        
        public string KeyWord { get; set; }

        [DefaultValue(false)]
        public bool DontShowZakupPrice { get; set; }


        [Display(Name = "Country", ResourceType = typeof(Resource))]
        [StringLength(200)]
        [DataMember(EmitDefaultValue = false)]
        public string Country { get; set; }

        public string Language { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string WebSite { get; set; }

        public string AddressForDelivery { get; set; }

        [Display(Name = "CompName", ResourceType = typeof(Resource))]
        [StringLength(200)]
        [DataMember(EmitDefaultValue = false)]
        public string CompanyName { get; set; }

        [DisplayName("Условия приняты")]
        public bool ConditionsAreAccepted { get; set; }

        [DisplayName("Направление продаж")]
        [DataMember]
        public byte SaleDirection { get; set; }

        [DisplayName("Внутренний идентификатор")]
        [StringLength(200)]
        [DataMember]
        public string InternalName { get; set; }

        [StringLength(100)]
        public string LatLng { get; set; }

        public virtual Partner Partner { get; set; }


    }

    /// <summary>
    /// Правила ценообразования по производителям  внутри точек
    /// </summary>
    public class PartnerPriceRule
    {
        [Key]
        public int PartnerPriceRuleId { get; set; }
        public int PartnerPointId { get; set; }
        public virtual PartnerPoint PartnerPoint { get; set; }
        public int ProducerId { get; set; }
        public virtual Producer Producer { get; set; }
        public decimal Discount { get; set; }

        [StringLength(30)]
        public string PriceType { get; set; }

        [Index]
        [DefaultValue(PriceTypeEnum.Zakup)] //3
        public PriceTypeEnum PType { get; set; }

        [DefaultValue(PriceListFor.Client)] //1
        public PriceListFor PriceInOut { get; set; }

    }


    /// <summary>
    /// Реализация
    /// </summary>
    public class Sale
    {
        [Key]
        public Guid GuidIn1S { get; set; }

        [StringLength(7)]
        public string PartnerId { get; set; }  // link to partner

        [DisplayName("Номер в учетной базе")]
        [StringLength(8)]
        public string NumberIn1S { get; set; }


        [DisplayName("Сумма")]
        [DefaultValue(0)]
        public decimal Total { get; set; }

        [DisplayName("Дата")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy")]
        public System.DateTime SaleDate { get; set; }

        public Guid GuidOfOrderIn1S { get; set; }

        [DefaultValue(5)]
        public int DepartmentId { get; set; }

        [StringLength(100)]
        public string Driver { get; set; }

        [StringLength(20)]
        public string PhoneNumberOfDriver { get; set; }

        [StringLength(20)]
        public string BrandOfAuto { get; set; }

        [StringLength(20)]
        public string RegNumOfAuto { get; set; }

        [StringLength(150)]
        public string DischargePoint { get; set; }

        public int PointId { get; set; }

        public bool IsDelivered { get; set; }

        [DisplayName("Комментарий к заказу")]
        [StringLength(200)]
        public string Comments { get; set; }

        public virtual ICollection<SaleDetail> SaleDetails { get; set; }

    }

    /// <summary>
    /// Таблица товаров в  реализациях
    /// </summary>
    public class SaleDetail
    {
        [Key]
        public long SaleDetailId { get; set; }
        public Guid SaleGuidIn1S { get; set; }

        public int RowNumber { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public virtual Product Product { get; set; }
        //public virtual Sale Sale { get; set; }
    }

    /// <summary>
    /// Применяемость акб по авто
    /// </summary>
    public class CarAkbRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CarRecordId { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; }

        [StringLength(100)]
        public string Modification { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        public int Year { get; set; }

        public int Length { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Capacity { get; set; }

        public int Starting_Current { get; set; }

        public bool Connection { get; set; }

        public int Voltage { get; set; }


    }

    /// <summary>
    /// Применяемость грузовых дисков
    /// </summary>
    [Table("CargoWheelsVehicle")]
    public class CargoWheelsVehicle
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        public string Article { get; set; }

        public string AppCars { get; set; }

        public string TyresSizes { get; set; }

        public string Analogs { get; set; }
    }


    /// <summary>
    /// Подбор шин и дисков по авто
    /// </summary>
    [Table("CarTyreDiskRecord")]
    public class CarRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarRecordId { get; set; }

        // tyre , disk , acc , akb, other
       /* [StringLength(20)]
        public string ProductType { get; set; } */

        public ProductType ProductType { get; set; } 

        // brand
        public Guid? VendorID { get; set; }

        [StringLength(100)]
        public string VendorName { get; set; }

        //model
        public Guid? CarId { get; set; }

        [StringLength(100)]
        public string CarName { get; set; }

        //modification
        public int BeginYear { get; set; }
        public int EndYear { get; set; }
        public string Bolts { get; set; }

        [StringLength(5)]
        public string Dia { get; set; }
        public bool IsBolts { get; set; }
        public Guid? ModificationId { get; set; }

        [StringLength(100)]
        public string ModificationName { get; set; }

        [StringLength(2)]
        public string PCDc { get; set; }
        [StringLength(10)]
        public string PCDd { get; set; }
        public string RecordName { get; set; }

        // typorazmer
        //[StringLength(2)]
        public double Diameter { get; set; }
        public int Mode { get; set; }
        public int Profile { get; set; }
        public double? Width { get; set; }
        public double? ET { get; set; }

        public double? RearWidth { get; set; }
        public double? RearDiameter { get; set; }
        public double? RearET { get; set; }

        public int? RearProfile { get; set; }

      /*  [StringLength(4)]
        public string Width { get; set; }

        [StringLength(4)]
        public string ET { get; set; } */
    }

    /// <summary>
    /// Новости
    /// </summary>
    public class News
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string NewsName { get; set; }
        public string NewsText { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] MainImg { get; set; }
        public byte[] PreviewImg { get; set; }
        [StringLength(10)]
        public string Culture { get; set; }
        public bool Active { get; set; }
        public DateTime DatePublish { get; set; }
    }
    /// <summary>
    /// Уведомления о прочтении  новостей (только )
    /// </summary>
    public class NewsNotifications
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string UserName { get; set; }
        public long NewsId { get; set; }
    }

    /// <summary>
    ///Дополнительные Картинки для товаров
    /// </summary>
    public class PhotoForProducts
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(255)]
        public string NamePhoto { get; set; }
        public int NumberPhoto { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Photo { get; set; }
    }

    /// <summary>
    /// Список стран 
    /// </summary>
    public class Countries
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Список кодов аббревиатур дисков для реплики
    /// </summary>
    public class ReplicaDisksForCars
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
