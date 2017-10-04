using PagedList;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Yst.Context;
using Yst.Services;
using Yst.Utils;
using Yst.ViewModels;
using Term.DAL;
using Term.Web.Services;
using YstTerm.Models;
using Term.Utils;

namespace YstProject.Services

{

   

    /// <summary>
    /// Used to cache all selects lists in product podbors
    /// </summary>
    public static class CachedCollectionsService
    {
        private static readonly AppDbContext _dbContext = new AppDbContext();
        private static readonly ICacheService _cache = new CacheService();
        private static readonly int TimeoutMin = 10;        
        private static string KeyDisks = Defaults.CacheSettings.KeyDisks;
        private static string KeyTyres = Defaults.CacheSettings.KeyTyres;
        private static string KeyAccs = Defaults.CacheSettings.KeyAccs;
        private static readonly object LockAkbObject = new object();
        private static readonly object LockProducersObject = new object();
        private static readonly object LockTiporazmersObject = new object();

        #region Private Members

        /// <summary>
        /// Получить подбор по всем дискам из базы
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="podborModel"></param>
        /// <returns></returns>
        private static IList<DiskSearchResult> GetAllDisksByPartnerPointFromDb(int pointId,DisksPodborView podborModel)
        {
        using (var productService = new ProductService())
        {
            var model = new DisksPodborView {FromOnWay = podborModel.FromOnWay, FromRests = podborModel.FromRests};
            return productService.GetDisks(model, pointId, false);
        }
        }

        /// <summary>
        /// Получить подбор по всем шинам из базы
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="podborModel"></param>
        /// <returns></returns>
        private static IList<TyreSearchResult> GetAllTyresByPartnerPointFromDb(int pointId, TyresPodborView podborModel)
        {

            using (var productService = new ProductService())
            {
                var model = new TyresPodborView();
                return productService.GetTyres(model, pointId);

            }


        }
             
        private static IEnumerable<string> GetDiskColoursFromDb()
        {
            
            {
              return _dbContext.Set<DiskColour>().Select(dc => dc.ColourName).OrderBy(p=>p).AsEnumerable<string>();
               
            }
        }
        #endregion

        #region Public Members
        public static void ClearCache()
        {
            _cache.Clear();
        
        }

        /// <summary>
        /// Удалить кэш по точке по дискам
        /// </summary>
        /// <param name="pointId"></param>
        public static void RemoveCacheWheels(string pointId)
        {

            _cache.Remove(String.Format("{0}.{1}.1", KeyDisks, pointId)); // очистить кэш на остатках
            _cache.Remove(String.Format("{0}.{1}.2", KeyDisks, pointId)); // очистить кэш в пути
            _cache.Remove(String.Format("{0}.{1}.3", KeyDisks, pointId)); // очистить кэш на остатках и в пути

        }



        public static void RemoveCacheAll(string pointId)
        {
            RemoveCacheWheels(pointId);
            _cache.Remove( $"{KeyTyres}.{pointId}" ); // очистить кэш по шинам
            _cache.Remove($"{KeyAccs}.{pointId}"); // очистить кэш аксессуары
            
        }


        public static IEnumerable<string>  GetKeys()
        {
            return _cache.GetKeys();

        }

        public static long Count { get {return _cache.Count;} }




            //
            // Сортировка в зависимости от модели
            //
        private static IQueryable<SearchResult> GetOrderedResults(this IQueryable<SearchResult> queryNotOrdered, CommonPodborView model)
        {

            IQueryable<SearchResult> query;
            switch (model.SortBy)
            {
                case SortBy.NameAsc: query = queryNotOrdered.OrderBy(p => p.Name); break;
                case SortBy.NameDesc: query = queryNotOrdered.OrderByDescending(p => p.Name); break;
                case SortBy.AmountAsc: query = queryNotOrdered.OrderBy(p => p.Rest); break;
                case SortBy.AmountDesc: query = queryNotOrdered.OrderByDescending(p => p.Rest); break;
                case SortBy.DeliveryAsc: query = queryNotOrdered.OrderBy(p => p.DaysToDepartment); break;
                case SortBy.DeliveryDesc: query = queryNotOrdered.OrderByDescending(p => p.DaysToDepartment); break;
                case SortBy.PriceAsc: query = queryNotOrdered.OrderBy(p => p.PriceOfClient); break;
                case SortBy.PriceDesc: query = queryNotOrdered.OrderByDescending(p => p.PriceOfClient); break;
                default: query = queryNotOrdered.OrderBy(p => p.Name); break;

            }
            return query;
        }



        /// <summary>
        /// Сделать выборку из кэша для дисков
        /// </summary>
        /// <param name="podborModel"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        /// 

        public static IPagedList<DiskSearchResult> GetAllDisksByPartnerPoint( DisksPodborView podborModel,PartnerPoint point)
        {
            //   string productname;
            int pointId = point.PartnerPointId;
            IList<DiskSearchResult> result = _cache.GetOrAdd(String.Format("{0}.{1}.{2}", KeyDisks, pointId, podborModel.RestOrOnWay), 
                () => GetAllDisksByPartnerPointFromDb(pointId, podborModel), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin));

            var query = result.AsQueryable();
            //IList<DiskSearchResult> query = result.ToList();


            if (podborModel.FromOnWay && !podborModel.FromRests) query = query.Where(p => p.DepartmentId==0);
            if (!podborModel.FromOnWay && podborModel.FromRests) query = query.Where(p => p.DepartmentId != 0);

            if (podborModel.OnlySale) query = query.Where(p => p.IsSaleProduct);

            if (podborModel.ProducerId!=null) query = query.Where(p => p.ProducerId == (int)podborModel.ProducerId);
            if (podborModel.Diametr != null) query = query.Where(p => p.Diametr == podborModel.Diametr);
            if (podborModel.Width != null) query = query.Where(p => p.Width == podborModel.Width);
            if (podborModel.PCD != null) query = query.Where(p => p.PCD == podborModel.PCD);
            if (podborModel.DIA != null) query = query.Where(p => p.DIA == podborModel.DIA);
            if (podborModel.ETto != null)
            {
                query = query.Where(p => p.ET.ToDouble() >= podborModel.ET.ToDouble() && p.ET.ToDouble() <= podborModel.ETto.ToDouble());
            }
            else
            {
                if (podborModel.ET != null) query = query.Where(p => p.ET.ToDouble() == podborModel.ET.ToDouble());
            }
            if (podborModel.Hole != null) query = query.Where(p => p.Holes == ((int)podborModel.Hole).ToString());
            if (podborModel.CargoWheels && podborModel.Brands == null)
            {
                var brands = Defaults.CargoWheelsProducers.Split(Defaults.CommaSign);
                query = query.Where(p => brands.Contains(p.ProducerId.ToString()));
            }
            else if (!podborModel.CargoWheels && podborModel.Brands == null)
            {
                var brands = Defaults.CargoWheelsProducers.Split(Defaults.CommaSign);
                query = query.Where(p => !brands.Contains(p.ProducerId.ToString()));
            }
            else
            {
                var brands = podborModel.Brands.Split(Defaults.CommaSign);
                query = query.Where(p => brands.Contains(p.ProducerId.ToString()));
            }
            
            query = query.Where(p => p.PriceOfClient >= podborModel.PriceMin);
            query = query.Where(p => p.PriceOfClient <= podborModel.PriceMax);



            ///фильтр по производителям которые не должны показываться для данного партнера

            string partnerId = point.Partner?.PartnerId;
            if (partnerId!=null)
            { 
            var producersNotDisplayed= _dbContext.ProducerNotDisplayedFromPartners.Where(p => p.PartnerId == partnerId).Select(p => p.ProducerId).ToArray();            
            query = query.Where(p => !producersNotDisplayed.Contains( p.ProducerId??0));
            }

            /*if (!String.IsNullOrEmpty(podborModel.Article))
                if (Regex.IsMatch(podborModel.Article, @"^\d+$")) article = podborModel.Article;  */
            string  article =  podborModel.Article;



       //   if (!String.IsNullOrEmpty(podborModel.Name)) productname = podborModel.Name;
            
         
            //  if (productname != null) query = query.Where(p => p.Name.ToLower().Contains(productname.ToLower()));
            if (article != null) query = query.Where(p => p.ProductId.ToString().Contains(article) || p.Name.ToLower().Contains(article.ToLower()));
            if (podborModel.AllOrByCarReplica && podborModel.CarName != null) query = query.Where(p => p.ModelName.ToString().ToLowerInvariant().Contains(podborModel.CarName.ToLowerInvariant()));
            if (podborModel.IsSet4Items) query = query.Where(p => p.Rest + p.RestOtherStock >= 4);



            // return   GetOrderedResults(podborModel, query).Cast<DiskSearchResult>().ToPagedList(podborModel.Page, podborModel.ItemsPerPage);

            return query.GetOrderedResults(podborModel).Cast<DiskSearchResult>().ToPagedList(podborModel.Page, podborModel.ItemsPerPage);

            //   return query.ToPagedList(podborModel.Page, podborModel.ItemsPerPage);

        }


       

        /// <summary>
        /// Выборка из кэша для шин
        /// </summary>
        /// <param name="podborModel"></param>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public static IPagedList<TyreSearchResult> GetAllTyresByPartnerPoint(TyresPodborView podborModel, int pointId)
        {
                       
            IList<TyreSearchResult> result = _cache.GetOrAdd(String.Format("{0}.{1}",KeyTyres, pointId), () => GetAllTyresByPartnerPointFromDb(pointId, podborModel), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin));

            var query = result.AsQueryable();

            if (podborModel.ProducerId != null) query = query.Where(p => p.ProducerId == (int)podborModel.ProducerId);
            if (podborModel.Diametr != null) query = query.Where(p => p.Diametr == podborModel.Diametr);
            if (podborModel.Width != null) query = query.Where(p => p.Width == podborModel.Width);
            if (podborModel.Height != null) query = query.Where(p => p.Height == podborModel.Height);
            if (podborModel.SeasonId != null) query = query.Where(p => p.Season == podborModel.SeasonId);


            // if (podborModel.SeasonId==Defaults.TyresSettings.Winter) 
            {
                if (podborModel.Ship == ShipForTyresPodbor.ShipShip) query = query.Where(p => p.Name.Contains(Defaults.TyresSettings.Ship) );
            if (podborModel.Ship == ShipForTyresPodbor.ShipNoShip) query = query.Where(p => !p.Name.Contains(Defaults.TyresSettings.Ship) && p.ParentId == Defaults.TyresSettings.ShipNoShipParentId);
            if (podborModel.Ship == ShipForTyresPodbor.Friction) query = query.Where(p => !p.Name.Contains(Defaults.TyresSettings.Ship) && p.ParentId == Defaults.TyresSettings.TyresFrictionParentId);
            }

            query = query.Where(p => p.PriceOfClient >= podborModel.PriceMin);
            query = query.Where(p => p.PriceOfClient <= podborModel.PriceMax);
                        
            string article = podborModel.Article?.ToLowerInvariant().TrimStart(Defaults.Space.ToCharArray()[0]);

            if (article != null) query = query.Where(p => p.ProductIdTo7Simbols.ToString().Contains(article) || p.Name.ToLower().Contains(article.ToLower())  || p.Article!=null && p.Article.StartsWith(article.ToLower()));

            //if (article != null) query = query.Where(p =>p.Article!=null && p.Article.StartsWith(article.ToLower()));

            if (podborModel.IsSet4Items) query = query.Where(p => p.Rest+p.RestOtherStock >= 4);

            

          return  query.GetOrderedResults(podborModel).Cast<TyreSearchResult>().ToPagedList(podborModel.Page, podborModel.ItemsPerPage);

        }


        public static IPagedList<AccSearchResult> GetAllAccsByPartnerPoint(AccPodborView podborModel, int pointId)
        {
            IList<AccSearchResult> result = _cache.GetOrAdd(String.Format("{0}.{1}", KeyAccs, pointId), () => GetAllAccsByPartnerPointFromDb(pointId, podborModel), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin));

            var query = result.AsQueryable();
            string article = podborModel.Article == null ? null : podborModel.Article.ToLowerInvariant().TrimStart(' ');
            string name = podborModel.Name == null ? null : podborModel.Name.ToLowerInvariant().TrimStart(' ');
            if (article != null) query = query.Where(p => p.ProductIdTo7Simbols.ToString().Contains(article) || p.Name.ToLower().Contains(article.ToLower()) || p.Article != null && p.Article.ToLower().StartsWith(article.ToLower()));
            if (name != null) query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            if (podborModel.OnlySale) query = query.Where(p => p.IsSaleProduct);
            if (podborModel.ProducerId != null) query = query.Where(p => p.ProducerId == (int)podborModel.ProducerId);
            if (podborModel.SelectedCategories.Count != 0) query = query.Where(p => podborModel.SelectedCategories.Contains(p.ParentId.ToString()));

          
            return query.GetOrderedResults(podborModel).Cast<AccSearchResult>().ToPagedList(podborModel.Page, podborModel.ItemsPerPage);
        }
        

        private static IList<AccSearchResult> GetAllAccsByPartnerPointFromDb(int pointId, AccPodborView podborModel)
        {
            using (var productService = new ProductService())
            {
                var model = new AccPodborView();
                return productService.GetAccs(model, pointId);
            }
        }

        /// <summary>
        /// Возвращает цвета дисков
        /// </summary>
        public static IEnumerable<string> DiskColours
        {
            get {
                return _cache.GetOrAdd("disk_colours", GetDiskColoursFromDb, DateTimeOffset.UtcNow.AddMinutes(TimeoutMin));
            }
        }

        private static IEnumerable<string> GetAkbPropertiesFromDb(string productProperty)
        {
            lock  (LockAkbObject)
            { 
              var allAkbs = _dbContext.Products.Where(p => p.ProductType == ProductType.Akb);
            var akbsIdOnRests = from akbs in allAkbs
                join rests in _dbContext.Rests on akbs.ProductId equals rests.ProductId
                select akbs.ProductId;

            var resultarr = _dbContext.ProductProperties.Where(p => akbsIdOnRests.Contains(p.ProductId) && p.Name.ToLower() == productProperty).Select(p => p.Value).OrderBy(p => p).Distinct().ToArray();
            
            Array.Sort(resultarr, new StringAsNumberComparer());
                return resultarr;
            }
        }

        

        /// <summary>
        /// Производители чьи остатки есть на складах
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="addparam">cargo | agricult</param>
        /// <returns></returns>
        private static IEnumerable<Producer> GetProducersOfType(ProductType productType,string addparam)
        {
            lock (LockProducersObject)
            {
                Expression<Func<Product, bool>> filter;
                if (productType == ProductType.CargoDisk)
                {
                    filter = p => (_dbContext.Rests.Select(prod => prod.ProductId).Contains(p.ProductId) || _dbContext.RestsOfSuppliers.Select(prod => prod.ProductId).Contains(p.ProductId)) && p.ProducerId.HasValue &&
                        p.ProductType == ProductType.Disk;
                }
                else
                {
                    filter = p => (_dbContext.Rests.Select(prod => prod.ProductId).Contains(p.ProductId) || _dbContext.RestsOfSuppliers.Select(prod => prod.ProductId).Contains(p.ProductId)) && p.ProducerId.HasValue &&
                        p.ProductType == productType;
                }

                IEnumerable<Producer> producers;
                if (productType == ProductType.Acc)
                
                    producers = _dbContext.Products.Where(filter).Select(p => p.Producer).Distinct().OrderBy(p => p.Name);
                
                else if (productType == ProductType.Tyre)

                    producers = _dbContext.Products.Where(filter.And(p=>p.Producer.Active && (addparam != "cargo" || p.Model.Season == "agricult" || p.Model.Season == "cargo"))).Select(p => p.Producer).Distinct().OrderBy(p => p.Name);
                
                else if (productType == ProductType.Disk)
                {

                    var brands = Defaults.CargoWheelsProducers.Split(',');
                    producers = _dbContext.Products.Where(filter.And(p => p.Producer.Active && !brands.Contains(p.ProducerId.ToString())))
                    .Select(p => p.Producer).ToList().Union(Defaults.ProducersForgedWheels)
                    .Distinct(new ProducerEqualityComparer()).OrderBy(p => p.Name);
                }
                else if (productType == ProductType.CargoDisk)
                {
                    var brands = Defaults.CargoWheelsProducers.Split(',');
                    producers = _dbContext.Products.Where(filter.And(p => p.Producer.Active && brands.Contains(p.ProducerId.ToString())))
                    .Select(p => p.Producer).ToList()
                    .Distinct(new ProducerEqualityComparer()).OrderBy(p => p.Name);

                }
                
                else
                    producers = _dbContext.Products.Where(filter.And(p=> p.Producer.Active)).Select(p => p.Producer).Distinct().OrderBy(p => p.Name);
                

                return producers.ToList();

            }
        }



        /// <summary>
        /// Получить свойства аккумуляторов и отсортировать их как числа по возрастанию
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAkbProperties(string propertyName)
        {
             return  _cache.GetOrAdd("podbor.akb."+propertyName, ()=>GetAkbPropertiesFromDb(propertyName), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin)); 
        }

        private static IEnumerable<ProductCategory> GetParentFolder(ProductType pType)
        {
            string sqltext = "exec spGetParentFolderForProduct {0}";
            return _dbContext.Database.SqlQuery<ProductCategory>(sqltext, pType).ToList();
        }

        public static IEnumerable<ProductCategory> GetParentFolderForOthersProduct(ProductType pType)
        {
            return _cache.GetOrAdd("podbor.others.parentfolder." + pType, () => GetParentFolder(pType), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin));
        }

        public static IEnumerable<ReplicaDisksForCars> GetCarsList
        {
            get
            {
                return _cache.GetOrAdd("podbor.carslist", () => GetCarsListFromDb(), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin));
            }
        }

        private static IEnumerable<ReplicaDisksForCars> GetCarsListFromDb()
        {
            {
                IEnumerable<ReplicaDisksForCars> cars;
                cars = _dbContext.ReplicaDisksForCars.Where(p => p.Name != string.Empty).Distinct().OrderBy(p => p.Name);
                return cars.ToList();
            }
        }

        /// <summary>
        /// Возвращает производителей вида товара , только по товарам которые есть на остатках из кэша = 10 мин
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="addparam">cargo, если грузовые шины</param>
        /// <returns></returns>
        public static IEnumerable<Producer> GetProducers(ProductType productType,string addparam="")
        {
              return _cache.GetOrAdd("podbor.producers."+productType.ToString()+addparam, () => GetProducersOfType(productType,addparam), DateTimeOffset.UtcNow.AddMinutes(TimeoutMin)); 
        }


       
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfProduct"></param>
        /// <param name="propertyname"></param>
        /// <param name="wheelType">set if season podbor/ otherwise null </param>
        /// <returns></returns>
        public static string[] GetTiporazmerPropertiesFromDb(ProductType typeOfProduct, string propertyname,WheelType? wheelType=null)
        {
            IQueryable<int> ids;
            IQueryable<int?> tids;
            lock(LockTiporazmersObject)
            { 
                ids = wheelType==null ? _dbContext.Set<RestOfProduct>().Select(p => p.ProductId) : _dbContext.Set<SeasonStockItem>().Select(p => p.ProductId);


            tids = _dbContext.Set<Product>().Where(p => p.ProductType == typeOfProduct && (wheelType == null || p.WheelType == wheelType) && ids.Contains(p.ProductId)).Select(tip => tip.TiporazmerId); // id типоразмеров
              

            var selector = GetSelector<Tiporazmer, string>(propertyname.ToLower());

            string[] resultarr = _dbContext.Set<Tiporazmer>().Where(tip => tids.Contains(tip.TiporazmerId)).Select(selector).Distinct().ToArray();  //товары на остатках

            var resultFilteredAndSorted = resultarr.Where(name => !String.IsNullOrEmpty(name)).OrderBy(name => name).Select(str => str.Replace(',', '.')).ToArray();

            Array.Sort(resultFilteredAndSorted, new StringAsNumberComparer());

            return resultFilteredAndSorted;
            }
        }


        /// <summary>
        /// Получить свойства грузовых шин из базы данных и отсортировать их как числа по возрастанию
        /// </summary>
        /// <param name="typeOfProduct"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>

        public static string[] GetCargoPropertiesFromDb(ProductType typeOfProduct, string propertyname)
        {
            var selector = GetSelector<Tiporazmer, string>(propertyname.ToLower());
            var models = _dbContext.Set<Model>().Where(p => p.Season == "cargo" || p.Season == "agricult").Select(p => p.ModelId);
            var ids = _dbContext.Set<RestOfProduct>().Select(p => p.ProductId);
            var tids = _dbContext.Set<Product>().Where(p => p.ProductType == typeOfProduct && models.Any(id => id == p.ModelId) && ids.Any(id => id == p.ProductId)).Select(tip => tip.TiporazmerId); // типоразмеры выбранного типа на остатках
            string[] resultarr = _dbContext.Set<Tiporazmer>().Where(tip => tids.Any(tid => tid == tip.TiporazmerId)).Select(selector).Distinct().ToArray();

            resultarr = resultarr.Where(p => !String.IsNullOrEmpty(p)).ToArray();
            Array.Sort(resultarr, new StringAsNumberComparer());
           
            return resultarr;

        }

        public static string[] GetOtherPropertiesFromDb(string propertyname, int? folder)
        {
            var selector = GetSelector<Tiporazmer, string>(propertyname.ToLower());
            var ids = _dbContext.Set<RestOfProduct>().Select(p => p.ProductId);
            var tids = _dbContext.Set<Product>().Where(p => p.ProductType == 0 && p.ParentId == folder && ids.Any(id => id == p.ProductId)).Select(tip => tip.TiporazmerId);
            string[] resultarr = _dbContext.Set<Tiporazmer>().Where(tip => tids.Any(tid => tid == tip.TiporazmerId)).Select(selector).Distinct().ToArray();

            resultarr = resultarr.Where(p => !String.IsNullOrEmpty(p)).ToArray();
            Array.Sort(resultarr, new StringAsNumberComparer());

            return resultarr;

        }

        /// <summary>
        /// Вызывается для списков параметров по типоразмерам шин и дисков в наличии
        /// </summary>
        /// <param name="typeOfProduct">тип товара</param>
        /// <param name="propertyname">название свойства</param>
        /// <returns></returns>
        public static IEnumerable<string> GetTiporazmerProperties(ProductType typeOfProduct, string propertyname)
        {
            return _cache.GetOrAdd("podbor.tiporazmer" + typeOfProduct + propertyname, () => GetTiporazmerPropertiesFromDb(typeOfProduct, propertyname), DateTimeOffset.UtcNow.AddMinutes(20));
        }

        public static IEnumerable<string> GetOtherProperties(string propertyname, int? folder)
        {
            return _cache.GetOrAdd("podbor.tiporazmer" + folder + propertyname, () => GetOtherPropertiesFromDb(propertyname, folder), DateTimeOffset.UtcNow.AddMinutes(20));
        }
        /// <summary>
        /// Вызывается для списков параметров по типоразмерам шин и дисков в наличии
        /// </summary>
        /// <param name="typeOfProduct">тип товара</param>
        /// <param name="propertyname">название свойства</param>
        /// <returns></returns>
        public static IEnumerable<string> GetCargoProperties(ProductType typeOfProduct, string propertyname)
        {
            return _cache.GetOrAdd("podbor.cargo" + typeOfProduct + propertyname, () => GetCargoPropertiesFromDb(typeOfProduct, propertyname), DateTimeOffset.UtcNow.AddMinutes(30));
        }

        /// <summary>
        /// Вызывается для списков параметров по типоразмерам  дисков в сезонном ассортименте
        /// </summary>
        /// <param name="typeOfProduct">тип товара</param>
        /// <param name="propertyname">название свойства</param>
        /// <returns></returns>
        public static IEnumerable<string> GetTiporazmerProperties(ProductType typeOfProduct, string propertyname,WheelType wheeltype)
        {
            return _cache.GetOrAdd(String.Format("podbor.season.wheels.{0}.{1}.{2}", typeOfProduct, propertyname, wheeltype), () => GetTiporazmerPropertiesFromDb(typeOfProduct, propertyname, wheeltype), DateTimeOffset.UtcNow.AddMinutes(30));
        }

        private static Expression<Func<T, TReturn>> GetSelector<T, TReturn>(string fieldName)
            where T : class
            where TReturn : class
        {
            var t = typeof(TReturn);
            ParameterExpression p = Expression.Parameter(typeof(T), "t");
            var body = Expression.Property(p, fieldName);
            return Expression.Lambda<Func<T, TReturn>>(body, new ParameterExpression[] { p });
        }

      
    }
}