using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Yst.ViewModels;
using System.Threading.Tasks;
using Term.ClaimServiceSoap;
using Yst.Services;
using System.Web;
using System.Globalization;
using System.Data.Entity;

namespace Term.Web.Controllers
{
    public class ClaimsController : BaseController
    {
        private readonly ClaimLocalService _cls = null;
        public ClaimsController(ClaimLocalService cls)
        {
            _cls = cls;
        }
        public ClaimsController()
            : this(
            new ClaimLocalService())
        {

        }

        public ActionResult Index(ClaimsViewModel model)
        {
            model.Claims = DbContext.Claims.Include(o => o.ClaimsDetails).Where(p => p.PartnerId == this.Partner.PartnerId 
            && (model.EndDate == null || p.ClaimDate <= model.BeginDate) 
            && (model.BeginDate == null || p.ClaimDate >= model.BeginDate) 
            && (model.NumberIn1S == null || p.NumberIn1S == model.NumberIn1S)
            && (model.ProductId == null || p.ClaimsDetails.Any(o => o.ProductId.ToString().Contains(model.ProductId)))
            && (model.SaleNumber == null || p.ClaimsDetails.Any(o => o.SaleNumber.Contains(model.SaleNumber)))
            )
            .OrderByDescending(p => p.ClaimDate).ToPagedList(model.Page, model.ItemsPerPage);
            return View(model);
        }

        [Authorize(Roles = "Newsmaker")]
        public ActionResult ListForAdmin(ClaimsViewModel model)
        {
            model.Claims = DbContext.Claims.Include(o => o.ClaimsDetails).Where(p => (
            model.EndDate == null || p.ClaimDate <= model.BeginDate) 
            && (model.BeginDate == null || p.ClaimDate >= model.BeginDate) 
            && (model.NumberIn1S == null || p.NumberIn1S == model.NumberIn1S)
            && (model.ProductId == null || p.ClaimsDetails.Any(o => o.ProductId.ToString().Contains(model.ProductId)))
            && (model.SaleNumber == null || p.ClaimsDetails.Any(o => o.SaleNumber.Contains(model.SaleNumber)))
            ).
            OrderByDescending(p => p.ClaimDate).ToPagedList(model.Page, model.ItemsPerPage);
            return View(model);
        }

        public ActionResult Details(Guid guid)
        {
            var model = new ClaimsViewWithDetails
            {
                Claim = DbContext.Claims.Where(p => p.GuidIn1S == guid).FirstOrDefault(),
                ClaimDetails = DbContext.ClaimsDetails.Where(p => p.GuidIn1S == guid).ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Newsmaker")]
        public ActionResult DetailsAdmin(Guid guid)
        {
            var model = new ClaimsViewWithDetails
            {
                Claim = DbContext.Claims.Where(p => p.GuidIn1S == guid).FirstOrDefault(),
                ClaimDetails = DbContext.ClaimsDetails.Where(p => p.GuidIn1S == guid).ToList()
            };
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new NewClaimViewModel()
            {
                PartnerName = this.Partner.Name,
                Inn = this.Partner.INN,
                Address = this.Partner.Address,
                Fio = this.Point.ContactFIO,
                Phone = this.Point.PhoneNumber
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveClaimData(ClaimsContainer psc)
        {
            List<Claim> ArrayOfClaims = new List<Claim>();
            if (psc.ClaimItems.Count() == 0)
            {
                var messageErr = "Нет товаров для рекламации";
                var resultErr = false;
                return Json(new { resultErr, messageErr });
            }
            foreach (var item in psc.ClaimItems)
            {
                Nullable<DateTime> autoYearDate = item.AutoYear != null ? new DateTime(Convert.ToInt32(item.AutoYear), 1, 1) : default(DateTime);
                var ItemOfClaims = new Claim
                {
                    INN = this.Partner.INN,
                    Responsible = psc.ContactFIO,
                    Code = item.ProductId.ToString().PadLeft(7, '0'),
                    Quantity = item.Count,
                    NumberOfDoc = item.DocNumber,
                    DateOfDoc = item.DocDate,
                    Condition = item.Condition,
                    DateOfProduction = item.DateOfManufacture.ToString(),
                    TypeOfDefect = item.Defect,
                    Commentary = item.DetailedDescriptionDefect,
                    HTTPLinkToAPicture = String.Empty,
                    Run = item.TireRunning == null ? String.Empty : item.TireRunning,
                    Model = item.Auto == null ? String.Empty : item.Auto,
                    SerialNumber = item.SerialNumber == null ? String.Empty : item.SerialNumber,
                    DiameterDisk = item.WheelDiametr == null ? 0 : float.Parse(item.WheelDiametr, CultureInfo.InvariantCulture.NumberFormat),
                    WidthRimDisk = item.WheelWidth == null ? 0 : float.Parse(item.WheelWidth, CultureInfo.InvariantCulture.NumberFormat),
                    Pressure = item.Pressure == null ? 0 : float.Parse(item.Pressure, CultureInfo.InvariantCulture.NumberFormat),
                    Location = item.Position,
                    YearCar = autoYearDate,
                    EngineCapacity = item.AutoEngine,
                    DateDefect = item.DefectDate,
                    DateSaleKlient = item.EndSaleDate,
                    InfoKlient = "",
                    InfoFormula = ""
                };
                ArrayOfClaims.Add(ItemOfClaims);
            }
            var resultWs = await Task.Run(() => WsClaims.SetClaim(ArrayOfClaims.ToArray(), false, false));

            var message = String.Empty;
            var resultInLocal = false;
            if (resultWs.Success)
            {
                resultInLocal = _cls.CreateClaimInLocal(psc, Guid.Parse(resultWs.ClaimGUID[0]), Convert.ToInt32(resultWs.ClaimNumber[0]));
                if (resultInLocal) message = "Рекламация создана успешно!";
                    else message = "Ошибка при создании рекламации!";
            }
            else
            {
                message = resultWs.Error;
            }

            var result = false;
            if (resultWs.Success && resultInLocal) result = true;

            return Json(new { result, message });
        }

        public ActionResult ClaimPrintForm(Guid guid)
        {
            if (base.Partner == null) throw new NullReferenceException("Partner not found in db");

            ViewBag.PartnerName = base.Partner.Name;
            ViewBag.PartnerINN = base.Partner.INN;
            ViewBag.PartnerAddressPhone = base.Partner.Address + " " + base.Partner.PhoneNumber;

            var model = new ClaimsViewWithDetails
            {
                Claim = DbContext.Claims.Where(p => p.GuidIn1S == guid).FirstOrDefault(),
                ClaimDetails = DbContext.ClaimsDetails.Where(p => p.GuidIn1S == guid).ToList()
            };
            
            if (model != null)
            {
                return View(model);
            }

            throw new HttpException(404, "Not found");

        }
    }
}