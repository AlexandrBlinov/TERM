using System.Web.Mvc;
using Yst.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Yst.ViewModels;
using Yst.Services;
using System.Web.Routing;
using YstProject.Services;
//using YstProject.WebReferenceTerm;
using System.Net;
using System.Configuration;
using System;
using System.Collections.Generic;
using Term.DAL;
using System.Globalization;
using System.Web;
using Term.Services;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

//using Term.Web.ResultDel.datasource;
//using Term.Web.WebReferenceTerm;


namespace Term.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {

        
        private AppDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;
        private ProductService _productService;
        private ShoppingCart _cart;
        private ServicePartnerPoint _servicePartnerPoint;
       // private OrderService _orderService;
        private ServiceTerminal _ws;
        private ILogger _logger;
        private SeasonProductService _sps;


        protected const string CartSessionKey = "CartId";
        protected const string PartnerIdSessionKey = "PartnerId";

        protected ILogger ErrorLogger{
        get{
            return _logger ?? (_logger= new Logger()); 
         }
    
        }

        protected ServiceTerminal WS
        {
            get
            {
                return _ws ?? (_ws=
                 new ServiceTerminal{
                    PreAuthenticate = true,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginWS"], 
                        ConfigurationManager.AppSettings["PasswordWS"])
                });

                

            }

        }

        protected AppDbContext DbContext
        {
            get { return _dbContext ?? (_dbContext = new AppDbContext()); }
        }

      
        protected UserManager<ApplicationUser> Manager
        {
            get {

                if (_userManager != null) return _userManager;
                _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DbContext));
                var provider = new DpapiDataProtectionProvider("Sample");
                _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
                return _userManager;
            }

        }

    


       


        protected ProductService Products
        {
            get { return _productService ?? (_productService = new ProductService(DbContext)); }
        }

        protected SeasonProductService SeasonProductService
        {
            get { return _sps ?? (_sps = new SeasonProductService()); }
        }

        protected ShoppingCart Cart
        {
            get { return _cart ?? (_cart = new ShoppingCart(DbContext)); }
        }


        protected ServicePartnerPoint ServicePP
        {
            get { return _servicePartnerPoint ?? (_servicePartnerPoint = new ServicePartnerPoint(DbContext)); }
        }

        protected PartnerPoint Point { get; private set; }

        protected Partner Partner { get; private set; }



       bool IsChildRequest(RequestContext requestContext)
       {
           return
               (String.Compare((string) requestContext.RouteData.Values["controller"], "ShoppingCart",
                   StringComparison.InvariantCultureIgnoreCase) == 0 &&
                String.Compare((string) requestContext.RouteData.Values["action"], "CartSummary",
                    StringComparison.InvariantCultureIgnoreCase) == 0);


       }

        protected override void Initialize(RequestContext requestContext)
        {
            string culture = Defaults.Culture_RU;
            var CultureKey = "Culture";

            var request = requestContext.HttpContext.Request;

            
            if (request.IsAuthenticated && !IsChildRequest(requestContext))
            {

                
                    Point = ServicePP.CurrentPoint;

                // Point = null при первом заходе когда не создана собственная точка
                    if (Point == null) Partner = null;
                    else Partner = Point.Partner;
                    
                    
                    
                
                //  Partner partner;
            //    if (ServicePP.IsPartner) partner = ServicePP.getPartner();
                
            //    else
            //    {
            //        int pointID = ServicePP.getPointID();
           //         string partnerId = ServicePP.getPartnerByPointId(pointID);
           //         partner = ServicePP.getPartnerById(partnerId);

           //     }
             //   string culture = Partner != null&& String.IsNullOrEmpty(Partner.Culture) ? Defaults.Culture_RU : Partner.Culture;


                if (Partner != null) culture = String.IsNullOrEmpty(Partner.Culture) ? Defaults.Culture_RU : Partner.Culture;
               
                var currentThread = System.Threading.Thread.CurrentThread;
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);

                if (!cultureInfo.Equals(currentThread.CurrentCulture)) currentThread.CurrentCulture = cultureInfo;
                if (!cultureInfo.Equals(currentThread.CurrentUICulture)) currentThread.CurrentUICulture = cultureInfo;

              
               HttpCookie cultureCookie = request.Cookies[CultureKey];
                if (cultureCookie == null || cultureCookie.Value.CompareTo(culture) != 0)
                {
                    var newCookie= new HttpCookie(CultureKey, culture);
                    newCookie.Expires = DateTime.Now.AddMinutes(30);
                   requestContext.HttpContext.Response.Cookies.Add(newCookie);

                } 
            }

           
            base.Initialize(requestContext);
        }


    



     

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
         

            if (!filterContext.IsChildAction && filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
            {
                ViewBag.CurrentPoint = this.Point;
                ViewBag.CurrentPartner = this.Partner;
                ViewBag.PartnerId = this.Partner?.PartnerId;
                ViewBag.PointId = this.Point?.PartnerPointId;


                var hasCheckPassThroughAttribute = filterContext.ActionDescriptor.IsDefined(typeof(CheckPassThroughKeyword), false);
               
             
                if (!hasCheckPassThroughAttribute)
                    Session[Defaults.KeyWord] = null;

            }
            base.OnActionExecuting(filterContext);
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {

                if (_dbContext != null) _dbContext.Dispose();
                if (_userManager != null) _userManager.Dispose();
                if (_productService != null) _productService.Dispose();
                if (_ws != null) _ws.Dispose();

            }

            base.Dispose(disposing);

        }
    }
}
