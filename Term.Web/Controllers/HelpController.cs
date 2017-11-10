using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Term.Web.Views.Resources;
using Term.DAL;
using YstTerm.Models;

using YstProject.Services;
using Term.Web.Services;

namespace Term.Web.Controllers
{
    public class HelpController : BaseController
    {
        
        private readonly UserService _userService;
        private readonly SendMailService _smtp;
        private readonly NotificationForUserService _ntf;
        public HelpController() : this(new UserService(), new SendMailService(), new NotificationForUserService())
        { }

        public HelpController(UserService userService, SendMailService sms, NotificationForUserService ntf)
        {
            _userService = userService;
            _smtp = sms;
            _ntf = ntf;
        }




        public ActionResult Index()
        {
            var additionalStock = ServicePP.CanPartnerUseAdditionalStock();
            ViewBag.AdditionalStock = additionalStock;
            if (ServicePP.IsPartner)
            return View("Index");
            else
            return View("PartialForChildPoint");
        }

        public ActionResult ColorDefinition()
        {
            return View();           
        }

        public ActionResult WheelsPhotos()
        {            
            return View();    
        }

        public ActionResult UrgentNews()
        {
            var user = User.Identity.Name;
            if (!_ntf.CheckIfExists(user)) _ntf.Add(user, "CartRulesChange");
            return View();
        }

        public ActionResult SendMail()
        {
            var model = new FeedbackForm();
            
            return View(model);

        }


        /// <summary>
        /// Отправка сообщения 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        
        [HttpPost]
        public async Task<ActionResult> SendMail(FeedbackForm model)
        {
            if (!ModelState.IsValid) return View(model);
            
           

            var pointId = ServicePP.getPointID();
            PartnerPoint pp = ServicePP.getPartnerPointById(pointId);

            var partner = ServicePP.GetPartnerById(pp.PartnerId);

            var managerEmail = partner.MainManager != null ? partner.MainManager.EMail : null;
           

          var  isError = !await _smtp.SendAsync(model, pp.PartnerId, pp.InternalName, pp.CompanyName, managerEmail);

            if (!isError)
            {
                DbContext.FeedbackFormDbSet.Add(model);
                await DbContext.SaveChangesAsync();
            }


            ViewBag.Notif = !isError ? Help.Feedback8 : Help.Feedback9;
            ViewBag.Err = isError;

            return View("SendMailSuccess");
            
          

           
        }

        public ActionResult Download()
        {
            
            string id = _userService.getUserId();
            string urlPrice = Url.Action("Index", "PriceList");
            string urlPriceAllDep = Url.Action("RestsOnAllDepartments", "PriceList");
            //urlAcc = $"/api/{Url.Action("accs", "xml", new { id = id })}";
            string urlAcc = "/api" + Url.Action("accs", "xml", new { id = id });
            string urlTyre = "/api" + Url.Action("tyre", "xml", new { id = id });
            string urlTyreOnlyYst = "/api" + Url.Action("tyre", "xml", new { id = id, typeofrests = 1 });
            string urlDisk = "/api" + Url.Action("disk", "xml", new { id = id });
            string urlAkb = "/api" + Url.Action("akb", "xml", new { id = id });
            ViewBag.urlAcc = urlAcc;
            ViewBag.urlPrice = urlPrice;
            ViewBag.urlPriceAllDep = urlPriceAllDep;
            ViewBag.urlTyre = urlTyre;
            ViewBag.urlTyreOnlyYst = urlTyreOnlyYst;
            ViewBag.urlDisk = urlDisk;
            ViewBag.urlAkb = urlAkb;

            return View();

        }

        public ActionResult ManagerInfo() {


            var partner = base.Partner;

            var managerModel = partner == null ? null : partner.MainManager;

            return View(managerModel);
            
        }
        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {

                if (_smtp != null) _smtp.Dispose();
                

            }

            base.Dispose(disposing);

        }
   
    }
}
