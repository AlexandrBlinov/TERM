
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yst.Context;
using Yst.Services;
using YstProject.Services;
using Term.DAL;
using System.Data.Entity;
using YstTerm.Models;
//using YstProject.WebReferenceTerm;
using System.Net;
using System.Configuration;
using System.Threading.Tasks;
using PagedList;
using Term.Utils;
using Term.Soapmodels;
using Term.Services;



#if !not_compile
namespace Term.Web.Controllers
{
 /// <summary>
 /// Manage season orders
 /// </summary>
   
    public class SeasonOrdersController : BaseController
 {
     private static readonly string _errorOrdernotFound = @"order not found";
       static int daysClientMayChangeOrder = 14;
       private SeasonOrderAnalizerService _soas;
        private ProductService _productService;
        private SoapServiceForSeasonOrders _soapServiceForSeasonOrders;

        private readonly string culturesToRestrictSeasonProducts = ConfigurationManager.AppSettings["CulturesToRestrictSeasonProducts"];

        public SeasonOrdersController(): this (new SoapServiceForSeasonOrders(),new SeasonOrderAnalizerService() ,new ProductService())        {        }

        public SeasonOrdersController(SoapServiceForSeasonOrders soapServiceForSeasonOrders, SeasonOrderAnalizerService soas, ProductService productService)
        {
            this._productService=productService;
            
            this._soas = soas;
            this._soapServiceForSeasonOrders = soapServiceForSeasonOrders;
            _soapServiceForSeasonOrders.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginWS"], ConfigurationManager.AppSettings["PasswordWS"]);
        
        }

        private SeasonOrder GetOrderByGuid(Guid guid, string partnerId)
        {
            return  DbContext.Set<SeasonOrder>().Where(so => so.OrderGuid == guid && so.PartnerId == partnerId).Include(p => p.OrderDetails).FirstOrDefault();
        
        }

        
        /// <summary>
        /// Отображает список сезонных заказов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Index(SeasonOrderViewModel model)
        {

            bool isForeign = ServicePP.IsForeignPartner;

            string partnerId = this.ServicePP.GetPartnerId();

            var query= DbContext.Set<SeasonOrder>().Include(p => p.OrderDetails).Where(so => so.PartnerId == partnerId && so.FromOnWay==model.FromOnWay);
            if (model.BeginDate.HasValue) query = query.Where(so => so.OrderDate >= model.BeginDate);
            if (model.EndDate.HasValue) query = query.Where(so => so.OrderDate <= model.EndDate);
            if (!String.IsNullOrEmpty(model.OrderNumber)) query = query.Where(so => so.NumberIn1S.Contains(model.OrderNumber));

            if (model.OrderStatus.HasValue && (int)model.OrderStatus > 0) query = query.Where(so => so.OrderStatus == model.OrderStatus);
            
            model.SeasonOrders=query.OrderByDescending(o => o.OrderDate).ToPagedList(1, 100);

            model.OrderStatuses = EnumDescriptionProvider.GetSelectListFromEnum<SeasonOrderStatus>();

            ViewBag.isForeign = isForeign;
         
            return View(model);
        
        }



        private Func<SeasonOrder,bool> UserCanChangeSeasonOrder = order => order.OrderStatus != SeasonOrderStatus.Cancelled && (DateTime.Now.Date - order.OrderDate.Date).Days <= daysClientMayChangeOrder;

        /// <summary>
        ///  Отображает конкретный сезонный заказ
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Details(Guid guid)
        {
          //  string partnerId = this.ServicePP.getPartnerId();

            string partnerId = Partner.PartnerId;
            var order=GetOrderByGuid(guid, partnerId);

            if (order == null) throw new HttpException(404, _errorOrdernotFound);

            ViewBag.CanModify = UserCanChangeSeasonOrder(order); 
            ViewBag.IsForeign = ServicePP.IsForeignPartner;
            
                return View( order);
        }

        

        /// <summary>
        /// Анализ конкретного  сезонного заказа
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Analyse(Guid guid)
        {
            string partnerId = this.ServicePP.GetPartnerId();
            var order = GetOrderByGuid(guid, partnerId);

            if (order == null) throw new HttpException(404, _errorOrdernotFound);
            var model =_soas.Analize(guid.ToString());

            
         
        return PartialView(model);
        
        }

        /// <summary>
        /// Анализ сезонного заказа для печати
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult AnalysePrint(Guid guid)
        {
            string partnerId = this.ServicePP.GetPartnerId();
            var order = GetOrderByGuid(guid, partnerId);


            if (order == null) throw new HttpException(404, _errorOrdernotFound);
            var model = _soas.Analize(guid.ToString());
            ViewBag.NumberIn1S = order.NumberIn1S;
            ViewBag.Orderdate = order.OrderDate.ToString(Defaults.DateFormat);

            
            return View(model);

        }

        /// <summary>
        /// При редактировании сезонного заказа
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid guid)
        
        {
            string partnerId = this.ServicePP.GetPartnerId();
            var order = GetOrderByGuid(guid, partnerId);
            ViewBag.IsForeign = ServicePP.IsForeignPartner;

            if (order != null && UserCanChangeSeasonOrder(order)) return View(order);


            throw new HttpException(404, "Not found");

        }


        /// <summary>
        /// Update items in order post via ajax
        /// </summary>
        /// <param name="seasonOrderChanged"></param>
        /// <returns>Json {Success,Message}</returns>
        [HttpPost]
        public JsonResult Edit(SeasonOrderChangedDTO seasonOrderChanged)
        {
            
            // 14 days since order was created is deadline

             string partnerId = ServicePP.GetPartnerId();
             bool isForeign = ServicePP.IsForeignPartner;

           var order=GetOrderByGuid(seasonOrderChanged.OrderGuid, partnerId);

           if (order == null) return Json(new { Success = false, Message = _errorOrdernotFound });

            if ((DateTime.Now - order.OrderDate).Days > daysClientMayChangeOrder) return Json(new { Success = false, Message = String.Format("One can't change order more than {0} days after creation", daysClientMayChangeOrder) });

            int rowNumber = 0;
            var orderdetails  = (from detailschanged in seasonOrderChanged.SeasonOrderDetailsDTO
                        join details in order.OrderDetails on detailschanged.ProductId equals details.ProductId
                                                            select new SeasonOrderDetail { OrderGuid = seasonOrderChanged.OrderGuid, RowNumber = ++rowNumber, ProductId = detailschanged.ProductId, Count = detailschanged.Count, Price = details.Price }).ToList();

            ///
            if (orderdetails.Any())
            {

                var products = orderdetails.Select(p => new SoapProduct { Code = p.ProductId.ToString(), Quantity = p.Count, Storage = "A"}).ToArray();

                try
                {
                 var   soapResult = _soapServiceForSeasonOrders.CreateSeasonOrder(partnerId, ServicePP.getPointID(), products, Defaults.StubDepartmentCode, (DateTime)order.DeliveryDate, order.Comments ?? String.Empty, order.OrderGuid.ToString());
                    
                    if (!soapResult.Success) throw new Exception(soapResult.Error);
                }
                catch (Exception exc)
                {
                    ErrorLogger.Error(exc);
                    return Json(new { Success = false, Message = "Error invoking web service " + exc.Message });

                }


                try
                {                  
                    order.OrderDetails = orderdetails;
                    order.Total = order.OrderDetails.Sum(p => p.Count * p.Price);
                    DbContext.SaveChanges();
                }
                catch (Exception exc)
                {
                    ErrorLogger.Error(exc);
                    return Json(new { Success = false, Message = "Error updating database.  "+ exc.Message });
                
                }

                return Json(new { Success = true, Message = "Season order was successfully updated", OrderGuid = order.OrderGuid });

                
            }

            return Json(new { Success = false, Message = "Can't save order with no items. You should cancell it" });

        }

     /// <summary>
     /// Cancel season order by guid
     /// </summary>
     /// <param name="guid"></param>
     /// <returns></returns>
     [HttpPost]
         
         public ActionResult Cancel(Guid guid)
        {
            
            string partnerId = ServicePP.GetPartnerId();

             var order = GetOrderByGuid(guid, partnerId);

             if (order == null) return Json(new { Success = false, Message = _errorOrdernotFound });

             if ((DateTime.Now - order.OrderDate).Days > daysClientMayChangeOrder) return Json(new { Success = false, Message = String.Format("One can't change order more than {0} days after creation", daysClientMayChangeOrder) });

             try
             {
                 // // теперь все турецкие заказы у нас в базе, раньше был .SetAlternateUrl()
                 //if (culturesToRestrictSeasonProducts.Contains(Point.Partner.Culture ?? Defaults.Culture_RU)) _soapServiceForSeasonOrders.SetAlternateUrl();
                 var result = _soapServiceForSeasonOrders.DeleteRestoreSeasonOrder(guid.ToString(), true);

                 if (!result.Success)
                 {
                     ErrorLogger.Error(result.Error);
                     return Json(new { Success = false, Message = "Error invoking web service on 1S side " + result.Error });
                 }
             

             }
             catch (Exception exc)
             {
                 ErrorLogger.Error(exc);
                 return Json(new { Success = false, Message = "Error invoking web service " + exc.Message });

             }

             try {
                 order.OrderStatus = SeasonOrderStatus.Cancelled;
                 DbContext.SaveChanges();
             }
             catch (Exception exc)
             {
                 ErrorLogger.Error(exc);
                 return Json(new { Success = false, Message = "Error updating database.  " + exc.Message });

             }

           //  return Json(new { Success = true, Message = "Season order was succesfully cancelled" });
             return RedirectToAction("Index");
        }
        
    }
}
#endif