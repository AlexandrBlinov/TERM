using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Yst.Context;
using YstProject.Services;

using Term.DAL;

namespace UnitTestProject1
{

    

    [TestClass]
    public class UnitTestAppDbContext
    {

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IList<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            dbSetMock.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => elements.Add(s));
            dbSetMock.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>((s) => elements.Remove(s));

        /*    dbSetMock.Setup(m => m.Find(It.IsAny<object[]>()))
    .Returns<object[]>(ids => dbSetMock.FirstOrDefault(d => d.ID == (int)ids[0])); */

            return dbSetMock;
        }

        [TestMethod]
        public void TestMethod_SetCity_Returns2Records()
        {

            
            IList<City> list=  new List<City> {new City {Id = "76", Name = "Yaroslavl", Abbreviation = "yar", DpdCode = "11"},new City {Id = "77", Name = "Moskow", Abbreviation = "yar", DpdCode = "22"}};
            //var mockCities = new Mock<DbSet<City>>();

            var mockCities = CreateDbSetMock(list);
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(m => m.Set<City>()).Returns(mockCities.Object);

            
            
            Assert.IsTrue(mockContext.Object.Set<City>().Count()==2);
        
        }

        [TestMethod]
        public void TestMethod_GetCostOfDelivery_ReturnsSum_MoreThanZero()
        {

            IList<Product> products = new List<Product>
            {
                new Product {ProductId = 1, Weight = 3, Name = "First product"},
                new Product {ProductId = 2, Weight = 8, Name = "Second product"},
                new Product {ProductId = 3, Weight = 12, Name = "Third product"},
                new Product {ProductId = 4, Weight = 20, Name = "Forth product"}

            };

            IList<Cart> cartItems = new List<Cart>
            {
                new Cart {CartId = "test1",ProductId = 1,Product=products[0],Count = 4},
                new Cart {CartId = "volod",ProductId = 2,Product=products[1],Count = 4},
                new Cart {CartId = "volod",ProductId = 3,Product=products[2],Count = 4},
                new Cart {CartId = "volod",ProductId = 4,Product=products[3],Count = 4},

            };

            IList<RateToMainCity> rates = new List<RateToMainCity>
            {
                new RateToMainCity {CityId = "76",Rate20 = 1,Rate20Door = 2,Rate20_Ekb = 3,Rate20Door_Ekb = 4,Rate40 = 5,Rate40_Ekb = 6,Rate40Door = 7,Rate40Door_Ekb = 8,RatePlus1 = 1,RatePlus1Door = 2,RatePlus1_Ekb = 3, RatePlus1Door_Ekb = 4},
                new RateToMainCity {CityId = "77",Rate20 = 10,Rate20Door = 20,Rate20_Ekb = 30,Rate20Door_Ekb = 40,Rate40 = 50,Rate40_Ekb = 60,Rate40Door = 70,Rate40Door_Ekb = 80,RatePlus1 = 10,RatePlus1Door = 20,RatePlus1_Ekb = 30, RatePlus1Door_Ekb = 40}

            };

            IList<RateToAdditionalCity> ratesAdditional = new List<RateToAdditionalCity>
            {
                new RateToAdditionalCity {CityId = "99",MainCityId = "76",Rate = 1000}

            };

            var httpcontext = new Mock<HttpContextBase>();
            
            
            var mockProducts = CreateDbSetMock<Product>(products);
            var mockCartItems = CreateDbSetMock<Cart>(cartItems);
            var mockRates = CreateDbSetMock<RateToMainCity>(rates);
            var mockRatesAdditional = CreateDbSetMock<RateToAdditionalCity>(ratesAdditional);


            mockRates.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockRates.Object.FirstOrDefault(d => d.CityId == (string)ids[0]));
            mockRatesAdditional.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockRatesAdditional.Object.FirstOrDefault(d => d.CityId == (string)ids[0]));


            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(m => m.Set<Product>()).Returns(mockProducts.Object);
            mockContext.Setup(m => m.Set<Cart>()).Returns(mockCartItems.Object);
            mockContext.Setup(m => m.Set<RateToMainCity>()).Returns(mockRates.Object);
            mockContext.Setup(m => m.Set<RateToAdditionalCity>()).Returns(mockRatesAdditional.Object);

            httpcontext.Setup(m => m.User.Identity.Name).Returns("volod");




            var service = new DeliveryCostCalculatorService(mockContext.Object, httpcontext.Object,null);

        /*    Assert.IsTrue(service.GetCostOfDelivery("99",true)>1000);
            Assert.IsTrue(service.GetCostOfDelivery("76", true) < 1000); */

            
            mockProducts.Object.Add(new Product { ProductId = 5, Weight = 20, Name = "Fifth product" });

            Assert.IsTrue(mockProducts.Object.Count()==5);

            mockProducts.Object.Remove(products[0]);

            Assert.IsTrue(mockProducts.Object.Count() == 4);
            //Assert.IsTrue(httpcontext.Object.User.Identity.Name == "volod");

        }

        [TestMethod]
        public void TestMethod2()
        {

            
            IList<City> list = new List<City> { new City { Id = "76", Name = "Yaroslavl", Abbreviation = "yar", DpdCode = "11" }, new City { Id = "77", Name = "Moskow", Abbreviation = "yar", DpdCode = "22" } };
            //var mockCities = new Mock<DbSet<City>>();

            var mockCities = CreateDbSetMock(list);
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(m => m.Set<City>()).Returns(mockCities.Object);



            Assert.IsTrue(mockContext.Object.Set<City>().Count() == 2);

        }


        private static DataTable CreateDataTable(IEnumerable<ProductWithCountOnDep> records)
        {
            DataTable table = new DataTable();
            table.Columns.Add("DepartmentId", typeof(int));
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Count", typeof(int));

            foreach (var record in records)
            {
                var row = table.NewRow();
                row["DepartmentId"] = record.DepartmentId;
                row["ProductId"] = record.ProductId;
                row["Count"] = record.Count;
                table.Rows.Add(row); 
            }
            
            return table;
        }

        [TestMethod]
        public void TestMethodExecuteSubstractCorrect()
        {

            
            var productWithCountOnDeps = new List<ProductWithCountOnDep>();
            
            productWithCountOnDeps.Add(new ProductWithCountOnDep{ DepartmentId = 5,ProductId = 123,Count= 2});
            productWithCountOnDeps.Add(new ProductWithCountOnDep { DepartmentId = 106, ProductId = 137, Count = 11 });
            using (var context = new AppDbContext())
            {

                var paramProductWithCountOnDeps = new SqlParameter("ProdWithCount",CreateDataTable(productWithCountOnDeps));
                
                paramProductWithCountOnDeps.SqlDbType = SqlDbType.Structured; 
    paramProductWithCountOnDeps.TypeName = "ProductWithCountOnDeps"; 
                
                

                context.Database.ExecuteSqlCommand("EXEC SubtractFromRests @ProdWithCount", paramProductWithCountOnDeps);
                var record=context.Rests.FirstOrDefault(p => p.DepartmentId == 5 && p.ProductId == 123);
                Assert.IsTrue(record.Rest==7);
            }
        }

        [TestMethod]
        public void TestMethodExecuteSubstractGenericCorrect()
        {
            
            var productWithCountOnDeps = new List<ProductWithCountOnDep>();

            //productWithCountOnDeps.Add(new ProductWithCountOnDep { DepartmentId = 5, ProductId = 123, Count = 2 });
            productWithCountOnDeps.Add(new ProductWithCountOnDep { DepartmentId = 106, ProductId = 154, Count = 2 });
            using (var context = new AppDbContext())
            {
                context.ExecuteTableValueProcedure(productWithCountOnDeps, "SubtractFromRests", "@ProdWithCount", "ProductWithCountOnDeps");
                
                
            }
        }

    }
}
