using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Yst.Context;
using Yst.ViewModels;
using Term.DAL;
using YstTerm.Models;
using PagedList;
using YstProject.Services;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Term.Web.Services;
using Term.Soapmodels;
using Term.Web.Models;
using System.Web.Mvc;

namespace Term.Services
{
    /// <summary>
    /// Сервис для работы с заказами
    /// </summary>
    public class OrderService  : BaseService
    {

        /// <summary>
        /// Получить адреса доставки для текущего партнера или точки
        /// </summary>
        public SelectList GetAddressesOfDeliveryForCurrentPoint() {
           var partnerId=  this.CurrentPoint?.PartnerId;
            var pointId = this.CurrentPoint?.PartnerPointId;

             Expression<Func<AddressOfPartner,bool>> predicate = p => p.PartnerId == partnerId ;

            bool flag = false;
            
            if (base.IsPartner && partnerId != null)  flag = true;
            
            // predicate stays unchanged

            else if (!base.IsPartner && partnerId != null && pointId.HasValue)
            {
              flag = true;
              predicate = p => p.PartnerId == partnerId && p.PointId == pointId;
              }

            if (flag) return new SelectList(DbContext.AddressOfPartners.Where(predicate).Select(p => new {
                Id = p.AddressId,
                Name = p.Address

            }).OrderBy(p => p.Name).ToList(), "Id", "Name");


            return new SelectList(Enumerable.Empty<SelectListItem>());
            
        }


        /// <summary>
        /// Получить заказ при создании из корзины
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public OrderViewWithDetailsExtended GetOrderWithDetailsByGuid(Guid guid)
        {

            var model = new OrderViewWithDetailsExtended
            {
                OrderDetails = (from orderdetail in DbContext.OrderDetails.Where(p => p.GuidIn1S == guid)
                                from product in
                                    _dbContext.Products.Where(pofpart => pofpart.ProductId == orderdetail.ProductId).DefaultIfEmpty()
                                select new OrderViewDetail
                                {
                                    RowNumber = orderdetail.RowNumber,
                                    ProductId = orderdetail.ProductId,
                                    Count = orderdetail.Count,
                                    Price = orderdetail.Price,
                                    PriceOfClient = orderdetail.PriceOfClient,
                                    ProductName = product == null ? "" : product.Name
                                }).ToList(),
                OrderData = DbContext.Orders.Where(o => o.GuidIn1S == guid).Select(o => new OrderViewModel
                {
                    NumberIn1S = o.NumberIn1S,
                    Comments = o.Comments,
                    OrderStatus = o.OrderStatus,
                    Total = o.Total,
                    TotalOfClient = o.TotalOfClient,
                    TotalOfPoint = o.TotalOfPoint,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    ContactFIOOfClient = o.ContactFIOOfClient,
                    PhoneNumberOfClient = o.PhoneNumberOfClient,
                    Order_guid = o.GuidIn1S,
                    DaysToDepartment = o.DaysToDepartment,
                    isReserve = o.isReserve,
                    RangeDeliveryDays = o.RangeDeliveryDays,
                    IsDeliveryByTk = o.IsDeliveryByTk,
                    SupplierId = o.SupplierId
                }).FirstOrDefault()
            };


            if (model.OrderData == null) return null;
            else
                return (model);
        } 

        /// <summary>
        /// Получить заказ по guid (открыть из списка заказов)
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="isPartner"></param>
        /// <param name="partnerId"></param>
        /// <param name="pointId"></param>
        /// <returns></returns>

        public OrderViewWithDetailsExtended GetOrderWithDetailsByGuid(Guid guid, bool isPartner, string partnerId, int pointId)
        {
            
            

            string sqlText = @"SELECT OrderDetails.RowNumber, OrderDetails.ProductId ProductId, ISNULL(Products.Name,'') ProductName , 
               OrderDetails.Count Count, 
                 CASE WHEN {1}=1 THEN OrderDetails.Price ELSE OrderDetails.PriceOfPoint END Price, 
                CASE WHEN {2}=1 THEN OrderDetails.PriceOfClient ELSE OrderDetails.PriceOfPoint END PriceOfClient 
                FROM OrderDetails
                LEFT JOIN  Products ON OrderDetails.ProductId= Products.ProductId
                WHERE GuidIn1S={0}  ORDER BY OrderDetails.RowNumber DESC";


            var model = new OrderViewWithDetailsExtended();
            model.OrderData = DbContext.Orders.Where(o => o.GuidIn1S == guid).Select(o => new OrderViewModel
            {   PointId=o.PointId,
                NumberIn1S = o.NumberIn1S,
                Comments = o.Comments,
                OrderStatus = o.OrderStatus,
                Total = isPartner ? o.Total:o.TotalOfPoint, // входную цену видит только головной Т
                TotalOfClient = ((isPartner && o.PointId == pointId)||!isPartner) ? o.TotalOfClient : o.TotalOfPoint,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                DateOfPayment = o.DateOfPayment,
                ContactFIOOfClient = o.ContactFIOOfClient,
                PhoneNumberOfClient = o.PhoneNumberOfClient,
                Order_guid = o.GuidIn1S,
                DaysToDepartment = o.DaysToDepartment,
                isReserve = o.isReserve,
                IsDeliveryByTk = o.IsDeliveryByTk,
                CostOfDelivery = o.CostOfDelivery,
                DeliveryDataString = o.DeliveryDataString,
                RangeDeliveryDays = o.RangeDeliveryDays,
                DpdDeliveryStatus = o.DpdDeliveryStatus ,
                SupplierId = o.SupplierId
            }).FirstOrDefault();

            if (model.OrderData == null)  return null;
            
            bool isPriceOfClient= ((isPartner && model.OrderData.PointId == pointId)||!isPartner) ? true : false;
            model.OrderDetails = DbContext.Database.SqlQuery<OrderViewDetail>(sqlText, guid, isPartner, isPriceOfClient).ToList();

                return (model);
        }

        public bool CheckIfCanCancelOrder(Order order, out string errorMessage)
        {

           
            errorMessage = String.Empty;

            if (order == null)
            {
                errorMessage = "Не возможно найти заказ";
                return false;
            }
            if (order.OrderDate < DateTime.Today.AddDays(-Defaults.orderCanChangeInDays)) // начало вчерашнего дня
            {
                errorMessage = "Нельзя отменять заказ датой ранее семи дней";
                return false;
            }
            if (order.GuidIn1S == Guid.Empty || order.GuidIn1S == null)
            {
                errorMessage = "У заказа отсутствует GUID";
                return false;
            }

            return true;
        }

   
        public bool ChancelOrder(Order order)
        {

            if (order != null)
            {
                order.OrderStatus = OrderStatuses.Chancelled;

                DbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public string GetSaleNumberByOrderGuid(Guid guidOrderIn1S)
        {

            Sale sale = DbContext.Sales.FirstOrDefault(o => o.GuidIn1S == guidOrderIn1S);
            if (sale == null) return String.Empty;
            return sale.NumberIn1S;
        }

        public Order GetOrderByGuid(Guid guid)
        {

            return  DbContext.Orders.Include(p=>p.OrderDetails).FirstOrDefault(o => o.GuidIn1S == guid); 
            
        }

        public async Task<Order> GetOrderByGuidAsync(Guid guid)
        {

            return await DbContext.Orders.Include(p => p.OrderDetails).FirstOrDefaultAsync(o => o.GuidIn1S == guid);

        }

        

        /// <summary>
        /// Получить информацию партнера для счета
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public string GetPartnerInfoForBill(string partnerId)
        {
            string result = "";
            Partner partner = DbContext.Partners.FirstOrDefault<Partner>(p => p.PartnerId == partnerId);

            if (partner != null)
                
                result = String.Concat(partner.INN ?? "", ",", partner.FullName ?? "", ",", partner.Address ?? "", ", тел:", partner.PhoneNumber);
            return result;

        }

        /// <summary>
        /// Получить историю заказа
        /// </summary>
        /// <param name="guidOrderIn1S"></param>
        /// <returns></returns>
        public IList<HistoryOfOrderstatus> GetHistoryOfOrderStatuses(Guid guidOrderIn1S)
        {
            return  DbContext.HistoryOfOrderstatuses.Where(p => p.GuidIn1S == guidOrderIn1S).OrderBy(p=>p.Date).ToList();
            
        }


        public async Task<IList<HistoryOfOrderstatus>> GetHistoryOfOrderStatusesAsync(Guid guidOrderIn1S)
        {
            return await DbContext.HistoryOfOrderstatuses.Where(p => p.GuidIn1S == guidOrderIn1S).OrderBy(p => p.Date).ToListAsync();

        }
        

        // партнер = головной терминал
        /// <summary>
        /// Функция возвращает список заказов головного териминала
        /// </summary>
        /// <param name="model"></param>
        /// <param name="partnerId"></param>
        /// <param name="pointId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public void GetListOfOrdersByPartnerId(OrdersViewModel model, string partnerId, int pointId)
        {


            var endDate = model.EndDate.HasValue ? ((DateTime) model.EndDate).AddDays(1).AddTicks(-1) : DateTime.MaxValue;
            Expression<Func<Order, bool>> filter = o => o.PartnerId == partnerId && (model.PointId == null || o.PointId == model.PointId) && /*!o.IsJoined &&*/
                (model.DepartmentId == null || o.DepartmentId == model.DepartmentId) && (model.BeginDate == null || o.OrderDate >= model.BeginDate ) 
                && (model.EndDate == null || o.OrderDate <= endDate) && ((int)o.OrderStatus == model.StatusId || model.StatusId == null) 
                && (model.OrderNumber == null || o.NumberIn1S.Contains(model.OrderNumber) || o.Comments.Contains(model.OrderNumber) )   && (!model.IsDeliveryByTk || o.IsDeliveryByTk) ;

            var filterall = filter;
            Expression<Func<Order, bool>> predicatebyProductId = _ => true;
           if (!String.IsNullOrEmpty(model.ProductName))
            
            {
                predicatebyProductId = o => (o.OrderDetails.Any(p => model.ProductName == null || p.ProductId.ToString().Contains(model.ProductName) || (p.Product.Name != null && p.Product.Name.Contains(model.ProductName)) || (p.Product.Article != null && p.Product.Article.Contains(model.ProductName))));
                 filterall = filter.And(predicatebyProductId);

            }


           model.Orders = DbContext.Orders.Where(filterall).Include(o => o.OrderDetails).Include(i => i.OrderDetails.Select(p => p.Product)).
               Include(p=>p.Point).Include(p=>p.Department)
             .OrderByDescending(o => o.OrderDate).ToPagedList(model.Page, model.ItemsPerPage);
            
        }



        // партнер = головной терминал
        /// <summary>
        /// Функция возвращает список заказов головного териминала
        /// </summary>
        /// <param name="model"></param>
        /// <param name="partnerId"></param>
        /// <param name="pointId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public void GetListOfOrdersByPartnerIdWithGuid(OrdersViewModel model, string partnerId, int pointId)
        {


            var endDate = model.EndDate.HasValue ? ((DateTime)model.EndDate).AddDays(1).AddTicks(-1) : DateTime.MaxValue;
            Expression<Func<Order, bool>> filter = o => o.PartnerId == partnerId && (model.PointId == null || o.PointId == model.PointId) && /*!o.IsJoined &&*/
                (model.DepartmentId == null || o.DepartmentId == model.DepartmentId) && (model.BeginDate == null || o.OrderDate >= model.BeginDate)
                && (model.EndDate == null || o.OrderDate <= endDate) && ((int)o.OrderStatus == model.StatusId || model.StatusId == null)
                && (model.OrderNumber == null || o.NumberIn1S.Contains(model.OrderNumber) || o.Comments.Contains(model.OrderNumber)) && (!model.IsDeliveryByTk || o.IsDeliveryByTk);

            var filterall = filter;
            Expression<Func<Order, bool>> predicatebyProductId = _ => true;
            if (!String.IsNullOrEmpty(model.ProductName))

            {
                predicatebyProductId = o => (o.OrderDetails.Any(p => model.ProductName == null || p.ProductId.ToString().Contains(model.ProductName) || (p.Product.Name != null && p.Product.Name.Contains(model.ProductName)) || (p.Product.Article != null && p.Product.Article.Contains(model.ProductName))));
                filterall = filter.And(predicatebyProductId);

            }

            model.OrdersWithGuid =DbContext.Orders.Where(filterall).Include(o => o.OrderDetails).Include(i => i.OrderDetails.Select(p => p.Product)).
                Include(p => p.Point).Include(p => p.Department).Select(p => new OrderWithGuidLink { Order = p,
                    Guid = DbContext.Sales.Where(s=> p.GuidIn1S==s.GuidOfOrderIn1S).Select(s=>s.GuidIn1S).LastOrDefault()
                }).OrderByDescending(o => o.Order.OrderDate).ToPagedList(model.Page, model.ItemsPerPage);

        }

        /// <summary>
        /// Получить список товаров партнерской точки
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pointId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public void GetListOfOrdersByPointId(OrdersViewModel model, int pointId)
        {

            Expression<Func<Order, bool>> filter = o =>
                    o.PointId == pointId && (model.DepartmentId == null || o.DepartmentId == model.DepartmentId)
                    /*&& !o.IsJoined */
                    && (o.OrderDate > model.BeginDate || model.BeginDate == null) &&
                    (o.OrderDate <= model.EndDate || model.EndDate == null)
                    && ((int)o.OrderStatus == model.StatusId || model.StatusId == null) &&
                    (model.OrderNumber == null || o.Comments.Contains(model.OrderNumber) || o.NumberIn1S.Contains(model.OrderNumber)  ); 
          

             var filterall = filter;
            Expression<Func<Order, bool>> predicatebyProductId = _ => true;
           if (!String.IsNullOrEmpty(model.ProductName))
            
            {
                predicatebyProductId = o => (o.OrderDetails.Any(p => model.ProductName == null ||  p.ProductId.ToString().Contains(model.ProductName) || (p.Product.Name!=null &&   p.Product.Name.Contains(model.ProductName)) || (p.Product.Article!=null && p.Product.Article.Contains(model.ProductName))));
                 filterall = filter.And(predicatebyProductId);

            }
           model.Orders = DbContext.Orders.Where(filterall).Include(o => o.OrderDetails).Include(i => i.OrderDetails.Select(p => p.Product)).
               Include(p => p.Point).Include(p => p.Department).OrderByDescending(o => o.OrderDate).ToPagedList(model.Page, model.ItemsPerPage); 

            
        }


        public bool ChangeOrder(Guid guid, IEnumerable<ItemInOrderViewModel> items, string comments, bool isReserve, bool IsDeliveryByTk, string deliveryDataString, string rangeDeliveryDays, DeliveryInfo di, DateTime? deliveryDate = null)
        {

            var orderToChange = DbContext.Orders.FirstOrDefault(q => q.GuidIn1S == guid);
            if (orderToChange == null) throw new NullReferenceException("order is not found");

            // new inserts
            orderToChange.Comments = comments;
            orderToChange.isReserve = isReserve;
            orderToChange.DeliveryDate = deliveryDate;

            orderToChange.IsDeliveryByTk = IsDeliveryByTk;
            orderToChange.RangeDeliveryDays = rangeDeliveryDays;
            orderToChange.DeliveryDataString = deliveryDataString;
            orderToChange.ContactFIOOfClient = di.ContactFio;
            orderToChange.CostOfDelivery = di.CostOfDelivery;
            orderToChange.PhoneNumberOfClient = di.ContactPhone;

            var OrderItems = DbContext.OrderDetails.Where(c => c.GuidIn1S == guid).ToList();
            foreach (var itemOld in OrderItems)
            {
                var foundItem = items.ToList().FirstOrDefault(x => x.ProductId == itemOld.ProductId);
                if (foundItem == null)
                    DbContext.OrderDetails.Remove(itemOld);
                else if (foundItem.Count != itemOld.Count)
                {
                    itemOld.Count = foundItem.Count;
                    DbContext.Entry(itemOld).State = System.Data.Entity.EntityState.Modified;
                }

            }

            DbContext.SaveChanges();
            orderToChange.CalculateTotals();

            DbContext.SaveChanges();
            return true;
        }
         

       

   
    }
       
}