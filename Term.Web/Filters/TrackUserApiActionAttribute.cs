using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http.Filters;
using Term.DAL;
using Yst.Context;
using Yst.ViewModels;

namespace Term.Web.Filters
{
    /// <summary>
    /// Логировать запросы к API контроллерам
    /// </summary>
    public class TrackUserApiActionAttribute : ActionFilterAttribute
    {
        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return null;
        }


        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            try
            {
                var ipAddress = GetClientIp(actionExecutedContext.Request);

                if (String.IsNullOrEmpty(ipAddress)) return;
                var lastword=actionExecutedContext.Request.RequestUri.AbsolutePath.Split('/').LastOrDefault();

                if (!String.IsNullOrEmpty(lastword))

              
                    using (var dbContext = new AppDbContext())
                    {
                        var user=dbContext.Set<ApplicationUser>().FirstOrDefault(u => u.Id == lastword);

                        var userLog = new DbUserActionLog {
                            Date = DateTime.Now,
                            UserName = user.UserName,
                            UserAction = actionExecutedContext.Request.RequestUri.AbsolutePath,
                            IpAddress=ipAddress
                        };
                        
                        dbContext.Set<DbUserActionLog>().Add(userLog);

                        dbContext.SaveChanges();
                    }
                

            }
            finally { }
            base.OnActionExecuted(actionExecutedContext);
        }


    }
}