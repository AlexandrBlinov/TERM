using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using DocumentFormat.OpenXml.Spreadsheet;
using Term.Utils;
using Term.Web.Views.Resources;
using Term.Web.HtmlHelpers;
using Yst.Services;


namespace YstProject.Services
{
    /// <summary>
    /// Модель для отображения дисков в xml
    /// </summary>
    [DataContract(Namespace = "", Name = "disk")]
    public class PriceListDiskXml
            {

        static readonly string[] _arrayOfForgedProducers = { "harp", "buffalo", "vissol" };
        static readonly string _toOrder60days = new ResourceManager(typeof(SeasonOrdersTexts)).GetString("ToOrder60Days");

        [Display(Order = 1)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        [DataMember(Order = 1, Name = "code")]
        public int ProductId { get; set; }

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("ProducerName")]
        [DataMember(Order = 2, Name = "brand")]
        public string ProducerName { get; set; }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("Model")]
        [DataMember(Order = 3, Name = "model")]
        public string ModelName { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("ProductName")]
        [DataMember(Order = 4, Name = "name")]
        public string Name { get; set; }

        [Display(Order = 5)]
        [LocalizedDisplayNameAttribute("Color")]
        [DataMember(Order = 5, Name = "color")]
        public string Color {
            get {               
                    return StringUtils.GetColourFromName(Name);                
            } 

            set { 
            
            } 
        }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("Width")]
        [DataMember(Order = 6, Name = "width")]
        public string Width { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("Diameter")]
        [DataMember(Order = 7, Name = "diametr")]
        public string Diametr { get; set; }

        [Display(Order = 8)]
        [DataMember(Order = 8, Name = "bolts_count")]
        public string Holes { get; set; }

        [Display(Order = 9)]
        [DataMember(Order = 9, Name = "bolts_spacing")]
        public string PCD { get; set; }

        [Display(Order = 10)]
        [DataMember(Order = 10, Name = "et")]
        public string ET { get; set; }

        [Display(Order = 11)]
        [DataMember(Order = 11, Name = "dia")]
        public string DIA { get; set; }

        [Display(Order = 19)]
        [LocalizedDisplayNameAttribute("Price")]
        [DataMember(Order = 12, Name = "price")]
        public decimal? Price { get; set; }

        [DataMember(Order = 13, Name = "price_recomend_opt")]
        public decimal? Price1 { get; set; }

        [Display(Order = 20)]
        [LocalizedDisplayNameAttribute("PriceRecRozn")]
        [DataMember(Order = 14, Name = "price_recomend_rozn")]
        public decimal? Price2 { get; set; }

        [Display(Order = 12)]
        [LocalizedDisplayNameAttribute("Weight")]
        [DataMember(Order = 15, Name = "weight")]
        public decimal? Weight { get; set; }

        [Display(Order = 13)]
        [LocalizedDisplayNameAttribute("Volume")]
        [DataMember(Order = 16, Name = "volume")]
        public decimal? Volume { get; set; }

        [Display(Order = 21)]
        [DataMember(Order = 17, Name = "picture", EmitDefaultValue = false)]
        public string PathToRemotePicture
        { get; set; }

        
        [Display(Order = 14)]
        [LocalizedDisplayNameAttribute("RestYar")]
        [DataMember(Order = 18, Name = "restyar")]
        public int RestMain { get; set; }

       // public string RestMain { get { return _restMain; } set { _restMain = value.ToString(); } }

        [Display(Order = 15)]
        [LocalizedDisplayNameAttribute("RestSpb")]
        [DataMember(Order = 19, Name = "restspb")]
        public int RestSpb { get; set; }

        [Display(Order = 16)]
        [LocalizedDisplayNameAttribute("RestEkb")]
        [DataMember(Order = 20, Name = "restekb")]
        public int RestEkb { get; set; }

        [Display(Order = 17)]
        [LocalizedDisplayNameAttribute("RestRnd")]
        [DataMember(Order = 21, Name = "restrnd")]
        public int RestRnd { get; set; }

        [Display(Order = 18)]
        [LocalizedDisplayNameAttribute("RestMsk")]
        [DataMember(Order = 22, Name = "restmsk")]
        public int RestMsk { get; set; }

        [Display(Order = 22)]
        [LocalizedDisplayNameAttribute("Comment")]
        [DataMember(Order = 23, Name = "comments")]
        public string Сomments { get {
               

            return ((ProducerName != null) && _arrayOfForgedProducers.Any(p => p == ProducerName.ToLower()) && 
                (RestMain + RestSpb + RestEkb + RestRnd + RestMsk)==0) ? _toOrder60days : String.Empty;
           
        
        } private set { } }

        [Display(Order = 25)]
        //[LocalizedDisplayNameAttribute("BarCode")]
        [DataMember(Order = 25, Name = "barcode", EmitDefaultValue = false)]
        public string Barcode { get; set; }


        [Display(Order = 26)]
        [LocalizedDisplayNameAttribute("Article")]
        [DataMember(Order = 26, Name = "article")]
        public string Article { get; set; }
       

    }



    /// <summary>
    /// Модель для отображения шин в xml
    /// </summary>
    [DataContract(Namespace = "", Name = "tyre")]
    public class PriceListTyreXml
    {
        private static readonly string _pattern = @"\b(\d+[\/\d+]?)([HNPQRSTUVWY])\b";
        private static readonly string _patternRF = @"\b(flat|zp|ssr|runonflat|rft|zps)\b";
        private static readonly string _ship = @"шип";
        private static readonly string _patternSL = @"\b(\d+)\s?сл";

        
        public int ProductId { get; set; }

        [Display(Order = 1)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        [DataMember(Order = 1, Name = "code")]
        public string ProductIdTo7S { get { return ProductId.ToString().PadLeft(7, '0'); } set { } }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("ParentName")]
        [DataMember(Order = 5, Name = "type")]
        public string ParentName { get; set; }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("Article")]
        [DataMember(Order = 2, Name = "article")]
        public string Article { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("ProductName")]
        [DataMember(Order = 3, Name = "name")]
        public string Name { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("ProducerName")]
        [DataMember(Order = 4, Name = "brand")]
        public string ProducerName { get; set; }

        [Display(Order = 5)]
        [LocalizedDisplayNameAttribute("Model")]
        [DataMember(Order = 6, Name = "model")]
        public string ModelName { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("Width")]
        [DataMember(Order = 7, Name = "width")]
        public string Width { get; set; }

        [Display(Order = 11)]
        [LocalizedDisplayNameAttribute("Height")]
        [DataMember(Order = 8, Name = "height",EmitDefaultValue = false)]
        public string Height { get; set; }

        [Display(Order = 12)]
        [LocalizedDisplayNameAttribute("Diameter")]
        [DataMember(Order = 9, Name = "diametr")]
        public string Diametr { get; set; }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("SpeedIndex")]
        [DataMember(Order = 10, Name = "speed_index")]
        public string SpeedIndex
        {
            get
            {

                Match m = Regex.Match(Name, _pattern);
                if (m.Length > 0 && m.Groups.Count > 0)
                    return m.Groups[m.Groups.Count - 1].ToString();
                return String.Empty;
            }
            set { }
        }

        [Display(Order = 9)]
        [LocalizedDisplayNameAttribute("LoadIndex")]
        [DataMember(Order = 11, Name = "load_index")]
        public string LoadIndex
        {
            get
            {
                Match m = Regex.Match(Name, _pattern);
                if (m.Length > 0 && m.Groups.Count > 1)
                    return m.Groups[m.Groups.Count - 2].ToString();
                return String.Empty;
            }
            set { }
        }

        [DataMember(Order = 12, Name = "thorn")]
        public byte Ship
        {
            get
            {
                return (byte)(Name.ToLower().Contains(_ship) ? 1 : 0);

            }
            set { }
        }

        [DataMember(Order = 13, Name = "runflat")]
        public byte RunFlat
        {
            get
            {
                return (byte)(Regex.IsMatch(Name.ToLower(), _patternRF)  ? 1 : 0);

            }
            set { }
        }

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("Season")]
        [DataMember(Order = 14, Name = "season")]
        public string Season { get; set; }

        [DataMember(Order = 15, Name = "sloy", EmitDefaultValue = false)]
        public int? Sloy
        {
            get
            {
                int result;
                Match m = Regex.Match(Name, _patternSL);
                if (m.Length > 0 && m.Groups.Count > 1 && Int32.TryParse(m.Groups[m.Groups.Count - 1].ToString(), out result))
                    return result;
                return null;

            }
            set { }
        }

        [Display(Order = 18)]
        [LocalizedDisplayNameAttribute("Price")]
        [DataMember(Order = 16, Name = "price")]
        public decimal? Price { get; set; }

        [Display(Order = 19)]
        [LocalizedDisplayNameAttribute("PriceRecRozn")]
        [DataMember(Order = 17, Name = "price_recomend_rozn",EmitDefaultValue =false)]
        public decimal? PriceIm { get; set; }
        

        [Display(Order = 33)]
        [LocalizedDisplayNameAttribute("PriceRecIm")]
        [DataMember(Order = 33, Name = "price_recomend_im",EmitDefaultValue = false)]
        public decimal? Price2 { get; set; }


        [Display(Order = 13)]
        [LocalizedDisplayNameAttribute("RestYar")]
        [DataMember(Order = 18, Name = "restyar")]
        public int RestMain { get; set; }

        [Display(Order = 14)]
        [LocalizedDisplayNameAttribute("RestSpb")]
        [DataMember(Order = 19, Name = "restspb")]
        public int RestSpb { get; set; }

        [Display(Order = 15)]
        [LocalizedDisplayNameAttribute("RestEkb")]
        [DataMember(Order = 20, Name = "restekb")]
        public int RestEkb { get; set; }

        [Display(Order = 16)]
        [LocalizedDisplayNameAttribute("RestRnd")]
        [DataMember(Order = 21, Name = "restrnd")]
        public int RestRnd { get; set; }

        [Display(Order = 17)]
        [LocalizedDisplayNameAttribute("RestMsk")]
        [DataMember(Order = 22, Name = "restmsk")]
        public int RestMsk { get; set; }

        public int? ModelId { get; set; }

        [Display(Order = 20)]
         [DataMember(Order = 23, Name = "picture", EmitDefaultValue = false)]
        public string PathToPicture
        {
            get { return PictureUtility.GetPictureOfTyre(ModelId); }

            set { }
        }

       
        [Display(Order = 25)]
        [LocalizedDisplayNameAttribute("Weight")]
        [DataMember(Order = 25, Name = "weight")]
        public decimal? Weight { get; set; }

        [Display(Order = 26)]
        [LocalizedDisplayNameAttribute("Volume")]
        [DataMember(Order = 26, Name = "volume")]
        public decimal? Volume { get; set; }


        [Display(Order = 27)]
        [LocalizedDisplayNameAttribute("RestOtherStock")]
        [DataMember(Order = 30, Name = "restotherstock")]
        public int RestOtherStock { get; set; }

        [Display(Order = 28)]
        [LocalizedDisplayNameAttribute("DaysOtherStock")]
        [DataMember(Order = 31, Name = "daysotherstock")]
        public int DaysOtherStock { get; set; }

    }

    /// <summary>
    /// Модель для отображения аксессуаров в xml
    /// </summary>

    [DataContract(Namespace = "", Name = "acc")]
    public class PriceListAccXml
    {
        [Display(Order = 1)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        [DataMember(Order = 1, Name = "code")]
        public int ProductId { get; set; }

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("Article")]
        [DataMember(Order = 2, Name = "article")]
        public string Article { get; set; }

        [Display(Order = 5)]
        [LocalizedDisplayNameAttribute("ProductName")]
        [DataMember(Order = 3, Name = "name")]
        public string Name { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("CategoryName")]
        [DataMember(Order = 4, Name = "group")]
        public string CategoryName { get; set; }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("ProducerName")]
        [DataMember(Order = 5, Name = "brand")]
        public string ProducerName { get; set; }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("Weight")]
        [DataMember(Order = 6, Name = "weight")]
        public decimal? Weight { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("Volume")]
        [DataMember(Order = 7, Name = "volume")]
        public decimal? Volume { get; set; }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("RestYar")]
        [DataMember(Order = 8, Name = "restyar")]
        public int RestMain { get; set; }

        [Display(Order = 9)]
        [LocalizedDisplayNameAttribute("RestSpb")]
        [DataMember(Order = 9, Name = "restspb")]
        public int RestSpb { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("RestEkb")]
        [DataMember(Order = 10, Name = "restekb")]
        public int RestEkb { get; set; }

        [Display(Order = 11)]
        [LocalizedDisplayNameAttribute("RestRnd")]
        [DataMember(Order = 11, Name = "restrnd")]
        public int RestRnd { get; set; }

        [Display(Order = 12)]
        [LocalizedDisplayNameAttribute("RestMsk")]
        [DataMember(Order = 12, Name = "restmsk")]
        public int RestMsk { get; set; }

        [Display(Order = 13)]
        [LocalizedDisplayNameAttribute("Price")]
        [DataMember(Order = 13, Name = "price")]
        public decimal? Price { get; set; }

        [Display(Order = 14)]
        [DataMember(Order = 14, Name = "picture")]
        public string PathToPicture
        {
            get
            {return PictureUtility.GetPictureOfAcc(ProductId); }
            set { }
        }

        [DataMember(Order = 15, Name = "barcode", EmitDefaultValue = false)]
        public string Barcode { get; set; }
    }

    [DataContract(Namespace = "", Name = "akb")]
    public class PriceListAkbXml
    {
        [Display(Order = 1)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        [DataMember(Order = 1, Name = "code")]
        public int ProductId { get; set; }

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("Article")]
        [DataMember(Order = 2, Name = "article")]
        public string Article { get; set; }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("ProducerName")]
        [DataMember(Order = 3, Name = "producername")]
        public string ProducerName { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("Brand")]
        [DataMember(Order = 4, Name = "brand")]
        public string Brand { get; set; }

        [Display(Order = 5)]
        [LocalizedDisplayNameAttribute("ProductName")]
        [DataMember(Order = 5, Name = "name")]
        public string Name { get; set; }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("Capacity")]
        [DataMember(Order = 6, Name = "Capacity")]
        public string Capacity { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("Polarity")]
        [DataMember(Order = 7, Name = "polarity")]
        public string Polarity { get; set; }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("CrankCurrent")]
        [DataMember(Order = 8, Name = "inrush_current")]
        public string Inrush_Current { get; set; }

        [Display(Order = 9)]
        [LocalizedDisplayNameAttribute("Dimensions")]
        [DataMember(Order = 9, Name = "size")]
        public string Size { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("Price")]
        [DataMember(Order = 10, Name = "price")]
        public decimal? Price { get; set; }

        [Display(Order = 11)]
        [LocalizedDisplayNameAttribute("RestYar")]
        [DataMember(Order = 11, Name = "restyar")]
        public int RestMain { get; set; }

        [Display(Order = 12)]
        [LocalizedDisplayNameAttribute("RestSpb")]
        [DataMember(Order = 12, Name = "restspb")]
        public int RestSpb { get; set; }

        [Display(Order = 13)]
        [LocalizedDisplayNameAttribute("RestEkb")]
        [DataMember(Order = 13, Name = "restekb")]
        public int RestEkb { get; set; }

        [Display(Order = 14)]
        [LocalizedDisplayNameAttribute("RestRnd")]
        [DataMember(Order = 14, Name = "restrnd")]
        public int RestRnd { get; set; }

        [Display(Order = 15)]
        [LocalizedDisplayNameAttribute("RestMsk")]
        [DataMember(Order = 15, Name = "restmsk")]
        public int RestMsk { get; set; }


        [Display(Order = 16)]
        [DataMember(Order = 16, Name = "picture")]
        public string PathToPicture
        {
            get
            {
            
                return PictureUtility.GetPictureOfAkb(ProductId); 

            }
            set { }
        }

        [Display(Order = 25)]
        [LocalizedDisplayNameAttribute("Weight")]
        [DataMember(Order = 25, Name = "weight")]
        public decimal? Weight { get; set; }

        [Display(Order = 26)]
        [LocalizedDisplayNameAttribute("Volume")]
        [DataMember(Order = 26, Name = "volume")]
        public decimal? Volume { get; set; }

       
    }
}