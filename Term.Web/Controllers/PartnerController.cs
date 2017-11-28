using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Yst.ViewModels;
using Term.DAL;
using YstProject.Services;
using YstTerm.Models;
using System.Threading.Tasks;
using Yst.Utils;
using Newtonsoft.Json;
using Term.Web.Views.Resources;


namespace Term.Web.Controllers
{
    [CheckSettings]
    public class PartnerController : BaseController
    {
        // имеет ли клиент доступ к чужим точкам
        private bool AccessAllowed(int pointId) => ServicePP.GetPartnerIdByPointId(pointId) == base.Partner.PartnerId;

        private readonly static Action<PartnerPriceRuleDTO> _actionPriceType = p => p.PriceType = p.PType.ToString().ToLower();

        // get partner points of this partner
        //[CustAuth("Partner")]
        [CheckPassThroughKeyword]
        public ActionResult Index()
        {

            int pointId = base.Point.PartnerPointId;

            // Если партнер, то смотрим список точек
            if (ServicePP.IsPartner)
            {
                // просматривать точки может только ПАРТНЕР
                string PartnerId = ServicePP.GetPartnerId();

                var partnerPoints = ServicePP.GetAllPointsByPartnerId(PartnerId);

                ViewBag.PointId = pointId; //ServicePP.getPointID();
                return View(partnerPoints);

            }
          
            // если не партнер, то заходим в точку
                return RedirectToAction("Edit", new { PointId =pointId });

          

         ///   throw new HttpException(404, "User is nor partner neither point");

        }



        /// <summary>
        /// Изменение правил точки (может только сама точка или головной терминал)
        /// </summary>
        /// <param name="PointId">Точка которую редактируем</param>
        /// <returns></returns>
        [CustAuth("Partner", "PartnerPoint")]
        [HttpGet]
        [CheckPassThroughKeyword]
        public ActionResult Edit(int PointId)
        {
            
            int userPointId = (int) base.Point.PartnerPointId;



            //       string PartnerId = ServicePP.GetPartnerId();
            //     Partner partner=ServicePP.GetPartnerById(PartnerId);

            Partner partner = base.Partner;

            ViewBag.PointId = userPointId;
            

            bool addStock = ServicePP.CanPartnerUseAdditionalStock();
            ViewBag.AddStock = addStock;
            //   if (!ServicePP.CheckIfCanPassTrough(String.Empty))
            //   return RedirectToAction("KeyWordEnter","Partner" ,new { ReturnUrl = Request.Url.AbsoluteUri });

           

            bool isMainTerminal=ServicePP.GetPartnerIdByPointId(PointId) == partner.PartnerId;
            // редактировать может сама точка    или  головной терминал
            if (userPointId == PointId || (ServicePP.IsPartner && isMainTerminal))
            {
                PartnerPoint pp = DbContext.Set<PartnerPoint>().FirstOrDefault(p => p.PartnerPointId == PointId);
               
                ViewBag.IsPoint = " ";
                if (userPointId == PointId && !ServicePP.IsPartner)   ViewBag.IsPoint = Defaults.Invisible;
                

                //головной терминал редактирует свою точку
                bool mainTerminalEditsOwnPoint= userPointId == PointId && ServicePP.IsPartner;

                var ppdto = PropsMyMapper<PartnerPointDTO>.CopyObjProperties(pp);

                ppdto.UserName = pp.Name;
                ppdto.DaysToDepartment = pp.DaysToDepartment;
                // { 20150702 --added
                if (partner != null)
                {
                    ppdto.CustomDutyVal = partner.CustomDutyVal;
                    ppdto.VatVal = partner.VatVal;
                    ppdto.IsForeignAndMainTerminal = partner.IsForeign && mainTerminalEditsOwnPoint;
                }
                else
                { 
                 ppdto.CustomDutyVal = 0;
                    ppdto.VatVal = 0;
                    ppdto.IsForeignAndMainTerminal = false;
                }
                // } 20150702 --

                if (String.IsNullOrEmpty(ppdto.Country)) ppdto.Country = Defaults.RussianCode;



                if (String.IsNullOrEmpty(ppdto.Language)) ppdto.Language = Defaults.Culture_RU;
                

                var priceFor = PriceListFor.Client;

                // это партнер, но точка дочерняя
                if (ServicePP.IsPartner && userPointId != PointId)    priceFor = PriceListFor.Point;


                ServicePP.GetPricingRules(ppdto, PointId, priceFor);

                ppdto.PricingRulesDisks.ToList().ForEach(_actionPriceType);
                ppdto.PricingRulesTyres.ToList().ForEach(_actionPriceType);
                ppdto.PricingRulesBat.ToList().ForEach(_actionPriceType);
                ppdto.PricingRulesAcc.ToList().ForEach(_actionPriceType);

                ViewBag.IsPartner = ServicePP.IsPartner;

                ViewBag.Errors = TempData["ErrorsEmptyFields"] as string;
                /*if (TempData["ErrorsEmptyFields"] != null)
                {
                    ViewBag.Errors = (string)TempData["ErrorsEmptyFields"];
} */
                
                if (mainTerminalEditsOwnPoint || userPointId == PointId)     return View("EditNew", ppdto);
                
                else  return View("EditPoint", ppdto);
                

            }
            throw new HttpException(401, "Not allowed to access resource");
        }

        [HttpGet]
        public ActionResult KeyWordEnter(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View("KeyWordEnter");

        }



        [HttpPost]
        [ActionName("KeyWordEnter")]
        public ActionResult KeyWordEnterPost(string ReturnUrl, string KeyWord)
        {
            if (ServicePP.CheckIfCanPassTrough(KeyWord))
                return Redirect(ReturnUrl);
            ViewBag.ReturnUrl = ReturnUrl;
            return View("KeyWordEnter");

        }


        /// <summary>
        // Партнер создает чужую точку
        /// </summary>
        /// <returns></returns>
        [CustAuth("Partner")]
        public ActionResult Create()
        {
            int pointId = (int)base.Point.PartnerPointId;
            string PartnerId = ServicePP.GetPartnerId();
            ViewBag.PointId = pointId;


            if (!ServicePP.IsPartner)   throw new HttpException(404, "Not found");
            // создавать точки может только ПАРТНЕР

            ViewBag.IsPartner = ServicePP.IsPartner;

            var ppdto = new PartnerPointDTOCreate();
            ViewBag.IsPartner = ServicePP.IsPartner;

            bool AddStock = ServicePP.CanPartnerUseAdditionalStock();
            ViewBag.AddStock = AddStock;

            var priceFor = PriceListFor.Client;

            // это партнер, но точка не партнера
            // if (ServicePP.IsPartner && ServicePP.getPointID() != PointId)
            priceFor = PriceListFor.Point;


            ServicePP.GetPricingRules(ppdto, -1, priceFor);



            ppdto.PricingRulesDisks.ToList().ForEach(_actionPriceType);
            ppdto.PricingRulesTyres.ToList().ForEach(_actionPriceType);
            ppdto.PricingRulesBat.ToList().ForEach(_actionPriceType);
            ppdto.PricingRulesAcc.ToList().ForEach(_actionPriceType);

            ViewBag.IsPartner = ServicePP.IsPartner;

            return View(ppdto);

        }


        /// <summary>
        // Партнер создает чужую точку пост
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreatePost(PartnerPointDTOCreate model)
        {

            // создавать точки может только ПАРТНЕР
            string PartnerId = ServicePP.GetPartnerId();
            if (String.IsNullOrEmpty(PartnerId)) throw new HttpException(404, "Not found");

            if (ModelState.IsValid)
            {
                var psc = new PointSettingsContainer();

                PropsMyMapper<PointSettingsContainer>.CopyObjProperties(psc, model, "ContactFIO,DaysToMainDepartment,DepartmentId,DaysToDepartment");

                psc.pricingrules = JsonConvert.DeserializeObject<IEnumerable<PartnerPriceRuleDTO>>(Request["pricingrules"]);

                int pointIdCreated = await ServicePP.CreatePartnerPoint(psc, PartnerId);


                if (pointIdCreated < 0)
                    throw new Exception("Не смог создать партнерскую точку");

                return RedirectToAction("Edit", new { PointId = pointIdCreated });

            }

            return View("Create", model);

        }


        /// <summary>
        /// Партнер создает собственную точку
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateOwn()
        {

            if (!ServicePP.IsPartner)
                throw new HttpException(404, "Not found");

            ViewBag.IsPartner = ServicePP.IsPartner;

            var ppdto = new PartnerPointDTO();
            ppdto.Country = Defaults.RussianCode;
            ppdto.Language = Defaults.Culture_RU;
            ViewBag.IsPartner = ServicePP.IsPartner;

            bool AddStock = ServicePP.CanPartnerUseAdditionalStock();
            ViewBag.AddStock = AddStock;

            var priceFor = PriceListFor.Client;


            ServicePP.GetPricingRules(ppdto, -1, priceFor);

            ppdto.PricingRulesDisks.ToList().ForEach(_actionPriceType);
            ppdto.PricingRulesTyres.ToList().ForEach(_actionPriceType);
            ppdto.PricingRulesBat.ToList().ForEach(_actionPriceType);
            ppdto.PricingRulesAcc.ToList().ForEach(_actionPriceType);


            return View(ppdto);
        }

       /// <summary>
        /// Партнер создает собственную точку через submit
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
 
        [HttpPost]
        public async Task<ActionResult> CreateOwnPost(PartnerPointDTO model)
        {
            if (ModelState.IsValid)
            {
                bool AddStock = ServicePP.CanPartnerUseAdditionalStock();
                ViewBag.AddStock = AddStock;
                // создавать точки может только ПАРТНЕР
                string PartnerId = ServicePP.GetPartnerId();
                if (String.IsNullOrEmpty(PartnerId)) throw new HttpException(404, "Not found");

                var psc = new PointSettingsContainer();

                PropsMyMapper<PointSettingsContainer>.CopyObjProperties(psc, model, "CompanyName,ContactFIO,DaysToMainDepartment,DepartmentId,DaysToDepartment,Country,Address,PhoneNumber,KeyWord,SaleDirection,WebSite,AddressForDelivery,Language,LatLng,Email");


                psc.pricingrules = JsonConvert.DeserializeObject<IEnumerable<PartnerPriceRuleDTO>>(Request["pricingrules"]);

                int pointIdCreated = await Task.Run<int>(()=>ServicePP.CreatePartnerOwnPoint(psc, PartnerId));


                if (pointIdCreated < 0)
                    throw new Exception(Settings.NotCreate);

                return RedirectToAction("UserAgreement");
            }
            else
            {
               
                var priceFor = PriceListFor.Client;
                bool AddStock = ServicePP.CanPartnerUseAdditionalStock();
                ViewBag.AddStock = AddStock;

                ServicePP.GetPricingRules(model, -1, priceFor);

                model.PricingRulesDisks.ToList().ForEach(_actionPriceType);
                model.PricingRulesTyres.ToList().ForEach(_actionPriceType);
                model.PricingRulesBat.ToList().ForEach(_actionPriceType);
                model.PricingRulesAcc.ToList().ForEach(_actionPriceType);
                return View("CreateOwn", model);
            }
            //  return Json(new { Success = true, Message = psc.pricingrules });
        }


        [CustAuth("Partner")]
        [HttpPost]
        public async Task<JsonResult> DisableEnable(int PointId, bool action)
        {
            bool success = false;

            string PartnerId = ServicePP.GetPartnerId();

            // нельзя деактивировать собственную точку
            if (ServicePP.IsPartner && PointId == ServicePP.getPointID())
                return Json(new { Success = false, Message = "Ошибка доступа." });

            // партнер может активировать точку
            if (ServicePP.IsPartner && ServicePP.GetPartnerIdByPointId(PointId) == PartnerId)
                success = await ServicePP.Disable(PointId, action);
            else
                return Json(new { Success = false, Message = "Ошибка доступа." });
            return Json(new { Success = success });

        }


        /// <summary>
        ///  Партнер удаляет точку
        /// </summary>
        /// <param name="PointId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(int PointId)
        {
            bool success = false;


            // нельзя удалять собственную точку
            if (ServicePP.IsPartner && PointId == ServicePP.getPointID())
                return Json(new { Success = false, Message = Settings.ErrorToDelMainPoint });

            if (ServicePP.CheckIfOrdersExist(PointId)) return Json(new { Success = false, Message = Settings.ErrorToDelPointHaveOrder });

            string PartnerId = ServicePP.GetPartnerId();



            if (ServicePP.IsPartner && ServicePP.GetPartnerIdByPointId(PointId) == PartnerId)
                success = await ServicePP.DeletePartnerPoint(PointId);
            else
                return Json(new { Success = false, Message = "Ошибка доступа." });
            return Json(new { Success = success });


        }

        /// <summary>
        /// Партнер сохраняет настройки своей точки
        /// </summary>
        /// <param name="psc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePointProfile(PointSettingsContainer psc)
        {
            string Message = String.Empty;
            if (ServicePP.IsPartner) ServicePP.UpdatePartnerIfNeeded(psc);
            bool Success = ServicePP.UpdatePricingRules(psc, ref Message);
            CachedCollectionsService.RemoveCacheAll(psc.PointId.ToString());
            return Json(new { Success, Message });
        }


        /// <summary>
        /// Пользовательское соглашение
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserAgreement()
        {
            // var pointId = ServicePP.getPointID();

            int pointId = Point.PartnerPointId;

            if (pointId > 0)
            {
                var pp = ServicePP.getPartnerPointById(pointId);
                if (pp.ConditionsAreAccepted) return RedirectToRoute("Default", new { controller = "Home", action = "Index" });

                return View(pp.ConditionsAreAccepted);
            }
            throw new Exception("Not found partner point");
        }

        /// <summary>
        /// Принять пользовательское соглашение
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptConditions()
        {
            int pointId = Point.PartnerPointId;

            if (pointId > 0)
            {
                ServicePP.AcceptConditions(pointId);

                return RedirectToRoute("Default", new { controller = "Home", action = "Index" });

            }
            throw new Exception("Not found partner point");
        }
        

        //----------------------------------------------------------------
        /*
        [CustAuth("Partner", "PartnerPoint")]
        [HttpGet]
        [CheckPassThroughKeyword]
        public ActionResult EditNew(int PointId)
        {
            int pointId = Point.PartnerPointId;
            string PartnerId = ServicePP.GetPartnerId();
            Partner partner = ServicePP.GetPartnerById(PartnerId);
            ViewBag.PointId = pointId;

            //  ViewBag.PointId = ServicePP.getPointID();

            ViewBag.AddStock = ServicePP.CanPartnerUseAdditionalStock();
            
            //   if (!ServicePP.CheckIfCanPassTrough(String.Empty))
            //   return RedirectToAction("KeyWordEnter","Partner" ,new { ReturnUrl = Request.Url.AbsoluteUri });

            bool isMainTerminal = ServicePP.GetPartnerIdByPointId(PointId) == PartnerId;
            // редактировать может сама точка                 головной терминал
            if (pointId == PointId || (ServicePP.IsPartner && isMainTerminal))
            {
                PartnerPoint pp = DbContext.Set<PartnerPoint>().FirstOrDefault(p => p.PartnerPointId == PointId);

                
                bool MainTerminalEditsOwnPoint = ServicePP.getPointID() == PointId && ServicePP.IsPartner;

                var ppdto = PropsMyMapper<PartnerPointDTO>.CopyObjProperties(pp);

                ppdto.UserName = pp.Name;
                ppdto.DaysToDepartment = pp.DaysToDepartment;
                // { 20150702 --added
                if (partner != null)
                {
                    ppdto.CustomDutyVal = partner.CustomDutyVal;
                    ppdto.VatVal = partner.VatVal;
                    ppdto.IsForeignAndMainTerminal = partner.IsForeign && MainTerminalEditsOwnPoint;
                }
                else
                {
                    ppdto.CustomDutyVal = 0;
                    ppdto.VatVal = 0;
                    ppdto.IsForeignAndMainTerminal = false;
                }
                // } 20150702 --

                if (String.IsNullOrEmpty(ppdto.Country))  ppdto.Country = Defaults.RussianCode;
                


                var priceFor = PriceListFor.Client;

                // это партнер, но точка не партнера
                if (ServicePP.IsPartner && ServicePP.getPointID() != PointId)
                    priceFor = PriceListFor.Point;


                ServicePP.GetPricingRules(ppdto, PointId, priceFor);

                ppdto.PricingRulesDisks.ToList().ForEach(p => p.PriceType = p.PType.ToString().ToLower());
                ppdto.PricingRulesTyres.ToList().ForEach(p => p.PriceType = p.PType.ToString().ToLower());
                ppdto.PricingRulesBat.ToList().ForEach(p => p.PriceType = p.PType.ToString().ToLower());
                ppdto.PricingRulesAcc.ToList().ForEach(p => p.PriceType = p.PType.ToString().ToLower());

                ViewBag.IsPartner = ServicePP.IsPartner;

                ViewBag.Errors = TempData["ErrorsEmptyFields"] as string;
                /*if (TempData["ErrorsEmptyFields"] != null)
                {
                    ViewBag.Errors = (string)TempData["ErrorsEmptyFields"];
} 



                return View(ppdto);
            }
            throw new HttpException(404, "Not found");
        }
*/
    }
}
