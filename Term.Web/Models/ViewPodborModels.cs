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
using Term.Web.Views.Resources;
using Yst.DropDowns;
using Yst.ViewModels;

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

        private static SelectList _orderStatuses = DropDownsFactory.GetOrderStatuses();
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
            ItemsPerPage = 100;
            AddressesIds = new SelectList(Enumerable.Empty<SelectListItem>());
        }

      public  SelectList AddressesIds { get; set; }

      
      public string AddressId { get; set; }

    }

    /// <summary>
    /// Модель журнала возврата товаров
    /// </summary>

    public class SaleReturnViewModel : BaseViewPodborModel
    {
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

        private static readonly SelectList _saleStatuses = DropDownsFactory.GetSaleStatuses();

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