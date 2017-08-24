using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yst.Utils;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class CacheTests
    {

       

        IList<string> GetStrings(int point) {
            string[] arr=  { point.ToString(),"first", "second", "third" };
            return arr.ToList();
        }


        [TestInitialize]
        void Initialize() {
             
        
        }

        [TestMethod]
        public void TestMethodAddValueToCache()
        {
            CacheService service = new CacheService();
            int point1 = 1;
            int point2 = 2;
            service.GetOrAdd("product.disks"+point1.ToString(), () => GetStrings(point1));
            service.GetOrAdd("product.disks" + point2.ToString(), () => GetStrings(point2));

            Assert.IsTrue(service.Count== 2);
     }

        [TestMethod]
        public void TestMethodRemoveValueFromCache()
        {
            CacheService service = new CacheService();
            int point1 = 1;
            int point2 = 2;
            service.GetOrAdd("product.disks" + point1.ToString(), () => GetStrings(point1));
            service.GetOrAdd("product.disks" + point2.ToString(), () => GetStrings(point2));

            Assert.IsTrue(service.Count == 2);

            Assert.IsNotNull(service.Get<List<string>>("product.disks" + point1.ToString()));
            service.Remove("product.disks" + point1.ToString());
            Assert.IsNull(service.Get < List<string>>("product.disks" + point1.ToString()));

            
        }

        [TestMethod]
        public void TestMethodRemoveValueFromCacheNotFails()
        {
            CacheService service = new CacheService();
            int point1 = 1;
            int point2 = 2;
            service.GetOrAdd("product.disks" + point1.ToString(), () => GetStrings(point1));
            service.GetOrAdd("product.disks" + point2.ToString(), () => GetStrings(point2));

            Assert.IsTrue(service.Count == 2);

            Assert.IsNotNull(service.Get<List<string>>("product.disks" + point1.ToString()));
            service.Remove("product.disks2344234234" + point1.ToString());
            Assert.IsTrue(service.Count == 2);


        }
    }
}
