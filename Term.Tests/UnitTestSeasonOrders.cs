using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Term.Services;
using System.Net;
using System.Configuration;
using System.Web;

using System.Linq;
using Term.Soapmodels;
using Yst.Context;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestSeasonOrders
    {
        private readonly string _login = ConfigurationManager.AppSettings["LoginWS"];
        private readonly string _password = ConfigurationManager.AppSettings["PasswordWS"];
        SoapServiceForSeasonOrders ss = new SoapServiceForSeasonOrders();
        
      
         
       // [TestMethod]

        [TestInitialize]
        public void Initialize()
        {
          
            ss.PreAuthenticate = true;
            ss.Credentials = new NetworkCredential(_login, _password);
        }
        public void TestMethodCreateSeasonOrder()
        {         

     

            SoapProduct[] products = new [] {new SoapProduct{Code="9100201",Quantity=4,Storage="00005"}, new SoapProduct{Code="9178528",Quantity=4,Storage="00005"}};
          var result =  ss.CreateSeasonOrder("92533",10, products,  "00005", DateTime.Now, String.Empty, String.Empty);
          Assert.AreEqual(result.Success, true);

        }


        [TestMethod]
        public void TestMethodAnalyseSeasonOrder()
        {
            // decimal val = 1234567;

            string guid = "492b1567-5588-11e6-963c-d4ae52b5e909";
            


            var result = ss.AnalyseSeasonOrder(guid);
            Assert.AreEqual(result.Success, true);

            Assert.IsTrue(result.Products.Count() > 0);
            
        }



        [TestMethod]
        public void TestMethodGroupBySeasonOrderResult()
        {
            // decimal val = 1234567;

            string guid = "8BE745A9-8DBB-11E5-8831-D4AE52B5E909";

            AppDbContext DbContext = new AppDbContext();
            var result = ss.AnalyseSeasonOrder(guid);
            
                var listOfProductIds = result.Products.Select(p => Int32.Parse(p.Code));
                var products = DbContext.Products.Where(p => listOfProductIds.Any(ls => ls == p.ProductId)).ToList();
                var onWayProducts = DbContext.OnWayItems.Where(p => listOfProductIds.Any(ls => ls == p.ProductId)).GroupBy(p => p.ProductId).Select(p => new { ProductId = p.Key, Count = p.Sum(cnt => cnt.Count) }).ToList();

       

            Assert.AreEqual(result.Success, true);

            Assert.IsTrue(onWayProducts.Count() > 0);

        } 
    }
}
