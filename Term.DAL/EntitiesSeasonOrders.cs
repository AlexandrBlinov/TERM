using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term.CustomAttributes;
using Term.DAL.Resources;


namespace Term.DAL
{
    

    /// <summary>
    ///  products & factories to show in selections
    /// </summary>
    public class SeasonStockItem
    {
        [Key]
        [ForeignKey("Product")]    
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
        
    
        [DefaultValue(true)]
        public bool Active { get; set; }
    }

    /// <summary>
    /// Season products for individual partner
    /// </summary>
    public class SeasonStockItemOfPartner
    {
        [Key,Column(Order=1)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Key, Column(Order = 2)]
        [ForeignKey("Partner")]
        [MaxLength(7)]
        public string PartnerId { get; set; }
        public virtual Partner Partner { get; set; }

    }

    
    /// <summary>
    /// Season shopping cart
    /// </summary>
       
     public class SeasonCart
     {
         [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public long Id { get; set; }
        
         [Index]
         [StringLength(50)]
         public string CartId { get; set; }
         public int ProductId { get; set; }

         public int Count { get; set; }

       //  public int DepartmentId { get; set; }
         public decimal Price { get; set; }
         public virtual Product Product { get; set; }
         [MaxLength(10)]
         public string Factory { get; set; } 
    

     }

     #region SeasonOrder
     
     
     /// <summary>
    /// Статусы сезонного заказа
    /// </summary>
     public enum SeasonOrderStatus
     {
         [MultiCultureDescription(typeof(Resource), "New")]
         New = 1,

         [MultiCultureDescription(typeof(Resource), "Confirmed")]
         Confirmed = 2,

         [MultiCultureDescription(typeof(Resource), "Cancelled")]
         Cancelled = 3
     }

    
    /// <summary>
    /// Сезонный заказ
    /// </summary>

     public class SeasonOrder
     {


         [Key]
         public Guid OrderGuid { get; set; }

         public SeasonOrder()
         {
             OrderDetails = new List<SeasonOrderDetail>();
             OrderStatus = SeasonOrderStatus.New;
             OrderDate = DateTime.Now;
         }


         [StringLength(Byte.MaxValue)]
         [Index]
         public string Username { get; set; }

         [StringLength(7)]
         public string PartnerId { get; set; }
         public virtual Partner Partner { get; set; }

        
         [DefaultValue(SeasonOrderStatus.New)]
         [DisplayName("Статус заказа")]
         public SeasonOrderStatus OrderStatus { get; set; }

         [DisplayName("Номер в учетной базе")]
         [StringLength(8)]
         public string NumberIn1S { get; set; }


         [DisplayName("Сумма")]
         [DefaultValue(0)]
         public decimal Total { get; set; }

         [DisplayName("Дата")]
         [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "dd.MM.yyyy")]
         public System.DateTime OrderDate { get; set; }


         [DisplayName("Дата отгрузки")]
         [DataType(DataType.Date)]
         public System.DateTime? DeliveryDate { get; set; }

         [DisplayName("Комментарий к заказу")]
         [StringLength(250)]
         public string Comments { get; set; }

         /// <summary>
         /// Если false - обычный сезонный заказ, если true - в пути
         /// </summary>
         [DefaultValue(false)]
         public bool FromOnWay { get; set; }

         public virtual IList<SeasonOrderDetail> OrderDetails { get; set; }



     }

     public class SeasonOrderDetail
     {
    
         [Key, Column(Order = 1)]
         public Guid OrderGuid { get; set; }

         [Key, Column(Order = 2)]
         public int RowNumber { get; set; }
         [ForeignKey("Product")]
         public int ProductId { get; set; }
         public int Count { get; set; }
         public decimal Price { get; set; }
         [ForeignKey("ProductId")]
         public virtual Product Product { get; set; }
         [ForeignKey("OrderGuid")]
         public virtual SeasonOrder SeasonOrder { get; set; }
     }
    #endregion
}
