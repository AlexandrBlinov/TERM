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
    /// <summary>
    /// Аттрибут для загрузки остатков и пр-го. Требует hash
    /// </summary>
    public class AdminHashAuthAttribute : AuthorizationFilterAttribute
    {
        private static readonly string hashkey = "hash";

        public override void OnAuthorization(HttpActionContext actionContext)

        {           
            if (actionContext.Request.Headers.Contains(hashkey) && actionContext.Request.Headers.GetValues(hashkey).FirstOrDefault() == ConfigurationManager.AppSettings[hashkey])
                return;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            actionContext.Response.Content = new StringContent("passed hash key is invalid");
        }

    }
}