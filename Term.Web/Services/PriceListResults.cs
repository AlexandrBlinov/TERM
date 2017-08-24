using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Yst.Services;
using Term.DAL;
using Term.Utils;
using Term.Web.HtmlHelpers;

namespace YstProject.Services
{
  

    public  class PriceListTyreBase 
    {
        protected int _productId;
        protected string _season;
        
        [Display(Order = 2)]
        [DisplayName("Сезон")]
        public string Season
        {
            get
            {
                if (Defaults.SeasonNames.ContainsKey(_season))
                    return Defaults.SeasonNames[_season];
                else
                    return String.Empty;
            }
            set { _season = value; }
        }

       

        public int ProductId { set { _productId = value; } get { return _productId; } }
      //   [DisplayName("Код")]
      //    public string ProductId { get{return _productId.PadLeft(7, '0');} set { _productId = value as string; } }
        
        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        public string ProductIdTo7S { get { return _productId.ToString().PadLeft(7, '0'); } }

       [Display(Order = 5)]
       [LocalizedDisplayNameAttribute("ManufacturerCode")]
        public string Article { get; set; }

       

        /* [DisplayName("Типоразмер")]
         public string TiporazmerName { get; set; } */


       
        //   [DisplayName("Номер п.п.")]
  //      [Display(Order = 1)]
  //      public Int64 row_number { get; set; }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("Brand")]
        public string ProducerName { get; set; }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("ProductName")]
        public string Name { get; set; }


        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("Width")]
        public string Width { get; set; }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("Height")]
        public string Height { get; set; }
        [Display(Order = 9)]

        //  [DisplayName("Диаметр")]
        [StringLength(50)]
        public string Diametr { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("Amount")]
        public int Rest { get; set; }

        public int? ModelId { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("DeliveryTime")]
        public int DaysToDepartment { get; set; }

        [Display(Order = 15)]
        [DisplayName("URL")]
        public string PathToImage
        {
            get
            {
                return PictureUtility.GetPictureOfTyre(ModelId);
                /*  string url = ConfigurationManager.AppSettings["RemotePathToPictures"] + "tires/" + ModelId.ToString().PadLeft(5, '0') + ".png";
                return url; */
            }
        }

        [Display(Order = 25)]
        [LocalizedDisplayNameAttribute("Weight")]
        public decimal? Weight { get; set; }
        
        [Display(Order = 26)]
        [LocalizedDisplayNameAttribute("Volume")]
        public decimal? Volume { get; set; }
    }


    public class PriceListPartnerTyreResult : PriceListTyreBase
    {
   
   
        [Display(Order = 11)]
        [LocalizedDisplayNameAttribute("EntryPrice")]
        public decimal? Price
        { get; set; }

        [Display(Order = 12)]
        [LocalizedDisplayNameAttribute("RetailPrice")]
        public decimal? Price2
        { get; set; }

    }


    public class PriceListPointTyreResult : PriceListTyreBase
    {
        [Display(Order = 13)]
        [LocalizedDisplayNameAttribute("EntryPrice")]
        public decimal? PriceOfPoint
        { get; set; }


        [Display(Order = 14)]
        [LocalizedDisplayNameAttribute("RetailPrice")]
        public decimal? Price2
        { get; set; }

    }


    public class PriceListAccResult 
    {
       // private int _productId;

     //   [Display(Order = 1)]
     //   public Int64 row_number { get; set; }

      //  public int ProductId { set { _productId = value; } get { return _productId; } }
        public int ProductId { get; set; }

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        public string ProductIdTo7S { get { return ProductId.ToString().PadLeft(7, '0'); } }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("ManufacturerCode")]
        public string Article { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("ProducerName")]
        public string ProducerName { get; set; }

        //public int ParentId { get; set; }

        [Display(Order = 5)]
        [LocalizedDisplayNameAttribute("Group")]
        public string CategoryName { get; set; }
            

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("ProductName")]
        public string Name { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("Amount")]
        public int Rest { get; set; }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("DeliveryTime")]
        public int DaysToDepartment { get; set; }

        [Display(Order = 9)]
        [LocalizedDisplayNameAttribute("EntryPrice")]
        public decimal? Price { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("RetailPrice2")]
        public decimal? PriceOfClient { get; set; }

        [Display(Order = 25)]
        [LocalizedDisplayNameAttribute("Weight")]
        public decimal? Weight { get; set; }

        [Display(Order = 26)]
        [LocalizedDisplayNameAttribute("Volume")]
        public decimal? Volume { get; set; }
    }


    public class PriceListAkbResult
    {
        private int _productId;

   //     [Display(Order = 1)]
   //     public Int64 row_number { get; set; }

        public int ProductId { set { _productId = value; } get { return _productId; } }

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        public string ProductIdTo7S { get { return _productId.ToString().PadLeft(7, '0'); } }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("ManufacturerCode")]
        public string Article { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("ProducerName")]
        public string ProducerName { get; set; }

        [Display(Order = 5)]
        [LocalizedDisplayNameAttribute("Brand")]
        public string Brand { get; set; }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("ProductName")]
        public string Name { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("Capacity")]
        public string Capacity { get; set; }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("Polarity")]
        public string Polarity { get; set; }

        [Display(Order = 9)]
        [LocalizedDisplayNameAttribute("CrankCurrent")]
        public string Inrush_Current { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("Dimensions")]
        public string Size { get; set; }

        [Display(Order = 11)]
        [LocalizedDisplayNameAttribute("Amount")]
        public int Rest { get; set; }

        [Display(Order = 12)]
        [LocalizedDisplayNameAttribute("DeliveryTime")]
        public int DaysToDepartment { get; set; }

        [Display(Order = 13)]
        [LocalizedDisplayNameAttribute("EntryPrice")]
        public decimal? Price { get; set; }

        [Display(Order = 14)]
        [LocalizedDisplayNameAttribute("RetailPrice2")]
        public decimal? PriceOfClient { get; set; }

        [Display(Order = 25)]
        [LocalizedDisplayNameAttribute("Weight")]
        public decimal? Weight { get; set; }

        [Display(Order = 26)]
        [LocalizedDisplayNameAttribute("Volume")]
        public decimal? Volume { get; set; }
    }

    // ProductId, ModelName,  Name, Diametr,Width,PCD,ET,DIA, Holes, Rest, Price

    public  class PriceListDiskBase 
    {
        private int _productId;
        //   [DisplayName("Номер п.п.")]
     //   [Display(Order = 1)]
     //   public Int64 row_number { get; set; }

        public int ProductId { set { _productId = value; } get { return _productId; } }
      

        [Display(Order = 2)]
        [LocalizedDisplayNameAttribute("StockNumber")]
        public string ProductIdTo7S { get { return _productId.ToString().PadLeft(7, '0'); } }

        [Display(Order = 3)]
        [LocalizedDisplayNameAttribute("Brand")]
        public string ProducerName { get; set; }

        [Display(Order = 6)]
        [LocalizedDisplayNameAttribute("ProductName")]
        public string Name { get; set; }

        [Display(Order = 4)]
        [LocalizedDisplayNameAttribute("Model")]
        public string ModelName { get; set; }

        [Display(Order = 7)]
        [LocalizedDisplayNameAttribute("WheelColor")]
        public string Color
        {
            get { return StringUtils.GetColourFromName(Name); }
         

        }

        [Display(Order = 8)]
        [LocalizedDisplayNameAttribute("Width")]
        public string Width { get; set; }

        [Display(Order = 9)]
        [LocalizedDisplayNameAttribute("Diameter")]
        public string Diametr { get; set; }

        [Display(Order = 10)]
        [LocalizedDisplayNameAttribute("Holes")]
        public string Holes { get; set; }

        [Display(Order = 11)]
        [DisplayName("PCD")]
        public string PCD { get; set; }

        [Display(Order = 12)]
        [DisplayName("ET")]
        public string ET { get; set; }

        [Display(Order = 13)]
        [DisplayName("DIA")]
        public string DIA { get; set; }

        [Display(Order = 14)]
        [LocalizedDisplayNameAttribute("Amount")]
        public int Rest { get; set; }

          [Display(Order = 15)]
          [LocalizedDisplayNameAttribute("DeliveryTime")]
        public int DaysToDepartment { get; set; }

          [Display(Order = 18)]
          [DisplayName("URL")]
          public string PathToRemotePicture
          { get; set; }

          [Display(Order = 25)]
          [LocalizedDisplayNameAttribute("Weight")]
          public decimal? Weight { get; set; }

          [Display(Order = 26)]
          [LocalizedDisplayNameAttribute("Volume")]
          public decimal? Volume { get; set; }

    }

    public class PriceListPartnerDiskResult : PriceListDiskBase
    {
        [Display(Order = 16)]
        [LocalizedDisplayNameAttribute("EntryPrice")]
        public decimal? Price
        { get; set; }


        [Display(Order = 17)]
        [LocalizedDisplayNameAttribute("RetailPrice")]
        public decimal? Price2
        { get; set; }


    }


    public class PriceListPointDiskResult : PriceListDiskBase
    {
        [Display(Order = 16)]
        [LocalizedDisplayNameAttribute("EntryPrice")]
        public decimal? PriceOfPoint
        { get; set; }

        [Display(Order = 17)]
        [LocalizedDisplayNameAttribute("RetailPrice")]
        public decimal? Price2
        { get; set; }
    
    
    }
}