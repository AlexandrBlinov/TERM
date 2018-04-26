using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Term.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Error - 404", "NotFound", new { controller = "Error", action = "NotFound"});

            routes.MapRoute("Thumbnail", "Thumbnail/{productType}/{file}",
          new { controller = "Thumbnail", action = "Index" });

  
            routes.MapRoute("PodborAuto", url: "PodborAutoTyresDisks/Index/{brand}/{model}/{year}/{engine}",
          defaults: new { controller = "PodborAutoTyresDisks", action = "Index", brand = UrlParameter.Optional, model = UrlParameter.Optional, year = UrlParameter.Optional, engine = UrlParameter.Optional });

            routes.MapRoute("PodborAkb", "PodborAkb/Index",
          defaults: new { controller = "PodborAkb", action = "Index" });

            routes.MapRoute("Others", "Home/Others/{OtherType}",
             new { controller = "Home", action = "Others", OthersType = 1 });

       /*     routes.MapRoute("Akb", "Home/Akb/{Brand}/{Inrush_Current}/{Volume}/{Polarity}/{Size}",
            new { controller = "Home", action = "Akb", Brand = "all", Inrush_Current = "all", Volume = "all", Polarity="all", Size="all"});   */

            routes.MapRoute("Tyres", "Home/Tyres/{ProducerId}/{SeasonId}/{Width}-{Height}-R{Diametr}",
            new { controller = "Home", action = "Tyres", ProducerId = "all", SeasonId = "all", Width = "all", Height = "all", Diametr = "all" });

            

            /* routes.MapRoute("CargoTyres", "Home/CargoTyres/{ProducerId}/{SeasonId}/{Width}-{Height}-R{Diametr}",
             new { controller = "Home", action = "CargoTyres", ProducerId = "all", SeasonId = "all", Width = "all", Height = "all", Diametr = "all" }); */

            routes.MapRoute("CargoTyres", "Home/CargoTyres",
          new { controller = "Home", action = "CargoTyres"});



            /*   routes.MapRoute("SeasonTyres", "SeasonProduct/Tyres/{SeasonPostId}/{SeasonId}/{Width}-{Height}-R{Diametr}",
             new { controller = "SeasonProduct", action = "Tyres", ProducerId = "all", SeasonId = "all", Width = "all", Height = "all", Diametr = "all" });
             *  */
            routes.MapRoute("SeasonTyres", "SeasonProduct/Tyres/{ProducerId}/{SeasonId}/{Width}-{Height}-R{Diametr}",
              new { controller = "SeasonProduct", action = "Tyres", ProducerId = "all", SeasonId = "all", Width = "all", Height = "all", Diametr = "all" });


               routes.MapRoute("Disks", "Home/Disks/{ProducerId}/{Width}x{Diametr}_{Hole}x{PCD}_ET{ET}_D{DIA}",
              new { controller = "Home", action = "Disks", ProducerId = "all", Width = "all", Diametr = "all", Hole = "all", PCD = "all", ET = "all", DIA = "all" });

            routes.MapRoute("DisksByParams", "Home/DisksByParams/{ProducerId}/{Width}x{Diametr}_{Hole}x{PCD}_ET{ET}_D{DIA}",
            new { controller = "Home", action = "DisksByParams", ProducerId = "all", Width = "all", Diametr = "all", Hole = "all", PCD = "all", ET = "all", DIA = "all" });


            routes.MapRoute("CargoDisks", "Home/CargoDisks/{ProducerId}/{Width}x{Diametr}_{Hole}x{PCD}_ET{ET}_D{DIA}",
              new { controller = "Home", action = "CargoDisks", ProducerId = "all", Width = "all", Diametr = "all", Hole = "all", PCD = "all", ET = "all", DIA = "all" });

         /*    routes.MapRoute("SeasonDisks", "SeasonProduct/Disks/{ProducerId}/{Width}x{Diametr}_{Hole}x{PCD}_ET{ET}_D{DIA}",
              new { controller = "SeasonProduct", action = "Disks", ProducerId = "all", Width = "all", Diametr = "all", Hole = "all", PCD = "all", ET = "all", DIA = "all" }); */

               routes.MapRoute("SeasonDisks", "SeasonProduct/Disks/{WheelType}",
                 new { controller = "SeasonProduct", action = "Disks", WheelType=1});



               routes.MapRoute("SparDisks", "Home/SparDisks/{Width}/{RearWidth}/{Diametr}/{RearDiametr}/{Holes}/{RearHoles}/{PCD}/{RearPCD}/{ET}/{RearET}/{DIA}/{RearDIA}",
               new { controller = "Home", action = "SparDisks", Width = "all", Diametr = "all", Holes = "all", PCD = "all", ET = "all", DIA = "all", RearWidth = "all", RearDiametr = "all", RearHoles = "all", RearPCD = "all", RearET = "all", RearDIA = "all" });

               routes.MapRoute("SparTyres", "Home/SparTyres/{Width}/{RearWidth}/{Height}/{RearHeight}/{Diametr}/{RearDiametr}",
               new { controller = "Home", action = "SparTyres", Width = "all", Height = "all", Diametr = "all", RearWidth = "all", RearHeight = "all", RearDiametr = "all" });


           
            routes.MapRoute(
                name: "Accs",
                url: "Home/Accs/{*pathInfo}",
                defaults: new { controller = "Home", action = "Accs" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}