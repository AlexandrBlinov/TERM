using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Yst.ViewModels;

namespace Term.Web.Controllers
{
    public class ClaimsController : BaseController
    {
        public ActionResult ListForAdmin(ClaimsViewModel model)
        {
            model.Claims = DbContext.Claims.Where(p => (model.EndDate == null || p.ClaimDate <= model.BeginDate) && (model.BeginDate == null || p.ClaimDate >= model.BeginDate) && (model.NumberIn1S == null || p.NumberIn1S == model.NumberIn1S)).OrderByDescending(p => p.ClaimDate).ToPagedList(model.Page, model.ItemsPerPage);
            return View(model);
        }

        public ActionResult DetailsAdmin(Guid guid)
        {
            var model = new ClaimsViewWithDetails
            {
                Claim = DbContext.Claims.Where(p => p.GuidIn1S == guid).FirstOrDefault(),
                ClaimDetails = DbContext.ClaimsDetails.Where(p => p.GuidIn1S == guid).ToList()
            };
            return View(model);
        }
    }
}