using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;


namespace Term.Web.Controllers
{
    /// <summary>
    /// Рендеринг блока для возврата тестов
    /// </summary>
    [Authorize]
    public class VideoOfProductController : Controller
    {
        private static string urlToGetJson = "http://api.kolesatyt.ru/video-for-product";
        private static string urlprefixToGetVideo = "http://content.yst.ru/";

        string result = String.Empty;
        // GET: VideoOfProduct
        public async Task<ActionResult> Index(int productId)
        {
            dynamic staff;
           var url = $"{urlToGetJson}={productId}";

            try
            {
                using (var client = new WebClient())
                {
                    string jsonResult = await client.DownloadStringTaskAsync(url);

                    staff = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResult);

                    if (staff.Count > 0) result = ((JValue)((JContainer)((JContainer)staff[0]).First).First).Value.ToString();
                }
            }
            catch
            {
                return new EmptyResult();
            }

            if (!string.IsNullOrEmpty(result))
            {
                var resultUrl= $"{urlprefixToGetVideo}{result}";
                return PartialView("Index",resultUrl);
            }
            
                return new EmptyResult();
            
        }
    }
}