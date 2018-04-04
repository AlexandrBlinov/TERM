using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Term.DAL;
using Term.Web.Services;
using WebGrease.Css.Extensions;
using Yst.Context;
using Yst.ViewModels;
using YstTerm.Models;

namespace Term.Web.Controllers.API
{
    public class ProductsApiController : ApiController
    {
        private static readonly int DefaultNumberToShow =10;
        private readonly AppDbContext _dbContext;
        private readonly PodborTyreDiskService _podborService;

        readonly int restrictedSeasonProductIdStart = int.Parse(ConfigurationManager.AppSettings["RestrictedSeasonProductId.Start"]);
        readonly int restrictedSeasonProductIdEnd = int.Parse(ConfigurationManager.AppSettings["RestrictedSeasonProductId.End"]);

        public ProductsApiController() : this(new AppDbContext(), new  PodborTyreDiskService()) { }
        public ProductsApiController(AppDbContext dbContext, PodborTyreDiskService service)
        {
            _dbContext = dbContext;
            _podborService = service;
        }

       
        [HttpGet]
        [ActionName("GetBrands")]
        
        public IEnumerable<string> GetBrands()
        {
            return _podborService.GetBrands();

        }

        [HttpGet]
        [ActionName("GetModels")]
        public IEnumerable<string> GetModels(string brand)
        {
            return _podborService.GetCars(brand);

        }

        [HttpGet]
        [ActionName("GetYears")]
        public IEnumerable<int> GetYears(string brand, string model)
        {
            return _podborService.GetYears(brand, model);
        }

        [HttpGet]
        [ActionName("GetEngines")]
        public IEnumerable<string> GetEngines(string brand, string model, int year)
        {
            return _podborService.GetEngines(brand, model, year);
        }

        [HttpGet]
        [ActionName("GetTiporazmers")]
        public IEnumerable<TiporazmerByCarModelView> GetResults(string brand, string model, int year, string engine)
        {
            return _podborService.GetResults(brand, model, year, engine);

        }

        /// <summary>
        /// Получить все товары для данного типа товара (для рекламации)
        /// </summary>
        /// <param name="term"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        public IEnumerable<AutoCompleteResult> GetProductsForAutoComplete(string term, ProductType productType)
        {

            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
             var digPattern = new Regex(@"^\d+$");

            term = term.TrimStart('0');

             var result = _dbContext.Products.Where(p => p.ProductType == productType && (p.Name.Contains(term) || p.ProductId.ToString().StartsWith(term))).Select(p => new AutoCompleteResult { Label = p.Name, ProductType = (int)productType, Value = p.ProductId.ToString() }).Take(DefaultNumberToShow).OrderBy(p=>p.Label).ToArray();
          
           result.ForEach(p=>p.Value=p.Value.PadLeft(7,'0'));
            
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
            return _dbContext.Products.Where(p =>  p.ProductId.ToString().StartsWith(term) || p.Article.StartsWith(term)).Select(p => new AutoCompleteResult { Label = p.Name, ProductType =(int)p.ProductType, Value = p.ProductId.ToString() }).Take(DefaultNumberToShow).OrderBy(p => p.Label);
            
        }


    }
}
