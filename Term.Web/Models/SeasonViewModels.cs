using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Term.Utils;
using Term.DAL;
using Term.Soapmodels;
using YstTerm.Models;

namespace YstTerm.Models

{

    public enum DisplaySeasonCart { 
    Plain = 0 ,
    GroupedByFactory = 1,
    }
    /*
    public class SeasonStockModelView
    {

        public SeasonStockModelView()
        {
            SeasonOffers = new List<SeasonStockItem>();
            ActiveProducers = new List<Producer>();
            SeasonStockItemsPerProducers = new Dictionary<SeasonPost, IEnumerable<SeasonStockItem>>();
        }

        public IEnumerable<SeasonStockItem> SeasonOffers { get; set; }
        public IEnumerable<Producer> ActiveProducers { get; set; }
        public IDictionary<SeasonPost, IEnumerable<SeasonStockItem>> SeasonStockItemsPerProducers { get; set; }

        public int? ActiveProducerId { get; set; }

               
    } */

    public class ActiveProducersModelView
    {
        public ActiveProducersModelView()
        {
            Producers = new List<Producer>();
        }
        public IEnumerable<Producer> Producers { get; set; }

    }
    public class PostToSeasonOrderDTO
    {
        public int Id { get; set; }
        public string ProducerName { get; set; }
        public int ProducerId { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

    }
    public class SeasonStockModelUpload : IValidatableObject
    {

        public SeasonStockModelUpload()
        {

            Producers = new List<Producer>();
        }
        public DateTime DateActiveTill { get; set; }
        public string Comments { get; set; }
        public IEnumerable<Producer> Producers { get; set; }
        //   [Required(ErrorMessage = "Необходимо выбрать производителя")]

        public int? ProducerId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ProducerId.HasValue) yield return new ValidationResult("Необходимо выбрать производителя", new[] { "ProducerId" });
            if (DateActiveTill < DateTime.Now.AddMonths(1))
                yield return new ValidationResult("Дата действия должна быть как минимум на месяц позднее чем текущая дата", new[] { "DateActiveTill" });
        }
    }


    public class SeasonShoppingCartAddRemoveViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public decimal CartWeight { get; set; }
        public decimal CartVolume { get; set; }
        public bool Success { get; set; }

    }

    /// <summary>
    /// Model for season cart
    /// contains item grouped by WheelType = Alloy, Steel, Forged
    /// </summary>
    public class SeasonCartViewModel
    {

        public SeasonCartViewModel()
        {
           
            ItemsByWheelType = new Dictionary<WheelType, IEnumerable<SeasonCart>>();
            ItemsByWheelTypeAndFactory = new Dictionary<WheelType, Dictionary<string, IEnumerable<SeasonCart>>>();
            Display = DisplaySeasonCart.Plain;
        }
        public List<SeasonCart> CartItems { get; set; }

        public bool IsForeign { get; set; }
        /// <summary>
        /// totals of cart
        /// </summary>
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public decimal CartVolume { get; set; }
        public decimal CartWeight { get; set; }

        
        public DateTime? DeliveryDate  { get; set; }


        [StringLength(250)]
        public string Comments { get; set; }


        public WheelType? ActiveWheelType { get; set; }
        public IDictionary<WheelType, IEnumerable<SeasonCart>> ItemsByWheelType { get; set; }

        public IDictionary<WheelType, Dictionary<string,IEnumerable<SeasonCart>>> ItemsByWheelTypeAndFactory { get; set; }

        public DisplaySeasonCart? Display;                       

    }


    public class SeasonOrdersViewResult
    {
        public SeasonOrdersViewResult()
        {
            ItemsCreated = new List<SeasonOrderCreatedViewItem>();
            ItemsCancelled = new List<SeasonOrderCancelledViewItem>();
        }
       public  IList<SeasonOrderCreatedViewItem> ItemsCreated;
        public IList<SeasonOrderCancelledViewItem> ItemsCancelled;
    
    }

    public class SeasonOrderCreatedViewItem {
        public string  NumberIn1S { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public string ProducerName { get; set; }
        public decimal Total { get; set; }
        public Guid OrderGuid { get; set; }
        public bool CanModify { get; set; }
        public SeasonOrderStatus OrderStatus { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }

    public class SeasonOrderCancelledViewItem
    {
                
        public DateTime? BeginDate { get; set; }
        public string ProducerName { get; set; }
        public DateTime CancelDate { get; set; }
        public bool CanModify { get; set; }
        public int PostId { get; set; }

    }

   
    /// <summary>
    /// Used  for list of season orders
    /// </summary>
    /// 


    public class SeasonOrderViewModel
    {

        public SeasonOrderViewModel()
        {
            FromOnWay = false;
        }
        public bool FromOnWay { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string OrderNumber { get; set; }
        public SeasonOrderStatus? OrderStatus { get; set; }
    
        public IPagedList<SeasonOrder> SeasonOrders { get; set; }

       
        public int ItemsPerPage { get; set; }
                     

        public IEnumerable<int> PagerList
        {
            get { return new int[] { 10, 20, 50, 100, 200 }; }
        }

       
        public IEnumerable<SelectListItem> OrderStatuses {get; set;}

    
    }

    /// <summary>
    /// DTO model passed via ajax when user changing season order
    /// </summary>
    public class SeasonOrderChangedDTO
  {
      public  struct SeasonOrderDetailDTO
        {
            public int ProductId { get; set; }
            public int Count { get; set; }

        }
      public Guid  OrderGuid { get; set; }
      public IEnumerable<SeasonOrderDetailDTO> SeasonOrderDetailsDTO { get; set; }

  }

    /// <summary>
    /// DTO модель для анализа сезонного заказа
    /// </summary>
    public class ProductSeasonOrderAnalized : ProductAnalyseSeasonOrder {

        public int ProductId { get; set; }
        public int QuantityOnWay { get; set; }
        public string Name { get; set; }
        public string QuantityOnWayToDisplay { get { return QuantityOnWay > 100 ? ">100" : QuantityOnWay.ToString(); } }
    }
    
   
}