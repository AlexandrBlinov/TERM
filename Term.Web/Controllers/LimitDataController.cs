using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Term.Services;
using Term.Soapmodels;
using Yst.Context;
using YstProject.Services;

namespace Term.Web.Controllers
{
    public class LimitDataController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpContextBase _context;
        private readonly BaseService _service;
        private ServiceTerminal _ws;
        private ILogger _logger;

        protected ILogger ErrorLogger
        {
            get
            {
                return _logger ?? (_logger = new Logger());
            }

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

        public LimitDataController() : this(new AppDbContext(), new HttpContextWrapper(System.Web.HttpContext.Current) , new BaseService())
        {

        }

        public LimitDataController(AppDbContext appDbContext , HttpContextBase context, BaseService service)
        {
            this._dbContext = appDbContext;
            this._context = context;
            this._service = service;
        }

        // GET: LimitData

        [OutputCache(CacheProfile = "VariedByUserOnServer10Min")]
        public async  Task<ActionResult> Index()
        {
            if (!_context.Request.IsAuthenticated) return new EmptyResult();

            var currentPartner = _service.Partner;
            
          if (currentPartner == null || currentPartner.IsForeign) return new EmptyResult();

            var model = new Dictionary<string, decimal>();

            
                var partnerId = currentPartner.PartnerId;

            var itemsFound =await _dbContext.PartnerPropertyValues.Where(p => p.PartnerId == partnerId).ToListAsync();

            foreach (var item in itemsFound)
            {
                decimal result;               
                  if(  Decimal.TryParse(item.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))

                model.Add(item.Name, result);
            }

            
            return PartialView(model);
        }


        public ActionResult GetDetailedData() {

            return View();
        }

        public async Task<ActionResult> DetailedData()
        {
            var result = new ResultDebt();

            if (!_context.Request.IsAuthenticated) return new EmptyResult();

            var currentPartner = _service.Partner;

            if (currentPartner == null || currentPartner.IsForeign) return new EmptyResult();

            try
            {
                 result = await Task.Run(() => WS.GetDebt(currentPartner.PartnerId));

            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex);
                result.Success = false;
            }


            return PartialView(result);

        }


        protected override void Initialize(RequestContext requestContext)
        {

            string culture = Defaults.Culture_RU;
          

            var request = requestContext.HttpContext.Request;
            var currentThread = System.Threading.Thread.CurrentThread;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);

            if (request.IsAuthenticated && _service.Partner != null)
            {

                culture = String.IsNullOrEmpty(_service.Partner.Culture) ? Defaults.Culture_RU : _service.Partner.Culture;
                if (!cultureInfo.Equals(currentThread.CurrentCulture)) currentThread.CurrentCulture = cultureInfo;
                if (!cultureInfo.Equals(currentThread.CurrentUICulture)) currentThread.CurrentUICulture = cultureInfo;

            }
                base.Initialize(requestContext);
        }
    }
}