using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Term.Web.Filters
{
    public class AdminHashAuthAttribute : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)

        {
            string hashkey = "hash";
            if (actionContext.Request.Headers.Contains(hashkey) && actionContext.Request.Headers.GetValues(hashkey).FirstOrDefault() == ConfigurationManager.AppSettings[hashkey])
                return;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            actionContext.Response.Content = new StringContent("passed hash key is invalid");
        }

    }
}