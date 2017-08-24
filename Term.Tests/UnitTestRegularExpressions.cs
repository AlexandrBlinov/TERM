﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Term.DAL;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using Term.Utils;
using Term.CustomAttributes;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestRegulars
    {
        [TestMethod]
        public void TestIfEnumerableReturnsChars()
        {
           var arr= string.Join("", Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray()).ToCharArray();
            //var arr = Enumerable.Range(0, 9).ToString().ToCharArray();
            Assert.AreEqual(arr[0],'0');
            Assert.AreEqual(arr[arr.Length-1], '9');
            //Enumerable.Range(1,9).f

        }

        [TestMethod]
        public void TestIfAhrefExtractedSuccess()
        {
            string source = "<a href=\"ghfgh\" sdfsdfdf sdf sdf sf sdf >";
            string pattern = "href=\"(.*)\"";


        //    string result = RegexExtractStringProvider.GetMatchedStringByOrder(source, pattern, 1);

            var output = Regex.Replace(source, pattern, "test1");

            Assert.AreEqual(output, "<a href=\"test1\" sdfsdfdf sdf sdf sf sdf >");

        }

        [TestMethod]
        public void TestIfGetMatchedStringByOrderIsTrue()
        {
            string source = "PXR0082003=PXR0386503";
            string pattern = @"(\w+)";


            string result=RegexExtractStringProvider.GetMatchedStringByOrder(source,pattern,1);

            Assert.AreEqual(result,"PXR0082003");

        }

        [TestMethod]
        public void TestIfGetMatchedStringByOrderIsFalse()
        {
            string source = "PXR0082003=PXR0386503";
            string pattern = @"(\w+)";


            string result = RegexExtractStringProvider.GetMatchedStringByOrder(source, pattern, 1);

            Assert.AreNotEqual(result, "PXR008200");

        }


        [TestMethod]
        public void TestIfReplaceSpecialCharacters()
        {
            string input = "hjksdf : . lsdkfj lksj dflkskldf ";
            string pattern = @"[\.\s\(\)\:\+]";
            string replacement = "_";
            string result = Regex.Replace(input, pattern, replacement);
          //  Regex rgx = new Regex(pattern);
          //  string result = rgx.Replace(input, replacement);
            Assert.AreEqual(result, "hjksdf_____lsdkfj_lksj_dflkskldf_");
        //    Assert.AreNotEqual(result, "PXR008200");

        }


        [TestMethod]
        public void TestStringUtilsReturnsArrayOfGuids()
        {
            string source = "eb497b27-afe4-11e6-b82b-d4ae52b5e909;eb497b27-afe4-11e6-b82b-d4ae52b5e908";
            var result = StringUtils.GetArrayOfGuidsFromString(source);
            Assert.AreEqual(result.Count, 2);
        }


        [TestMethod]
        public void TestStringGetNumberOfDeliveryDays()
        {
            string source = "132";

            var result =source.Split('-').Last();
            
            Assert.AreEqual(result, "132");
        }


        [TestMethod]
        public void TestDateTimeToString()
        {
            DateTime dd= DateTime.Now;

       

            var x=String.Format("{0:yyyyMMdd}", dd);
            var y = dd.ToString("yyyyMMdd");

            Assert.AreEqual(x, y);
        }
    }
}
