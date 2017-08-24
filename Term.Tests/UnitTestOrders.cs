using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Term.Services;
using System.Net;
using System.Configuration;
using System.Web;

using System.Linq;
using System.Web.Services.Protocols;
using Term.Soapmodels;
using Yst.Context;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestOrders
    {
        
        private  readonly string _login = ConfigurationManager.AppSettings["LoginWS"];
        private  readonly string _password = ConfigurationManager.AppSettings["PasswordWS"];


        // [TestMethod]

        [TestInitialize]
        public void Initialize()
        {
          
           
        }
         
        [TestMethod]
        public void TestMethodCreateOrderWithTransportData()
        {
            ServiceTerminal ss = new ServiceTerminal();

            DeliveryInfo di = new DeliveryInfo
            {
                ContactFio = "Иванов Иван",
                ContactPhone = "89301263747",
                BlockType = "",
                CityId = "760323023",
                House = "23",
                CostOfDelivery = 25,
                PostalCode = "150060",
                RegionId = "76",
                Street = "Leningradsky prospect",
                StreetType = "pr-ct",
                TerminalOrAddress = false,
                TerminalCode = ""
            };

         
            string partnerId = "00094";
            int pointId = 130;

            SoapProduct[] products = new[] { new SoapProduct { Code = "9100201", Quantity = 4, Storage = "00005" }, 
                new SoapProduct { Code = "9178528", Quantity = 4, Storage = "00005" } };

            ss.PreAuthenticate = true;
            ss.Credentials = new NetworkCredential(_login, _password);
            var result2 = ss.CreateOrderWithTransportData(partnerId, pointId, products, "", String.Format("{0:yyyyMMdd}", DateTime.Now), false, true, di,null);
            

           // var result = ss.CreateOrder(partnerId, pointId, products, "", String.Format("{0:yyyyMMdd}", DateTime.Now), false, true);
            //string rawXml = ss.Xml;
         // Assert.AreEqual(result.Success, true);

        }

        [TestMethod]
        public void TestMethodCreateOrder()
        {
            var ss = new ServiceTerminal();

            // Client570
          
            string partnerId = "92532";
            int pointId = 1381;

            SoapProduct[] products = new[] { new SoapProduct { Code = "9167154", Quantity = 4, Storage = "00005" }, 
                new SoapProduct { Code = "9200480", Quantity = 4, Storage = "00005" } };

            ss.PreAuthenticate = true;
            ss.Credentials = new NetworkCredential(_login, _password);
            var result2 = ss.CreateOrder(partnerId, pointId, products, "", String.Format("{0:yyyyMMdd}", DateTime.Now), false, true);


            // var result = ss.CreateOrder(partnerId, pointId, products, "", String.Format("{0:yyyyMMdd}", DateTime.Now), false, true);
            //string rawXml = ss.Xml;
            Assert.AreEqual(result2.Success, true);

        }

        [TestMethod]
        public void TestMethodChangeOrderStatus()
        {
            var ss = new ServiceTerminal();

         

            ss.PreAuthenticate = true;
            ss.Credentials = new NetworkCredential(_login, _password);
            var result2 = ss.ChangeOrderStatus("c975c3d4-c35d-11e6-b82b-d4ae52b5e909",3);


            // var result = ss.CreateOrder(partnerId, pointId, products, "", String.Format("{0:yyyyMMdd}", DateTime.Now), false, true);
            //string rawXml = ss.Xml;
            Assert.AreEqual(result2.Success, false);

        }

        [TestMethod]
        public void TestMethodGetDebt()
        {
            var ss = new ServiceTerminal();



            ss.PreAuthenticate = true;
            ss.Credentials = new NetworkCredential(_login, _password);
            var result = ss.GetDebt("92498");

           
            Assert.AreEqual(result.Success, true);
            Assert.IsTrue(result.ExpiredDebt.Length>0);
            Assert.IsTrue(result.PlanDebt.Length > 0);

        }



    }
}
