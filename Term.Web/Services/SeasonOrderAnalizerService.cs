using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using Microsoft.Ajax.Utilities;
using Term.Services;
using Yst.Context;
using YstProject.Models;
using Term.DAL;
using YstTerm.Models;

namespace YstProject.Services
{
    /// <summary>
    /// Получение модели для анализа сезонного заказа 
    /// </summary>
    public class SeasonOrderAnalizerService
    {

        private readonly SoapServiceForSeasonOrders _soapServiceForSeasonOrders;

         private readonly AppDbContext _dbContext ;
         public SeasonOrderAnalizerService(AppDbContext dbContext, SoapServiceForSeasonOrders soapServiceForSeasonOrders)
        {
            _dbContext=dbContext;
         _soapServiceForSeasonOrders=soapServiceForSeasonOrders;
         this._soapServiceForSeasonOrders = soapServiceForSeasonOrders;
         _soapServiceForSeasonOrders.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginWS"], ConfigurationManager.AppSettings["PasswordWS"]);

        }

        public SeasonOrderAnalizerService() : this(new AppDbContext(), new SoapServiceForSeasonOrders()) {
        }



        private Expression<Func<Product, bool>> ProductsInList(IEnumerable<int> listOfProds)
        {
            return p => listOfProds.Contains(p.ProductId);
        }

        /// <summary>
        /// Анализ сезонного заказа
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IEnumerable<ProductSeasonOrderAnalized> Analize(string guid)
        {
            
            var result = _soapServiceForSeasonOrders.AnalyseSeasonOrder(guid.ToString());

            var listOfProductIds = result.Products.Select(p => Int32.Parse(p.Code));
            
            var products = _dbContext.Set<Product>().Where(ProductsInList(listOfProductIds)).ToList();
        
            var onWayProducts = _dbContext.Set<OnWayItem>().Where(p => listOfProductIds.Contains(p.ProductId)).GroupBy(p => p.ProductId).Select(p => new ProductCount { ProductId = p.Key, Count = p.Sum(cnt => cnt.Count) }).ToList();




            var model = (from results in result.Products
                         join prod in products on int.Parse(results.Code) equals prod.ProductId
                         from onway in onWayProducts.Where(p => p.ProductId == int.Parse(results.Code)).DefaultIfEmpty()
                         select new ProductSeasonOrderAnalized
                         {
                             ProductId = prod.ProductId,
                             Name = prod.Name,
                             Quantity = results.Quantity,
                             QuantityFact = results.QuantityFact,
                             Reserve = results.Reserve,
                             Rest = results.Rest,
                             QuantityOnWay = onway != null ? onway.Count : 0

                         });

            return model;
        }
    }
}