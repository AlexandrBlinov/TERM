using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Term.DAL;
using Term.Web.Filters;
using Yst.Context;

namespace Term.Web.Controllers.API
{
    [AdminHashAuth]
    public class PricesApiController : ApiController
    {
         private readonly AppDbContext _dbContext;
        public PricesApiController() : this(new AppDbContext()) { }
        public PricesApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Выводит товары которые есть на остатках или отварах в пути (для сравнения в 1С)
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>

        [HttpGet]
        public IQueryable<PriceOfPartner> Get([FromUri] string partnerId)
        {
            var productsInRest =_dbContext.Set<RestOfProduct>().Select(p => p.ProductId);

            var productsOnWay = _dbContext.Set<OnWayItem>().Select(p => p.ProductId);
            return _dbContext.Set<PriceOfPartner>().Where(o => o.PartnerId == partnerId && (productsInRest.Contains(o.ProductId) ||productsOnWay.Contains(o.ProductId) )).AsQueryable();
           
        }

       
    }
}
