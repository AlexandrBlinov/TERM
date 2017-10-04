using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YstProject.Models;
using Term.DAL;
using YstTerm.Models;
using YstProject.Services;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Web.Routing;
using System.Web.UI;
using PagedList;
using Term.Services;
using Yst.Services;
using Yst.DropDowns;
using Term.Web.Views.Resources;

namespace Term.Web.Controllers
{
    [CheckIfSupplier(Order = 1)]
    [CheckSettings(Order = 2)]
    
    public class HomeController : BaseController
    {
        private static readonly string _invisible = Defaults.Invisible, _space =Defaults.Space;

       
        private const int MAX_ITEMS_TO_SHOW = int.MaxValue;

        private readonly SeasonProductService _seasonproductservice = null;
        private readonly ProducerForSelectionService _producers = null;
        private readonly OrderService _orderService;
        private readonly DaysToDepartmentService _daysToDepartmentService;
        private readonly NewsService _newsService;
    /*    private string _partnerId; int _pointId;
        private bool _isForeign;
        private Partner _partner;
       private PartnerPoint _point; */


        public HomeController(ProducerForSelectionService producers, SeasonProductService sps, OrderService orderService, DaysToDepartmentService daysToDepartmentService, NewsService newsService, HttpContextBase context)
        {
            _producers = producers;
            _seasonproductservice = sps;
            _orderService = orderService;
            _daysToDepartmentService = daysToDepartmentService;
            _newsService = newsService;
        }

       public HomeController()
           : this(
               new ProducerForSelectionService(), new SeasonProductService(), new OrderService(),
               new DaysToDepartmentService(),new NewsService(), new HttpContextWrapper(System.Web.HttpContext.Current))
       {
          
       }


     
       /// <summary>
        /// Подбор дисков
        /// </summary>
        /// <param name="podborModel"></param>
        /// <param name="exactsize"></param>
        /// <param name="zamena"></param>
        /// <returns></returns>
       public ActionResult Disks([ModelBinder(typeof(DisksModelBinder))]SeasonDisksPodborView podborModel,  int exactsize = 1, int zamena = 0 /*, bool OnlySale = false*/)
        
        {
            // Если ничего не выбрано, берем с остатков
            if (podborModel.RestOrOnWay == 0) podborModel.FromRests = true;

            podborModel.CargoWheels = false;

            podborModel.DiskColors = CachedCollectionsService.DiskColours;
            podborModel.Producers = CachedCollectionsService.GetProducers(ProductType.Disk);
            podborModel.CarsList = CachedCollectionsService.GetCarsList;

            //  int pointId = ServicePP.getPointID();

            // var isPartner = ServicePP.IsPartner;
            bool isSale = ServicePP.IsSaleAvailable;
            ViewBag.SaleMode = isSale;
           

           podborModel.Diametrs = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Diametr");
           podborModel.Widths = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Width");
           podborModel.Pcds = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "PCD");
           podborModel.Ets = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "ET");
           podborModel.Dias = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Dia");

            ViewBag.NoDsplAutoPodbor = _space;
            ViewBag.YesDsplAutoPodbor = _invisible;

            if (!base.Partner.IsForeign)
            {
                ViewBag.MaxPrice = Defaults.PriceMaxRus;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideRus;
                if (podborModel.PriceMax == 0)  podborModel.PriceMax = Defaults.PriceMaxRus; 
            }
            else
            {
                ViewBag.MaxPrice = Defaults.PriceMaxEng;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideEng;
                if (podborModel.PriceMax == 0)  podborModel.PriceMax = Defaults.PriceMaxEng; 
            }

            exactsize = zamena == 1 ? 0 : exactsize;

            if (Request.IsAjaxRequest())
            {
                ViewBag.NoDsplAutoPodbor = _invisible;
                ViewBag.YesDsplAutoPodbor = _space;
               
                podborModel.ItemsPerPage = MAX_ITEMS_TO_SHOW;
                podborModel.SearchResults = Products.GetDisks(podborModel, Point.PartnerPointId, isSale, exactsize).ToPagedList(podborModel.Page, podborModel.ItemsPerPage);
            }
           else podborModel.SearchResults = CachedCollectionsService.GetAllDisksByPartnerPoint(podborModel, Point);
             
            
            // no cache
            /*
            ViewBag.NoDsplAutoPodbor = _invisible;
            ViewBag.YesDsplAutoPodbor = _space;
            podbor_model.ItemsPerPage = 50;
            podbor_model.SearchResults = Products.GetDisks(podbor_model, PointId, isSale, exactsize).ToPagedList(podbor_model.Page, podbor_model.ItemsPerPage);  
            */



            podborModel.HasAlloyOffers = _seasonproductservice.HasAlloyOffers;
            podborModel.HasSteelOffers = _seasonproductservice.HasSteelOffers;

            podborModel.BackDepartmentWithInfo = _daysToDepartmentService.GetDepartmentInfoWithMaxDaysForPoint(Point.PartnerPointId);

            if (Request.IsAjaxRequest())     return PartialView("_Disks", podborModel); 


            return View( podborModel);
        }


       public ActionResult CargoDisks([ModelBinder(typeof(DisksModelBinder))]SeasonDisksPodborView podborModel, int exactsize = 1, int zamena = 0 /*, bool OnlySale = false*/)
       {
           // Если ничего не выбрано, берем с остатков
           if (podborModel.RestOrOnWay == 0) podborModel.FromRests = true;

           podborModel.CargoWheels = true;

           podborModel.DiskColors = CachedCollectionsService.DiskColours;
           podborModel.Producers = CachedCollectionsService.GetProducers(ProductType.CargoDisk);

           bool isSale = ServicePP.IsSaleAvailable;
           ViewBag.SaleMode = isSale;

           podborModel.Diametrs = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Diametr");
           podborModel.Widths = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Width");
           podborModel.Pcds = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "PCD");
           podborModel.Ets = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "ET");
           podborModel.Dias = CachedCollectionsService.GetTiporazmerProperties(ProductType.Disk, "Dia");

           if (!base.Partner.IsForeign)
           {
               ViewBag.MaxPrice = Defaults.PriceMaxRus;
               ViewBag.PriceStepSlide = Defaults.PriceStepSlideRus;
               if (podborModel.PriceMax == 0) podborModel.PriceMax = Defaults.PriceMaxRus;
           }
           else
           {
               ViewBag.MaxPrice = Defaults.PriceMaxEng;
               ViewBag.PriceStepSlide = Defaults.PriceStepSlideEng;
               if (podborModel.PriceMax == 0) podborModel.PriceMax = Defaults.PriceMaxEng;
           }

           podborModel.SearchResults = CachedCollectionsService.GetAllDisksByPartnerPoint(podborModel, Point);

           podborModel.HasAlloyOffers = _seasonproductservice.HasAlloyOffers;
           podborModel.HasSteelOffers = _seasonproductservice.HasSteelOffers;

           podborModel.BackDepartmentWithInfo = _daysToDepartmentService.GetDepartmentInfoWithMaxDaysForPoint(Point.PartnerPointId);

           return View(podborModel);
       }

    //   [OutputCache(VaryByParam = "ProducerId;SeasonId;Width;Height;Diametr;Sortby;DisplayView;ItemsPerPage;Ship;PriceMin;PriceMax;page", VaryByCustom = "User;RestsImportDateTime", Duration = 600, Location = OutputCacheLocation.Server)]
        public ActionResult Tyres([ModelBinder(typeof(TyresModelBinder))] TyresPodborView podborModel)
        {

            string partnerId = base.Partner.PartnerId;
            podborModel.Producers = CachedCollectionsService.GetProducers(ProductType.Tyre);
            podborModel.Heights = CachedCollectionsService.GetTiporazmerProperties(ProductType.Tyre, "height");

            ViewBag.HideNoStud = DbContext.PartnerPropertyValues.Any(p => p.PartnerId == partnerId && p.Name==Defaults.PartnerProperties.HideNoStud);
                         
            
            //    int partnerPointId = ServicePP.getPointID();
            ViewBag.NoDsplAutoPodbor = _space;
            ViewBag.YesDsplAutoPodbor = _invisible;

            if (!base.Partner.IsForeign)
            {
                ViewBag.MaxPrice = Defaults.PriceMaxRus;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideRus;
                if (podborModel.PriceMax == 0)  podborModel.PriceMax = Defaults.PriceMaxRus; 
            }
            else
            {
                ViewBag.MaxPrice = Defaults.PriceMaxEng;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideEng;
                if (podborModel.PriceMax == 0)  podborModel.PriceMax = Defaults.PriceMaxEng; 
            }
            if (Request.IsAjaxRequest())
            {
                ViewBag.NoDsplAutoPodbor = _invisible;
                ViewBag.YesDsplAutoPodbor = _space;
                podborModel.ItemsPerPage = MAX_ITEMS_TO_SHOW;
            }

          

            int page = podborModel.Page;
            //if (podborModel.Ship == ShipForTyresPodbor.ShipShip || podborModel.Ship == ShipForTyresPodbor.ShipNoShip) podborModel.SeasonId = "winter";
           // Products.GetTyres(podborModel, Point.PartnerPointId);
            podborModel.SearchResults = CachedCollectionsService.GetAllTyresByPartnerPoint(podborModel, Point.PartnerPointId);


            if (Request.IsAjaxRequest()) return PartialView("_Tyres", podborModel);

            podborModel.BackDepartmentWithInfo = _daysToDepartmentService.GetDepartmentInfoWithMaxDaysForPoint(Point.PartnerPointId);
            return View(podborModel);
            
        }

        public ActionResult Akb(/* [ModelBinder(typeof(AkbModelBinder))]*/ AkbPodborView podborModel )
        {

            podborModel.Brands = CachedCollectionsService.GetAkbProperties("brand");
            podborModel.Sizes = CachedCollectionsService.GetAkbProperties("sizes");
            podborModel.Polarities = CachedCollectionsService.GetAkbProperties("polarity");
            podborModel.Volumes = CachedCollectionsService.GetAkbProperties("volume");
            podborModel.InrushCurrents = CachedCollectionsService.GetAkbProperties("inrush_current");
            
            
          //  podborModel.Producers = CachedCollectionsService.GetProducers(ProductType.Akb);

            podborModel.Producers = CachedCollectionsService.GetAkbProperties("producer");
            podborModel.AkbTypes = CachedCollectionsService.GetAkbProperties("akbtype");
          

            if (!base.Partner.IsForeign)
            {
                ViewBag.MaxPrice = Defaults.PriceMaxRus;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideRus;
                if (podborModel.PriceMax == 0)  podborModel.PriceMax = Defaults.PriceMaxRus; 
            }
            else
            {
                ViewBag.MaxPrice = Defaults.PriceMaxEng;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideEng;
                if (podborModel.PriceMax == 0)  podborModel.PriceMax = Defaults.PriceMaxEng; 
            }

            Products.GetAkbs(podborModel, Point.PartnerPointId);



            return View(podborModel);

           
        }

        public ActionResult Accs(AccPodborView podborModel /* , int page = 1, int ItemsPerPage = 50, Display DisplayView = Display.Table */ )
        {
         /*   podborModel.Page = page;
            podborModel.ItemsPerPage = ItemsPerPage;
            podborModel.DisplayView = DisplayView; */

            podborModel.DisplayView = Display.Table;
            var categories = CachedCollectionsService.GetParentFolderForOthersProduct(ProductType.Acc);
            ViewBag.Categories = categories;
            podborModel.Producers = CachedCollectionsService.GetProducers(ProductType.Acc);
            //var cat = Products.getCategories(ProductType.Acc);

            if (RouteData.Values.ContainsKey("pathInfo"))
            {
                var pathInfo = (string)RouteData.Values["pathInfo"];
                if (!string.IsNullOrWhiteSpace(pathInfo))
                {
                    var parts = pathInfo.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                    var producersPart = parts.FirstOrDefault(p => p.StartsWith("Categories-"));
                    if (producersPart != null)
                    {
                        var producersIds = producersPart.Substring("Categories-".Length).Split(',').ToArray();

                        foreach (var producersId in producersIds)
                        {
                            podborModel.SelectedCategories.Add(producersId);
                            var folder = categories.Where(p => p.ProductId == Convert.ToInt32(producersId) && p.ParentId != null).Select(p => p.ParentId).FirstOrDefault();
                            podborModel.SelectedFolders.Add(folder.ToString());
                        }
                    }
                }
            }



            podborModel.SearchResults = CachedCollectionsService.GetAllAccsByPartnerPoint(podborModel, Point.PartnerPointId);

            return View( podborModel);


        }


        /// <summary>
        /// Стартовая страница 
        /// </summary>
        /// <returns></returns>
     //  [OutputCache(CacheProfile = "VariedByUserOnServer")]
         public ActionResult Index()
        {
            
            ViewBag.UserName = Request.IsAuthenticated ? User.Identity.Name : String.Empty;

            foreach (var p in Enum.GetValues(typeof(ProductType)).Cast<ProductType>())
            {
                ViewData[Enum.GetName(typeof(ProductType),p)] = Products.GetCountOfProducts(p);
            }
            ViewBag.NewsId = _newsService.getNewsId();
       
            return View();
        }


       /// <summary>
       /// Partial view to show exact size
       /// </summary>
       /// <param name="Id"></param>
       /// <param name="exactsize">точное соответствие параметрам или нет</param>
       /// <returns></returns>
       [ActionName("AppToCars")]
        public ActionResult ApplicationToCars(int Id,int exactsize=1)
        {
            ViewBag.CarsOfProducts = Products.GetCarsFromProduct(Id,exactsize);
            return PartialView("_AppToCars");
        }
      
       /// <summary>
       /// Detailed page of product
       /// </summary>
       /// <param name="Id"></param>
       /// <returns></returns>
        public ActionResult Details(int Id)
        {

                string partnerId =Point.Partner.PartnerId;
              
                
                int days = 0;

            /*
               var model = new ProductViewModel
               {
                   Product = Products.getProduct(Id),
                   DepId = _daysToDepartmentService.GetDepartmentIdForProduct(partnerPointId, Id, ref days),
                   ProductProperties = Products.getProductProperties(Id),
                   
               }; */
                //var dep = Products.getDepartmentForProduct(PartnerPointId, Id, ref days);

                ViewBag.DepId = _daysToDepartmentService.GetDepartmentIdForProduct(Point.PartnerPointId, Id, ref days);
            ViewBag.Invisible = " ";
            ViewBag.Days = days;
                 //dep.DepartmentId; 

                ViewBag.DepartmentWithRests = _daysToDepartmentService.GetDepartmentsWithRests(Point.PartnerPointId, productId: Id);
                
               
                var product = Products.GetProduct(Id);
                if (product == null) throw new HttpException(404, "Not found");

                ViewBag.CarsOfProducts = null;

               if (product.ProductType == ProductType.Other)
               {
                   ViewBag.PriceClient = Products.GetPriceOfProduct(Id);
                   ViewBag.Invisible = Defaults.Invisible;
                   
                   Regex regexNut = new Regex(Defaults.Nut);
                   Regex regexBolt = new Regex(Defaults.Bolt);
                   if (regexNut.IsMatch(product.Name) || regexBolt.IsMatch(product.Name))
                   {
                    bool   isBolts = regexBolt.IsMatch(product.Name) ;
                       ViewBag.CarsOfProducts = Products.GetCarsFromOthersProduct(isBolts, product.Tiporazmer.Name);
                   }
               }

                else ViewBag.PriceClient = Products.GetPriceOfClient(Id, Point.PartnerPointId, partnerId);
                
                
                var productProperties = Products.GetProductProperties(Id);

                if (productProperties != null && productProperties.Any(p => p.Name == Defaults.FixingCode)) 
                    ViewBag.FixingCode = productProperties.Where(p => p.Name == Defaults.FixingCode).Select(p => p.Value).FirstOrDefault();
                
                else  ViewBag.FixingCode = "#";
                

                ViewBag.ProductProperties = productProperties;
                ViewBag.CountPhoto = Products.GetCountOfAddPhoto(product.PathToRemotePicture);
                if (product.ProductType == ProductType.Disk  || (product.ProductType == ProductType.Tyre && product.Tiporazmer!=null && !product.Tiporazmer.Diametr.Contains("C"))) ViewBag.CarsOfProducts = Products.GetCarsFromProduct(Id);
                    
                if (Defaults.CargoWheelsProducers.Contains(product.ProducerId.ToString())) ViewBag.CarsFromCargoWheels = Products.GetCarsFromCargoWheels(product.Article);
                
                /*
                ViewBag.Analogs = null;
                if (product.ProductType == ProductType.Disk && product.ProducerId != 3654 && product.ProducerId != 3656)
                {
                    var tr = product.Tiporazmer;
                    var podbormodelAnalogs = new DisksPodborView{                    
                    Diametr = tr.Diametr, DIA = tr.DIA,
                    ET = tr.ET,Hole = Convert.ToInt32(tr.Holes),
                    PCD = tr.PCD,Width = tr.Width,FromRests=true,FromOnWay=false };

                    ICollection<SearchResult> Analogs = Products.GetDisks(podbormodelAnalogs, partnerPointId, false, 1).ToArray();
                    ViewBag.Analogs = Analogs; 
                    
                } 

                if (product.ProductType == ProductType.Tyre)
                {
                    var season = Products.GetSeasonForProduct(Id);
                    var tr = product.Tiporazmer;
                    var podborModel = new TyresPodborView
                    {
                        Diametr = tr.Diametr,
                        Width = tr.Width,
                        Height = tr.Height,
                        SeasonId = season
                    };

                    Products.getTyres(podborModel, partnerPointId);

                    ICollection<SearchResult> Analogs = podborModel.SearchResults.ToArray();
                    ViewBag.Analogs = Analogs;
                }        */        
                               
                                
                    return View(product);
                
           
                

        }

        public ActionResult Others(OthersPodborView podborModel)
        {
            if (podborModel.RestOrOnWay == 0) podborModel.FromRests = true;
            podborModel.Departments = DropDownsFactory.DepartmentsInclude(Point);
            podborModel.Diametrs = CachedCollectionsService.GetOtherProperties("Diametr", podborModel.Folder);
            podborModel.Widths = CachedCollectionsService.GetOtherProperties("Width", podborModel.Folder);
            podborModel.ProductFolders = CachedCollectionsService.GetParentFolderForOthersProduct(ProductType.Other);

            podborModel.ProductFolders = podborModel.OthersType == OthersProductType.Cams ? podborModel.ProductFolders.Where(p => p.ParentId == 1746) : podborModel.ProductFolders.Where(p => p.ParentId == 701);

            Products.GetOthersFromPodbor(podborModel, Point.PartnerPointId);
            return View(podborModel);
        }

        public ActionResult CargoTyres(CargoTyresPodborView podborModel)
        {
        
            podborModel.Producers = CachedCollectionsService.GetProducers(ProductType.Tyre,"cargo");

            podborModel.CargoWidth = CachedCollectionsService.GetCargoProperties(ProductType.Tyre, "Width");
            podborModel.CargoHeight = CachedCollectionsService.GetCargoProperties(ProductType.Tyre, "Height");
            podborModel.CargoDiametr= CachedCollectionsService.GetCargoProperties(ProductType.Tyre, "Diametr");

            
            if (!Partner.IsForeign)
            {
                ViewBag.MaxPrice = Defaults.CargoPriceMaxRus;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideRus;
                if (podborModel.PriceMax == 0) { podborModel.PriceMax = Defaults.CargoPriceMaxRus; }
            }
            else
            {
                ViewBag.MaxPrice = Defaults.CargoPriceMaxEng;
                ViewBag.PriceStepSlide = Defaults.PriceStepSlideEng;
                if (podborModel.PriceMax == 0) { podborModel.PriceMax = Defaults.CargoPriceMaxEng; }
            }

          //  int page = podborModel.Page;
            Products.GetCargoTyres(podborModel, Point.PartnerPointId);

            return View(podborModel);
        }

        public ActionResult SparDisks(SparPodborView spar_param)
        {
            var podborModel = new SeasonDisksPodborView
            {
                Width = spar_param.Width,
                Diametr = spar_param.Diametr,
                Hole = Convert.ToInt32(spar_param.Holes),
                PCD = spar_param.PCD,
                ET = spar_param.ET,
                DIA = spar_param.DIA,
                Page = 1,
                ItemsPerPage = MAX_ITEMS_TO_SHOW
            };

            var podborModelRear = new SeasonDisksPodborView
            {
                Width = spar_param.RearWidth,
                Diametr = spar_param.RearDiametr,
                Hole = Convert.ToInt32(spar_param.RearHoles),
                PCD = spar_param.RearPCD,
                ET = spar_param.RearET,
                DIA = spar_param.RearDIA,
                Page = 1,
                ItemsPerPage = MAX_ITEMS_TO_SHOW
            };
            var podborModelResult = new SeasonDisksPodborView();
            var goods = new List<DiskSearchResult>();
            if (podborModel.RestOrOnWay == 0) podborModel.FromRests = true;
            if (podborModelRear.RestOrOnWay == 0) podborModelRear.FromRests = true;
            bool isSale = ServicePP.IsSaleAvailable;
            ViewBag.SaleMode = isSale;
            IPagedList<DiskSearchResult> first = Products.GetDisks(podborModel, Point.PartnerPointId, isSale, 0).ToPagedList(podborModel.Page, podborModel.ItemsPerPage);
            IPagedList<DiskSearchResult> second = Products.GetDisks(podborModelRear, Point.PartnerPointId, isSale, 0).ToPagedList(podborModelRear.Page, podborModelRear.ItemsPerPage);
            foreach (var item in first)
            {
                if (item.Width == spar_param.Width)
                {
                    DiskSearchResult good = second.Where(p => p.ModelName == item.ModelName && p.Width == spar_param.RearWidth && p.PathToImage == item.PathToImage).FirstOrDefault();
                    if (good != null)
                    {
                        goods.Add(item);
                        goods.Add(good);
                    }
                }
            }
            podborModelResult.SearchResults = goods.ToPagedList(1, MAX_ITEMS_TO_SHOW);
            return PartialView("_Disks", podborModelResult);
        }

        public ActionResult SparTyres(SparPodborView spar_param)
        {
            var podborModel = new TyresPodborView
            {
                Width = spar_param.Width,
                Diametr = spar_param.Diametr,
                Height = spar_param.Height,
                Page = 1,
                ItemsPerPage = MAX_ITEMS_TO_SHOW
            };
            var podborModelRear = new TyresPodborView
            {
                Width = spar_param.RearWidth,
                Diametr = spar_param.RearDiametr,
                Height = spar_param.RearHeight,
                Page = 1,
                ItemsPerPage = MAX_ITEMS_TO_SHOW
            };
            var podborModelResult = new TyresPodborView();
            var goods = new List<TyreSearchResult>();
            bool isSale = ServicePP.IsSaleAvailable;
            ViewBag.SaleMode = isSale;
            IPagedList<TyreSearchResult> first = CachedCollectionsService.GetAllTyresByPartnerPoint(podborModel, Point.PartnerPointId);
            IPagedList<TyreSearchResult> second = CachedCollectionsService.GetAllTyresByPartnerPoint(podborModelRear, Point.PartnerPointId);
            foreach (var item in first)
            {
                TyreSearchResult good = second.Where(p => p.ModelName == item.ModelName).FirstOrDefault();
                if (good != null)
                {
                    goods.Add(item);
                    goods.Add(good);
                }
            }
            podborModelResult.SearchResults = goods.ToPagedList(1, MAX_ITEMS_TO_SHOW);
            return PartialView("_Tyres", podborModelResult);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (_producers != null) _producers.Dispose();
            base.Dispose(disposing);
                }
        }



        
    }
}
