using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
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
  /*  [KnownType(typeof(SearchResult))]
    [KnownType(typeof(DiskSearchResult))] */
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


        /// <summary>
        ///  API подбора по авто по типоразмеру дисков
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="tiporazmer"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DiskSearchResult> GetDisksByTiporazmer(string pointId, string tiporazmer)
        {
            var podborModel = new DisksPodborView {FromRests=true,FromOnWay=false };

            podborModel.FillFromTiporazmer(tiporazmer);

            return _productService.GetDisks(podborModel, Int32.Parse(pointId), false,0);
             
        }

        /// <summary>
        ///  API подбора по авто по типоразмеру шин
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="tiporazmer"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<TyreSearchResult> GetTyresByTiporazmer(string pointId, string tiporazmer)
        {
            var podborModel = new TyresPodborView();

            podborModel.FillFromTiporazmer(tiporazmer);

            return _productService.GetTyres(podborModel, Int32.Parse(pointId));

        }


    }
}
