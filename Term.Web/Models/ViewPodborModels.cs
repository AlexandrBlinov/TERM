using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Term.DAL;
using Term.Services;
using Term.Web.Views.Resources;
using Yst.DropDowns;
using Yst.ViewModels;
using YstProject.Services;

namespace Term.Web.Models
{

    /// <summary>
    /// Базовая модель подборов
    /// </summary>
    public class BaseViewPodborModel
    {
        public BaseViewPodborModel()
        {
            ItemsPerPage = 20;
            Page = 1;
        }
        [Display(Name = "SearchFromDate", ResourceType = typeof(CartAndOrders))]
        public System.DateTime? BeginDate { get; set; }

        [Display(Name = "To", ResourceType = typeof(CartAndOrders))]
        public System.DateTime? EndDate { get; set; }


        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
    }



    /// <summary>
    /// Модель журнала заказов
    /// </summary>
    public class OrdersViewModel : BaseViewPodborModel
    {

        public OrdersViewModel()
        {
            ItemsPerPage = 50;
        }

        private static SelectList _orderStatuses = OrderService.GetOrderStatuses();
        private static readonly Department[] _departments = DropDownsFactory.Departments;


        public SelectList PartnerPoints { get; set; }

        public int? PointId { get; set; }

        public int? DepartmentId { get; set; }

        // поиск по коду 1С и по коду заказа
        //[StringLength(8)]
        public string OrderNumber { get; set; }

        //   public string Status { get; set; }

        public int? StatusId { get; set; }

        public SelectList Statuses
        {
            get { return _orderStatuses; }
        }

        [DisplayName("Номер в учетной базе")]
        //[StringLength(8)]
        public string NumberIn1S { get; set; }


        public IPagedList<Order> Orders { get; set; }

        public IPagedList<OrderWithGuidLink> OrdersWithGuid { get; set; }

        public IEnumerable<Department> Departments
        {
            get { return _departments; }
        }

        public string ProductName { get; set; }

        public bool IsDeliveryByTk { get; set; }
    }


    public class ExtendedOrdersViewModel : OrdersViewModel
    {
        public ExtendedOrdersViewModel()
        {
            ItemsPerPage = Defaults.MaxItemsPerPage;
            AddressesIds = new SelectList(Enumerable.Empty<SelectListItem>());
        }

      public  SelectList AddressesIds { get; set; }

      
      public string AddressId { get; set; }

    }

    /// <summary>
    /// Расширенная модель для списка заказов
    /// </summary>
    public class OrdersViewModelForList : OrdersViewModel
    {
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


        // способ доставки 1- самовывоз, 0- доставка наша, 2 - доставка dpd
        public int WayOfDelivery { get; set; }


        [DisplayName(@"Дата отгрузки")]
        [DataType(DataType.Date)]
          public DateTime? DeliveryDate  { get; set; }

        public DayOfWeekToDeliver DayOfWeekToDeliver { get; set; }
        
        
        public string LogistikDepartment { get; set; } = Defaults.MainDepartmentCode;


    }

    /// <summary>
    /// Модель для обработки заказов
    /// </summary>
    public class OrdersViewModelToProcess
    {
        public DateTime DeliveryDate { get; set; }
        // способ доставки 1- самовывоз, 0- доставка наша, 2 - доставка dpd , 3 - транспортной компанией
        public int WayOfDelivery { get; set; }

        public string AddressId { get; set; }
        public string TkId { get; set; }

        public IList<Guid> OrderGuids { get; set; }
        public DayOfWeekToDeliver DayOfWeekToDeliver { get; set; }
    }


    /// <summary>
    /// Модель журнала возврата товаров
    /// </summary>

    public class SaleReturnViewModel : BaseViewPodborModel
    {
        public string ProductId { get; set; }
        public string SaleNumber { get; set; }
        public int? NumberIn1S { get; set; }
        public IPagedList<SaleReturnDto> SaleReturns { get; set; }

    }

    public class SaleReturnDto
    {

        public Guid GuidIn1S { get; set; }

        public string NumberIn1S { get; set; }

        public DateTime DocDate { get; set; }
      
        public string PartnerId { get; set; }
    
        public int PointId { get; set; }

        public decimal Sum { get; set; }
        public decimal Count { get; set; }
    }

    /// <summary>
    /// Модель журнала реализаций товаров
    /// </summary>

    public class SalesViewModel : BaseViewPodborModel
    {

        private static readonly SelectList _saleStatuses = SalesService.GetSaleStatuses();

        public SelectList PartnerPoints { get; set; }


        public IPagedList<SaleViewModel> Sales { get; set; }

        public string ProductName { get; set; }

        public int? PointId { get; set; }

        public SelectList Statuses
        {
            get { return _saleStatuses; }
        }

        public int? StatusId { get; set; }
    }
}