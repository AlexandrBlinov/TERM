using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Yst.Context;
using System.Data.Entity;
using System.Web.Caching;
using YstTerm.Models;
using YstProject.Models;
using YstProject.Services;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Formatting;
using Term.Utils;
using YstProject;
using Yst.ViewModels;
using Term.Web.Models;

namespace Term.Web
{
    

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();


            ModelBinders.Binders.Add(typeof(TyresPodborView), new TyresModelBinder());
            ModelBinders.Binders.Add(typeof(DisksPodborView), new DisksModelBinder());
     //       ModelBinders.Binders.Add(typeof(AkbPodborView), new AkbModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
        //    ModelBinders.Binders.Add(typeof(CaseLogistik), new EnumModelBinder<CaseLogistik>(CaseLogistik.NoCase));



            GlobalConfiguration.Configure(WebApiConfig.Register);
           // WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        // не указывать автомиграции ни на локальном ни на product серверах!!!
       //   Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, YstProject.Migrations.Configuration>());
                     Database.SetInitializer<AppDbContext>(null);
                    Database.SetInitializer<MtsLocationsContext>(null);

#if !DEBUG
                JobScheduler.Start();
#endif


        }


        /// <summary>
        /// Кэширование для пользователя в зависимости от даты импорта остатков
        /// </summary>
        /// <param name="context"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            string result = String.Empty;
            //if (String.IsNullOrEmpty(arg)) return base.GetVaryByCustomString(context, arg);

           if  (arg.Equals("user",StringComparison.InvariantCultureIgnoreCase))            
            
             result=(context.Request.IsAuthenticated) ? context.User.Identity.Name:String.Empty;
                
            
            if (arg.Contains( "RestsImportDateTime"))
            {
                object obj = context.Application["RestsImportDateTime"];
                
                if (obj != null && obj is DateTime) result+=((DateTime)obj).ToString(CultureInfo.InvariantCulture);
               
            }

            return result;
            
        }
     
  
       
    }
}