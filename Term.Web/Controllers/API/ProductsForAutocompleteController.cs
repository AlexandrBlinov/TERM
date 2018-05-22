using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Term.DAL;
using Yst.Context;
using Yst.ViewModels;

namespace Term.Web.Controllers.API
{
    public class ProductsForAutocompleteController : ApiController
    {
        private static readonly int DefaultNumberToShow = 10;
        readonly int restrictedSeasonProductIdStart = int.Parse(ConfigurationManager.AppSettings["RestrictedSeasonProductId.Start"]);
        readonly int restrictedSeasonProductIdEnd = int.Parse(ConfigurationManager.AppSettings["RestrictedSeasonProductId.End"]);
        private readonly AppDbContext _dbContext;

        public ProductsForAutocompleteController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

       
        /// <summary>
        /// Получить все товары для данного типа товара (для рекламации)
        /// </summary>
        /// <param name="term"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        public IEnumerable<AutoCompleteResult> GetProductsForAutoComplete(string term, ProductType productType)
        {

         //      HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            var digPattern = new Regex(@"^\d+$");

            term = term.TrimStart('0');

            var result = _dbContext.Products.Where(p => p.ProductType == productType && (p.Name.Contains(term) || p.ProductId.ToString().StartsWith(term))).Select(p => new AutoCompleteResult { Label = p.Name, ProductType = (int)productType, Value = p.ProductId.ToString() }).Take(DefaultNumberToShow).OrderBy(p => p.Label).ToArray();

          //  result.ForEach(p => p.Value = p.Value.PadLeft(7, '0'));

            return result;
        }

        public IEnumerable<AutoCompleteResult> GetProductsForAutoCompleteFromRests(string term)
        {
            var digPattern = new Regex(@"^\d+$");

            String sqltext = @"SELECT Distinct TOP 50 Products.Name Label, RIGHT('0000'+convert(varchar(7), Products.ProductId), 7) Value, Products.ProductType ProductType FROM Products INNER JOIN
        (SELECT  ProductId, CASE WHEN Rest>50 THEN 50 ELSE Rest End Rest FROM RestOfProducts WHERE DepartmentId=5) RestOfProducts 
         ON Products.ProductId=RestOfProducts.ProductId
         WHERE ";
            if (!digPattern.IsMatch(term))
                sqltext += "(Products.Name LIKE {0})";
            else
                sqltext += "(RIGHT('0000'+convert(varchar(7), Products.ProductId), 7) LIKE {0})";
            var result = _dbContext.Database.SqlQuery<AutoCompleteResult>(sqltext, "%" + term + "%").ToArray();
            return result;
        }

        public IEnumerable<AutoCompleteResult> GetSeasonProductsForAutoComplete(string term, int wheeltype)
        {
            var digPattern = new Regex(@"^\d+$");

            String sqltext = @"SELECT Distinct TOP 50 Products.Name Label, RIGHT('0000'+convert(varchar(7), Products.ProductId), 7) Value, Products.ProductType ProductType FROM Products INNER JOIN
         SeasonStockItems 
         ON Products.ProductId=SeasonStockItems.ProductId
         WHERE ";
            if (!digPattern.IsMatch(term))
                sqltext += "(Products.Name LIKE {0} and Products.WheelType={1})";
            else
                sqltext += "(RIGHT('0000'+convert(varchar(7), Products.ProductId), 7) LIKE {0} and Products.WheelType={1})";
            var result = _dbContext.Database.SqlQuery<AutoCompleteResult>(sqltext, "%" + term + "%", wheeltype).ToArray();
            return result;
        }

        /// <summary>
        /// Получение всех товаров у которых есть картинки (из 1С)
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public  IQueryable<AutoCompleteResult>     GetAllProductsWherePictureExists( ProductType productType)
            {
            var result =  _dbContext.Products.Where(p => p.ProductType == productType && p.PathToRemotePicture!=null && (p.ProductId<restrictedSeasonProductIdStart || p.ProductId> restrictedSeasonProductIdEnd))
                .Select(p =>  new AutoCompleteResult { Label = p.ProductId.ToString(), Value = p.PathToRemotePicture, ProductType = (int)productType });

            return result;


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public IQueryable<AutoCompleteResult> GetProductsByCode(string term)
        {
            return _dbContext.Products.Where(p => p.ProductId.ToString().StartsWith(term) || p.Article.StartsWith(term)).Select(p => new AutoCompleteResult { Label = p.Name, ProductType = (int)p.ProductType, Value = p.ProductId.ToString() }).Take(DefaultNumberToShow).OrderBy(p => p.Label);

        }
    }
}
