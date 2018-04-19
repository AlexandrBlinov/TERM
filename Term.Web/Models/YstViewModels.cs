using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Yst.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using Term.DAL;
using System.Collections;
using System.Runtime.Serialization;
//using Yst.CustomAttributes;
using Yst.DropDowns;
using PagedList;

using YstTerm.Models;

using Term.Utils;
using Term.CustomAttributes;
using Term.Web.HtmlHelpers;
using Term.Web.Views.Resources;


namespace Yst.ViewModels
{
    /// <summary>
    /// Данные пользователя (профиль)
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // является ли пользователь партнером
        public bool IsPartner { get; set; }

        // связь пользователя и партнера, чтобы потом привязываться к нему
        [StringLength(7)]
        public string PartnerId { get; set; }

        [DisplayName("ФИО")]
        [StringLength(100)]
        public string ContactFIO { get; set; }

        public int? DepartmentId { get; set; }

        public int? PartnerPointId { get; set; }

        public int? SupplierId { get; set; }
        /*
        [DefaultValue(0)]
        public bool IsSupplier { get; set; }

        [DefaultValue(0)]
        public bool IsSaleRep { get; set; } */

    }


    /*
    *  Выбор логистики по отправке товаров
    */
    public enum CaseLogistik
    {
        // Логистика Головное подразделение
        // С головного подразделения от поставщика не ждем
        FromSupplierNotWait = 21,
        
        // С головного подразделения от поставщика  ждем
        FromSupplierWait = 22,


        // Логистика Филиал
        // Только головного подразделения через филиал  ждем
        FromMainToDep = 41,

        // С головного подразделения через филиал не  ждем
        FromMainDepNotWait = 51,

        // С головного подразделения через филиал не  ждем
        FromMainDepWait = 52,
    }

    /// <summary>
    /// День доставки , когда отгрузка в пятницу
    /// </summary>
    public enum DayOfWeekToDeliver
    {
        Saturday,
        Monday
    }


    /// <summary>
    /// Модель для корзины
    /// </summary>
    public class ShoppingCartViewModel
    {


        public IList<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public decimal CartTotalOfClient { get; set; }
        public decimal CartCount { get; set; }

        public Dictionary<string, string> Errors { get; set; }


        //[StringLength(100)]
        public string ContactFIOOfClient { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessageResourceType = typeof(ShoppingCartErrors), ErrorMessageResourceName = "IncorrectPhoneNumber")]
        //@"((8|\+7)-?)?\(?\d{3}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}
        public string PhoneNumberOfClient { get; set; }

        [DisplayName(@"Дата отгрузки")]
        [DataType(DataType.Date)]
        [ValidateDate(0, 10)]
        public DateTime? DeliveryDate
        { get; set; }

        [DisplayName(@"Комментарий к заказу")]
        //[StringLength(250)]
        public string Comments { get; set; }

       // public bool isReserve { get; set; }

         // на доставку
        public bool IsDelivery { get; set; }

        
        // вообще есть договоры со *
        public bool HasStar { get; set; }
        
        // в корзине выбран *
        public bool IsStar { get; set; }

        // предоплатный
        public bool IsPrepay { get; set; }
        
        // способ доставки 1- самовывоз, 0- доставка наша, 2 - доставка dpd
        public int WayOfDelivery { get; set; }


        // доставка транспортной компанией
        public bool IsDeliveryByTk { get; set; }

        
        public decimal TotalWeight { get; set; }

       // договором предусмотрена сезонная отсрочка (только для головного терминала)
        public bool HasSeasonAdjournment { get; set; }

        // выбрал сезонную отсрочку (только для головного терминала)
        public bool IsSeasonAdjournment { get; set; }

        public DayOfWeekToDeliver DayOfWeekToDeliver { get; set; }
    }

   

    /// <summary>
    /// Данные модели корзины с информацией о доставке
    /// </summary>
    public class ShoppingCartViewModelExtended : ShoppingCartViewModel
    {

        public ShoppingCartViewModelExtended()
        {
            AddressesIds = new SelectList(Enumerable.Empty<SelectListItem>());
        }
        /// <summary>
        /// Видит ли пользователь доставку DPD 
        /// </summary>
        /// 

        

        public bool CanUserUseDpdDelivery { get; set; }
        // Кладр региона
        public string RegionId { get; set; }

        // строковое представление города
        public string City { get; set; }

        // Кладр города
        public string CityId { get; set; }

        // Радио если Terminal=true, Address=false
        public bool TerminalOrAddress { get; set; }

        public string TerminalsDpd { get; set; }
        // адрес dpd строкой
        public string Address { get; set; }

        public decimal CostOfDelivery { get; set; }

        public string PostalCode { get; set; }

        public string StreetType { get; set; }

        public string Street { get; set; }
        
        public string House { get; set; }

        public string BlockType { get; set; }

        // список адресов
        public SelectList AddressesIds { get; set; }

        // список трансп-х компаний
        public SelectList TkIds { get; set; }

        // Id самодоставки
        public SelectList SelfDeliveryIds { get; set; }

        // идентификатор 1С адреса доставки ( не dpd)
        public string AddressId { get; set; }

        public string TkId { get; set; }

        public string SelfDeliveryId { get; set; }

        public string LogistikDepartment { get; set; }

        // новое ТЗ
        public DateTime? DeliveryDate2 { get; set; }
       public   CaseLogistik? CaseForLogistik { get; set; }


    }


    /// <summary>
    /// Модель корзины при добавлении/удалении товара
    /// </summary>
    public class ShoppingCartAddRemoveViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public decimal CartTotalOfClient { get; set; }
        public int CartCount { get; set; }
        public bool Success { get; set; }
        public decimal TotalWeight { get; set; }

    }


    #region DTO for Order
    /// <summary>
    /// DTO for order
    /// </summary>


    [DataContract(Namespace = "")]
    public class OrderViewModel 
    {


        [DataMember(Order = 1)]
      public Guid Order_guid { get; set; }
       public string Username { get; set; }

        [StringLength(7)]
       public string PartnerId { get; set; }  // link to partner
        public string PartnerName { get; set; }

        public string InternalName { get; set; }

        public int PointId { get; set; }

        [DisplayName(@"Подразделение")]
        public string DepartmentName { get; set; }

        public int DaysToDepartment { get; set; }

        public OrderStatuses OrderStatus { get; set; }

        [DisplayName(@"Статус заказа")]
       
        public string OrderStatusName
        {
            get
            {
                return EnumDescriptionProvider.GetDescription(OrderStatus);

            }
        }

        [DataMember(Order = 2)]
        [DisplayName("Номер в учетной базе")]
        public string NumberIn1S { get; set; }

        
        [DisplayName("Сумма")]
        [DataMember(Order = 7)]
        public decimal Total { get; set; }

        [DisplayName("Сумма точки")]
        public decimal TotalOfPoint { get; set; }

        [DisplayName("Сумма клиента")]
        public decimal TotalOfClient { get; set; }

        [DisplayName("Дата")]
        [DataType(DataType.Date)]
        [DataMember(Order=3)]
        public System.DateTime OrderDate { get; set; }


        [DisplayName("Дата отгрузки")]
        public System.DateTime? DeliveryDate { get; set; }

        [DisplayName("Дата оплаты")]
        public System.DateTime? DateOfPayment { get; set; }


        [DataMember(Order = 9)]
        [DisplayName("Комментарий к заказу")]
        public string Comments { get; set; }

        
        [StringLength(50)]
        [DataMember(Order=4)]
        public string ContactFIOOfClient { get; set; }

        [StringLength(50)]
        [DataMember(Order = 5)]
        public string PhoneNumberOfClient { get; set; }

        [DisplayName("В резерв")]
        public bool isReserve { get; set; }

        [DataMember(Order=6)]
        public string DeliveryDataString { get; set; }

        public string RangeDeliveryDays { get; set; }

        /// <summary>
        /// доставка транспортной компанией
        /// </summary>
        public bool IsDeliveryByTk { get; set; }
        
        public decimal CostOfDelivery { get; set; }


        
        /// <summary>
        /// статусы из Dpd
        /// </summary>
        /// 
        public DpdDeliveryStatus? DpdDeliveryStatus { get; set; }


        public string DpdDeliveryStatusName
        {
            get
            {
                if (!DpdDeliveryStatus.HasValue) return String.Empty;

                return EnumDescriptionProvider.GetDescription(DpdDeliveryStatus);

            }
        }

        public int SupplierId { get; set; }

      public  ICollection<OrderDetail> OrderDetails { get; set; }

    }

    [DataContract(Namespace = "")]
    public class OrderViewWithDetails
    {
        public OrderViewWithDetails()
        {
            Files = Enumerable.Empty<Term.DAL.File>();
        }

        public IEnumerable<Term.DAL.File> Files { get; set; }

        public bool IsDelivery {
            get { return !Order.isReserve; }
        }
        [DataMember]
        public OrderViewModel OrderData { get; set; }

        [DataMember]
        public virtual ICollection<OrderViewDetail> OrderDetails { get; set; }
        public FieldsForDpdViewModel FildsForDpdForm { get; set; }

        // Добавил ссылку на заказ чтобы не раздувать  OrderData

        public Order Order { get; set; }
        public decimal Total { get; set; }
        public decimal TotalOfClient { get; set; }
        
        public string DepartmentName { get; set; }

        public string OrderStatusName
        {
            get
            {
                return EnumDescriptionProvider.GetDescription(Order.OrderStatus);
            }
        }
    }

    /// <summary>
    /// Информация о водителе
    /// </summary>
    public class DriverInfo
    {
        public string Driver { get; set; }
        public string PhoneNumberOfDriver { get; set; }
        public string BrandOfAuto { get; set; }
        public string RegNumOfAuto { get; set; }
    }


    //[DataContract(Namespace = "")]
    public class OrderViewWithDetailsExtended : OrderViewWithDetails
    {

        public OrderViewWithDetailsExtended()
        {
            HistoryOfOrderStatuses= new List<HistoryOfOrderstatus>();
            
        }

        /// <summary>
        /// Видит ли пользователь доставку DPD 
        /// </summary>
        /// 
        public bool CanUserChangeDpdOrder { get; set; }

        public bool CanUserUseDpdDelivery { get; set; }
        // Кладр региона
        public string RegionId { get; set; }

        // строковое представление города
        public string City { get; set; }

        // Кладр города
        public string CityId { get; set; }

        // Радио если Terminal=true, Address=false
        public bool TerminalOrAddress { get; set; }

        public string TerminalsDpd { get; set; }
        // адрес строкой
        public string Address { get; set; }

        public decimal CostOfDelivery { get; set; }

        public string PostalCode { get; set; }

        public string StreetType { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        public string BlockType { get; set; }

        public IList<HistoryOfOrderstatus> HistoryOfOrderStatuses { get; set; }
        
        public string DeliveryDataString { get; set; }

        // адрес доставки не dpd
        public string AddressOfDelivery { get; set; }

        // способ доставки 1- самовывоз, 0- доставка наша, 2 - доставка dpd
        public int WayOfDelivery { get; set; }

        // список адресов
        public SelectList AddressesIds { get; set; }

        // идентификатор 1С адреса доставки ( не dpd)
        public string AddressId { get; set; }

        public string DefaultAddressId { get; set; }

        // список трансп-х компаний
        public SelectList TkIds { get; set; }

        public string TkId { get; set; }

        // вообще есть договоры со *
        public bool HasStar { get; set; }

        // в корзине выбран *
        public bool IsStar { get; set; }
        public bool IsPrepay { get;  set; }

        public string LogistikDepartment { get; set; }

        public DriverInfo DriverInfo { get; set; }
    }

    [DataContract(Namespace = "")]
    public class OrderViewDetail
    {
        [DataMember]
        public int RowNumber { get; set; }

        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public decimal PriceOfClient { get; set; }

    }

    /// <summary>
    /// Возвращает товар с картинкой
    /// </summary>
    public class OrderViewDetailWithPicture : OrderViewDetail
    {
        public string RemotePicture {
            get { return PictureUtility.GetRemotePictureByProductId(this.ProductId); }
        }
        
    }
    /*
     *  Доп. поля для клиента для Dpd
     */
    public class FieldsForDpdViewModel
    {
        public string PartnerName { get; set; }
        public string Address { get; set; }
        public string PartnerInnKpp { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalWeight { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryCity { get; set; }
        public string SaleNumber { get; set; }
        public string ContractDate { get; set; }
        public string ContractNumber { get; set; }
        public string PartnerManager { get; set; }
        public string EMailManager { get; set; }
    }


    #endregion



    public class UserViewModel
    {

        [DisplayName("Имя")]
        public string UserName { get; set; }

        [DisplayName("Партнер")]
        public bool IsPartner { get; set; }

        [DisplayName("Код партнера")]
        public string PartnerId { get; set; }

        [DisplayName("Наименование")]
        public string PartnerName { get; set; }

        public string FullPartnerName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        [DisplayName("ИНН/КПП")]
        public string INNKPP { get; set; }

        [DisplayName("ФИО")]
        [StringLength(100)]
        public string ContactFIO { get; set; }

       
        public int? DepartmentId { get; set; }

        [DisplayName("Подразделение")]
        public string DepartmentName { get; set; }

   
    }


    public class CarRecordViewDetail
    {
        public string VendorName { get; set; }
        public string CarName { get; set; }
          public int BeginYear { get; set; }
          public int EndYear { get; set; } 
        public string ModificationName { get; set; }
        public string Bolts { get; set; }
        public bool IsBolts { get; set; }
        public string BoltsSize { get { return (IsBolts == false) ? SearchByAuto.Nut + " " + Bolts : SearchByAuto.Bolt + " " + Bolts; } }
        public int Rear { get; set; }
        public override string ToString()
        {
            return String.Format("{0} {1}", CarName, ModificationName);

        }
    }
   

    [DataContract(Namespace = "")]
    public class AutoCompleteResult
    {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public int ProductType { get; set; }
    }

    public class SaleViewWithDetails
    {

        public SaleViewModel SaleData { get; set; }
        public IEnumerable<SaleViewDetail> SaleDetails { get; set; }
        // public IEnumerable<OrderViewDetail> OrderDetails { get; set; }

    }

    /// <summary>
    /// Модель реализации
    /// </summary>
    public class SaleViewModel
    {
        public Guid GuidIn1S { get; set; }
        public string PartnerId { get; set; }
        public string NumberIn1S { get; set; }
        public decimal Total { get; set; }
        public System.DateTime SaleDate { get; set; }
        public Guid GuidOfOrderIn1S { get; set; }
        public int DepartmentId { get; set; }
        public string Driver { get; set; }
        public string PhoneNumberOfDriver { get; set; }
        public string BrandOfAuto { get; set; }
        public string RegNumOfAuto { get; set; }
        public string DischargePoint { get; set; }
        public string Comments { get; set; }
        public int PointId { get; set; }
        public IEnumerable<SaleDetail> SaleDetails { get; set; }
        public DpdDeliveryStatus? DpdDeliveryStatus { get; set; }


        public string DpdDeliveryStatusName
        {
            get
            {
                if (!DpdDeliveryStatus.HasValue) return String.Empty;

                return EnumDescriptionProvider.GetDescription(DpdDeliveryStatus);

            }
        }

        public bool IsDelivered { get; set; }
    }

    public class SaleViewDetail
    {
        public long SaleDetailId { get; set; }
        public int RowNumber { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
    }

    
    
}