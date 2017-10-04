using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;

namespace Term.Web.Models
{
    public class DtoOrdersAndProducts
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
    }

    public class DtoJoinOrders
    {
        public DtoJoinOrders()
        {
            //    OrdersAndProducts = Enumerable.Empty<DtoOrdersAndProducts>();
            OrdersAndProducts = new List<DtoOrdersAndProducts>();
        }
        public IEnumerable<DtoOrdersAndProducts> OrdersAndProducts { get; set; }
        public string Comments { get; set; }
        public string AddressId { get; set; }
        public DateTime ShippingDay { get; set; }
    }

    /// <summary>
    /// заказ с ссылкой на объединенный заказ
    /// </summary>
    public class OrderWithGuidLink {
        public Guid? Guid { get; set; }
        public Order Order { get; set; }
    }
}