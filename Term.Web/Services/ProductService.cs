using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.ViewModels;
using Yst.Context;
using System.Data.SqlClient;
using Term.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using YstTerm.Models;
using YstProject.Services;
using PagedList;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Term.Utils;
using System.Text;
using System.Linq.Expressions;
using Term.Web.Views.Resources;
using System.Data.Entity;
using Term.Web.Services;

namespace Yst.Services
{
    /// <summary>
    /// Класс для сортировки по ковыным дискам
    /// </summary>
    class DiskComparerByProducer : IComparer<int>
    {
        private static readonly IEnumerable<int> producersForgedWheels = Defaults.ProducersForgedWheels.Select(p => p.ProducerId);

        public int Compare(int x, int y)
        {
          if ( producersForgedWheels.Any(p=> p==x) && !producersForgedWheels.Any(p=>p==y)) return -1;
            if (!producersForgedWheels.Any(p => p == x) && producersForgedWheels.Any(p => p == y)) return 1;
            return 0;
        }
    }

    /// <summary>
    /// Основной сервис для подборов
    /// </summary>
    public class ProductService : BaseService, IDisposable
    {
        private bool _allocDBContext = false;
     //   public const string PartnerIdSessionKey = "PartnerId";

        public ProductService() : this(new AppDbContext()) { 
        _allocDBContext = true;
        }

        public ProductService(AppDbContext dbcontext): base(dbcontext)        {        }

        /// <summary>
        /// Возвращает результат подбора дисков (вызов хранимой процедуры)
        /// </summary>
        /// <param name="podborModel"></param>
        /// <param name="pointId"></param>
        /// <param name="SaleMode"></param>
        /// <param name="exactsize"></param>
        /// <returns></returns>
        public IList<DiskSearchResult> GetDisks(DisksPodborView podborModel, int pointId, bool SaleMode, int exactsize = 1)
        {
            string productname=null, article=null;

            if (podborModel.PriceMax == 0) podborModel.PriceMax = Defaults.PriceMaxRus;
            string partnerId = GetPartnerIdByPointId(pointId);
            string parametersToString = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Hole, @Dia,@PCD, @ET, @Article, @ProductName, @DiskColor,@ExactSize, @SortBy,@ForPriceExcel,@OnlySale,@Brands, @PriceMin, @PriceMax,@TypeOfRests,@IsSet4Items";


            string sqltext = (IsPartner ? @"exec spGetDisksPartnerToClient " : "exec spGetDisksPointToClient ") + parametersToString;


            if (!String.IsNullOrEmpty(podborModel.Article))
                if (Regex.IsMatch(podborModel.Article, @"^\d+$")) article = podborModel.Article; else productname = podborModel.Article;

            var parameters = new SqlObjectParameterCollection();
            parameters.Add("PartnerID", partnerId).Add("PartnerPointID", pointId).Add("SortBy", podborModel.SortBy.ToString()).Add("Article", (object)article??DBNull.Value)
            .Add("ProductName", (object)productname ?? DBNull.Value).Add("ExactSize", exactsize).Add("ForPriceExcel",0).Add("OnlySale", podborModel.OnlySale && IsPartner && SaleMode ? 1 : 0)
            .Add("Brands", (object)podborModel.Brands ?? DBNull.Value).Add("PriceMin", (object)podborModel.PriceMin ?? 0).Add("PriceMax", (object)podborModel.PriceMax ?? Defaults.PriceMaxRus).Add("TypeOfRests", (object)podborModel.RestOrOnWay).Add("IsSet4Items", podborModel.IsSet4Items ? 1 : 0);
            
            parameters.GetParametersFromObject(podborModel, "ProducerId", "Diametr", "Width", "Hole", "DIA", "PCD", "ET", "DiskColor");

            var results= DbContext.Database.SqlQuery<DiskSearchResult>(sqltext, parameters.ToArray()).ToList();

            // сортируем сперва по кованым, потом по наименованию

            //  var selector = ReflectionExtensions.GetSelector<DiskSearchResult, string>(podborModel.SortBy.ToString().ToLower());

             results = results.OrderBy(p => p.ProducerId != null ? (int)p.ProducerId : 0, new DiskComparerByProducer()).GetOrderedResultsThenBy(podborModel.SortBy).ToList();


            return results;
        }



        

        /// <summary>
        /// Подбор шин по параметрам (вызов хранимой процедуры)
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="pointId"></param>
        public IList<TyreSearchResult> GetTyres(TyresPodborView pb, int pointId)
        {
            string partnerId = GetPartnerIdByPointId(pointId);

            string parametersStr = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Height, @SeasonId, @Article, @ProductName, @SortBy, @Ship, @PriceMin, @PriceMax";
            string sqltext = String.Format("exec {0} {1}", IsPartner ? "spGetTyresPartnerToClient" : "spGetTyresPointToClient", parametersStr);
            
            var parameters = new SqlObjectParameterCollection();
            parameters.Add("PartnerID", partnerId);
            parameters.Add("PartnerPointID", pointId);
            parameters.Add("ProductName", pb.Name);
            parameters.Add("SortBy", pb.SortBy.ToString());
            parameters.GetParametersFromObject(pb, "ProducerId", "Diametr", "Width", "Height", "SeasonId", "Article", "Ship", "PriceMin", "PriceMax");
                     
            return DbContext.Database.SqlQuery<TyreSearchResult>(sqltext, parameters.ToArray()).ToList();

        }

        /// <summary>
        /// Подбор по грузовым шинам
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="pointId"></param>
        public void GetCargoTyres(CargoTyresPodborView pb, int pointId)
        {

            string partnerId = GetPartnerIdByPointId(pointId);
            if (pb.Width != null && Defaults.CargoTyreWidthAnalog.TryGetValue(pb.Width, out string value))
            {
                pb.WidthAnalog = Defaults.CargoTyreWidthAnalog[pb.Width];
            }

            string parametersStr = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Height, @Article, @ProductName, @SortBy, @PriceMin, @PriceMax, @WidthAnalog";
            string sqltext = String.Format("exec {0} {1}", IsPartner ? "spGetCargoTyresPartnerToClient" : "spGetCargoTyresPointToClient", parametersStr);


            var parameters = new SqlObjectParameterCollection();
            parameters.Add("PartnerID", partnerId);
            parameters.Add("PartnerPointID", pointId);
            parameters.Add("ProductName", pb.Name);
            parameters.Add("SortBy", pb.SortBy.ToString());
            parameters.GetParametersFromObject(pb, "ProducerId", "Diametr", "Width", "Height", "Article", "PriceMin", "PriceMax", "WidthAnalog");

            pb.SearchResults = DbContext.Database.SqlQuery<TyreSearchResult>(sqltext, parameters.ToArray()).ToArray().ToPagedList(pb.Page, pb.ItemsPerPage);

        }

        /// <summary>
        /// Получить аккумуляторы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pointId"></param>
        public void GetAkbs(AkbPodborView model,  int pointId)
        {

            string partnerId = GetPartnerIdByPointId(pointId);

            //string sqltext = (IsPartner ? "exec spGetAkb @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName, @SortBy, @PriceMin, @PriceMax" : "exec spGetAkbPointToClient @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName, @SortBy, @PriceMin, @PriceMax");

            string parametersStr = " @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName,@Producer,@AkbType, @SortBy, @PriceMin, @PriceMax";
            string sqltext = String.Format("exec {0} {1}", IsPartner ? "spGetAkb" : "spGetAkbPointToClient", parametersStr);

            var parameters = new SqlObjectParameterCollection();
            parameters.Add("PartnerID", partnerId);
            parameters.Add("PartnerPointID", pointId);
            parameters.Add("ProductName", model.Name);
            parameters.Add("SortBy", model.SortBy.ToString());
         //   parameters.Add("Brand", model.Brand == null ? null : model.Brand.Join('|'));

            parameters.Add("Brand", model.Brand == null ? null : String.Join("|", model.Brand));

            parameters.GetParametersFromObject(model, 
                "ProducerId", "Inrush_Current", "Volume", "Polarity", /*"Brand", */
            "Size", "Article",
                "ProductName", "Producer","AkbType","SortBy","PriceMin","PriceMax");
            

            var results = DbContext.Database.SqlQuery<AkbSearchResult>(sqltext, parameters.ToArray()).ToArray();
            model.SearchResults = results.Where(p =>!model.OnlySale ||p.IsSaleProduct).ToPagedList(model.Page, model.ItemsPerPage);
            
            //pb.SearchResults = (pb.SearchResults as IPagedList<AkbSearchResult>).Where(p => p.IsSaleProduct);


        }

        public IList<AccSearchResult> GetAccs(AccPodborView pb,  int pointId)
        {
            

            var categories = pb.SelectedCategories.Count > 0 ? (object)String.Join(",", pb.SelectedCategories.Select(p => p.ToString()).ToArray()) : DBNull.Value;
            string partnerId = GetPartnerIdByPointId(pointId);

            
            string sqltext = (IsPartner ? "exec spGetAcc @PartnerId, @PartnerPointId, @Categories, @Article, @ProducerId, @ProductName, @SortBy" : "exec spGetAccPointToClient @PartnerId, @PartnerPointId, @Categories, @Article, @ProducerId, @ProductName, @SortBy");

            List<SqlParameter> sqlparams = new List<SqlParameter>
            {new SqlParameter("PartnerID", partnerId),  new SqlParameter("PartnerPointID", pointId),
            new SqlParameter ( "Categories", categories ),
              new SqlParameter { ParameterName = "@Article", Value = (object)pb.Article ?? DBNull.Value },
              new SqlParameter { ParameterName = "@ProducerId", Value = (object)pb.ProducerId ?? DBNull.Value } ,
            new SqlParameter { ParameterName = "@ProductName", Value = (object)pb.Name ?? DBNull.Value } ,
             new SqlParameter { ParameterName = "@SortBy", Value = (object)pb.SortBy.ToString() ?? "NameAsc" } 
            };


          //  pb.SearchResults = DbContext.Database.SqlQuery<AccSearchResult>(sqltext, sqlparams.ToArray()).ToArray().ToPagedList(pb.Page, pb.ItemsPerPage);

            return DbContext.Database.SqlQuery<AccSearchResult>(sqltext, sqlparams.ToArray()).ToList();

            //pb.SearchResults = results.Where(p => !pb.OnlySale || p.IsSaleProduct).ToPagedList(pb.Page, pb.ItemsPerPage);

        }




       



        public void GetOthersFromPodbor(OthersPodborView podbor, int pointId)
        {
            podbor.DepartmentId = podbor.DepartmentId ?? Defaults.MainDepartment;
            var parameters = new SqlObjectParameterCollection();
            parameters.Add("PartnerPointID", pointId);
            parameters.GetParametersFromObject(podbor, "Folder", "RestOrOnWay", "Name", "DepartmentId", "Diametr", "Width");
            string sqltext = "exec spGetOthersProduct @PartnerPointId, @Folder, @RestOrOnWay, @Name, @DepartmentId, @Diametr, @Width";

            podbor.SearchResults = DbContext.Database.SqlQuery<SearchResult>(sqltext, parameters.ToArray()).ToArray().ToPagedList((int)podbor.Page, podbor.ItemsPerPage);
        }



        #region prices

        /// <summary>
        /// Получить цену товара для партнера
        /// </summary>
        /// <param name="productId">код товара</param>
        /// <param name="partnerId">код партнера</param>
        /// <returns></returns>
        public decimal GetPriceOfProduct(int productId, string partnerId = null)
        {

             bool? usePrepayPrices = base.CurrentPoint?.Partner?.UsePrepayPrices;

            Expression<Func<PriceOfPartner, decimal>> funcPrice = p => p.Price;
            
            // если есть  предоплатные цены и они активны, то берем цену 2 (предоплатную)

            if (usePrepayPrices.HasValue && usePrepayPrices.Value) funcPrice = p => p.Price2 > 0 ? p.Price2 : p.Price;
            

            if (!String.IsNullOrEmpty(partnerId))
            {
                decimal partnerPrice = DbContext.PriceOfPartners.Where(p => p.PartnerId == partnerId && p.ProductId == productId).Select(funcPrice).FirstOrDefault();

                if (partnerPrice > 0) return partnerPrice;
            }

            decimal? priceOfProduct = DbContext.PriceOfProducts.Where(p => p.ProductId == productId && p.Price != null).Select(p => p.Price).FirstOrDefault();

            return (priceOfProduct ?? Decimal.Zero);

        }


        
        /// <summary>
        /// Получить цену для партнерской точки
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="partnerPointId"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public decimal GetPriceOfPoint(int productId, int partnerPointId, string partnerId)
        {

            Partner partner = GetPartnerById(partnerId);

            //bool isForeign = partner == null ? false : partner.IsForeign;
            bool isForeign = partner != null && partner.IsForeign;
            decimal priceOfClient = 0M;
            // 1. определяем производителя

            //int ProducerId = DbContext.Products.Where(p => p.ProductId == ProductId).Select(p => p.ProducerId).FirstOrDefault() ?? 0;

            var product = DbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            int producerId = product.ProducerId ?? 0;

            if (producerId == 0) return 0; // no such producer
            //if (product == null) return 0; // no such product

            if (product.ProductType == ProductType.Akb) producerId = 3387;

            if (product.ProductType == ProductType.Acc) producerId = 3039;
            // 2. смотрим правила для партнерской точки по данному производителю
            var priceRule = DbContext.PartnerPriceRules.Where(p => p.PartnerPointId == partnerPointId && p.ProducerId == producerId && p.PriceInOut == PriceListFor.Point).Select(p => new { PriceType = p.PriceType, PType = p.PType, Discount = p.Discount }).FirstOrDefault();

            if (priceRule == null) return GetOnePriceOfProduct(productId, p => p.Price1);//getRecommendedWholeSailPriceOfProduct(ProductId);

            if (priceRule.PType == PriceTypeEnum.Dont_Show_Price) return 0;

            switch (priceRule.PType)
            {
                case PriceTypeEnum.Base:
                  //  PriceOfClient = Math.Round(getBasePriceOfProduct(ProductId) * (1 + PriceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                    priceOfClient = Math.Round(GetOnePriceOfProduct(productId, p =>p.PriceBase) * (1 + priceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                    break;
                case PriceTypeEnum.Zakup:
                    if (!isForeign) priceOfClient = Math.Round(GetPriceOfProduct(productId, partnerId) * (1 + priceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                    else priceOfClient = Math.Round((1 + (decimal)partner.CustomDutyVal / 100) * (1 + (decimal)partner.VatVal / 100) * GetPriceOfProduct(productId, partnerId) * (1 + priceRule.Discount / 100), 2, MidpointRounding.AwayFromZero);
                    break;
                case PriceTypeEnum.Recommend:
                    priceOfClient = //getRecommendedWholeSailPriceOfProduct(ProductId); 
                        GetOnePriceOfProduct(productId, p => p.Price1);
                    break;
                default: priceOfClient = 0;
                    break;
            }

            return priceOfClient;

        }


        
        /// <summary>
        /// Получить цену для конечного клиента
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="partnerPointId"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public decimal GetPriceOfClient(int productId, int partnerPointId, string partnerId)
        {
            decimal PriceOfClient = 0M;
            Partner partner = GetPartnerById(partnerId);
            bool isForeign = partner == null ? false : partner.IsForeign;

            // 1. определяем производителя

            //int ProducerId = DbContext.Products.Where(p => p.ProductId == ProductId).Select(p => p.ProducerId).FirstOrDefault() ?? 0;

            var product = DbContext.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null) return 0; // no such product
            int ProducerId = product.ProducerId ?? 0;

            if (ProducerId == 0) return 0;


            if (product.ProductType == ProductType.Akb) ProducerId = 3387;

            if (product.ProductType == ProductType.Acc) ProducerId = 3039;

            // 2. смотрим правила для партнерской точки по данному производителю

            var PriceRule = DbContext.PartnerPriceRules.Where(p => p.PartnerPointId == partnerPointId && p.ProducerId == ProducerId && p.PriceInOut == PriceListFor.Client).Select(p => new { PriceType = p.PriceType, PType = p.PType, Discount = p.Discount }).FirstOrDefault();

            if (PriceRule == null) return GetOnePriceOfProduct(productId, p => p.Price2);//getRecommendedRetailPriceOfProduct(ProductId);

            if (PriceRule.PType == PriceTypeEnum.Dont_Show_Price) return 0;

            switch (PriceRule.PType)
            {

                case PriceTypeEnum.Base:
                    PriceOfClient = Math.Round(GetOnePriceOfProduct(productId, p => p.PriceBase) * (1 + PriceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                    break;
                case PriceTypeEnum.Zakup:
               /*     if (new ServicePartnerPoint(DbContext).IsPartner)  PriceOfClient = Math.Round(getPriceOfProduct(ProductId, PartnerId) * (1 + PriceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                    else  PriceOfClient = Math.Round(getPriceOfPoint(ProductId, PartnerPointId, PartnerId) * (1 + PriceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                    */
                    using (var service = new ServicePartnerPoint(DbContext))
                    {
                        if (!isForeign) // для российских контрагентов
                        {
                            if (service.IsPartner) PriceOfClient = Math.Round(GetPriceOfProduct(productId, partnerId) * (1 + PriceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                            else PriceOfClient = Math.Round(GetPriceOfPoint(productId, partnerPointId, partnerId) * (1 + PriceRule.Discount / 100), 0, MidpointRounding.AwayFromZero);
                        }
                        else // для иностранных контрагентов
                        {
                            if (service.IsPartner) PriceOfClient = Math.Round((1+(decimal)partner.CustomDutyVal/100) * (1+(decimal)partner.VatVal/100) * GetPriceOfProduct(productId, partnerId) * (1 + PriceRule.Discount / 100), 2, MidpointRounding.AwayFromZero);
                            else PriceOfClient = Math.Round(/*don't need multiply vat */ GetPriceOfPoint(productId, partnerPointId, partnerId) * (1 + PriceRule.Discount / 100), 2, MidpointRounding.AwayFromZero);
                        }
                    }

                    break;
                case PriceTypeEnum.Recommend:
                    PriceOfClient = GetOnePriceOfProduct(productId, p => p.Price2); //getRecommendedRetailPriceOfProduct(ProductId);
                    break;
                default: PriceOfClient = 0;
                    break;
            }

            return PriceOfClient;
        }

        protected decimal GetOnePriceOfProduct(int productId, Func<PriceOfProduct, decimal?> func)
        {
            return DbContext.PriceOfProducts.Where(p => p.ProductId == productId).Select(func).FirstOrDefault() ?? Decimal.Zero;
        }

        #endregion


        


    /// <summary>
    /// Get product with related entities
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
        public Product GetProduct(int id)
        {
             return DbContext.Products.Include(p=>p.Producer).Include(p=>p.Model).Include(p=>p.Tiporazmer) 
                 .FirstOrDefault(p => p.ProductId == id);

            
        }

        public string GetProductProperty(int id, Func<Product,string> selector)
        {
              var product = DbContext.Products.FirstOrDefault(p => p.ProductId == id);

            if (product==null) return String.Empty;
                      
            return (selector(product));
            
            
        }

      



        public void Dispose()
        {
            if (_allocDBContext)
            DbContext.Dispose();
        }

        /// <summary>
        /// Prepares string grouped by modification
        /// </summary>
        /// <param name="carRecords"></param>
        /// <returns></returns>
        public string GetModifications(IEnumerable<CarRecordViewDetail> carRecords)
        {
            Func<int, string> valToAdd = x => (x > 0) ? ", " : String.Empty;

            var formerCars = new HashSet<string>();

            var result = new StringBuilder();

            if (!carRecords.Any()) return string.Empty;
            int counter = 0;
            string rear = string.Empty;
            foreach (var carRecord in carRecords)
            {
                int endYear;
                rear = carRecord.Rear == 1 ? ForSearchResult.Rear : "";
                result.Append(valToAdd(counter));
                endYear = carRecord.EndYear == 0 ? endYear = DateTime.Now.Year : endYear = carRecord.EndYear;
                if (formerCars.Contains(carRecord.CarName)) result.AppendFormat("{0} ({1}-{2})({3})<em>{4}</em>", carRecord.ModificationName, carRecord.BeginYear, endYear, carRecord.BoltsSize, rear);
                else
                {
                    result.AppendFormat("<b>{0}</b> {1} ({2}-{3})({4})<em>{5}</em>", carRecord.CarName, carRecord.ModificationName, carRecord.BeginYear, endYear, carRecord.BoltsSize, rear);
                    formerCars.Add(carRecord.CarName);
                }
                counter++;
            }
            return result.ToString();

        }

        private string GetModificationsForOthersProducts(IEnumerable<CarRecordViewDetail> carRecords)
        {
            Func<int, string> valToAdd = x => (x > 0) ? ", " : String.Empty;

            var formerCars = new HashSet<string>();
            var formerModifications = new HashSet<string>();

            var result = new StringBuilder();

            if (!carRecords.Any()) return string.Empty;
            int counter = 0;
            foreach (var carRecord in carRecords)
            {
                if (!formerModifications.Contains(carRecord.ModificationName))
                {
                    result.Append(valToAdd(counter));

                    if (formerCars.Contains(carRecord.CarName))
                    {
                        result.AppendFormat("{0} ", carRecord.ModificationName);
                        formerModifications.Add(carRecord.ModificationName);
                    }
                    else
                    {
                        result.AppendFormat("{0} {1} ", carRecord.CarName, carRecord.ModificationName);
                        formerModifications.Clear();
                        formerModifications.Add(carRecord.ModificationName);
                        formerCars.Add(carRecord.CarName);
                    }
                    counter++;
                }
            }
            return result.ToString();

        }

        /// <summary>
        /// Если число авто больше Defaults.NumberOfApplicationOfCarsToLimit, 
        /// то возвращается применяемость не более чем 5 лет с текущего года
        /// 
        /// </summary>
        private void GetCarRecordsReduced(CarRecordViewDetail[] carRecordsMain)
        {
            if (carRecordsMain.Length > Defaults.NumberOfApplicationOfCarsToLimit)
            {
               var newRecords = carRecordsMain.Where(p => p.EndYear <= DateTime.Now.Year && p.EndYear >= DateTime.Now.Year - Defaults.NumberOfApplicationOfYearsToLimit).ToArray();
                if (newRecords.Length > 0) carRecordsMain = newRecords;
            }
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="productId">Код товара</param>
       /// <param name="exactsize">0 - с допуском, 1 - точное совпадение</param>
       /// <param name="funcToFormat"> функция для форматирования</param>
       /// <returns></returns>
        public Dictionary<string, string> GetCarsFromProduct(int productId, int exactsize , Func<IEnumerable<CarRecordViewDetail>,string> funcToFormat)
        {
            var product = GetProduct(productId);

          var producerId= $"{ product?.ProducerId}";

            string vendorForProduct = null;
            string[] replicaWheelProducers = Defaults.ReplicaWheelsProducers.Split(Defaults.CommaSign);
            // если 
            if (replicaWheelProducers.Contains(producerId) && product.ProductType == ProductType.Disk)
            {
                var modelName = product.Model?.Name ;
                var digitsArray = string.Join("", Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray()).ToCharArray();
                if (modelName != null)
                {
                    var modelname = modelName.Replace("_Concept-", "").Replace("-S", "").Trim(digitsArray).Trim();
                    // var modelname = modelName.Trim(new Char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).Replace("_Concept-", "").Trim();
                    vendorForProduct = CachedCollectionsService.GetCarsList.FirstOrDefault(p => p.Id == modelname)?.Name;
                }
            }

            var vendorsAndModifications = new Dictionary<string, string>();

         //   string result = String.Empty;
            string sqltext = @"EXEC spGetCarsFromProduct {0},{1}";

            var carRecords = DbContext.Database.SqlQuery<CarRecordViewDetail>(sqltext, productId, exactsize).ToArray();
            // основной 
            var carRecordsMain = carRecords.Where(p => p.Rear == 0).ToArray();

            // задний 
            var carRecordsRear = carRecords.Where(p => p.Rear == 1).ToArray();

            if (vendorForProduct != null)
            {
                if (exactsize == 1 && replicaWheelProducers.Contains(producerId) && product.ProductType == ProductType.Disk)
                {
                    var newRecords = carRecordsMain.Where(p => p.VendorName == vendorForProduct).ToArray();
                    if (newRecords.Length > 0) carRecordsMain = newRecords;
                    GetCarRecordsReduced(carRecordsMain);
                }
            }
            if (exactsize == 1 && !replicaWheelProducers.Contains(producerId) && product.ProductType == ProductType.Disk)   GetCarRecordsReduced(carRecordsMain);
            
            if (exactsize == 0 && product.ProductType == ProductType.Disk) GetCarRecordsReduced(carRecordsMain);
            

            var carRecordsFinish = carRecordsMain.Concat(carRecordsRear).OrderBy(p => p.CarName).ToArray();
            var vendors = carRecordsFinish.Select(p => p.VendorName).Distinct().ToArray();

             
            foreach (var vendor in vendors)
            {
                //   func = GetModifications(carRecordsFinish.Where(p => p.VendorName == vendor).ToArray();
               var data= funcToFormat(carRecordsFinish.Where(p => p.VendorName == vendor).ToArray());
                vendorsAndModifications.Add(vendor, data); /*GetModifications(carRecordsFinish.Where(p => p.VendorName == vendor).ToArray() )*/

            }

            return vendorsAndModifications;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsBolts"></param>
        /// <param name="Bolts"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCarsFromOthersProduct(bool IsBolts, string Bolts)
        {
            Dictionary<string, string> vendorsAndModifications = new Dictionary<string, string>();
            var carRecords = DbContext.CarRecords.Where(p => p.IsBolts == IsBolts && (p.Bolts == Bolts || p.Bolts == Bolts.Replace(".", ","))).Select(
               p => new CarRecordViewDetail
               {
                   VendorName = p.VendorName,
                   CarName = p.CarName,
                   ModificationName = p.ModificationName
               }
                ).ToArray();
            string[] Vendors = carRecords.Select(p => p.VendorName).Distinct().ToArray();


            foreach (var vendor in Vendors) vendorsAndModifications.Add(vendor, GetModificationsForOthersProducts(carRecords.Where(p => p.VendorName == vendor).ToArray()));

            return vendorsAndModifications;
        }

        public Dictionary<string, string> GetCarsFromCargoWheels(string article)
        {
            Dictionary<string, string> appToCargoWweels = new Dictionary<string, string>();

            var productProperties = DbContext.CargoWheelsVehicles.Where(p => p.Article == article).ToArray();
            if (!productProperties.Any())
                return null;
            appToCargoWweels.Add(ForSearchResult.Perfect_for, productProperties[0].AppCars);
            appToCargoWweels.Add(ForSearchResult.Analogs, productProperties[0].Analogs);
            appToCargoWweels.Add(ForSearchResult.Permitted_tire_sizes, productProperties[0].TyresSizes);
            return appToCargoWweels;
        }


        public ProductPropertyValue[] GetProductProperties(int productId)
        {
            var productProperties = DbContext.ProductProperties.Include(path=>path.ProductPropertyDescription).Where(p => p.ProductId == productId).ToArray();

            if (!productProperties.Any())
                return null;

            return productProperties;

        }

        public int GetCountOfAddPhoto(string namePhoto)
        {
            return _dbContext.PhotoForProducts.Count(p => p.NamePhoto == namePhoto);
        }

        /// <summary>
        /// function returns cont of products of given type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public int GetCountOfProducts(ProductType productType)
        {
            var idsOfProducts=DbContext.Products.Where(p => p.ProductType == productType).Select(p=>p.ProductId);
          //  Debug.WriteLine("GetCountOfProducts " + productType.ToString());
            return DbContext.Rests.Where(r => idsOfProducts.Contains(r.ProductId)).Select(p => p.ProductId).Distinct().Count();

      
        }
    }
}