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
using Term.Web.Filters;

namespace Term.Web.Controllers.API
{
    [AdminHashAuth]
    public class SeasonOrdersApiController : ApiController
    {
         AppDbContext _dbContext;
        public SeasonOrdersApiController() : this(new AppDbContext()) { }
        public SeasonOrdersApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [NonAction]
        private IQueryable<SeasonOrderDTO> GetDocuments(DateTime beginDate, DateTime endDate, Expression<Func<SeasonOrder, bool>> predicate = null)
        {
            var query = _dbContext.Set<SeasonOrder>().Where(o => o.OrderDate >= beginDate && o.OrderDate <= endDate);
            if (predicate != null) query = query.Where(predicate);
            return query.Select(p => new SeasonOrderDTO { DateInternal = p.OrderDate, NumberIn1S = p.NumberIn1S, Totals = p.Total,OrderStatus=(int)p.OrderStatus });

        }

        /// <summary>
        /// gets all sales from begindate to enddate 
        /// call: api/seasonordersapi/get?begindate=ДФ=yyyy-MM-dd&enddate=ДФ=yyyy-MM-dd
        /// </summary>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public IEnumerable<SeasonOrderDTO> Get([FromUri]DateTime beginDate, [FromUri] DateTime endDate)
        {

            /// приводим дату к концу текущего дня
            endDate = endDate.AddDays(1).AddTicks(-1);

            return GetDocuments(beginDate, endDate);


        }

        /// <summary>
        /// gets all sales from begindate to enddate by partnerId
        /// call: api/seasonordersapi/getbypartner?begindate=ДФ=yyyy-MM-dd&enddate=ДФ=yyyy-MM-dd
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public IEnumerable<SeasonOrderDTO> GetByPartner([FromUri]DateTime beginDate, [FromUri] DateTime endDate, string partnerId)
        {

            /// приводим дату к концу текущего дня
            endDate = endDate.AddDays(1).AddTicks(-1);

            var query = GetDocuments(beginDate, endDate, o => o.PartnerId.Equals(partnerId));

            return query;



        }
    }
}
