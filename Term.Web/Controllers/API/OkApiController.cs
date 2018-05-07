using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Term.Web.Controllers.API
{
    public class OkApiController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Index() => Ok();
    }
}
