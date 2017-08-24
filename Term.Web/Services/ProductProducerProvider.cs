using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.Context;
using Term.DAL;
using System.Data.Entity;
using Yst.Utils;

namespace Term.Services
{
    /// <summary>
    /// Производитель для списка выбора в подборе
    /// </summary>
    public class ProducerForSelectionService:IDisposable
    {

        private AppDbContext _dbContext;
        private static readonly ICacheService _cache = new CacheService();
        
        public ProducerForSelectionService():this (new AppDbContext())
        {

        }
        public ProducerForSelectionService(AppDbContext dbContext)
        { _dbContext = dbContext; }


       public IEnumerable<Producer> GetProducersOfType(ProductType producttype)
        {
            return _dbContext.Set<Producer>().Where(p => p.ProductType == producttype &&p.Active).OrderBy(p=>p.Name).ToList();
        }

        
        /// <summary>
        /// Gets Producers of wheeltype in Season Stock Items
        /// </summary>
        /// <param name="producttype"></param>
        /// <param name="wheelType"></param>
        /// <returns></returns>
       public  IEnumerable<Producer> GetProducersByWheelTypeInSeasonStockItemsFromDb(ProductType producttype,WheelType wheelType)
        {
            var rests = _dbContext.Set<SeasonStockItem>();
            var query =_dbContext.Set<Product>()
                    .Where( p => rests.Any(rest => rest.ProductId == p.ProductId) && p.ProductType == producttype &&
                            p.Producer.Active && p.WheelType == wheelType)
                    .Include(p => p.Producer)
                    .Select(p => p.Producer)
                    .Distinct()
                    .OrderBy(p => p.Name);

             return    query.ToArray(); //.AsEnumerable();

            

        }

        /// <summary>
       /// Gets Producers of wheeltype in Season Stock Items with personal season stock
        /// </summary>
        /// <param name="producttype"></param>
        /// <param name="wheelType"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
       public IEnumerable<Producer> GetProducersByWheelTypeInSeasonStockItemsForPartnerId(ProductType producttype, WheelType wheelType,string  partnerId)
       {
           var rests = _dbContext.Set<SeasonStockItem>().Select(p => p.ProductId).
               Union(
               _dbContext.Set<SeasonStockItemOfPartner>().Where(p => p.PartnerId == partnerId).Select(p => p.ProductId));

           var query = _dbContext.Set<Product>()
                   .Where(p => rests.Contains(p.ProductId) && p.ProductType == producttype &&
                           p.Producer.Active && p.WheelType == wheelType)
                   .Include(p => p.Producer)
                   .Select(p => p.Producer)
                   .Distinct()
                   .OrderBy(p => p.Name);

           return query.ToArray(); //.AsEnumerable();


           

       }

        /// <summary>
        /// Получаем производителей дисков по сезонному ассортименту из хэша
        /// </summary>
        /// <param name="producttype"></param>
        /// <param name="wheelType"></param>
        /// <returns></returns>
        public IEnumerable<Producer> GetProducersByWheelTypeInSeasonStockItems(ProductType producttype,WheelType wheelType)
        {
            return _cache.GetOrAdd("produsers.seasonstock.wheels." + wheelType.ToString(), () => GetProducersByWheelTypeInSeasonStockItemsFromDb(producttype, wheelType), DateTimeOffset.UtcNow.AddMinutes(20)); 
        }

        public IEnumerable<Producer> GetProducersByWheelTypeInSeasonStockItemsWithPartnerId(ProductType producttype, WheelType wheelType,string parnerId)
        {
            return _cache.GetOrAdd( String.Format("produsers.seasonstock.wheels.{0}.{1}",wheelType,parnerId), () => GetProducersByWheelTypeInSeasonStockItemsForPartnerId(producttype, wheelType, parnerId), DateTimeOffset.UtcNow.AddMinutes(20));
        }


        public void Dispose()
       {
           if (_dbContext != null)
           {
               _dbContext.Dispose();
               _dbContext = null;
           }

       }
    }
}