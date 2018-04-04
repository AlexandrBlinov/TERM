using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Term.Services;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestDurationGoogleApi
    {
        private static string path = @"D:\Projects\Terminal\Term.Tests\XMLFileGoogleResponse.xml";
        [TestMethod]
        public void TestMethod1()
        {
            //var xmldoc = new XmlDocument();
            var xdoc = XDocument.Load(path);
            
            var result = xdoc.Elements("DistanceMatrixResponse").Elements("status").Any(el => el.Value == "OK");

            Assert.AreEqual(result, true);

            Assert.AreEqual(xdoc.Descendants("element").Count(), 4);

            var distance =
            
                xdoc.Descendants("element")
                    .Elements("duration")
                    .Elements("value")
                    .Sum(el => int.Parse(el.Value));

            Assert.IsTrue(distance > 0);
            
        }

        [TestMethod]
        public void TestMethodGetDuration()

        {
            
            Stream stream = File.OpenRead(path);
            var duration = GoogleDistanceService.GetTotalValueFromXml(stream, "duration");
            Assert.AreEqual(duration , 89037);

        }

        [TestMethod]
        public void TestMethodGetDurationFromService()
        {
            string origin = "57.6600331,39.83873785";
            string dest = "55.089490,36.6329070|54.558099,36.3500409|54.036373,35.7717197|54.168975,37.6339178";
            var duration = new GoogleDistanceService().GetDurationInSeconds(origin, dest);
            Assert.AreEqual(duration, 89037);

        }

    }
}