using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Yst.Context;
using YstProject.Services;
using Term.DAL;
using Term.Web.Filters;

namespace Term.Web.Controllers.API
{
 /// <summary>
 /// Товары в пути
 /// </summary>

    public class OnWayItemsController : ApiController
    {

        private readonly AppDbContext _dbContext;
        private readonly ServicePartnerPoint _sp;
        private readonly DaysToDepartmentService _dds;
        
        private readonly Expression<Func<OnWayItem, OnWayItemDto>> _selector = p => 
        new OnWayItemDto
        {
            ProductId = p.ProductId,
            ProdOrWay = p.ProdOrWay,
            DateOfArrival = p.DateOfArrival,
            Count = p.Count
        };
        
         public OnWayItemsController ():this(new AppDbContext() ,new ServicePartnerPoint(),new DaysToDepartmentService())
	      {	      }

            public OnWayItemsController(AppDbContext db, ServicePartnerPoint sp,DaysToDepartmentService dds)
	    {
                _dbContext=db;
                _sp = sp;
                _dds = dds;
	    }


            // GET api/onwayitems
            public IQueryable<OnWayItemDto> Get()
            {
               return _dbContext.Set<OnWayItem>().Select(_selector);
            
            }

            // GET api/onwayitems/5
            public IEnumerable<NumberOfDaysWithCount> Get(int id)
            {
                int point_id =_sp.getPointID();
              //  int point_id = 130;
                return _dds.GetDaysProductFromOnWays(point_id, productid: id);
                            
            }


    }
}
