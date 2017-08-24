using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Term.DAL;
using Yst.Context;
using YstTerm.Models;

namespace Term.Web.Controllers.API
{
    public class PartnerServiceApiController : ApiController
    {
        private readonly AppDbContext _dbContext;
        
        public PartnerServiceApiController()
            : this(new AppDbContext())
        {

        }
        public PartnerServiceApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        
        /// <summary>
        /// from fiddler
        /// Content-Type: application/json
        ///  ["AVATYRE FREEZE 175/65R14 82Q шип.","111-B Amtel Planet DC 82H 185/60R14"]
        /// </summary>
        [HttpPost]
        [ActionName("getproductprices")]
        public IEnumerable<ProductForServiceDto> GetProductsByNames(IEnumerable<string> values)
        {
            var arrValues = values.ToArray();
            var restsProductIds = _dbContext.Set<RestOfProduct>().Select(rest => rest.ProductId);
            var products = _dbContext.Set<Product>().Where(p => (arrValues.Contains(p.Name) && restsProductIds.Contains(p.ProductId)));

            var pricesOfProducts = _dbContext.Set<PriceOfProduct>().Where(pop => products.Select(p => p.ProductId).Contains(pop.ProductId));

            return (from prod in products
                    join pop in pricesOfProducts on prod.ProductId equals pop.ProductId
                    select new ProductForServiceDto { ProductId = prod.ProductId, PriceReccOpt = (double)(pop.Price1??0),PriceReccRozn = (double)(pop.Price2??0), Name = prod.Name }).AsEnumerable();


        }
    }
}
