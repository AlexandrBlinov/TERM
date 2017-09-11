using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestLinq
    {
        OrderDetailDto[] items = new[] { new OrderDetailDto { OrderGuid=Guid.Parse("72B274B1-700C-11E6-851A-D4AE52B5E909"),Count=4,ProductId=1,DepartmentId=5,NumberIn1S="1",PointId=1 },
                new OrderDetailDto { OrderGuid=Guid.Parse("0A8E3F77-7434-11E6-851A-D4AE52B5E909"),Count=4,ProductId=3,DepartmentId=5,NumberIn1S="2",PointId=2 },
                new OrderDetailDto { OrderGuid=Guid.Parse("72B274B1-700C-11E6-851A-D4AE52B5E909"),Count=4,ProductId=2,DepartmentId=5,NumberIn1S="1",PointId=1 },
                new OrderDetailDto { OrderGuid=Guid.Parse("0A8E3F77-7434-11E6-851A-D4AE52B5E909"),Count=4,ProductId=4,DepartmentId=5,NumberIn1S="1",PointId=2 },
                new OrderDetailDto { OrderGuid=Guid.Parse("0A8E3F77-7434-11E6-851A-D4AE52B5E909"),Count=4,ProductId=4,DepartmentId=5,NumberIn1S="1",PointId=2 }
            };

        class OrderDetailDto {
            public Guid OrderGuid { get; set; }
            public string NumberIn1S { get; set; }
            public int ProductId { get; set; }
            public int Count { get; set; }
            public int DepartmentId { get; set; }
            public int PointId { get; set; }           
            
        }

        [TestMethod]
        public void TestMethodCheckGrouping()
        {

           


           var grouped= items.GroupBy(p => p.OrderGuid);

            foreach (var groupedItem in grouped)
            {
               

                if (groupedItem.Key == Guid.Parse("72B274B1-700C-11E6-851A-D4AE52B5E909"))
                    Assert.IsTrue(groupedItem.First().NumberIn1S == "1" && groupedItem.Count() == 2);

                if (groupedItem.Key == Guid.Parse("0A8E3F77-7434-11E6-851A-D4AE52B5E909"))
                    Assert.IsTrue(groupedItem.First().NumberIn1S == "2" && groupedItem.Count() == 3);


                foreach (OrderDetailDto detail in groupedItem)
                 Debug.WriteLine(detail.ProductId); 
                
            }
        }
    }
}
