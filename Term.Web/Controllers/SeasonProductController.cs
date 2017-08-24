#define not_complie
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yst.Services;
using Yst.ViewModels;
using YstProject.Services;
using Term.DAL;
using PagedList;
using System.Data.Entity;
using Yst.Context;
using YstTerm.Models;
using YstProject.Models;
using System.Text.RegularExpressions;
using Term.Services;
using Yst.DropDowns;
using System.Globalization;


namespace Term.Web.Controllers
{
    /// <summary>
    ///     Класс - подборщик товаров для шин и дисков
    /// </summary>
    [Authorize]
    public class SeasonProductController : BaseController
    {

        /// <summary>
        /// Ограничивать производителей для турецких клиентов
        /// </summary>
        private static readonly int[] ProducerIdsToRestrict = Defaults.ProducersIdToRestrict;

        private readonly SeasonProductService _seasonproductservice = null;
        private readonly ProducerForSelectionService _producers = null;

        private static readonly string CulturesToRestrictSeasonProducts = ConfigurationManager.AppSettings["CulturesToRestrictSeasonProducts"];
        private static readonly string RestrictedSeasonProductIdStartString = ConfigurationManager.AppSettings["RestrictedSeasonProductId.Start"];
        private static readonly string RestrictedSeasonProductIdEndString = ConfigurationManager.AppSettings["RestrictedSeasonProductId.End"];

        public SeasonProductController(ProducerForSelectionService producers,SeasonProductService sps)
        {
            _producers = producers;
            _seasonproductservice = sps;

        }

        public SeasonProductController()
            : this(new ProducerForSelectionService(), new SeasonProductService())
        {

        }

        
        
        /// <summary>
        /// Функция сужает диапазон кодов , для турецких клиентов Alcasta, Nz, LegeArtis, для остальных - не включая в диапазон  [restrictedSeasonProductIdStart,restrictedSeasonProductIdEnd]
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private IQueryable<Product>  RestrictSeasonProductsIfNeeded(IQueryable<Product> results)
        {   

            int restrictedSeasonProductIdStart = Int32.Parse(RestrictedSeasonProductIdStartString);
            int restrictedSeasonProductIdEnd = Int32.Parse(RestrictedSeasonProductIdEndString);

            if (!String.IsNullOrEmpty(CulturesToRestrictSeasonProducts)  && CulturesToRestrictSeasonProducts.Contains(Point.Partner.Culture??Defaults.Culture_RU) )

                results = results.Where(p => ProducerIdsToRestrict.Contains(p.Producer.ProducerId));

            else
                
            
            results = results.Where(p => p.ProductId < restrictedSeasonProductIdStart || p.ProductId > restrictedSeasonProductIdEnd);
            return results;
        }

        /// <summary>
        /// Функция сужает диапазон для производителей турецких клиентов (Alcasta, Nz, LegeArtis)
        /// </summary>
        /// <param name="producers"></param>
        /// <returns></returns>
        private IEnumerable<Producer> RestrictProducers(IEnumerable<Producer> producers)
        {
            if (!String.IsNullOrEmpty(CulturesToRestrictSeasonProducts)  &&
                CulturesToRestrictSeasonProducts.Contains(Point.Partner.Culture ?? Defaults.Culture_RU))

                return producers.Where(p => ProducerIdsToRestrict.Contains(p.ProducerId));
            else

                return producers;
        }

        /// <summary>
        ///     Подборщик товаров для дисков
        /// </summary>
     
        /// <param name="podborModel"></param>
     
        /// <returns></returns>
        public ActionResult Disks(SeasonDisksPodborView podborModel )
        {

            var partnerId = Point.PartnerId;
            
            podborModel.DiskColors = CachedCollectionsService.DiskColours;
            
            // старый вызов до использования персонального сезонного ассортимента
            //podborModel.Producers = RestrictProducers(_producers.GetProducersByWheelTypeInSeasonStockItems(ProductType.Disk, podborModel.WheelType));

            // новый вызов с использованием персонального сезонного ассортимента по partnerId
            podborModel.Producers = RestrictProducers(_producers.GetProducersByWheelTypeInSeasonStockItemsForPartnerId(ProductType.Disk, podborModel.WheelType,partnerId));
            
            podborModel.IsForeign = ServicePP.IsForeignPartner;
            podborModel.HasSteelOffers = _seasonproductservice.HasSteelOffers;
            podborModel.HasAlloyOffers = _seasonproductservice.HasAlloyOffers;


            ViewBag.ShowSeasonCart = true;

            // общий сезонный ассортимент
            var ssitemsIds = DbContext.Set<SeasonStockItem>().Where
                (p => p.Product.ProductType == ProductType.Disk && p.Product.WheelType == podborModel.WheelType).Select(p=>p.ProductId).
                // объединяются с персональным сезонным ассортиментом
            Union(DbContext.Set<SeasonStockItemOfPartner>()
                .Where(p => p.PartnerId == partnerId)
                .Select(p => p.ProductId)).Distinct();

            var results = DbContext.Set<Product>().Where(p => ssitemsIds.Contains(p.ProductId) && p.ProductType == ProductType.Disk && p.WheelType == podborModel.WheelType);

           

            podborModel.Diametrs = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Diametr",podborModel.WheelType);
            podborModel.Widths = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Width", podborModel.WheelType);
            podborModel.Pcds = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "PCD", podborModel.WheelType);
            podborModel.Ets = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "ET", podborModel.WheelType);
            podborModel.Dias = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Dia", podborModel.WheelType);
          

             results= RestrictSeasonProductsIfNeeded(results);

           var   ids = results.Select(p => p.ProductId).ToArray();
            //
            // работает для кованых дисков в подборе по авто
            //
             if (Request.IsAjaxRequest())
             {
                 float range_dia = 0.4f;
                 float range_et = 3;
                 results = results.ToList().AsQueryable();
                 var dia = float.Parse(podborModel.DIA, CultureInfo.InvariantCulture.NumberFormat) - range_dia;
                 var et = float.Parse(podborModel.ET, CultureInfo.InvariantCulture.NumberFormat) + range_et;

                 if (podborModel.Diametr != null) results = results.Where(p => p.Tiporazmer.Diametr == podborModel.Diametr);
                 if (podborModel.PCD != null) results = results.Where(p => p.Tiporazmer.PCD == podborModel.PCD);
                 if (podborModel.Hole != null) results = results.Where(p => p.Tiporazmer.Holes == podborModel.Hole.ToString());
                 if (podborModel.DIA != null) results = results.Where(p => float.Parse(p.Tiporazmer.DIA, CultureInfo.InvariantCulture.NumberFormat) >= dia);
                 if (podborModel.ET != null) results = results.Where(p => float.Parse(p.Tiporazmer.ET, CultureInfo.InvariantCulture.NumberFormat) <= et);
                 results = results.ToList().AsQueryable();
                 var pricesOfProducts = DbContext.Set<PriceOfProduct>().ToList().AsQueryable();
                 podborModel.SearchResults = (from prod in results
                                              from pop in pricesOfProducts.Where(pop => pop.ProductId == prod.ProductId).DefaultIfEmpty()
                                              select new DiskSearchResult
                                              {
                                                  ProducerName = prod.Producer.Name,
                                                  ProductId = prod.ProductId,
                                                  ProductType = "disk",
                                                  Factory = prod.Factory,
                                                  Name = prod.Name,
                                                  Price = pop.Price ?? (prod.Price ?? 0),
                                                  ModelName = prod.Model.Name,
                                                  Rest = 4
                                              }).OrderBy(p => p.Name).ToPagedList(podborModel.Page, podborModel.ItemsPerPage);
             }
             else
             {
                 if (podborModel.PCD != null) results = results.Where(p => p.Tiporazmer.PCD == podborModel.PCD);
                 if (podborModel.ProducerId != null) results = results.Where(p => p.ProducerId == podborModel.ProducerId);
                 if (podborModel.DIA != null) results = results.Where(p => p.Tiporazmer.DIA == podborModel.DIA);
                 if (podborModel.Diametr != null) results = results.Where(p => p.Tiporazmer.Diametr == podborModel.Diametr);
                 if (podborModel.Width != null) results = results.Where(p => p.Tiporazmer.Width == podborModel.Width);
                 if (podborModel.ET != null) results = results.Where(p => p.Tiporazmer.ET == podborModel.ET);
                 if (podborModel.Hole != null) results = results.Where(p => p.Tiporazmer.Holes == podborModel.Hole.ToString());
                 //if (podbor_model.DiskColor != null) results = results.Where(p => p. == ???);


                  

                 // Код или название в зависимости от товара
                 if (podborModel.Article != null)
                 {
                     if (Regex.IsMatch(podborModel.Article, @"^\d+$"))
                         results = results.Where(p => p.Article.Contains(podborModel.Article) || p.ProductId.ToString().Contains(podborModel.Article));
                     else
                         results = results.Where(p => p.Name.ToLower().Contains(podborModel.Article.ToLower()));
                 }

                 var pricesOfPartners = DbContext.Set<PriceOfPartner>().Where(pp => pp.PartnerId == partnerId);
                 var pricesOfProducts = DbContext.Set<PriceOfProduct>();

                 podborModel.SearchResults = (from prod in results
                                              from pop in pricesOfProducts.Where(pop => pop.ProductId == prod.ProductId).DefaultIfEmpty()
                                              from pofpart in pricesOfPartners.Where(pofpart => pofpart.ProductId == prod.ProductId).DefaultIfEmpty()
                                              select new DiskSearchResult
                                              {
                                                  ProducerName = prod.Producer.Name,
                                                  ProductId = prod.ProductId,
                                                  ProductType = "disk",
                                                  Factory = prod.Factory,
                                                  Name = prod.Name,
                                                  Price = pofpart== null ? pop.Price ?? 0 : pofpart.Price,
                                                  ModelName = prod.Model.Name,
                                                  Rest = 4
                                              }).OrderBy(p => p.Name).ToPagedList(podborModel.Page, podborModel.ItemsPerPage);
             }

             

           

            if (Request.IsAjaxRequest()) return PartialView("_SeasonDisks", podborModel);
            return View(podborModel);
        
        }



        /// <summary>
        ///     Подборщик товаров для шин
        /// </summary>
        /// <param name="pb">Модель с параметрами</param>
        /// <param name="page"> номер страницы</param>
        /// <param name="ItemsPerPage">Число товаров на страницу</param>
        /// <returns></returns>
        /// 
#if not_compile
          public ActionResult Tyres([ModelBinder(typeof(TyresModelBinder))] SeasonTyresPodborView pb, int page = 1, int ItemsPerPage = 50)
        {

            pb.ItemsPerPage = ItemsPerPage;

            // в куках храним cookie_season_postid, чтобы в корзине понят какую закладку показывать
            string cookie_season_postid = (_httpContext.Request.Cookies[season_postid] == null) ? null :  _httpContext.Request.Cookies[season_postid].Value.ToString();

            var active_seasonpost = _dbContext.Set<SeasonPost>().FirstOrDefault(post => post.Id == pb.SeasonPostId);

            if (active_seasonpost == null) throw new NotImplementedException("Not found season");

            if (cookie_season_postid != pb.SeasonPostId.ToString())
            {
                var newCookie = new HttpCookie(season_postid, pb.SeasonPostId.ToString()) { Expires = DateTime.Now.AddYears(1) };
                _httpContext.Response.Cookies.Add(newCookie);
              
            }

            
            ViewData["SeasonProduct"] = true;
            var partnerId = _servicePP.getPartnerId();

        
            var results = _dbContext.Set<SeasonStockItem>().Where(p => p.Product.ProductType == ProductType.Tyre && p.SeasonPost.Active &&  p.SeasonPost.Id == pb.SeasonPostId);
           
           /* if (pb.SeasonId!=null)
                results = results.Where(p=>p.Product.Model==pb.SeasonId);  */

            
            if (pb.Diametr != null)
                results = results.Where(p => p.Product.Tiporazmer.Diametr==pb.Diametr);
            
            if (pb.Width != null)
                results = results.Where(p => p.Product.Tiporazmer.Width == pb.Width);
            if (pb.Height != null)
                results = results.Where(p => p.Product.Tiporazmer.Height == pb.Height);

            if (pb.Article != null)
                results = results.Where(p => p.Product.Article.Contains(pb.Article));




            var PricesOfPartners = _dbContext.Set<PriceOfPartner>().Where(pp => pp.PartnerId == partnerId);
            var PricesOfProducts = _dbContext.Set<PriceOfProduct>();

            pb.SearchResults = (from prod in results
                       join pop in PricesOfProducts on prod.ProductId equals pop.ProductId
                       from pofpart in PricesOfPartners.Where(pofpart => pofpart.ProductId == prod.ProductId).DefaultIfEmpty()
                                select new SearchResult { ProductId = prod.ProductId, ProductType = "tyre", Name = prod.Product.Name, Price = pofpart.Price == null ? pop.Price : pofpart.Price, ModelId = prod.Product.ModelId, Season = prod.Product.Model.Season, Rest = 4 }).OrderBy(p => p.Name).ToPagedList(page, ItemsPerPage);

          


            return View("Tyres", pb);
        }

#endif
        
      
    }
}
