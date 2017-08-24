
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Term.DAL;
using Yst.ViewModels;
using Yst.Context;
using Yst.Services;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections;
//using YstProject.WebReferenceTerm;
using System.Threading.Tasks;
using YstProject.Services;
using YstTerm.Models;
using System.Net;
using System.Configuration;
using System.Data.Entity;
using YstProject.Models;
using System.Diagnostics;
using Term.Soapmodels;
using Term.Services;
using Term.Web.Views.Resources;

#if !not_compile
namespace Term.Web.Controllers
{
    /// <summary>
    ///     
    /// </summary>
    public class SeasonShoppingCartController : BaseController
    {

        private readonly HttpContextBase _httpContext;
        
        private readonly ServicePartnerPoint _servicePP;
        private readonly ProductService _productService;
        private readonly SeasonShoppingCart _seasonShoppingCartService;
        private readonly SoapServiceForSeasonOrders _soapServiceForSeasonOrders;

        private readonly string stubDepartment = Defaults.StubDepartmentCode;
        static int _minMonthes = 2, max_monthes = 10;
        readonly string culturesToRestrictSeasonProducts = ConfigurationManager.AppSettings["CulturesToRestrictSeasonProducts"];
        
        

        public SeasonShoppingCartController(ServicePartnerPoint servicePP, SeasonShoppingCart shoppingCart, 
            ProductService productService, HttpContextBase httpContext,
            SoapServiceForSeasonOrders soapServiceForSeasonOrders)
        {
            _servicePP = servicePP;
            _seasonShoppingCartService = shoppingCart;
            _productService = productService;
            _httpContext = httpContext;
           
            _soapServiceForSeasonOrders = soapServiceForSeasonOrders;
            
            _soapServiceForSeasonOrders.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginWS"], ConfigurationManager.AppSettings["PasswordWS"]);
          
            
        }

        public SeasonShoppingCartController():this (new ServicePartnerPoint(),new SeasonShoppingCart(),new ProductService(),new  HttpContextWrapper(System.Web.HttpContext.Current),  new SoapServiceForSeasonOrders()) {}

        /// <summary>
        /// Получаем всю необходимую информацию о товарах в корзине
        /// </summary>
        /// <returns></returns>
        private  SeasonCartViewModel FillInSeasonCartModel() {

            var viewModel = new SeasonCartViewModel();
            FillInSeasonCartModel(viewModel);
            
            return viewModel;
        }
        #region Public Members

        private void FillInSeasonCartModel(SeasonCartViewModel model)
        { 
         model.CartItems = _seasonShoppingCartService.GetCartItems();
         model.CartTotal = _seasonShoppingCartService.GetTotalsByProperty(p => p.Price * p.Count);
         model.CartVolume = _seasonShoppingCartService.GetTotalsByProperty(p => p.Product.Volume * p.Count);
         model.CartWeight = _seasonShoppingCartService.GetTotalsByProperty(p => p.Product.Weight * p.Count);
         model.CartCount = (int)_seasonShoppingCartService.GetTotalsByProperty(p => p.Count);
        }
        /// <summary>
        /// Отображение корзины, товары разбиты по закладкам - производителям
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index([ModelBinder(typeof(SeasonCartViewModelBinder))]SeasonCartViewModel viewModel)
        {
            bool isForeign = _servicePP.IsForeignPartner;
            viewModel.IsForeign = isForeign;
            ViewBag.ShowSeasonCart = true;
          // var viewModel = FillInSeasonCartModel();

            FillInSeasonCartModel(viewModel);
            viewModel.ItemsByWheelType = viewModel.CartItems.GroupBy(p => p.Product.WheelType).ToDictionary(p => p.Key, q=>q.AsEnumerable<SeasonCart>());


            if (isForeign)
            {   
                viewModel.Display = viewModel.Display ?? DisplaySeasonCart.GroupedByFactory;
             
                if (viewModel.Display == DisplaySeasonCart.GroupedByFactory)
                {
                    viewModel.ItemsByWheelTypeAndFactory = viewModel.CartItems.GroupBy(p => p.Product.WheelType).ToDictionary(p => p.Key, q => q.AsEnumerable<SeasonCart>().GroupBy(p => p.Factory).ToDictionary(k => k.Key, k => k.AsEnumerable<SeasonCart>()));
                    return View("GroupedByFactory", viewModel);
                }
            }
            // Return the view for russian partner
            return View(viewModel) ;
        }

        /// <summary>
        /// Create via web service collection of season orders
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("createorder")]
        public ActionResult CreateSeasonOrders([ModelBinder(typeof(SeasonCartViewModelBinder))] SeasonCartViewModel viewModel)
        {
             bool isForeign = Point.Partner.IsForeign;
            ViewBag.ShowSeasonCart = true;
            ViewBag.IsForeignPartner = Point.Partner.IsForeign;

            // check DeliveryDate
            if (!isForeign)
            {
                if (!viewModel.DeliveryDate.HasValue) ModelState.AddModelError("", SeasonOrdersTexts.IndicateDate);

                else
                {
                    DateTime minDate = DateTime.Now.AddMonths(_minMonthes); DateTime maxDate = DateTime.Now.AddMonths(max_monthes);


                    if ((DateTime)viewModel.DeliveryDate < minDate || (DateTime)viewModel.DeliveryDate > maxDate) ModelState.AddModelError("", String.Format("{2} {0} - {1}", minDate.ToShortDateString(), maxDate.ToShortDateString(), SeasonOrdersTexts.DateValidity));
                }
            }

            FillInSeasonCartModel(viewModel);

            if (viewModel.CartCount == 0) return RedirectToAction("Index");

            if (!ModelState.IsValid)
            { 
             
             viewModel.ItemsByWheelType = viewModel.CartItems.GroupBy(p => p.Product.WheelType).ToDictionary(p => p.Key, q => q.AsEnumerable<SeasonCart>());


             if (isForeign)
             {
                 
                 viewModel.Display = viewModel.Display ?? DisplaySeasonCart.GroupedByFactory;
                 
                 if (viewModel.Display == DisplaySeasonCart.GroupedByFactory)
                 {
                     viewModel.ItemsByWheelTypeAndFactory = viewModel.CartItems.GroupBy(p => p.Product.WheelType).ToDictionary(p => p.Key, q => q.AsEnumerable<SeasonCart>().GroupBy(p => p.Factory).ToDictionary(k => k.Key, k => k.AsEnumerable<SeasonCart>()));
                     return View("GroupedByFactory", viewModel);
                 }
             }
             return View("Index", viewModel);
            }

            /////////////////
            //If model is valid we invoke soap service and create order in local db
            /////////////////

            string partnerId = Point.PartnerId;

            

             var products = _seasonShoppingCartService.GetCartItems(viewModel.ActiveWheelType).Select(p => new SoapProduct { Code = p.Product.ToString(), Quantity = p.Count, Storage = p.Factory }).ToArray();

            
             // if partner is foreign datetoPass =00:00:00.0000000 UTC, January 1, 0001, in the Gregorian calendar
             var datetoPass = isForeign ? DateTime.MinValue : (DateTime)viewModel.DeliveryDate;
             ResultSeasonOrder soapResult;
            try
            {
                // теперь все турецкие заказы у нас в базе, раньше был .SetAlternateUrl()
              //  if(culturesToRestrictSeasonProducts.Contains(Point.Partner.Culture??Defaults.Culture_RU)) _soapServiceForSeasonOrders.SetAlternateUrl();

                soapResult = _soapServiceForSeasonOrders.CreateSeasonOrder(partnerId, Point.PartnerPointId,products, stubDepartment, datetoPass, viewModel.Comments ?? String.Empty, String.Empty);
                if (!soapResult.Success) throw new Exception(soapResult.Error);

                }
            catch (Exception exc)
            {
                ErrorLogger.Error(exc);
                throw;

            }

            IEnumerable<SeasonOrder> modelOrders;
            // if foreign - separate one order into small orders
            if (isForeign)
            {
                viewModel.DeliveryDate = DateTime.Now; // stub
                var orderGuids = _seasonShoppingCartService.CreateOrdersInLocalDb(soapResult, viewModel);
                 modelOrders = DbContext.Set<SeasonOrder>().Include(b => b.OrderDetails.Select(p => p.Product)).Where(p => orderGuids.Any(o => o == p.OrderGuid)).AsEnumerable<SeasonOrder>();
                

               
            }
            else
            {
                var orderguid = _seasonShoppingCartService.CreateOneOrderInLocalDb(soapResult, viewModel);
                 modelOrders = DbContext.Set<SeasonOrder>().Include(b => b.OrderDetails.Select(p => p.Product)).Where(p => p.OrderGuid == orderguid).AsEnumerable<SeasonOrder>();

            }

            return View("OrdersCreated", modelOrders);

        }



        /// <summary>
        /// Обновляем товары в корзине через Ajax
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <returns></returns>
 
        [HttpPost]
        public ActionResult UpdateToCart(int id, int count = 1)
        {
            
            bool success = true;
            string message = String.Empty;

            decimal price;
            
            
            _seasonShoppingCartService.UpdateItemCount(id, count,out price);

            var factory  =_seasonShoppingCartService.GetFactoryByProduct(id);

            int count_by_factory = _seasonShoppingCartService.GetCountByFactory(factory);
            decimal sum_by_factory =_seasonShoppingCartService.GetSumByFactory(factory);

            var viewModel = FillInSeasonCartModel();

            return Json(new { success, 
                ItemPrice = price, 
                CartCount = viewModel.CartCount, 
                CartTotal = viewModel.CartTotal, 
                CartWeight = viewModel.CartWeight, 
                CartVolume = viewModel.CartVolume,
                              CountByFactory = count_by_factory,
                              SumByFactory = sum_by_factory
            });

        }

        // добавляем товары в корзину
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ProductId</param>
        /// <param name="count">Amount of products</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddToCart(int id, int count = 1)
        {
            string message = null;
            
            string partnerId = Point.PartnerId;
                       

            var productToAdd = _productService.GetProduct(id);
            if (productToAdd == null) message = SeasonOrdersTexts.Error_NoProductFoundByCode;

            if (String.IsNullOrEmpty(productToAdd.Factory))  message =SeasonOrdersTexts.Error_NoFactoryForProduct;

            _seasonShoppingCartService.CheckIfItemCanBeAddedToCart(id, count, ref message, partnerId);

            if (message != null)
                return Json(new SeasonShoppingCartAddRemoveViewModel
                {
                    CartTotal = 0,
                    CartCount = 0,
                    Message = message,
                    Success = false
                });
          

            var price = _productService.GetPriceOfProduct(id, partnerId);


            _seasonShoppingCartService.AddToCart(productToAdd, ref count, price);
            message = String.Format(ForSearchResult.MsgAddToCart1 + " {0} " + ForSearchResult.MsgAddToCart2 + " " + ForSearchResult.MsgAddToCart3 + " {1} ", productToAdd.Name, count);

            var results = new SeasonShoppingCartAddRemoveViewModel
            {
                Message = message,
                CartTotal = _seasonShoppingCartService.GetTotal(),
                CartCount = _seasonShoppingCartService.GetCount(),
                Success = true
            };
          
            return Json(results);
        }

        

       // удаляем из корзины
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {

            string Message = null;
            bool success = true;

            var removedProduct = _productService.GetProduct(id);


            int itemCount = 0;

            if (removedProduct != null)
            {
                itemCount = _seasonShoppingCartService.RemoveFromCart(id);
                Message = Server.HtmlEncode(removedProduct.Name) + " " + ForSearchResult.MsgDelFromCart1;
            }
            else
            {
                Message = ForSearchResult.MsgDelFromCart2;
                success = false;
                return Json(new {Success=success,Message=Message});
            }

            

            var viewModel=FillInSeasonCartModel();

            var factory = _seasonShoppingCartService.GetFactoryByProduct(id);

            int countByFactory = _seasonShoppingCartService.GetCountByFactory(factory);
            decimal sumByFactory = _seasonShoppingCartService.GetSumByFactory(factory);


           return  (Json(
            new{
                Message = Message,
                CartTotal = viewModel.CartTotal,
                CartCount = viewModel.CartCount,
                CartWeight = viewModel.CartWeight,
                CartVolume = viewModel.CartVolume,
                Success = success,
                  CountByFactory = countByFactory,
                              SumByFactory = sumByFactory
            }));


            

        }
        #endregion

        //[ChildActionOnly]
        public ActionResult CartSummary()
        {
           var cart = _seasonShoppingCartService;
           
         
           ViewData["CartCount"] = cart.GetCount();
           ViewData["CartTotal"] = cart.GetTotal();

            return PartialView("CartSummary");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_servicePP != null) _servicePP.Dispose();
                if (_productService != null) _productService.Dispose();
          

            }
            base.Dispose(disposing);
        }

    }
}
#endif