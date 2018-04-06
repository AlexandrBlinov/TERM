using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using Term.Web.Views.Resources;
using Yst.Services;
using Yst.ViewModels;

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

        /// <summary>
        /// ids - коды товаров
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IHttpActionResult Get(string ids)
        {
            var codes = ids.Split(';');
            var result = new Dictionary<string, string>();
            foreach (var code in codes)
            {
                var app = _productService.GetCarsFromProduct(Convert.ToInt32(code), 1, _productService.GetModifications);
                if (app.Count == 0) app = _productService.GetCarsFromProduct(Convert.ToInt32(code), 0, _productService.GetModifications);
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


        /// <summary>
        /// ids - коды товаров
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IHttpActionResult Get2(string ids)
        {
            var codes = ids.Split(';');
            var result = new Dictionary<string, string>();
            foreach (var code in codes)
            {
                var appRecords = _productService.GetCarsFromProduct(Convert.ToInt32(code), 1, GetModifications2);
                if (appRecords.Count == 0) appRecords = _productService.GetCarsFromProduct(Convert.ToInt32(code), 0, GetModifications2);


                StringBuilder sb= new StringBuilder();
                
                foreach (var appRecord in appRecords) sb.Append(sb.Length == 0 ? appRecord.Value : ";" + appRecord.Value);

                result.Add(code, sb.ToString());
                
            }

            return Ok(new { results = result });

        }


        /// <summary>
        /// Prepares string grouped by modification
        /// </summary>
        /// <param name="carRecords"></param>
        /// <returns></returns>
        public string GetModifications2(IEnumerable<CarRecordViewDetail> carRecords)
        {
            Func<int, string> valToAdd = x => (x > 0) ? "; " : String.Empty;

            var result = new StringBuilder();

            if (!carRecords.Any()) return string.Empty;
            int counter = 0;
           
            foreach (var carRecord in carRecords)
            {
                int endYear;
                result.Append(valToAdd(counter));
                endYear = carRecord.EndYear == 0 ? endYear = DateTime.Now.Year : endYear = carRecord.EndYear;
                result.Append($"{carRecord.VendorName} {carRecord.CarName} {carRecord.ModificationName} {carRecord.BeginYear}-{carRecord.EndYear}");
                counter++;
            }
            return result.ToString();

        }


    }
}