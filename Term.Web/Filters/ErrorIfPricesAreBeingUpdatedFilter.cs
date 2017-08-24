using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using Term.DAL;
using Yst.Context;
using Yst.ViewModels;
using YstProject.Services;



namespace Term.Web.Filters
{
    
    /// <summary>
    /// Проверка если цены меняются, то при получении xml возвращаем ошибку
    /// </summary>
    public class ErrorIfPricesAreBeingUpdatedFilter :System.Web.Http.Filters.ActionFilterAttribute
    {
        private static readonly string _key = "prices.isbeingloaded";
        private static readonly string _value = "1"; // prices are changed
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            using (var dbContext = new AppDbContext())
            {
                 var key =dbContext.Set<StoredKeyValueItem>().FirstOrDefault(p=>p.Key==_key);

                 if (key != null && key.Value.Equals(_value)) throw new NotImplementedException("prices are being changed");
            }

            base.OnActionExecuting(actionContext);
        }

     
    }

    /// <summary>
    /// Проверка если цены меняются, то при получении excel возвращаем ошибку
    /// </summary>
    public class ErrorIfPricesAreBeingUpdatedMvcFilter : System.Web.Mvc.FilterAttribute, System.Web.Mvc.IActionFilter
    {
        private static readonly string _key = "prices.isbeingloaded";
        private static readonly string _value = "1"; // prices are changed
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
              using (var dbContext = new AppDbContext())
            {
                 var key =dbContext.Set<StoredKeyValueItem>().FirstOrDefault(p=>p.Key==_key);

                 if (key != null && key.Value.Equals(_value)) throw new NotImplementedException("prices are being changed");
            }
            
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
         
        }
    }
}