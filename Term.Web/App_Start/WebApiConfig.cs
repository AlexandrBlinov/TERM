using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Term.Utils;

namespace Term.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(name: "Exchange",
           routeTemplate: "exchange/{action}", defaults: new { controller = "Exchange" });

            config.Routes.MapHttpRoute(name: "OnWayItems",
          routeTemplate: "api/onwayitems/{id}", defaults: new { controller = "OnWayItems", id = RouteParameter.Optional });

           config.Routes.MapHttpRoute(name: "PricesApi",routeTemplate: "api/prices", defaults: new { controller = "PricesApi"}); 
            

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {  id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new MyDateTimeConvertor());
            //var json = config.Formatters.JsonFormatter;
        //    config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
        }
    }
}
