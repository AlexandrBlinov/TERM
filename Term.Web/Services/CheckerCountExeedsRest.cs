using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.Context;
using YstProject.Models;
using Term.DAL;
using Term.Soapmodels;

namespace YstProject.Services
{
    /// <summary>
    /// Класс для проверки, превышает ли количество в корзине по товарам с товарами в пути
    /// </summary>
    public class CheckerCountExeedsRest
    {

         private readonly AppDbContext _dbContext ;
        public CheckerCountExeedsRest( AppDbContext dbContext)
        {
            _dbContext=dbContext;
         

        }

        public CheckerCountExeedsRest() : this(new AppDbContext()) { }


        /// <summary>
        /// Проверяет количество которое есть в строках с подразделением =0 по остаткам на товарах в пути, возвращает список ошибок
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="SuccessStatus"></param>
        /// <returns></returns>
        public IList<ProductResult> Check(IList<Cart> cartItems, ref bool SuccessStatus)
        {

            var listOfErrors = new List<ProductResult>();
            var CartitemsOnWay = cartItems.Where(p => p.DepartmentId == 0).Select(p => new { ProductId = p.ProductId, Count = p.Count }).ToList();

            if (CartitemsOnWay.Any())
            {
                var onWayProducts = _dbContext.Set<OnWayItem>().GroupBy(p => p.ProductId).Select(p => new ProductCount { ProductId = p.Key, Count = p.Sum(cnt => cnt.Count) }).ToArray();

                var errorRecords = from records in
                                       (from cartItemsOnWay in CartitemsOnWay
                                        from onway in onWayProducts.Where(p => p.ProductId == cartItemsOnWay.ProductId).DefaultIfEmpty()

                                        select new
                                        {
                                            ProductId = cartItemsOnWay.ProductId,
                                            Count = cartItemsOnWay.Count,
                                            Rest = onway != null ? onway.Count :0
                                        })
                                   where records.Count > records.Rest
                                   select new ProductWithRestCount { ProductId = records.ProductId, Count = records.Count, Rest = records.Rest };

                

                Array.ForEach(errorRecords.ToArray(), pwrc => listOfErrors.Add(new ProductResult { Code = pwrc.ProductId.ToString(), Quantity = pwrc.Rest }));
               


            }
            return listOfErrors;
        }

        /// <summary>
        /// Если у партнера существует остаток по данному товару,
        ///  то берем только с головного подразделения
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool CheckIfExistsRestOfPartner(string partnerId, int productId)
        {
            return _dbContext.Set<RestOfPartner>().Any(p => p.PartnerId == partnerId && p.ProductId == productId);
        }


        /// <summary>
        /// Получить число товаров под резерв партнера 
        /// (только для головного подразделения)
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public int GetCountOfProductOnRestsOfPartner(string partnerId, int productId)
        {
            var record= _dbContext.Set<RestOfPartner>().FirstOrDefault(p => p.PartnerId == partnerId && p.ProductId == productId);

            return record == null ? 0 : record.Rest;
        }

    }
}