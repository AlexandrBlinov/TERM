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
using Yst.Services;
using Yst.ViewModels;
using YstTerm.Models;

namespace Term.Web.Controllers.API
{
    public class ProductsApiController : ApiController
    {
       
        private readonly AppDbContext _dbContext;
        private readonly PodborTyreDiskService _podborService;
        private readonly ProductService _productService;



        public ProductsApiController() : this(
            new AppDbContext(), 
            new  PodborTyreDiskService(),
            new ProductService()
            ) { }
        public ProductsApiController(AppDbContext dbContext, PodborTyreDiskService service, ProductService productService)
        {
            _dbContext = dbContext;
            _podborService = service;
            _productService = productService;
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

        [HttpGet]
        public IEnumerable<SearchResult> GetDisksByTiporazmer(string pointId, string tiporazmer)
        {
            DisksPodborView podborModel = new DisksPodborView();

            return _productService.GetDisks(podborModel, Int32.Parse(pointId), false,0);
             
        }


    }
}
