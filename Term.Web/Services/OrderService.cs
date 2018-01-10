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
    public class OrderService : BaseService
    {

        /// <summary>
        /// получить адреса самодоставки из поля Partners SelfDeliveryAddresses, разделенного ;
        /// </summary>
        /// <returns></returns>
        public SelectList SelfDeliveryAddresses
        {
            get {
                var partnerId = this.CurrentPoint?.PartnerId;
                string sda = DbContext.Partners.FirstOrDefault(p => p.PartnerId == partnerId)?.SelfDeliveryAddresses;
                string[] ids = { };
                if (sda != null)
                {
                    ids = sda.Split(Defaults.Semicolon);
                    if (ids.Any())
                    {
                        return new SelectList(DbContext.SelfDeliveryAddresses.Where(p => ids.Contains(p.Id)).Select(p => new
                        {
                            Id = p.Id,
                            Name = p.Name

                        }).OrderBy(p => p.Name).ToList(), "Id", "Name");
                    }
                }
                return new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        /// <summary>
        
        /// Получить адреса доставки для  партнера или точки
        /// </summary>
        public SelectList AddressesOfDelivery {

            get
            {
                var partnerId = this.CurrentPoint?.PartnerId;
                var pointId = this.CurrentPoint?.PartnerPointId; // текущий номер точки

                // Если головной терминал то список точек
                Expression<Func<AddressOfPartner, bool>> predicateForMain = p => p.PartnerId == partnerId && p.Active;


                bool notIsMain = !base.IsPartner && partnerId != null && pointId.HasValue;

                // если не головной терминал
                if (notIsMain)
                {
                    // точка головного терминала
                    var partnerPointId = DbContext.Users.FirstOrDefault(p => p.PartnerId == partnerId && p.IsPartner)?.PartnerPointId;

                    // адрес самой точки, пустой или равный головному терминалу
                    // то есть адреса не принадлежащие другим точкам
                    Expression<Func<AddressOfPartner, bool>> predicate = p =>
                    p.PartnerId == partnerId && p.Active &&
                    (p.PointId == pointId || p.PointId == null || (partnerPointId != null && p.PointId == partnerPointId));

                    var list2 = DbContext.AddressOfPartners.Where(predicate).ToList();

                    if (list2.Any()) // если есть точки то выгружаем

                        return new SelectList(list2.Select(p => new
                        {
                            Id = p.AddressId,
                            Name = p.Address

                        }).OrderBy(p => p.Name).ToList(), "Id", "Name");

                }


                var list = DbContext.AddressOfPartners.Where(predicateForMain).ToList();

                if (list.Any())
                {

                    return new SelectList(list.Select(p => new
                    {
                        Id = p.AddressId,
                        Name = p.Address

                    }).OrderBy(p => p.Name).ToList(), "Id", "Name");
                }


                return new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

       /// <summary>
       /// Получить транспортные компании
       /// </summary>
       /// <returns></returns>
        public SelectList TkIds => new SelectList(DbContext.TransportCompanies.Select(p => new
                { Id = p.Id, Name = p.Name    }).OrderBy(p => p.Name).ToList(), "Id", "Name");

        /// <summary>
        /// Получить адрес точки который будет адресом по умолчанию в списке
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public string GetDefaultAddressId(string partnerId, int pointId)
        {
            var result = DbContext.AddressOfPartners.FirstOrDefault(p => p.PartnerId == partnerId && p.Active && p.PointId == pointId)?.AddressId;

            // если результат пустой то выбираем c более широким условием
            if (result == null) result = DbContext.AddressOfPartners.FirstOrDefault(p => p.PartnerId == partnerId && p.Active)?.AddressId;

            return result;
            
            

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
                Order= GetOrderByGuid(guid)
                
            };


            return model;
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
            
            
            var model = new OrderViewWithDetailsExtended
            {
                Order = GetOrderByGuid(guid)
               
             };
            var order = model.Order;

            model.Total = isPartner ? order.Total : order.TotalOfPoint;
          
            bool isPriceOfClient = ((isPartner && order.PointId == pointId) || !isPartner);

            model.TotalOfClient = isPriceOfClient ? order.TotalOfClient : order.TotalOfPoint;

            model.OrderDetails = DbContext.Database.SqlQuery<OrderViewDetail>(sqlText, guid, isPartner, isPriceOfClient).ToList();

            model.AddressOfDelivery = order.AddressId != null ? DbContext.AddressOfPartners.FirstOrDefault(p => p.AddressId == order.AddressId && p.PartnerId == partnerId)?.Address :null;

            model.AddressId = order.AddressId;

            model.TkId = order.TkId;

            model.WayOfDelivery = order.WayOfDelivery;
            
            
            

            return (model);
        }


        /// <summary>
        /// Проверка можно ли отменить заказы
        /// </summary>
        /// <param name="order"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>

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

   /// <summary>
   /// Отменить заказ в базе
   /// </summary>
   /// <param name="order"></param>
   /// <returns></returns>
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

        /// <summary>
        /// Изменить заказ в базе данных
        /// </summary>        
        
        public void ChangeOrder(Guid guid, IEnumerable<ItemInOrderViewModel> items, string comments, bool isReserve, bool IsDeliveryByTk, 
            string deliveryDataString, string rangeDeliveryDays, DeliveryInfo di, DateTime? deliveryDate , 
            int wayOfDelivery, bool isStar, string AddressId, string TkId)
        {

            var orderToChange = DbContext.Orders.FirstOrDefault(q => q.GuidIn1S == guid);
            if (orderToChange == null) throw new NullReferenceException("order is not found");

            // new inserts
            orderToChange.Comments = comments;
            orderToChange.isReserve = isReserve;
            orderToChange.DeliveryDate = deliveryDate;
            orderToChange.WayOfDelivery = wayOfDelivery;
            orderToChange.AddressId = !isReserve && wayOfDelivery == (int)WaysOfDelivery.Delivery ? AddressId:null; // обнуляем id адреса             
            orderToChange.IsStar = isStar;

            orderToChange.TkId = !isReserve && wayOfDelivery == (int)WaysOfDelivery.ByTk? TkId: null;

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
         
        }
         

       

   
    }
       
}