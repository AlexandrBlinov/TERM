using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yst.Context;
using YstProject.Models;
using Term.DAL;

namespace Term.Web.Controllers.API
{
    /// <summary>
    /// Gets all sales for sync with 1S
    /// </summary>
    public class SalesApiController : ApiController
    {
       private readonly AppDbContext _dbContext;
        public SalesApiController() : this(new AppDbContext()) { }
        public SalesApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [NonAction]
        private IQueryable<SaleDTO> GetDocuments(DateTime beginDate, DateTime endDate, Expression<Func<Sale, bool>> predicate = null)
        {
            var query = _dbContext.Set<Sale>().Where(o => o.SaleDate >= beginDate && o.SaleDate <= endDate);
            if (predicate != null) query = query.Where(predicate);
            return query.Select(p => new SaleDTO { DateInternal = p.SaleDate, NumberIn1S = p.NumberIn1S, Totals = p.Total });

        }

        /// <summary>
        /// gets all sales from begindate to enddate 
        /// call: api/salesapi/get?begindate=ДФ=yyyy-MM-dd&enddate=ДФ=yyyy-MM-dd
        /// </summary>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public IEnumerable<SaleDTO> Get([FromUri]DateTime beginDate, [FromUri] DateTime endDate)
        {

            /// приводим дату к концу текущего дня
            endDate = endDate.AddDays(1).AddTicks(-1);

            return GetDocuments(beginDate, endDate);


        }

        /// <summary>
        /// gets all sales from begindate to enddate by partnerId
        /// call: api/salesapi/getbypartner?begindate=ДФ=yyyy-MM-dd&enddate=ДФ=yyyy-MM-dd
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public IEnumerable<SaleDTO> GetByPartner([FromUri]DateTime beginDate, [FromUri] DateTime endDate, string partnerId)
        {

            /// приводим дату к концу текущего дня
            endDate = endDate.AddDays(1).AddTicks(-1);

            var query = GetDocuments(beginDate, endDate, o => o.PartnerId.Equals(partnerId));

            return query;



        }

        
      
    }
}
