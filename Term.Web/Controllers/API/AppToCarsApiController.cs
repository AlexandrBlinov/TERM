using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using Yst.Services;


namespace Term.Web.Controllers.API
{
    public class AppToCarsApiController : ApiController
    {
        private readonly ProductService _productService;
        public AppToCarsApiController() : this(new ProductService()) { }
        public AppToCarsApiController(ProductService service)
        {
            _productService = service;
        }

        public IHttpActionResult Get(string ids)
        {
            var codes = ids.Split(';');
            var result = new Dictionary<string, string>();
            foreach (var code in codes)
            {
                var app = _productService.GetCarsFromProduct(Convert.ToInt32(code), 1);
                if (app.Count == 0) app = _productService.GetCarsFromProduct(Convert.ToInt32(code), 0);
                if (app.Count > 0)
                {
                    var appcarsstring = "||";
                    foreach (var car in app)
                    {
                        appcarsstring += car.Key + " " + Regex.Replace(car.Value, "<.*?>", String.Empty) + "||";
                    }
                    result.Add(code, appcarsstring);
                }
                else
                {
                    result.Add(code, String.Empty);
                }
            }

            return Ok(new { results = result });

        }

    }
}