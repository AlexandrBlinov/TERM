using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Term.DAL;
using Term.Web.Services;
using Yst.Context;
using System.Data.Entity;
using System.Runtime.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Term.Web.Controllers.API
{
    /// <summary>
    /// Класс для отображения остатков и поставщиков
    /// </summary>
    public class RestsOfSuppliersApiController : ApiController
    {

        [DataContract (Namespace = "",IsReference = false,Name = "RestItem")]
        public class RestOfSupplierDto 
       {
            [DataMember]
            public int ProductId { get; set; }
            [DataMember]
            public int SupplierId { get; set; }
            [DataMember]
            public int Rest { get; set; }
       }

        [DataContract(Namespace = "", IsReference = false, Name = "Item")]
        public class RestWithPriceItemDto
        {
            [DataMember]
            public int ProductId { get; set; }
            [DataMember]
            public int DepartmentId { get; set; }
            [DataMember]
            public int Rest { get; set; }
            [DataMember]
            public decimal Price { get; set; }
        }

        private readonly AppDbContext _dbContext;
        private readonly HttpContextBase _context;
       


        public RestsOfSuppliersApiController() : this(new AppDbContext(), 
          
            new HttpContextWrapper(System.Web.HttpContext.Current)) { }

        public RestsOfSuppliersApiController(AppDbContext appDbContext, HttpContextWrapper httpContextWrapper)
        {
            // TODO: Complete member initialization
            this._dbContext = appDbContext;
            this._context = httpContextWrapper;
        }

        /// <summary>
        /// Получить доступные остатки сторонних поставщиков
        /// </summary>
        /// <returns></returns>
        public IQueryable<RestOfSupplierDto> GetRests()
        {
            return _dbContext.Set<RestOfSupplier>().Where(p=>p.Supplier.Active).Select(p => new RestOfSupplierDto
            {
                ProductId = p.ProductId,
                SupplierId = p.SupplierId,
                Rest = p.Rest
            });
        }

        /// <summary>
        /// Rests by departments with prices
        /// </summary>
        /// <returns></returns>
        public IQueryable<RestWithPriceItemDto> GetRestsWithPrices()
        {
          return 
           (from rests in  _dbContext.Set<RestOfProduct>()
                from pop in _dbContext.Set<PriceOfProduct>().Where(pop => pop.ProductId == rests.ProductId).DefaultIfEmpty()
           select new RestWithPriceItemDto
                {
               ProductId= rests.ProductId,
               DepartmentId = rests.DepartmentId,
               Rest=rests.Rest,
               Price=pop.PriceOpt1 ?? 0               
           });

        }

        /// <summary>
        /// Получить список всех поставщиков
        /// </summary>
        /// <returns></returns>
        public IQueryable<Supplier> GetSuppliers()
        {
            return _dbContext.Set<Supplier>();

        }



    }
}
