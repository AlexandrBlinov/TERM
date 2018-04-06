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
            Char space = ' ';
            Func<int, string> valToAdd = x => (x > 0) ? "; " : String.Empty;

            var result = new StringBuilder();

            if (!carRecords.Any()) return string.Empty;
            int counter = 0;
           
            foreach (var carRecord in carRecords)
            {
                string vendorName = String.Empty;
                string carName = String.Empty;

                result.Append(valToAdd(counter));
                
               string endYear= carRecord.EndYear == 0 || carRecord.EndYear == DateTime.Now.Year ? String.Empty: carRecord.EndYear.ToString();

               if (!String.IsNullOrEmpty(carRecord.VendorName) && !carRecord.ModificationName.Contains(carRecord.VendorName.Trim())) vendorName = carRecord.VendorName +space;

                if (!String.IsNullOrEmpty(carRecord.CarName) && !carRecord.ModificationName.Contains(carRecord.CarName.Trim())) carName = carRecord.CarName+space;


                var resultString = $"{vendorName}{carName}{carRecord.ModificationName} {carRecord.BeginYear}-{endYear}";
                result.Append(resultString);
                counter++;
            }
            return result.ToString();

        }


    }
}