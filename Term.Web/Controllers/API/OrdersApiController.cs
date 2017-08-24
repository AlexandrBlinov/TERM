using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Yst.Context;
using YstProject.Models;
using Term.DAL;
using Term.Services;
using Term.Web.Filters;
using Yst.ViewModels;
using YstProject.Services;

namespace Term.Web.Controllers.API
{
    /// <summary>
    /// Gets all orders for sync with 1S
    /// </summary>
    public class OrdersApiController : ApiController
    {

        private  ServiceTerminal _ws;
        private readonly ILogger _logger;

        public struct OrderGuidWithStatusDto
        {
            public Guid Guid { get; set; }
            public OrderStatuses Status { get; set; }
        }

        readonly AppDbContext _dbContext;

        public OrdersApiController() : this(new AppDbContext(), new Logger()) { }
        public OrdersApiController(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected ServiceTerminal WS
        {
            get
            {
                return _ws ?? (_ws =
                 new ServiceTerminal
                 {
                     PreAuthenticate = true,
                     Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginWS"],
                         ConfigurationManager.AppSettings["PasswordWS"])
                 });



            }

        }

        [NonAction]
        private IQueryable<OrderNumberWithStatusDTO> GetDocuments(DateTime beginDate, DateTime endDate, Expression<Func<Order, bool>> predicate = null)
        {
            var query = _dbContext.Set<Order>().Where(o => o.OrderDate >= beginDate && o.OrderDate <= endDate);
            if (predicate != null) query = query.Where(predicate);
            return query.Select(p => new OrderNumberWithStatusDTO { DateInternal = p.OrderDate, NumberIn1S = p.NumberIn1S, Totals = p.Total,OrderStatus=(int)p.OrderStatus });

        }

        public IQueryable<OrderNumberWithStatusDTO> Get([FromUri]DateTime beginDate, [FromUri] DateTime endDate)
        {


            endDate = endDate.AddDays(1).AddTicks(-1);

            return GetDocuments(beginDate, endDate);


        }

/*
        public Order Get(Guid guid)
        {
           return  _dbContext.Set<Order>().Include(p => p.OrderDetails).FirstOrDefault(o=>o.GuidIn1S==guid);


        }
 */ 

        public IQueryable<OrderNumberWithStatusDTO> GetByPartner([FromUri]DateTime beginDate, [FromUri] DateTime endDate, string partnerId)
        {

            // приводим дату к концу текущего дня
            endDate = endDate.AddDays(1).AddTicks(-1);

            return this.GetDocuments(beginDate, endDate, o => o.PartnerId.Equals(partnerId));

        }

        /// <summary>
        /// Загрузка заказов из 1С
        /// </summary>
        /// <returns></returns>
         [AdminHashAuth]
        public async Task<HttpResponseMessage> ImportOrders()
        {
            int result = 0;
            string errorMsg;

            var stream=await Request.Content.ReadAsStreamAsync();
            

            var parameters = new [] {
               
                new SqlParameter{ParameterName="@xmlData",SqlDbType=SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml( stream)},
                new SqlParameter { ParameterName="@b",SqlDbType=SqlDbType.Int, Direction=ParameterDirection.ReturnValue },
               new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output }};

            result = SPExecutor.Execute("spImportOrders", parameters, out errorMsg);
            
            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error  " + errorMsg) ,StatusCode = HttpStatusCode.InternalServerError};
            
            return new HttpResponseMessage { Content = new StringContent(result.ToString())};
        }

        /// <summary>
        /// Изменение статуса заказа покупателя из рабочего места поставщика 
        /// http://localhost:9090/api/ordersapi/changestatusofsupplier
        /// {'guid':'9f8328f2-d3be-11e4-b12b-d4ae52b5e909'
        ///,status:11
        /// }
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public void ChangeStatusOfSupplier([FromBody]OrderGuidWithStatusDto model)
        {

            var messageIfWrongStatus = @"status must be BeingConfirmedBySupplier";
            var orderFound=_dbContext.Orders.Find(model.Guid);
            if (orderFound == null) throw new NullReferenceException("order is not found");

            if ((orderFound.OrderStatus == OrderStatuses.BeingConfirmedBySupplier) &&
                (model.Status == OrderStatuses.Chancelled || model.Status == OrderStatuses.CancelledBySupplier ||
                 model.Status == OrderStatuses.BeingDeliveredToStockFromSupplier))

            {
                try
                {
                    var result = WS.ChangeOrderStatus(model.Guid.ToString(), (int)model.Status);


                    if (result.Success)
                    {
                        orderFound.OrderStatus = model.Status;
                        // отражаем статус для понимания что нажал поставщик и когда
                        orderFound.StatusOfSupplier = model.Status == OrderStatuses.CancelledBySupplier
                            ? StatusForOrderItemOfSupplier.Rejected
                            : StatusForOrderItemOfSupplier.Confirmed;
                        orderFound.DateProcessedBySupplier = DateTime.Now;
                        _dbContext.SaveChanges();
                        return;
                    }
                }
                catch (Exception exc)
                {
                    _logger.Error(exc.ToString());
                    throw;
                }
                
            }
            _logger.Error(
                String.Format("{0} {1} {2}", messageIfWrongStatus, orderFound, model.Status));
                
            throw new ArgumentException(messageIfWrongStatus);
            
        }


        /// <summary>
        /// Получить все заказы которые должны быть отгружены через DPD 
        /// по заявке отдела логистики
        /// </summary>
        /// http://localhost:9090/api/ordersapi/GetOrdersWithDpdDelivery?beginDate=2016-11-01&endDate=2017-02-01
        /// <returns></returns>
        [HttpGet]

        public IEnumerable<OrderViewWithDetails> GetOrdersWithDpdDelivery([FromUri]DateTime beginDate,[FromUri] DateTime endDate)
        {

            endDate = endDate.AddDays(1).AddTicks(-1);

            return _dbContext.Orders.Where(o => o.IsDeliveryByTk && (beginDate == null || beginDate <= o.OrderDate) && (endDate == null || endDate >= o.OrderDate)).Include(od => od.OrderDetails).Select(o => new OrderViewWithDetails
            {
                OrderData = new OrderViewModel
                {
                    NumberIn1S = o.NumberIn1S,
                    Comments = o.Comments,
                    OrderStatus = o.OrderStatus,
                    Total = o.Total,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    ContactFIOOfClient = o.ContactFIOOfClient,
                    PhoneNumberOfClient = o.PhoneNumberOfClient,
                    Order_guid = o.GuidIn1S,
                    DaysToDepartment = o.DaysToDepartment,
                    CostOfDelivery = o.CostOfDelivery,
                    DeliveryDataString = o.DeliveryDataString,
                    RangeDeliveryDays = o.RangeDeliveryDays,
                    
                },
                OrderDetails = o.OrderDetails.Select(p=>new OrderViewDetail {
                    ProductId = p.ProductId,
                    RowNumber = p.RowNumber,
                    PriceOfClient = p.PriceOfClient,
                    ProductName = _dbContext.Products.FirstOrDefault(prod => prod.ProductId == p.ProductId).Name??"",
                    Price = p.Price,
                     Count = p.Count
            
            }).ToList()
                

            }).OrderBy(p=>p.OrderData);
        } 



        

    }
}
