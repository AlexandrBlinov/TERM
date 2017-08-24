using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using Term.DAL;
using Yst.Context;
using Yst.ViewModels;
using YstProject.Models;
using YstProject.Services;
using OfficeOpenXml;
using System.Threading.Tasks;

namespace Term.Web.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {

        private class OrderForSupplierFirstResult
        {
            public string NumberIn1S { get; set; }
            public Guid GuidIn1S { get; set; }
            public DateTime OrderDate { get; set; }
            public OrderStatuses OrderStatus { get; set; }
            public string ProductName { get; set; }
            public int ProductId { get; set; }
            public string Article { get; set; }
            public int Count { get; set; }
            public int RowNumber { get; set; }
            public int RestOfSupplier { get; set; }
            public string BarCode { get; set; }
        }

        private readonly UserManager<ApplicationUser> _userManager;
         private readonly AppDbContext _dbContext;
        private readonly HttpContextBase _context;
        public SupplierController() : this(new AppDbContext(), new HttpContextWrapper(System.Web.HttpContext.Current)) { }
        public SupplierController(AppDbContext dbContext,HttpContextBase context)
        {
            _dbContext = dbContext;
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            _context = context;
        }

        private int SupplierId {
            get
            {
                var user= _userManager.FindById(_context.User.Identity.GetUserId());
                return user.SupplierId.Value;
                
            }
        }

        /// <summary>
        /// Отобразить все заказы покупателей для данного поставщика
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(int page=1)
        {

            var supplier =_dbContext.Suppliers.FirstOrDefault(p => p.Id == this.SupplierId);

        

            ViewBag.SupplierName = supplier.Name;

              var  resultset = (from order in _dbContext.Orders.Where(p => p.SupplierId == this.SupplierId)
                join orderdetail in _dbContext.OrderDetails on order.GuidIn1S equals orderdetail.GuidIn1S
                from product in
                    _dbContext.Products.Where(pofpart => pofpart.ProductId == orderdetail.ProductId).DefaultIfEmpty()
                from restOfSupplier in _dbContext.RestsOfSuppliers.Where(ros => ros.ProductId == orderdetail.ProductId && ros.SupplierId==this.SupplierId).DefaultIfEmpty()
                  select new OrderForSupplierFirstResult
                {
                  NumberIn1S=  order.NumberIn1S,
                 OrderDate=   order.OrderDate,
                GuidIn1S= order.GuidIn1S,
                OrderStatus= order.OrderStatus,
                 ProductId= product.ProductId,
                    ProductName = product.Name,
                  Count=  orderdetail.Count,
                  Article=  product.Article,
                  RowNumber=  orderdetail.RowNumber,
                  BarCode = product.BarCode,
                  RestOfSupplier = restOfSupplier == null ? 0 : restOfSupplier.Rest
                  
                    
                }).OrderByDescending(p => p.OrderDate).ToList().Select(p=> new OrderForSupplierResult
                {
                    OrderDto = new OrderDto
                    {
                        NumberIn1S = p.NumberIn1S,
                        OrderDate = p.OrderDate,
                        GuidIn1S = p.GuidIn1S,
                        OrderStatus = p.OrderStatus

                        
                    },
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Count = p.Count,
                    Article = p.Article,
                    RowNumber = p.RowNumber,
                    RestOfSupplier =p.RestOfSupplier,
                    BarCode =p.BarCode

                }).GroupBy(p=>p.OrderDto).ToPagedList(page,10);
            

            return View(resultset);
        }


        protected override void Initialize(RequestContext requestContext)
        {
            
            var CultureKey = "Culture";

            var request = requestContext.HttpContext.Request;

            var culture = Defaults.Culture_RU ;

            var currentThread = System.Threading.Thread.CurrentThread;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);

            if (!cultureInfo.Equals(currentThread.CurrentCulture)) currentThread.CurrentCulture = cultureInfo;
            if (!cultureInfo.Equals(currentThread.CurrentUICulture)) currentThread.CurrentUICulture = cultureInfo;


            HttpCookie cultureCookie = request.Cookies[CultureKey];
            if (cultureCookie == null || cultureCookie.Value.CompareTo(culture) != 0)
            {
                var newCookie = new HttpCookie(CultureKey, culture);
                newCookie.Expires = DateTime.Now.AddMinutes(30);
                requestContext.HttpContext.Response.Cookies.Add(newCookie);

            } 
            base.Initialize(requestContext);
        }

        public async Task ImportOrderToExcel(Guid guid)
        {
            var products = (from order in _dbContext.Orders.Where(p => p.GuidIn1S == guid)
                            join orderdetail in _dbContext.OrderDetails on order.GuidIn1S equals orderdetail.GuidIn1S
                            from product in
                                _dbContext.Products.Where(pofpart => pofpart.ProductId == orderdetail.ProductId).DefaultIfEmpty()
                            from restOfSupplier in _dbContext.RestsOfSuppliers.Where(ros => ros.ProductId == orderdetail.ProductId && ros.SupplierId == this.SupplierId).DefaultIfEmpty()
                            select new OrderForSupplierFirstResult
                            {
                                NumberIn1S = order.NumberIn1S,
                                OrderDate = order.OrderDate,
                                GuidIn1S = order.GuidIn1S,
                                OrderStatus = order.OrderStatus,
                                ProductId = product.ProductId,
                                ProductName = product.Name,
                                Count = orderdetail.Count,
                                Article = product.Article,
                                RowNumber = orderdetail.RowNumber,
                                BarCode = product.BarCode,
                                RestOfSupplier = restOfSupplier == null ? 0 : restOfSupplier.Rest


                            }).ToArray();
            byte[] result;

            using (var pck = new ExcelPackage())
            {
                ExcelWorksheet wsOrder = pck.Workbook.Worksheets.Add(products[0].NumberIn1S);

                wsOrder.Cells["A2"].Value = "Заказ " + products[0].NumberIn1S + " от " + products[0].OrderDate;
                wsOrder.Cells["B5:E5"].AutoFilter = true;
                wsOrder.Cells["B5"].Value = "Артикул";
                wsOrder.Cells["C5"].Value = "Штрих-код";
                wsOrder.Cells["D5"].Value = "Наименование";
                wsOrder.Cells["E5"].Value = "Кол-во в заказ";
                int i = 6;

                foreach (var item in products)
                {
                    wsOrder.Cells[i, 2].Value = item.Article;

                    wsOrder.Cells[i, 3].Value = item.BarCode;
                    wsOrder.Cells[i, 4].Value = item.ProductName;
                    wsOrder.Cells[i, 5].Value = item.Count;
                    i++;
                }

                for (i = 2; i < 7; i++)
                    wsOrder.Column(i).AutoFit();

                result = pck.GetAsByteArray();
            }
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + products[0].NumberIn1S + ".xlsx");
            Response.BinaryWrite(result);
        }

        /// <summary>
        /// Модель заказа для отображения
        /// </summary>
        public class OrderForSupplierResult
    {
        public OrderDto OrderDto { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string Article { get; set; }
        public int Count { get; set; }
        public int RowNumber { get; set; }
        public int RestOfSupplier { get; set; }
            public string BarCode { get; set; }
    }
        /// <summary>
        /// 
        /// </summary>
    public struct OrderDto
    {
        public string  NumberIn1S { get; set; }
        public Guid GuidIn1S { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatuses OrderStatus { get; set; }
    }



        
}

   
}
