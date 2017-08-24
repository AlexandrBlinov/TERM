using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Yst.Context;
using YstProject.Models;
using Term.DAL;
using Term.Web.Filters;
using YstProject.Services;

namespace Term.Web.Controllers.API
{
    /// <summary>
    /// Gets all sales for sync with 1S
    /// </summary>
    [AdminHashAuth]
    public class SalesApiController : ApiController
    {
        AppDbContext _dbContext;
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
        public IQueryable<SaleDTO> Get([FromUri]DateTime beginDate, [FromUri] DateTime endDate)
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
        public IQueryable<SaleDTO> GetByPartner([FromUri]DateTime beginDate, [FromUri] DateTime endDate, string partnerId)
        {

            /// приводим дату к концу текущего дня
            endDate = endDate.AddDays(1).AddTicks(-1);

            var query = GetDocuments(beginDate, endDate, o => o.PartnerId.Equals(partnerId));

            return query;

        }

        public async Task<HttpResponseMessage> ImportSales()
        {
            int result = 0;
            string errorMsg;

            var stream = await Request.Content.ReadAsStreamAsync();


            var parameters = new[] {
               
                new SqlParameter{ParameterName="@xmlData",SqlDbType=SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml( stream)},
                new SqlParameter { ParameterName="@b",SqlDbType=SqlDbType.Int, Direction=ParameterDirection.ReturnValue },
               new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output }};

            result = SPExecutor.Execute("spImportSales", parameters, out errorMsg);

            if (result != 0) return new HttpResponseMessage { Content = new StringContent("Error  " + errorMsg), StatusCode = HttpStatusCode.InternalServerError };

            return new HttpResponseMessage { Content = new StringContent(result.ToString()) };
        }

    }
}