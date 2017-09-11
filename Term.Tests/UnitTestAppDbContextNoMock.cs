using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yst.Context;
using System.Linq;
using Term.DAL;
using System.Collections.Generic;
using System.Data.Entity;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestAppDbContextNoMock
    {
        private AppDbContext _dbContext;
        [TestInitialize]
        public void Initialize()
        {
            _dbContext = new AppDbContext();

        }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1, 1);
        }

       
        [TestMethod]
        public void CheckIfOrderDetailsExists()
        {
            Assert.IsTrue(_dbContext.OrderDetails.Count() > 0);
        }

        
        [TestMethod]
        public void CheckOrderDetailsInOneOrder()
        {
            Guid guid =Guid.Parse("343F6508-9469-41CE-A3CF-11CD84B964D7");
           var order = _dbContext.Orders.Find(guid);        
            
            Assert.AreEqual(order.OrderDetails.Count() ,7);
                      
        }

        [TestMethod]
        public void SubstituteOrderDetails()
        {
            Guid guid = Guid.Parse("AB865ECF-63B5-11E2-8C05-14DAE9DDA2A1");
            var order = _dbContext.Orders.Include(o=>o.OrderDetails).First(p=>p.GuidIn1S==guid);

            var newList = new List<OrderDetail>{
                new OrderDetail { RowNumber = 1, ProductId = 9123456, Count = 1, GuidIn1S = guid, Price = 10, PriceInitial = 11, PriceOfClient = 12, PriceOfPoint = 13 },
                new OrderDetail { RowNumber = 2, ProductId = 9123457, Count = 2, GuidIn1S = guid, Price = 10, PriceInitial = 11, PriceOfClient = 12, PriceOfPoint = 13 },
                new OrderDetail { RowNumber = 3, ProductId = 9123458, Count = 3, GuidIn1S = guid, Price = 10, PriceInitial = 11, PriceOfClient = 12, PriceOfPoint = 13 }
                };
            _dbContext.OrderDetails.RemoveRange(order.OrderDetails);
            
            order.OrderDetails = newList;
          //  _dbContext.Orders.Attach(order);
           
          // _dbContext.Entry(order).State = EntityState.Modified;

            
            order.CalculateTotals();
            _dbContext.SaveChanges();
            Assert.AreEqual(order.OrderDetails.Count, 3);

        }


    }
}
