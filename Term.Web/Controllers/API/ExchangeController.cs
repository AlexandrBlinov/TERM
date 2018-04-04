using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using YstProject.Services;

namespace Term.Web.Controllers.API
{
    public class ExchangeController : ApiController
    {
        private static readonly string _importDirectory = ConfigurationManager.AppSettings["ImportDirectory"];
        private HttpContextBase _context;
        public ExchangeController( HttpContextBase context)
        {
            _context = context;
        }
        public ExchangeController()
            : this(new HttpContextWrapper(HttpContext.Current))
        {
            
        }


        /// <summary>
        /// Refresh disks cache
        /// </summary>
        /// <returns></returns>
      //  [Authorize]
        [HttpGet]
        public HttpResponseMessage ClearCacheDisks()
        {
            var keys = String.Join("=====", CachedCollectionsService.GetKeys());


            int countbeforeclear = (int)CachedCollectionsService.Count;
            CachedCollectionsService.ClearCache();
            return (new HttpResponseMessage
            {
                Content = new StringContent(countbeforeclear.ToString() +":"+keys),
                StatusCode = System.Net.HttpStatusCode.OK
            });
        
        }


        [HttpGet]
        public HttpResponseMessage GetCacheKeys()
        {
            var keys = String.Join("=====", CachedCollectionsService.GetKeys());


            int countbeforeclear = (int)CachedCollectionsService.Count;
            
            return (new HttpResponseMessage
            {
                Content = new StringContent(countbeforeclear.ToString() + ":" + keys),
                StatusCode = System.Net.HttpStatusCode.OK
            });

        }


        /* [HttpGet]
        public int RemoveCacheKey([FromUri] string point)
        {
            CachedCollectionsService.Remove(point);
            return 0;
        } */

        [Authorize]
        [HttpGet]
        [ActionName("ImportProducts")]
        public HttpResponseMessage ImportProducts()
        {
            int result = 0;
            string errorMsg;



            String fullPathToDirectory = Path.Combine(_context.Server.MapPath(_context.Request.ApplicationPath), _importDirectory);

            var nvc = new NameValueCollection{
         {"spImportProducers", "PathToProducersFile"},
         {"spImportModels", "PathToModelsFile"},
         {"spImportTiporazmers", "PathToTiporazmersFile"},
         {"spImportProducts", "PathToProductsFile"},
          {"spImportPartners", "PathToPartnersFile"}};

            foreach (string key in nvc.AllKeys)
            {
                var filename = Path.Combine(fullPathToDirectory, ConfigurationManager.AppSettings[nvc[key]]);

                result = SPExecutor.Execute(key, filename, out errorMsg);
                if (result != 0)
                    return (new HttpResponseMessage
                    {
                        Content = new StringContent(String.Format("Error: {0} {1} {2}", key, filename, errorMsg)),
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
                    });


            }

          //  _dbactiologs.Add("Products");

            return (new HttpResponseMessage
            {
                Content = new StringContent("OK"),
                StatusCode = System.Net.HttpStatusCode.OK
            });

        }


        [Authorize]
        [HttpGet]
        [ActionName("ImportPrices")]
        public object ImportPrices()
        {

            int result = 0;
            string errorMsg;

            
            String fullPathToDirectory = Path.Combine(_context.Server.MapPath(_context.Request.ApplicationPath), _importDirectory);
            NameValueCollection nvc = new NameValueCollection{
         {"spImportPricesOfProducts", "PathToPricesOfProductsFile"},
         {"spImportPricesOfPartners", "PathToPricesFile"}};

            foreach (string key in nvc.AllKeys)
            {
                var filename = Path.Combine(fullPathToDirectory, ConfigurationManager.AppSettings[nvc[key]]);

                result = SPExecutor.Execute(key, filename, out errorMsg);
                if (result != 0)
                    return (new HttpResponseMessage
                    {
                        Content = new StringContent(String.Format("Error: {0} {1} {2}", key, filename, errorMsg)),
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
                    });
            }


         //   _dbactiologs.Add("Prices");

            return (new HttpResponseMessage
            {
                Content = new StringContent("OK"),
                StatusCode = System.Net.HttpStatusCode.OK
            });
        }




        /// <summary>
        /// Дозагрузка прайсов
        /// </summary>
        /// <returns></returns>
        [ActionName("ImportPricesAdded")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportPricesAdded()
        {
            string errorMsg;
           
            String fullPathToFile = Path.Combine(_context.Server.MapPath(_context.Request.ApplicationPath), _importDirectory,"pricesAdded.txt");

            Stream stream = await Request.Content.ReadAsStreamAsync();

            using (var fileStream = new FileStream(fullPathToFile, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            int result = SPExecutor.Execute("spImportPricesAdded", new SqlParameter { ParameterName = "@FilePath"}, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };

            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };

            
        }


        [ActionName("ImportOrders")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportOrders()
        {

            string errorMsg;

            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportOrders", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg)};
          
            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }

        /// <summary>
        /// Загрузить сезонный ассортимент
        /// </summary>
        /// <returns></returns>
        
        [HttpPost]
        public async Task<HttpResponseMessage> ImportSeasonOffers()
        {

            string errorMsg;

            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportSeasonStockItems", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };

            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }

        /// <summary>
        /// Загрузить скидочный ассортимент
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<HttpResponseMessage> ImportSaleOffers()
        {

            string errorMsg;

            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportSaleItems", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };

            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }
        /// <summary>
        /// Товары в пути и в производстве
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> ImportOnWayItems()
        {

            string errorMsg;

            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportOnWayItems", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };

            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }

        /// <summary>
        /// Загрузить сезонные заказы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> ImportSeasonOrders()
        {

            string errorMsg;

            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportSeasonOrders", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };
            
            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }

        /// <summary>
        /// Загрузить реализации 1
        /// </summary>
        /// <returns></returns>
        [ActionName("ImportSales")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportSales()
        {
            string errorMsg;
            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportSales", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };
         //   _dbactiologs.Add("Sales");
            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }

        /// <summary>
        /// lapenkov:9090/exchange/importrests
        /// </summary>
        /// <returns></returns>
        [ActionName("ImportRests")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportRests()
        {
            string errorMsg;
            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportRests", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };
        
            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };


        }


        /// <summary>
        /// Загрузка остатков - резервов для клиентов
        /// lapenkov_vi:9090/api/exchange/importrestsofpartners
        /// </summary>
        /// <returns></returns>
        [ActionName("ImportRestsOfPartners")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportRestsOfPartners()
        {
            string errorMsg;
         
            var stream = await Request.Content.ReadAsStreamAsync();

            
            int result = SPExecutor.Execute("spImportRestsOfPartners", 
                new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(stream) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };

            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };


        }

        /// <summary>
        /// http://terminal.yst.ru/api/exchange/ImportPricesForOneClient
        /// </summary>
        /// <returns>result=0 if ok,else return result from stored procedure</returns>
        [ActionName("ImportPricesForOneClient")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportPricesForOneClient()
        {


            string errorMsg;
            string resultxml = await Request.Content.ReadAsStringAsync();
            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(resultxml));


            int result = SPExecutor.Execute("spImportPricesOfOneClient", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error : " + errorMsg) };

            return new HttpResponseMessage() { Content = new StringContent(result.ToString()) };



        }

        /// <summary>
        /// Возвраты товаров загружаем обратно
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ImportSaleReturns() =>await ExecuteStoredProcedureWithXmlParameter(Request, "spImportSaleReturns");
     

        /// <summary>
        /// Возвраты товаров загружаем обратно
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ImportClaims() =>     
             await ExecuteStoredProcedureWithXmlParameter(Request, "spImportClaims");
     


        /// <summary>
        /// Загрузка заданий на отгрузку
        /// http://terminal.yst.ru/api/exchange/ImportJobsForShipment
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ImportJobsForShipment()
        {

          return  await ExecuteStoredProcedureWithXmlParameter(Request, "spImportJobsForShipment");
     
        }



        private async Task<HttpResponseMessage> ExecuteStoredProcedureWithXmlParameter(HttpRequestMessage request, string procname)
        {
            int result = 0;
            string errorMsg;
                        
            var stream = await request.Content.ReadAsStreamAsync();

            var parameters = new[] {

                new SqlParameter{ParameterName="@xmlData",SqlDbType=SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml( stream)},
                new SqlParameter { ParameterName="@b",SqlDbType=SqlDbType.Int, Direction=ParameterDirection.ReturnValue },
               new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output }};

            result = SPExecutor.Execute(procname, parameters, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error  " + errorMsg), StatusCode = HttpStatusCode.InternalServerError };

            return new HttpResponseMessage { Content = new StringContent(result.ToString()) };


        }
    }



}
