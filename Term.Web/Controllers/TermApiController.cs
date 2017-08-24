using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yst.Context;
using YstStore.Domain;
using Yst.ViewModels;
using YstTerm.Models;
using System.Web.Mvc;

namespace YstProject.Controllers
{

  

    public class TermApiController : ApiController
    {
     /*  
         [HttpPost]
        public  SavePointProfile(PointSettingsContainer psc)

        {
             var id = psc.PointId;
             var pricingrules = psc.pricingrules; 

            var ClearedPricingRules = pricingrules.Where(p => p.PriceType == "base" || p.PriceType == "zakup" || p.PriceType == "dont_show_price" || p.PriceType == "dont_show_producer").ToArray();

            try
            {
                using (var _dbcontext = new YstContext())
                {
                    var partnerPoint =_dbcontext.PartnerPoints.FirstOrDefault(pp => pp.PartnerPointId == id);

                    if (partnerPoint != null)
                    {
                        partnerPoint.PhoneNumber = psc.PhoneNumber;
                        partnerPoint.ContactFIO = psc.ContactFIO;
                        partnerPoint.Address = psc.Address;
                        partnerPoint.DaysToMainDepartment = psc.DaysToMainDepartment;
                        partnerPoint.DaysToDepartment = psc.DaysToDepartment ?? 0;
                        partnerPoint.DepartmentId = psc.DepartmentId;

                        string PartnerId=  _dbcontext.Set<ApplicationUser>().Where(u => u.UserName == User.Identity.Name && u.IsPartner).Select(u => u.PartnerId).FirstOrDefault() ?? String.Empty;

                        // это значение менять може только партнер, но не партнерская точка
                        if( PartnerId!=String.Empty)
                        partnerPoint.DontShowZakupPrice = psc.DontShowZakupPrice;
                    }
                  

                    var oldrules = _dbcontext.PartnerPriceRules.Where(pp => pp.PartnerPointId == id);

                    foreach (var rule in oldrules)
                    {
                        _dbcontext.PartnerPriceRules.Remove(rule);

                    }
                    _dbcontext.SaveChanges();

                    foreach (var rule in ClearedPricingRules)
                    {
                        var newrule = new PartnerPriceRule();

                        
                        newrule.PartnerPointId = id;
                        newrule.Discount = rule.Discount;
                        newrule.PriceType = rule.PriceType;
                        newrule.ProducerId = rule.ProducerId;
                        _dbcontext.PartnerPriceRules.Add(newrule);
                    }
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return e.InnerException.Message;
            }
            
          return Json(new { success=true});
        } */
    }
}
