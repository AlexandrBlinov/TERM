using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Term.DAL;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Web.Mvc;
using Term.Utils;
using Term.CustomAttributes;
using YstProject.Services;
using YstTerm.Models;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestEnums
    {
      /*  [TestMethod]
        public void CheckifMCAistrue()
        {
            //var v1=Enum.Parse(typeof(SeasonOrderStatus), "New");
            var fi = typeof(SeasonOrderStatus).GetField("New");
            var attr=fi.GetCustomAttributes(typeof(MultiCultureDescriptionAttribute), false).First();
            Assert.AreEqual(((MultiCultureDescriptionAttribute)attr).Description, "Новый");
            
        } */

        [TestMethod]
        public void TestIfMulticultureDescriptionReturnsNoviy()
        {
            var list=EnumDescriptionProvider.GetSelectListFromEnum<SeasonOrderStatus>();
            Assert.IsTrue(list.ToArray()[0].Text.Equals("Новый"));
            Assert.IsTrue(list.ToArray()[2].Text.Equals("Отменен"));

        }

        [TestMethod]
        public void TestIfMulticultureDescriptionReturnsNoviy2()
        {
            var list = EnumDescriptionProvider.GetSelectListFromEnum<OrderStatuses>();
            Assert.IsTrue(list.ToArray()[0].Text.Equals("Новый"));
            Assert.IsTrue(list.ToArray()[2].Text.Equals("Отменен"));

        }


        [TestMethod]
        public void TestIfMulticultureDescriptionReturnsNew()
        {
            var cultureInfo=CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Dictionary<int, string> dict = Enum.GetValues(typeof(SeasonOrderStatus)).Cast<SeasonOrderStatus>().Select(p => new
            {
                Id = (int)p,
                Name = EnumDescriptionProvider.GetMultiCultureDescription(p)

            }).ToDictionary(p => p.Id, p => p.Name);
            Assert.IsTrue(dict[1].Equals("New"));

        }

        [TestMethod]
        public void ViewDataDictionaryInitialize()
        {
            var dict = new ViewDataDictionary { { "first", "firstvalue" }, { "second", "secondvalue" } };

            Assert.IsTrue(dict["first"].Equals("firstvalue"));
        }


        [TestMethod]
        public void EqualityComparerReturnsDistinct()
        {

            Producer[] producers1 = new[] { new Producer { ProducerId = 3657, Name = "VISSOL", ProductType = ProductType.Disk } };
            Producer[] producers2 = Defaults.ProducersForgedWheels;

            var result=producers2.Union(producers1,new ProducerEqualityComparer()).ToArray();
            Assert.AreEqual(result.Count(),3);
        }

        
    }
}
