using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Term.DAL;

namespace YstProject.Services
{
    
    
    public static class Defaults
    {
        
        public static readonly int[] ProducersIdToRestrict = new[] {100, 517, 65};

        public static readonly Producer[] ProducersForgedWheels = new[]
        {
            new Producer {ProducerId = 3657, Name = "VISSOL", ProductType = ProductType.Disk},
            new Producer {ProducerId = 3654, Name = "HARP", ProductType = ProductType.Disk},
            new Producer {ProducerId = 3656, Name = "BUFFALO", ProductType = ProductType.Disk}
        };

        public static readonly string DateFormat = "dd.MM.yyyy";
        public static readonly string ResourceNotFound = "Resource not found";
        public static readonly string FixingCode = "fixingcode";
        public static readonly int DaysAllowedToReturn  = 30;
        public static readonly string Ship = "шип";
        public static readonly string Invisible = "invisible";
        public static readonly string Space =  " ";
        public static readonly char CommaSign = ',';
        public static readonly double RateNDS = 1.18;
        
        public static readonly int MaxNumberAddToCart = 200;
        public static readonly double RateForDelivery = 1.75;
        public static readonly string StubDepartmentCode = "00005";
        public static readonly string EmptyDepartmentCode = "00000";
        public static readonly uint MaxUploadedFileSize = 10485760; // 10Mb
        public static readonly string  Culture_RU = "ru-RU"; // 10Mb
        public static readonly string CargoWheelsProducers = "99,734,752,775,3308,3582,3596,3627,3646,3666,3762";
        public static readonly string ReplicaWheelsProducers = "65,3333,3754";
        public const string RussianCode = "7_1";
        public const int MainDepartment = 5;
        public const int DepartmentEkb = 138;
        public const int NewsPreviewPhoto = 1;
        public const int NewsMainPhoto = 2;
        public const string DefaultKeyword = "1234";
        public const string Nut = "Гайка";
        public const string Bolt = "Болт";
        public static readonly Dictionary<string,string> PathToFullImages= new Dictionary<string,string>{
        {"tyre","tires/"},{"disk","allwheels/"},{"akb", "batteries/"}, {"acc","accessories/"},{"others","other/"}};
      //  public static readonly string ClientsForNotifications = "Client571,Client5691,Client539,Client7357,Client5479,Client5757,Client6503,Client7160,Client6918,Client5476,Client4280,Client7291,Client434,Client2663,Client644,Client540,Client6959,Client395,Client1295,Client5340,Client872,Client1922,Client5343,Client6495,Client7454,Client988,Client181,Client4234,Client4261,Client759,Client7552,Client093,Client4906,Client4860,Client7645,Client393,Client7539,Client7647,Client5274,Client3530,Client5205,Client639,Client5806,Client1315,Client6631,Client605,Client2284,Client2227,Client3911,Client3528,Client510,Client1307,Client4003,Client7553,Client3657,Client079,Client1756,Client6981,Client1903,Client7316,Client3518,Client6697,Client628,Client7493,Client6313,Client7240,Client1846,Client1481,Client2611,Client1920,Client6958,Client002,Client371,Client907,Client1291,Client1941,Client4265,Client1329,Client527,Client7292,Client1375,Client2273,Client3378,Client5330,Client501,Client402,Client372,Client7270,Client6477,Client5439,Client1432,Client5206,Client7515,Client5808,Client1352,Client1330,Client1930,Client6966,Client5275,Client7519,Client6803,Client7339,Client3209,Client396,Client417, Client1656,Client986,Client569,Client4923,Client5901,Client7358,Client2604,Client5590,Client6947";
        public static readonly Dictionary<string, string> SeasonNames = new Dictionary<string, string>{
        {"winter","зима"},{"summer","лето"},{"allseason", "всесезон"}};

        //public const int MAINDEPARTMENT_ID = 5;
        public static readonly int Endyear = DateTime.Now.Year;
        public static readonly int BeginYear = 1996;

        public const decimal priceRateAkbPoint = 1.1M;
        public const decimal priceRateAkbClient = 1.4M;
        public const decimal priceRateAccPoint = 1.2M;
        public const decimal priceRateAccClient = 1.5M;
        public const uint orderCanChangeInDays = 7;

        public static readonly Dictionary<string, Size> Sizes = new Dictionary<string, Size>{
        {"tyre", new Size{Width=50,Height=75}},{"disk", new Size{Width=75,Height=75}}, {"akb", new Size{Width=75,Height=75}},
        {"acc", new Size{Width=75,Height=75}}, {"others",new Size{Width=75,Height=75}}};

        public static string KeyWord = "Keyword";
        public static string Hash = "Hash";

        public static readonly int TotalMinutes = 10;

        public const int PriceMaxRus = 70000;
        public const int PriceMaxEng = 500;
        public const int CargoPriceMaxRus = 300000;
        public const int CargoPriceMaxEng = 3000;
        public const int PriceStepSlideRus = 500;
        public const int PriceStepSlideEng = 10;

        public const int TyreCamsFolder = 1127;
        public const int WheelsBoltsFolder = 702;
        public const int TyreOthersFolder = 1746;
        public const int WheelsOthersFolder = 701;
        public const int WheelsNutsFolder = 713;
    }
    

   public  struct Size
    {
       public uint Width { get; set; }
        public uint Height { get; set; }

    }


}