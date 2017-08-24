using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject1
{

    public interface ILoggerDependency
    {
        string GetCurrentDirectory();
        string GetDirectoryByLoggerName(string loggerName);
        string DefaultLogger { get; }
    } 

    [TestClass]
    public class UnitTestInterface
    {
        [TestMethod]
        public void TestMethod1()
        {
            ILoggerDependency loggerDependency =
    Mock.Of<ILoggerDependency>(d => d.GetCurrentDirectory() == "D:\\Temp");
            var currentDirectory = loggerDependency.GetCurrentDirectory();

            Assert.AreEqual(currentDirectory,"D:\\Temp");
        }
    }
}
