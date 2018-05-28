using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Term.DAL;
using System.ComponentModel;
using Yst.DropDowns;
using System.Configuration;
using PagedList;
using Yst.Context;
using Yst.Services;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Term.Web.HtmlHelpers;
using Term.Web.Views.Resources;
using YstProject.Services;
using Term.Utils;
using Term.CustomAttributes;

namespace YstTerm.Models
{

    public class BreadcrumbModel
    {
        public string  CurrentActionName { get; set; }
    }

    /// <summary>
    /// Select all tiporazmers by car
    /// </summary>
     [DataContract(Namespace = "", Name = "tiporazmer")]
    public class TiporazmerByCarModelView
    {
        private int _beginYear, _endYear, _mode, _profile;
        private string _pcdc, _pcdd, _dia, _bolts;
        private bool _isbolts;
        private double? _width, _et, _diameter, _rearwidth, _rearet, _reardiameter;
        private int? _rearprofile;
        public ProductType ProductType;

        public TiporazmerByCarModelView(CarRecord carRecord)
        {
            _beginYear = carRecord.BeginYear; _endYear = carRecord.EndYear; _pcdc = carRecord.PCDc; _pcdd = carRecord.PCDd; _diameter = carRecord.Diameter;
            _mode = carRecord.Mode; _profile = carRecord.Profile; _width = carRecord.Width; _et = carRecord.ET; ProductType = carRecord.ProductType;
            _dia = carRecord.Dia; _rearwidth = carRecord.RearWidth; _rearet = carRecord.RearET; _reardiameter = carRecord.RearDiameter; _rearprofile = carRecord.RearProfile; _bolts = carRecord.Bolts; _isbolts = carRecord.IsBolts;
        }

        [DataMember]
        public string Tiporazmer
        {
            get
            {
                return
                ProductType == ProductType.Tyre ? String.Format("{0}/{1}R{2}", _width, _profile, _diameter) : String.Format(CultureInfo.InvariantCulture, "{0}x{1}/{2}x{3} ET{4} D{5}", _width, _diameter, _pcdc, _pcdd, _et, _dia);

            }
            private set
            { }
        }

        public string TiporazmerRear
        {
            get
            {
                if (_rearprofile == null && _rearet == null) return String.Empty;
                
                else return ProductType == ProductType.Tyre ? String.Format("{0}/{1}R{2}", _rearwidth, _rearprofile, _reardiameter) : String.Format(CultureInfo.InvariantCulture, "{0}x{1}/{2}x{3} ET{4} D{5}", _rearwidth, _reardiameter, _pcdc, _pcdd, _rearet, _dia);
                
            }
        }
        //public string ProdType { get { return _productType; } }
        public string Prop1 { get { return (_mode == 0) ? SearchByAuto.Factory : SearchByAuto.Replacement; } }
        public string BoltsSize { get { return (_isbolts == false) ? SearchByAuto.Nut + " " + _bolts : SearchByAuto.Bolt + " " + _bolts; } }
        public bool IsBolts { get { return _isbolts; } }
        public string Size1 { get { return (_bolts != null && _bolts.Contains('*')) ? _bolts.Split('*')[0].Replace(',', '.') : String.Empty; } }
        public string Size2 { get { return (_bolts != null && _bolts.Contains('*')) ? _bolts.Split('*')[1].Replace(',', '.') : String.Empty; } }

        /// <summary>
        /// Creates link for tiporazmer
        /// </summary>
        public string HRef
        {
            get
            {

                var tag = new TagBuilder("a");
                tag.AddCssClass("a_text");
                tag.InnerHtml = Tiporazmer;

                if (ProductType == ProductType.Tyre) tag.MergeAttribute("href", String.Format(CultureInfo.InvariantCulture, "/Home/Tyres/all/all/{0}-{1}-R{2}", _width, _profile, _diameter));
                else tag.MergeAttribute("href", String.Format(CultureInfo.InvariantCulture, "/Home/Disks/all/{0}x{1}_{2}x{3}_ET{4}_D{5}/{6}", _width, _diameter, _pcdc, _pcdd, _et, _dia, Mode == 1 ? "?zamena=1" : ""));

                return tag.ToString();
            }

        }

        public string HRefRear
        {
            get
            {
                if (_rearprofile == null && _rearet == null)
                {
                    return String.Empty;
                }
                else
                {
                    var tag = new TagBuilder("a");
                    
                    tag.AddCssClass("a_text");
                    tag.InnerHtml = TiporazmerRear;

                    if (ProductType == ProductType.Tyre) tag.MergeAttribute("href", String.Format(CultureInfo.InvariantCulture, "/Home/Tyres/all/all/{0}-{1}-R{2}", _rearwidth, _rearprofile, _reardiameter));
                    else tag.MergeAttribute("href", String.Format(CultureInfo.InvariantCulture, "/Home/Disks/all/{0}x{1}_{2}x{3}_ET{4}_D{5}/{6}", _rearwidth, _reardiameter, _pcdc, _pcdd, _rearet, _dia, Mode == 1 ? "?zamena=1" : ""));


                    return tag.ToString();
                }

            }

        }
        public string Spark
        {
            get
            {
                if (_rearprofile == null && _rearet == null)
                {
                    return String.Empty;
                }
                else
                {
                    var tag = new TagBuilder("a");
                    tag.AddCssClass("a_text a_spark glyphicon glyphicon-refresh");
                    tag.InnerHtml = " ";
                    tag.MergeAttribute("title", "спарка");
                    if (ProductType == ProductType.Tyre)
                    {
                        tag.MergeAttribute("href", String.Format(CultureInfo.InvariantCulture, "/Home/SparTyres/{0}/{1}/{2}/{3}/{4}/{5}", _width, _rearwidth, _profile, _rearprofile, _diameter, _reardiameter));

                    }
                    else
                    {
                        tag.MergeAttribute("href", String.Format(CultureInfo.InvariantCulture, "/Home/SparDisks/{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}/", _width, _rearwidth, _diameter, _reardiameter, _pcdc, _pcdc, _pcdd, _pcdd, _et, _rearet, _dia, _dia));
                    }

                    return tag.ToString();
                }

            }

        }
        public int Mode { get { return _mode; } }

    }

    /// <summary>
    /// Items in order dto
    /// </summary>
    public class ItemInOrderViewModel
    {
        public int ProductId { get; set; }
        public int Count { get; set; }

        public string ProductIdTo7Simbols => ProductId.ProductIdTo7Symbols();
        
    }

    /// <summary>
    /// Change order via post
    /// </summary>
    public class ChangeOrderViewModel
    {
        public IEnumerable<ItemInOrderViewModel> Items { get; set; }
        public Guid OrderGuid { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool IsReserve { get; set; }
        public string Comments { get; set; }
        public string RegionId { get; set; }
        public bool IsDeliveryByTk { get; set; }
        public string ContactFIOOfClient { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessageResourceType = typeof(ShoppingCartErrors), ErrorMessageResourceName = "IncorrectPhoneNumber")]
        //@"((8|\+7)-?)?\(?\d{3}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}
        public string PhoneNumberOfClient { get; set; }
        // строковое представление города
        public string City { get; set; }

        // Кладр города
        public string CityId { get; set; }

        // Радио если Terminal=true, Address=false
        public bool TerminalOrAddress { get; set; }

        public string TerminalsDpd { get; set; }
        // адрес строкой
        public string Address { get; set; }

        public decimal CostOfDelivery { get; set; }

        public string PostalCode { get; set; }

        public string StreetType { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        public string BlockType { get; set; }

        // способ доставки 1- самовывоз, 0- доставка наша, 2 - доставка dpd
        public int WayOfDelivery { get; set; }
        
        // идентификатор 1С адреса доставки ( не dpd)
        public string AddressId { get; set; }

        public string TkId { get; set; }

        public bool IsStar { get; set; }
    }


    
    /// для сериализации в api
    [DataContract(Namespace ="")]

    /// <summary>
    /// Base class for search results of all types
    /// </summary>
    public class SearchResult
    {

        [DataMember]
        public string ProducerName { get; set; }

        [DataMember]
        public string Article { get; set; }

        [DataMember]
        public int ProductId { get; set; }

       [DataMember]
        public int DepartmentId { get; set; }

        [DataMember]
        public int DaysToDepartment { get; set; }

       [DataMember]
        public string ProductType { get; set; }

        public string ProductIdTo7Simbols => ProductId.ProductIdTo7Symbols();


        // наименование товара
        [DataMember]
        public string Name { get; set; }

        // остаток
        [DataMember]
        public int Rest { get; set; }


        // остаток на другом складе (если склад не привязан, то 0 )
      //  [DataMember]
        public int RestOtherStock { get; set; }

        [Range(0, 99)]
        [StringLength(4)]
        
        public virtual int DefaultNumberToOrder => Defaults.PodborsSettings.DefaultnumberToOrder;
        
        
        public int OrderCount { get; set; }

        [DataMember]
        [DisplayName("Цена входная")]
        public decimal? Price { get; set; }

        [DataMember]
        [DisplayName("Цена клиента")]
        public decimal? PriceOfClient
        { get; set; }

        [DisplayName("Сезон")]
        public string Season { get; set; }

        [DisplayName("Модель")]
        public string ModelName { get; set; }


        public int? ModelId { get; set; }

        public int? ParentId { get; set; }

        // сторонний поставщик 
        public int? SupplierId { get; set; }


        public decimal? Weight { get; set; }

        public decimal? Volume { get; set; }

        public int CountPhoto { get; set; }

        public virtual string PathToImage => ProductId.ProductIdTo7Symbols();
        
    }


    /// <summary>
    /// Search result for tyres
    /// </summary>

    public class TyreSearchResult : SearchResult
    {
        public int? ProducerId { get; set; }
        public string Width { get; set; }
        public string Diametr { get; set; }
        public string Height { get; set; }

        public override int DefaultNumberToOrder
        {
            get
            {
                return 4;
            }
        }
        public override string PathToImage
        {
            get
            {
                 return (ModelId.HasValue ? ModelId.ToString().PadLeft(5, '0') : String.Empty);
            }
        }
    
    }

    /// <summary>
    /// Search result for disks
    /// </summary>
    [DataContract(Namespace = "", Name = "DiskSearchResult")]
    public class DiskSearchResult :SearchResult
    {
        [DataMember]
        public int? ProducerId { get; set; }

       
        public string Width { get; set; }

        public string PCD { get; set; }

        public string ET { get; set; }

        public string DIA { get; set; }

        public string Holes { get; set; }

        public string Diametr { get; set; }

        public string DiskColor { get; set; }

        public DiskSearchResult()
        {
            Factory = String.Empty;
        }
        [DataMember]
        public bool IsSaleProduct
        {
            get;
            set;
        }

        public override int DefaultNumberToOrder => 4;
        
        public string Factory { get; set; }

        [DataMember]
        public override string PathToImage =>    PictureUtility.GetPictureOfThumbnailForDisk(this.ProducerId, this.ModelName, this.Name, this.ProductId);


        [DataMember]
        public WheelType WheelType { get; set; }
    }

      
      
   
   

    /// <summary>
    /// Sorting inside selections
    /// </summary>
    /// 
    public enum SortBy {
    NameAsc,
    NameDesc,
    PriceAsc,
    PriceDesc,
    AmountAsc,
    AmountDesc,
    DeliveryAsc,
    DeliveryDesc
    }

    /// <summary>
    /// Displaying by list or cells
    /// </summary>
    public enum Display
    {
       Table,
       Plitka
    }


    
        /// <summary>
        /// Общая модель подбора для всех товаров
        /// </summary>
    public class CommonPodborView
    {
        private int _itemsPerPage = Defaults.MaxItemsPerPage;

        public CommonPodborView()
        {
            Page = 1;           
            PriceMax = 70000;
          
        }
        public IEnumerable<Producer> Producers { get; set; }

        public int? DepartmentId { get; set; }

        public DepartmentInfo BackDepartmentWithInfo { get; set; }
        
        // наименование
        public string Name { get; set; }

        // артикул производителя
        public string Article { get; set; }


        public int? ProducerId { get; set; }


        
        public int Page { get; set; }

        [DisplayName("Выводить по")]        
        public int ItemsPerPage { get => _itemsPerPage;
            
                  set => this._itemsPerPage = value > Defaults.MaxItemsPerPage ? Defaults.MaxItemsPerPage : value;
            } 

        public int PriceMin { get; set; }
        public int PriceMax { get; set; }


        // результаты поиска

        public IPagedList<SearchResult> SearchResults { get; set; }

        //public PagingInfo PagingInfo { get; set; }

        public IEnumerable<int> PagerList
        {
            get { return new int[] { 10, 20, 50, 100 }; }
        }

        //public string SortBy { get; set; }

        public  SortBy SortBy { get; set; }

        public Display DisplayView { get; set; }

        public bool OnlySale { get; set; }

        public virtual void FillFromTiporazmer(string tiporazmer) { }
    }


    public enum TyresSeasons {
        [MultiCultureDescription(typeof(ForSearchResult), "Winter")]
        winter,
        [MultiCultureDescription(typeof(ForSearchResult), "Summer")]
        summer,
        [MultiCultureDescription(typeof(ForSearchResult), "All")]
        allseason
    }
    /// <summary>
    /// Модель подбора для шин
    /// </summary>
    public class TyresPodborView : CommonPodborView
    {

        private static  SelectList GetSeasons()
        {
            var nvc = new NameValueCollection { { "winter", ForSearchResult.Winter }, 
            { "summer", ForSearchResult.Summer },
            { "allseason", ForSearchResult.Allseason } };
            return new SelectList(nvc.AllKeys.SelectMany(nvc.GetValues, (x, y) => new { SeasonId = x, SeasonName = y }).ToList(), "SeasonId", "SeasonName");

        }
    
   
        private static readonly SelectList _seasons = GetSeasons();



        public string Width { get; set; }

        
        public string Height { get; set; }

        
        public string Diametr { get; set; }

       
        public string SeasonId { get; set; }

        public ShipForTyresPodbor Ship { get; set; }

        public IEnumerable<string> Widths
        {
            get
            {
                for (int i = 135; i <= 325; i = i + 10) yield return i.ToString();
            }

        }

        public IEnumerable<string> Heights { get; set; }

     
        public IEnumerable<string> Diametrs
        {
            get
            {
                for (int i = 12; i <= 22; i++)
                {
                    yield return i.ToString();
                    if (i == 12 || i == 13 || i == 14 || i == 15 || i == 16)
                        yield return String.Format("{0}C", i);
                }
            }
        }

       

        public SelectList Seasons { get { return _seasons; } }
        public bool IsSet4Items { get; set; }

        /// <summary>
        /// Заполнить параметры из типоразмера
        /// </summary>
        /// <param name="tiporazmer"></param>
        public override void FillFromTiporazmer(string tiporazmer)
        {
            var pattern =Defaults.PodborsSettings.TyrePattern;
            var parameters = RegexExtractStringProvider.GetParametersFromTiporazmer(tiporazmer, pattern);
            this.Width = parameters[0];
            this.Height = parameters[1];
            this.Diametr = parameters[2];
            
        }
    }

    /// <summary>
    /// Модель подбора для грузовых шин
    /// </summary>
    public class CargoTyresPodborView : CommonPodborView
    {
      
        public IEnumerable<string> CargoWidth { get; set; }

        public IEnumerable<string> CargoHeight { get; set; }

        public IEnumerable<string> CargoDiametr { get; set; }

        [StringLength(10)]
        [DisplayName("Ширина")]
        public string Width { get; set; }

        [StringLength(10)]
        [DisplayName("Высота")]
        public string Height { get; set; }

        [StringLength(10)]
        [DisplayName("Диаметр")]
        public string Diametr { get; set; }

        [StringLength(10)]
        public string WidthAnalog { get; set; }
    }

    /// <summary>
    /// Модель подбора для дисков
    /// </summary>
    public class DisksPodborView : CommonPodborView
    {

        public bool FromRests { get; set; }
        public bool FromOnWay { get; set; }

        public bool CargoWheels { get; set; }

        public int RestOrOnWay
        {
            get
            {   // typeOfRests =1 (only rests), 2 (only onway), 3 (rests + onway)
                return  Convert.ToInt16(FromRests) + 2 * Convert.ToInt16(FromOnWay);
            }
        
        }

        public string Brands { get; set; }

        public bool AllOrByCarReplica { get; set; }
        public string CarName { get; set; }

        [StringLength(10)]
        [DisplayName("Ширина")]
        public string Width { get; set; }

        [StringLength(10)]
        [DisplayName("PCD")]
        public string PCD { get; set; }

        [StringLength(10)]
        [DisplayName("ET")]
        public string ET { get; set; }

        [StringLength(10)]
        [DisplayName("ET to")]
        public string ETto { get; set; }

        [StringLength(10)]
        [DisplayName("DIA")]
        public string DIA { get; set; }

        //[StringLength(10)]
        [DisplayName("Hole")]
        public int? Hole { get; set; }

        [StringLength(10)]
        [DisplayName("Диаметр")]
        public string Diametr { get; set; }

        [StringLength(20)]
        [DisplayName("Цвет диска")]
        public string DiskColor { get; set; }

        public bool IsSet4Items { get; set; }

        public IEnumerable<string> Widths
        { get; set; }

        public IEnumerable<string> Pcds
        { get; set; }

        public IEnumerable<string> Ets
        { get; set; }

        public IEnumerable<string> Dias
        { get; set; }

        public IEnumerable<int> Holes
        {
            get { return (new int[] { 4, 5, 6, 8, 10 }); }
        }

        public IEnumerable<string> Diametrs
        { get; set; }


        public IEnumerable<string> DiskColors
        {
            get;
            set;
        }

        public IEnumerable<ReplicaDisksForCars> CarsList
        { get; set; }

       public override  void FillFromTiporazmer(string tiporazmer )
        {
            var parameters = RegexExtractStringProvider.GetParametersFromTiporazmer(tiporazmer,Defaults.PodborsSettings.DiskPattern);
            this.Width = parameters[0];
            this.Diametr = parameters[1];
            this.Hole =Int32.Parse(parameters[2]);
            this.PCD = parameters[3];
            this.ET = parameters[4];
            this.DIA = parameters[5];

        }
        

    }

    /// <summary>
    /// Модель подбора для других товаров
    /// </summary>
    public class OthersPodborView : CommonPodborView
    {

        public OthersPodborView()
        {
            ItemsPerPage = 500;
        }

        public IEnumerable<Department> Departments
        { get; set; }

        public OthersProductType OthersType { get; set; }

        public IEnumerable<ProductCategory> ProductFolders
        { get; set; }

        public int? Folder { get; set; }
        public int DaysToDepartment { get; set; }

        public bool FromRests { get; set; }
        public bool FromOnWay { get; set; }

        public int RestOrOnWay
        {
            get
            { // typeOfRests =1 (only rests), 2 (only onway), 3 (rests + onway)
                return Convert.ToInt16(FromRests) + 2 * Convert.ToInt16(FromOnWay);
            }

        }

        public IEnumerable<string> Diametrs
        { get; set; }
        public IEnumerable<string> Widths
        { get; set; }

        [StringLength(10)]
        [DisplayName("Диаметр")]
        public string Diametr { get; set; }
        [StringLength(10)]
        [DisplayName("Ширина")]
        public string Width { get; set; }
    }


    /// <summary>
    /// Прочие товары делим на камеры и крепёж
    /// </summary>

    public enum OthersProductType
    {
        Bolts = 0,
        Cams = 1
    }
    public enum RestsOrOnWay
    {
        Rest,
        OnWay,
        RestAndOnWay
    }

    /// <summary>
    /// Расширенная модель подбора для дисков
    /// </summary>
    public class SeasonDisksPodborView : DisksPodborView
    {
                
        public WheelType WheelType { get; set; }
        public bool HasSteelOffers { get; set; }
        public bool HasAlloyOffers { get; set; }
        public bool IsForeign { get; set; }
        [DefaultValue(false)]
        public bool OnProduction { get; set; }

    }
    
    /// <summary>
    /// Модель подбора для АКБ
    /// </summary>

    public class AkbPodborView : CommonPodborView
    {
        public IEnumerable<string> Volumes { get; set; }

        public IEnumerable<string> InrushCurrents { get; set; }

        public IEnumerable<string> Polarities { get; set; }

        public IEnumerable<string> Brands { get; set; }

        public IEnumerable<string> Sizes { get; set; }

        public new IEnumerable<string> Producers { get; set; }

        public IEnumerable<string> AkbTypes { get; set; }
        
        [DisplayName("Пусковой ток")]
        public string Inrush_Current { get; set; }

        [DisplayName(@"Емкость")]
        public string Volume { get; set; }

        [DisplayName("Полярность")]
        public string Polarity { get; set; }

        [DisplayName("Brand")]
        public string[] Brand { get; set; }

        [DisplayName("Sizes")]
        public string Size { get; set; }
        
        public string Producer { get; set; }
        
        public string AkbType { get; set; }

    //    public bool OnlySale { get; set; }

    }

    /// <summary>
    /// Модель подбора для аксессуаров
    /// </summary>
    public class AccPodborView : CommonPodborView
    {

        public AccPodborView()
        {
            SelectedCategories = new HashSet<string>();
            SelectedFolders = new HashSet<string>();
        }

        public HashSet<string> SelectedCategories { get; private set; }
        public HashSet<string> SelectedFolders { get; private set; }

      //  public  bool OnlySale { get; set; }

    }

    /// <summary>
    /// Модель для детальной страницы товара /Home/Details/
    /// </summary>
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public int DepId { get; set; }

        public int Days { get; set; }

        public Product Product { get; set; }
        
        ICollection<SearchResult> Analogs { get; set; }

        public IEnumerable<ProductPropertyValue> ProductProperties { get; set; }

        private IEnumerable<DepartmentWithRests> DepartmentWithRests { get; set; }

        public Dictionary<string, string> CarsOfProducts { get; set; }

    }

    /// <summary>
    /// Модель подбора дисков по авто
    /// </summary>
    public class PodborAutoView
    {
               
       
         public string brand { get; set; }

        public string model { get; set; }

        public int year { get; set; }

        public string engine { get; set; }

        public IEnumerable<string> Brands { get; set; }

        public IEnumerable<string> Models { get; set; }

        public IEnumerable<int> Years { get; set; }

        public IEnumerable<string> Engines { get; set; }

        public bool IsForeign { get; set; }

        public string SaleMode { get; set; }

        /// <summary>
        /// Максимальное число типоразмеров м-у шинами и дисками для отображения в таблице
        /// </summary>
        public int MaxLength { get; set; }

        public IList<TiporazmerByCarModelView> TyreTiporazmersResults { get; set; }
        public IList<TiporazmerByCarModelView> DiskTiporazmersResults { get; set; }

    }


    public class PartnerPointView
    {
        [StringLength(100)]
        public string ContactFIO { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

    }



    public class PartnerPriceRuleDTO
    {
        public int ProducerId { get; set; }
        public string Name { get; set; }
        public string PriceType { get; set; }
        public decimal Discount { get; set; }
        public PriceTypeEnum PType { get; set; }

    }


    public class PointSettingsContainer
    {

        public IEnumerable<PartnerPriceRuleDTO> pricingrules { get; set; }
        public int PointId { get; set; }
        public string ContactFIO { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string KeyWord { get; set; }
        public string AddressForDelivery { get; set; }
        public int DaysToMainDepartment { get; set; }

        public int? DepartmentId { get; set; }

        public int? DaysToDepartment { get; set; }

        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }

        public string WebSite { get; set; }
        public string Email { get; set; }
        public bool ConditionsAreAccepted { get; set; }
        public byte SaleDirection { get; set; }

        public int CustomDutyVal { get; set; }
        public int VatVal { get; set; }
        public string LatLng { get; set; }
        public bool ShowOnlySeasonOrder { get; set; }
    }





    public class PartnerPointDTOForList
    {

        public int PartnerPointId { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get { return String.Concat("Point", PartnerPointId.ToString()); } } // equals username 


        public string PartnerId { get; set; }

        public string ContactFIO { get; set; }

        public string CompanyName { get; set; }

        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        public string InternalName { get; set; }

        public bool IsLocked { get; set; }
    }

    # region partnerPointDTO
    public class PartnerPointBase
    {
        public static Department[] _departments = (new Department[] { new Department { DepartmentId = -1, Name = Settings.NotSelected } }).Union(DropDownsFactory.Departments).ToArray();

        public PartnerPointBase()
        {
            PricingRulesDisks = new List<PartnerPriceRuleDTO>();
            PricingRulesTyres = new List<PartnerPriceRuleDTO>();
            PricingRulesBat = new List<PartnerPriceRuleDTO>();
            PricingRulesAcc = new List<PartnerPriceRuleDTO>();

        }

        public string UserName { get; set; }

        /* [Display(Name = "Число дней от центрального склада")]*/
        [Range(1, 45)]
        [Required(ErrorMessageResourceName = "ErrorDays", ErrorMessageResourceType = typeof(Settings))]
        [Display(Name = "DeliveryFromCentralStock", ResourceType = typeof(Settings))]
        public int DaysToMainDepartment { get; set; }

        [Display(Name = "AdditionalStock", ResourceType = typeof(Settings))]
        public int? DepartmentId { get; set; }

        /*[Display(Name = "Дней до дополнительного склада")]
        [Range(1, 10, ErrorMessage = "Число дней должно быть между 1 и 10.")]*/
        [Range(1, 45)]
        //[Required(ErrorMessageResourceName = "ErrorDays", ErrorMessageResourceType = typeof(Settings))]
        [Display(Name = "DeliveryFromAddStock", ResourceType = typeof(Settings))]
        public int? DaysToDepartment { get; set; }


        public IEnumerable<Department> Departments
        {
            get { return _departments; }
        }
       
        public IList<PartnerPriceRuleDTO> PricingRulesDisks;
        public IList<PartnerPriceRuleDTO> PricingRulesTyres;
        public IList<PartnerPriceRuleDTO> PricingRulesBat;
        public IList<PartnerPriceRuleDTO> PricingRulesAcc;
    }

    /*
    public class PartnerPointDTOCreateOwn : PartnerPointBase
    {
        private static readonly SelectList _countries = DropDownsFactory.Countries;

        public SelectList Countries
        {
            get { return _countries; }
        }

        [DisplayName("Страна")]
        [Required(ErrorMessage = "Заполните страну")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Заполните Фамилия Имя Отчество")]
        [RegularExpression(@"^[А-Я]+[ а-яА-Я-_]*$", ErrorMessage = "Введите корректные ФИО")] 
        public string ContactFIO { get; set; }

        [Required(ErrorMessage = "Заполните телефон")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Заполните адрес")]
        public string Address { get; set; }

        [DisplayName("Имя компании")]
        [Required(ErrorMessage = "Заполните название вашей компании")]
        public string CompanyName { get; set; }

        [DisplayName("Направление продаж")]
        public byte SaleDirection { get; set; }

        public bool IsOpt { get { return (SaleDirection & (1 << 1 - 1)) != 0; } }

        public bool IsRetail { get { return (SaleDirection & (1 << 2 - 1)) != 0; } }

        public bool IsInetShop { get { return (SaleDirection & (1 << 3 - 1)) != 0; } }

        public bool IsEndCustomer { get { return (SaleDirection & (1 << 4 - 1)) != 0; } }

       
    }*/

    public class PartnerPointDTOCreate : PartnerPointBase
    {
        public string ContactFIO { get; set; }
    }

    public class PartnerPointDTO : PartnerPointBase
    {
        private static Countries[] _countries = DropDownsFactory.Countries;

        private static Languages[] _languages = DropDownsFactory.Languages;

        public int PartnerPointId { get; set; }

        public string Password { get; set; }
        public string KeyWord { get; set; }

        [Required(ErrorMessageResourceName = "FillInName", ErrorMessageResourceType = typeof(Settings))]
        public string ContactFIO { get; set; }

        [Required(ErrorMessageResourceName = "FillInPhone", ErrorMessageResourceType = typeof(Settings))]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceName = "FillInAddress", ErrorMessageResourceType = typeof(Settings))]
        public string Address { get; set; }


        public IEnumerable<Countries> Countries
        {
            get { return _countries; }
        }

        public IEnumerable<Languages> Languages
        {
            get { return _languages; }
        }

        public string Language { get; set; }

        
        public string WebSite { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Заполните адрес электронной почты")]
        public string Email { get; set; }

        public string InternalName { get; set; }

        public string AddressForDelivery { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Settings))]
        [Required(ErrorMessage = "Заполните страну")]
        public string Country { get; set; }


        [Display(Name = "CompName", ResourceType = typeof(Settings))]
        [Required(ErrorMessageResourceName = "FillInCompName", ErrorMessageResourceType = typeof(Settings))]
        public string CompanyName { get; set; }

        public string LatLng { get; set; }

        [DisplayName("Направление продаж")]
        public byte SaleDirection { get; set; }

        public bool IsOpt { get { return (SaleDirection & (1 << 1 - 1)) != 0; } }

        public bool IsRetail { get { return (SaleDirection & (1 << 2 - 1)) != 0; } }

        public bool IsInetShop { get { return (SaleDirection & (1 << 3 - 1)) != 0; } }

        public bool IsEndCustomer { get { return (SaleDirection & (1 << 4 - 1)) != 0; } }

        [DefaultValue(0)]
        public int CustomDutyVal { get; set; }

        [DefaultValue(0)]
        public int VatVal { get; set; }

        [DefaultValue(false)]
        public bool IsForeignAndMainTerminal { get; set; }

        [DefaultValue(false)]
        public bool ShowOnlySeasonOrder { get; set; }
    }

    #endregion

    [Table("FeedbackFormDbSet")]
    public class FeedbackForm
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(FeedbackForms))]
        [Required]
        
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(FeedbackForms))]
        public string Email { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Phone", ResourceType = typeof(FeedbackForms))]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Message", ResourceType = typeof(FeedbackForms))]
        public string Message { get; set; }
    }

    /// <summary>
    /// Search result for akb
    /// </summary>
    public class AkbSearchResult : SearchResult
    {
        public AkbSearchResult()
        {
            this.PropertyChanged += AkbSearchResult_PropertyChanged;
        }

        private void AkbSearchResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Size == null) return;
            Match m = Regex.Match(this.Size, @"(\d+)x(\d+)x(\d+)", RegexOptions.IgnoreCase);

            if (m.Length > 0 && m.Groups.Count > 1)
            {
                try
                {
                    _length = Int32.Parse(m.Groups[1].ToString());
                    _width = Int32.Parse(m.Groups[2].ToString());
                    _height = Int32.Parse(m.Groups[3].ToString());
                }
                finally { }

            }
        }

        private int _length = 0, _width = 0, _height = 0;
        private string _size;
        private static string _directconnection = "п.п.";
        private static string _reverseconnection = "о.п.";
        public override string PathToImage  
        {
            get
            {
                //return ProductId.ProductIdTo7Symbols;
                return ProductId.ToString().PadLeft(7, '0');
            }
        }


        public string Brand { get; set; }
        public string Size { get { return _size; } set { _size = value; RaisePropChanged("size"); } }


       // public string Volume { get; set; }

        public string Capacity { get; set; }

        public string Polarity { get; set; }
        public string Inrush_Current { get; set; }

        public bool IsSaleProduct {get;set;}


        public int Length { get { return _length; } }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public bool? Connection
        {
            get
            {
                if (Name.Contains(_directconnection)) return true;
                if (Name.Contains(_reverseconnection)) return false;
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }


    }

    public class AccSearchResult : SearchResult
    {
        public bool IsSaleProduct { get; set; }
        public int? ProducerId { get; set; }
    }

    public struct ProductCategory
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int CountOfProducts { get; set; }
        public int? ParentId { get; set; }
    }

    /// <summary>
    /// В структуру передаются значения для отображения цен для брендовых прайсов
    /// </summary>

    [DataContract(Namespace = "")]
    public class ProductForServiceDto
    {
        [DataMember(Order = 1)]
        public int ProductId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public double PriceReccOpt { get; set; }

        [DataMember(Order = 4)]
        public double PriceReccRozn { get; set; }
     //   public int Rest { get; set; }
      
    }


    public class NewsViewModel
    {
        public long Id { get; set; }
        public string NewsName { get; set; }
        public string NewsText { get; set; }
        public string ContentType { get; set; }
        public byte[] MainImg { get; set; }
        public byte[] PreviewImg { get; set; }
        public string Culture { get; set; }
        public bool Active { get; set; }
        public DateTime DatePublish { get; set; }
        private static Languages[] _languages = DropDownsFactory.Languages;
        public IEnumerable<NewsViewModel> News { get; set; }
        public IEnumerable<Languages> Languages
        {
            get { return _languages; }
        }
    }


    /// <summary>
    /// Условия шиповки в подборе шин
    /// </summary>
    public enum ShipForTyresPodbor
    {
        // все
        ShipAll=0,

        // шипованные
        ShipShip,

        // неошипованные но шупуемые
        ShipNoShip,

        // нешипуемые (фрикционные)
        Friction
    }

    public class SparPodborView
    {
        public string Width { get; set; }

        public string Height { get; set; }

        public string PCD { get; set; }

        public string ET { get; set; }

        public string DIA { get; set; }

        public string Holes { get; set; }

        public string Diametr { get; set; }

        public string RearWidth { get; set; }

        public string RearHeight { get; set; }

        public string RearPCD { get; set; }

        public string RearET { get; set; }

        public string RearDIA { get; set; }

        public string RearHoles { get; set; }

        public string RearDiametr { get; set; }
    }

    public class PhotoViewModel
    {
        public string NamePhoto { get; set; }
        public int NumberPhoto { get; set; }
        public string ContentType { get; set; }
        public byte[] Photo { get; set; }
    }
    /*
     *  Модель для отображения менеджеров
     */
    public class ManagerViewModel
    {
        public ManagerViewModel()
        {
            Assistants = new List<Manager>();
        }
        public Manager Manager { get; set; }

        public Manager Operator { get; set; }

        public IList<Manager> Assistants { get; set; }
    }
}