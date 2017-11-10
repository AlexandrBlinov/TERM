using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Term.Services;
using Yst.ViewModels;
using Yst.Context;
using System.Net;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace Term.Web.Controllers
{
    public class ReportsController : Controller
    {
        readonly AppDbContext _dbContext;
        private ServiceTerminal _ws;
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
        public ReportsController() : this(new AppDbContext()) { }
        public ReportsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult> Freeman(ReportModel model)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            model.PartnerId = "91735";
            if (model.BeginDate != null)
            {
                var start = model.BeginDate ?? DateTime.Now;
                var stop = model.EndDate ?? DateTime.Now;
                model.ReturnItems = await Task.Run(() => WS.ReturnOfDefectiveReport(model.PartnerId, start, stop));
            }
            return View(model);
        }

        public ActionResult Details(string claimnumber, int productId)
        {
            var model = new ClaimsViewWithDetails();
            var num = Convert.ToInt32(claimnumber.Remove(1, 1));
            model.Claim = _dbContext.Claims.Where(p => p.NumberIn1S == num).FirstOrDefault();
            if (model.Claim != null)
                model.ClaimDetails = _dbContext.ClaimsDetails.Where(p => p.GuidIn1S == model.Claim.GuidIn1S && p.ProductId == productId).ToList();

            return View(model);
        }
    }
}
