using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Term.Web.Controllers
{
    /// <summary>
    /// 404 and 500 errors are redirected to errorController actions1
    /// </summary>
    public class ErrorController : Controller
    {
        //
        ///// InternalError (written in Global.asax)
        //
        public ActionResult InternalError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return View();
        }
        /// <summary>
        /// Not found (written in Global.asax)
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }

    }
}
