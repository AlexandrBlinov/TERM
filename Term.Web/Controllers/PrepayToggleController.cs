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
    /// <summary>
    /// Контроллер для переключателя (предоплата/отсрочка)
    /// </summary>
    public class PrepayToggleController : Controller
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

        public PrepayToggleController() : this(new AppDbContext(), new HttpContextWrapper(System.Web.HttpContext.Current) , new BaseService())
        {

        }

        public PrepayToggleController(AppDbContext appDbContext , HttpContextBase context, BaseService service)
        {
            this._dbContext = appDbContext;
            this._context = context;
            this._service = service;
        }

        // GET: LimitData
                
        public ActionResult Index()
        {
            if (!_context.Request.IsAuthenticated) return new EmptyResult();

            var currentPartner = _service.Partner;
            
          if (currentPartner == null || currentPartner.IsForeign) return new EmptyResult();

            if (currentPartner.HasPrepay &&!currentPartner.PrePay )                return PartialView(currentPartner.UsePrepayPrices);

            return new EmptyResult();
        }

        
        /// <summary>
        /// Установить что использовать (предоплатные или постоплатные цены)
        /// </summary>
        /// <param name="prepay"></param>
        /// <returns></returns>
        [HttpPost]
         public async Task<ActionResult> Set( bool prepay) {

            var currentPartner = _service.Partner;

            string partnerId = currentPartner.PartnerId;

           var partner = await _dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId);

            partner.UsePrepayPrices = prepay;
                        

            try
            {

                int pointId = _service.CurrentPoint.PartnerPointId;

                await Task.Run(() => CachedCollectionsService.RemoveCacheAll(pointId.ToString()));
            } catch (Exception e)
            {
                _logger.Error(e);

            }

            await  _dbContext.SaveChangesAsync();

            return Json(new { Success = true });
        }

        /*
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
        */

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