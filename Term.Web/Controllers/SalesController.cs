using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Term.DAL;
using Yst.ViewModels;
using Yst.Context;
//using YstProject.WebReferenceTerm;
using YstTerm.Models;
using System.Globalization;
using System.Threading.Tasks;
using Term.Services;
using Term.Web.Models;
using YstProject.Services;
using Yst.Services;

namespace Term.Web.Controllers
{
    [Authorize]
    [CheckSettings]
    public class SalesController : BaseController
    {

      private  Func<PartnerPoint, string> _func1 = p => String.Format("Point{0}", p.PartnerPointId), _func2 = p => p.InternalName ?? String.Empty;

        private readonly SalesService _salesService;
        public SalesController():this(new SalesService())
        {       }

        public SalesController(SalesService salesService)
        {
            _salesService = salesService;
        }


        [CheckPassThroughKeyword]
        public ActionResult List(SalesViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isPartner = ServicePP.IsPartner;
                

                int pointId = Point.PartnerPointId;
                string partnerId = Point.PartnerId;

                ViewBag.IsPartner = isPartner;
             
                if (isPartner) // PartnerId is set too
                {

                    model.PartnerPoints = ServicePP.GetPointNamesByPartner(partnerId, _func2);
                    if (!ServicePP.CheckIfCanPassTrough(String.Empty))
                        return RedirectToAction("KeyWordEnter", "Partner", new { ReturnUrl = Request.Url.AbsoluteUri });

                    model = _salesService.GetListOfSalesByPartnerId(model, partnerId);


                }
                else
                {
                    if (!ServicePP.CheckIfCanPassTrough(String.Empty))
                        return RedirectToAction("KeyWordEnter", "Partner", new { ReturnUrl = Request.Url.AbsoluteUri });
                    model = _salesService.GetListOfSalesByPointId(model, pointId);
                   
                }
                return View(model);
            }

            throw new HttpException(404, "Not found");

        }

        /// <summary>
        /// Детальная информация о реализации
        /// </summary>
        /// <param name="guidSaleIn1S"></param>
        /// <returns></returns>
        public ActionResult Details(Guid guidSaleIn1S)
        {
            bool isPartner = ServicePP.IsPartner;
            ViewBag.IsPartner = isPartner;
            
            ViewBag.IsForeign = Point.Partner.IsForeign;
            
            SaleViewWithDetails model = _salesService.GetSaleByGuid(guidSaleIn1S);

            ViewBag.OrderStatus = model.SaleData.IsDelivered ? OrderStatuses.DeliveredToClient: OrderStatuses.ShippedForSale;


            if (model != null)
            {
                return View(model);
            }

            throw new HttpException(404, "Not found");

        }

        /// <summary>
        /// Счет на оплату
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Bill(Guid guid)
        {
            SaleViewWithDetails model = _salesService.GetSaleByGuid(guid);
            Partner partner = Point.Partner;
            if (partner == null)
                throw new NullReferenceException("Partner not found in db");

            ViewBag.PartnerInfo = partner.ToString();

            if (model != null) return View(model);


            throw new HttpException(404, "Not found");

        }
    }
}