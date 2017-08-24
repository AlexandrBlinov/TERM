using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YstProject.Services;

namespace Term.Web.Filters
{
    public class LoggedExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {


            if (filterContext.IsChildAction) return;
            StringBuilder result = new StringBuilder();
            ILogger _logger = new Logger();


            string innerError = String.Empty;
            // wrap code in try catch for not getting error
            try
            {
                var context = filterContext.HttpContext;

                if (!context.Request.IsAuthenticated || context.Request.IsAjaxRequest()) return;

                result.AppendLine("Path:" + context.Request.Url.AbsolutePath).AppendLine("Method:" + context.Request.HttpMethod);


                string action = filterContext.RouteData.Values["action"].ToString();
                string controller = filterContext.RouteData.Values["controller"].ToString();

                if (controller != null) result.AppendLine("controller:" + controller);
                if (action != null) result.AppendLine("action:" + action);

                var refferer = context.Request.UrlReferrer;

                if (refferer != null) result.AppendLine("urlrefferer:" + refferer.AbsolutePath);

                if (context != null && context.User != null && context.Request.IsAuthenticated)

                    result.AppendLine("User:" + context.User.Identity.Name);

                // record info
                result.Append(filterContext.Exception.ToString());
                _logger.Error(result.ToString());

            }
            finally { }
            /*     catch (Exception exc){
                     result.Append("with inner exception:" + exc.ToString());
                 } 

                 finally {
                     result.Append(filterContext.Exception.ToString());
                     _logger.Error(result.ToString());
                 }
            
                 */

        }
    }
}