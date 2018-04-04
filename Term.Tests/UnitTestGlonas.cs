using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using Term.DAL;
using Yst.Context;
using Term.Web.Services;
using Term.Web.Models;
using Term.Services;
using System.Globalization;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestGlonas
    {

        Mock<AppDbContext> mockContext;
        
        Mock<DbSet<Sale>> saleSet;
        Mock<DbSet<JobForShipment>> jobSet;

        Guid guidOfOrder1 = Guid.Parse("EC535B93-3656-11E7-8505-D4AE52B5E909");
        Guid guidOfOrder2 = Guid.Parse("FD6C5014-2FC9-11E7-A5E2-D4AE52B5E909");
        Guid guidOfOrder3 = Guid.Parse("6855597C-0678-11E8-BB28-D4AE52B5E909");

        IQueryable<Sale> sales;

        Guid guidOfJob1 = Guid.Parse("C1B643D5-0598-11E8-BB28-D4AE52B5E909");
        Guid guidOfJob2 = Guid.Parse("66625B7D-068F-11E8-BB28-D4AE52B5E909");

        IQueryable<JobForShipment> jobs;

        [TestInitialize]
        public void Initialize()
        {
            mockContext = new Mock<AppDbContext>();



            sales = new List<Sale>
            {
               new Sale { GuidIn1S = Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909"),GuidOfOrderIn1S= guidOfOrder1 },
               new Sale { GuidIn1S = Guid.Parse("CB5BFA86-3700-11E7-8505-D4AE52B5E909"),GuidOfOrderIn1S= guidOfOrder2 },

            }.AsQueryable();

            jobs =  new List<JobForShipment>
            {
                new JobForShipment {GuidIn1S=guidOfJob1, Latitude=57.65731F,Longitude=39.83808F,
                    Details= new List<JobForShipmentDetail> {
                        new JobForShipmentDetail{ Id=11,GuidIn1S=guidOfJob1,NumberOfQueue=1, Latitude=57.0991f, Longitude=41.71928f, GuidOfOrderIn1S=guidOfOrder2 ,IsDelivered=false  },
                        new JobForShipmentDetail{ Id=12,GuidIn1S=guidOfJob1,NumberOfQueue=2, Latitude=58.0991f, Longitude=42.71928f, GuidOfOrderIn1S=guidOfOrder1  },
                        //new JobForShipmentDetail{ Id=12,GuidIn1S=guidOfJob1,NumberOfQueue=2, Latitude=57.0991f, Longitude=41.71928f, GuidOfOrderIn1S=guidOfOrder2  },
                    } },

                    new JobForShipment {GuidIn1S=guidOfJob2, Latitude=57.65731F,Longitude=39.83808F,
                    Details= new List<JobForShipmentDetail> {
                        new JobForShipmentDetail{ Id=13,GuidIn1S=guidOfJob2,NumberOfQueue=1, Latitude=57.0991f, Longitude=41.71928f, GuidOfOrderIn1S=guidOfOrder3  },
                        new JobForShipmentDetail{ Id=14,GuidIn1S=guidOfJob2,NumberOfQueue=2, Latitude=57.0991f, Longitude=41.71928f, GuidOfOrderIn1S=guidOfOrder3  },
                        //new JobForShipmentDetail{ Id=12,GuidIn1S=guidOfJob1,NumberOfQueue=2, Latitude=57.0991f, Longitude=41.71928f, GuidOfOrderIn1S=guidOfOrder2  },
                    },



             }}.AsQueryable();


             saleSet = new Mock<DbSet<Sale>>();
            saleSet.As<IQueryable<Sale>>().Setup(m => m.Provider).Returns(sales.Provider);
            saleSet.As<IQueryable<Sale>>().Setup(m => m.Expression).Returns(sales.Expression);
            saleSet.As<IQueryable<Sale>>().Setup(m => m.ElementType).Returns(sales.ElementType);
            saleSet.As<IQueryable<Sale>>().Setup(m => m.GetEnumerator()).Returns(sales.GetEnumerator());

            jobSet = new Mock<DbSet<JobForShipment>>();
            jobSet.As<IQueryable<JobForShipment>>().Setup(m => m.Provider).Returns(jobs.Provider);
            jobSet.As<IQueryable<JobForShipment>>().Setup(m => m.Expression).Returns(jobs.Expression);
            jobSet.As<IQueryable<JobForShipment>>().Setup(m => m.ElementType).Returns(jobs.ElementType);
            jobSet.As<IQueryable<JobForShipment>>().Setup(m => m.GetEnumerator()).Returns(jobs.GetEnumerator());

            mockContext.Setup(c => c.Set<Sale>()).Returns(saleSet.Object);
            mockContext.Setup(c => c.Set<JobForShipment>()).Returns(jobSet.Object);
        }


        [TestMethod]
        public void Method_ReturnsDoubleToString()
        {
            double d = 41.71928f;
            var x =d.ToString("#.#######",CultureInfo.InvariantCulture);
            Assert.AreEqual(x, "41.7193");
        }

        [TestMethod]
        public void TestMethodCheckIfFoundJob()
        {
            

            Assert.IsTrue(mockContext.Object.Set<Sale>().Count() == 2);
            Assert.IsTrue(mockContext.Object.Set<JobForShipment>().Count() == 2);

            var service=new GlonasService(mockContext.Object, null ,null);
              var job1  =     service.GetJobForSaleByGuid(Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909"));

            Assert.IsTrue(job1 != null && job1.GuidIn1S== guidOfJob1);


        }

        [TestMethod]
        public void TestMethodCheckIfAnyDelivered_Returnstrue()
        {            

            var service = new GlonasService(mockContext.Object,null, null);
            var job1 = service.GetJobForSaleByGuid(Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909"));
            
            Assert.IsTrue(job1 != null && job1.GuidIn1S == guidOfJob1);

            Assert.IsFalse(service.GetIfAnyItemsOfJobDelivered(job1));


        }

        [TestMethod]
        public void TestMethodGetDetails_Returns2()
        {

            var service = new GlonasService(mockContext.Object,null, null);
            var job1 = service.GetJobForSaleByGuid(Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909"));

            Assert.IsTrue(job1 != null && job1.GuidIn1S == guidOfJob1);

            Assert.AreEqual(service.GetDetailsOfJob(job1).Count,2);

        }

        [TestMethod]
        public void TestMethodGetOrder_ReturnsSecondRecord()
        {
            var saleGuid = Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909");
            var service = new GlonasService(mockContext.Object,null,  null);
            var job1 = service.GetJobForSaleByGuid(saleGuid);

            Assert.IsTrue(job1 != null && job1.GuidIn1S == guidOfJob1);
            Guid orderGuid = service.GetGuidOfOrder(saleGuid)??Guid.Empty;

            Assert.AreNotEqual(orderGuid, Guid.Empty);

          var detail=  service.GetDetailForOrderOfJob(job1, orderGuid);


            Assert.IsTrue(detail != null && detail.NumberOfQueue == 2);
            //Assert.AreEqual(service.GetDetailsOfJob(job1).Count, 2);

        }


        [TestMethod]
        public void TestMethodCompareStructs()
        {
            

            IList<Coordinates> listDest = new List<Coordinates>();

            IList<Coordinates> listSrc = new List<Coordinates>
            {
                new Coordinates(1, 1),
                new Coordinates(1, 1),
                new Coordinates(2, 1),
                new Coordinates(2, 1),
                new Coordinates(2, 3),
            };

            foreach (var x in listSrc)
            {
                if (!listDest.Contains(x))
                    listDest.Add(x);
            }

           Assert.AreEqual(listDest.Count, 3);
            Assert.AreEqual(new Coordinates(1, 1), new Coordinates(1, 1));
           
            ;        }

        [TestMethod]
        public void TestMethodGetListOfCoorinates_Returns2()
        {


            var saleGuid = Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909");
            var service = new GlonasService(mockContext.Object,null, null);
            var job1 = service.GetJobForSaleByGuid(saleGuid);

            var list=service.GetListOfCoordinates(job1, 0, 2);

            
            Assert.AreEqual(list.Count , 2);

             list = service.GetListOfCoordinates(job1, 0, 1);

            
            Assert.AreEqual(list.Count, 1);

            ;
        }

        [TestMethod]
        public void TestMethodGetListOfCoorinates_CalculateDistance()
        {

           AppDbContext _dbContext = new AppDbContext();
            MtsLocationsContext _dbMtsContext = new MtsLocationsContext();


            var saleGuid = Guid.Parse("CB5BFA84-3700-11E7-8505-D4AE52B5E909");
            var service = new GlonasService(mockContext.Object, _dbMtsContext, null);
            var job1 = service.GetJobForSaleByGuid(saleGuid);

            var list = service.GetListOfCoordinates(job1, 0, 2);

            var coords = service.GetDriverCoordinates("Лапшин Н.Ю.");

            
           string dest = String.Join("|", list);

            string origin = coords.ToString();

            // string origin = "57.6600331,39.83873785";
            //string dest = "55.089490,36.6329070|54.558099,36.3500409|54.036373,35.7717197|54.168975,37.6339178";

            var duration = new GoogleDistanceService().GetDurationInSeconds(origin, dest).Result;
            Assert.IsTrue(duration>0);

            Assert.IsNotNull(job1);
            ;
        }

        
        


    }


}
