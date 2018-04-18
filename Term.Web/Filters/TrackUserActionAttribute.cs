using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Mvc;
using Term.DAL;
using Yst.Context;

namespace Term.Web.Filters
{
    /// <summary>
    /// Класс используется для логирования изменений с MVC actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TrackUserActionAttribute : ActionFilterAttribute
    {
        
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            try
            {

                var ipAddress = filterContext.HttpContext.Request.UserHostAddress;


                    using (var dbContext = new AppDbContext())
                    {
                        var userName = HttpContext.Current?.User?.Identity?.Name;

                        var userLog = new DbUserActionLog
                        {
                            Date = DateTime.Now,
                            UserName = userName,
                            UserAction = filterContext.HttpContext.Request.Url.AbsoluteUri,
                            IpAddress = ipAddress
                        };

                        dbContext.Set<DbUserActionLog>().Add(userLog);

                        dbContext.SaveChanges();
                    }

            }
            finally { }
            base.OnActionExecuted(filterContext);
        }

        
    }
}