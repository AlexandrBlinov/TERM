using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Yst.Context;
using System.Collections;
using System.Collections.Generic;
using System.Web.Routing;
using System.Linq;
using System.ComponentModel;
using System.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Yst.ViewModels;


namespace YstProject.Services
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string resourceName;
        public LocalizedDisplayNameAttribute(string resourceName)
            : base()
        {
            this.resourceName = resourceName;
        }

        public override string DisplayName
        {
            get
            {
                return Term.Web.Views.Resources.ForSearchResult.ResourceManager.GetString(this.resourceName);
            }
        }
    }


    public class CustAuthAttribute : AuthorizeAttribute
    {
     

   private readonly string[] allowedroles;
   public CustAuthAttribute(params string[] roles)  
   {
       this.allowedroles = roles;  
   }
      

       
        // Partner PartnerPoint
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            using (var servicePP = new ServicePartnerPoint())
            {
            foreach (var role in allowedroles)
            {
                if (role.Equals("partner",StringComparison.InvariantCultureIgnoreCase))
                {
                 
                    {
                        authorize = authorize | (servicePP.GetPartnerId()!=null);
                    
                    }
                }
                else if(role.Equals("partnerpoint",StringComparison.InvariantCultureIgnoreCase))
                {
                    
                        int PartnerPointId = servicePP.getPointID();
                        string paramPointId = httpContext.Request.Params["PointId"];

                        authorize = authorize | (PartnerPointId > 0 && paramPointId == PartnerPointId.ToString());

                    }
                
                }
                                         
            }
            return authorize;
        }
        
       
    }

    public class IPAuthAttribute : System.Web.Http.AuthorizeAttribute
    {


        private  string[] IpAddresses;
        public IPAuthAttribute(params string[] ipAdresses)
        {
            this.IpAddresses = ipAdresses;
        }


        public override void OnAuthorization(HttpActionContext actionContext)
        {

            if (AuthorizeCore((HttpContextBase)actionContext.Request.Properties["MS_HttpContext"]))
                return;

            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            
                    
        }

        private bool AuthorizeCore(HttpContextBase httpContext)
        {
           
           
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            string userIpAddress = httpContext.Request.UserHostAddress;

            bool ipAllowed = ((IList<string>)IpAddresses).Contains(userIpAddress)||httpContext.Request.IsLocal;
            

            return ipAllowed;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckIfSupplier : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            if (!filterContext.IsChildAction && filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
            {
                var userName=filterContext.RequestContext.HttpContext.User.Identity.Name;

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new AppDbContext()));

                var user = userManager.FindByName(userName);

                if (user!=null)
                {
                    if (userManager.IsInRole(user.Id, "Supplier") && user.SupplierId.HasValue)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary { { "controller", "Supplier" }, { "action", "Index" } });
                        return;
                    }
                }
            }
            



            
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
         
        }
    }



    // Нужен для последовательного прохождения цепочки 1. Создание точки -> 2. Лиц соглашение -> 3. Работа
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckSettingsAttribute :FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
        
       string ControllerName = filterContext.RouteData.Values["Controller"].ToString();
       string ActionName = filterContext.RouteData.Values["Action"].ToString();



       string[] actionNamesForPartner =  { "CreateOwn", "CreateOwnPost" };
       string[] actionNamesForPoint =  { "KeyWordEnter", "Edit", "EditPost", "SavePointProfile" };
       string[] actionNamesAgreement =  { "UserAgreement", "AcceptConditions" };


       string[] joined = actionNamesForPartner.Union(actionNamesForPoint).ToArray();
            
           using (var  ServicePP = new ServicePartnerPoint()  )
           {
       if (!filterContext.IsChildAction && filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
       {
            // **********
           // если партнер, но у него еще нет собственной партнерской точки
           // **********
       if ( ServicePP.IsPartner && !(ControllerName.Equals("partner",StringComparison.InvariantCultureIgnoreCase) && Array.IndexOf(actionNamesForPartner, ActionName) > -1))
           {

           // если партнерской точки нет для данного пользователя
           if (ServicePP.getPointID()==-1)
           {
               filterContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary
                           {{"controller", "Partner"}, {"action", "CreateOwn"}});
               return;
               }
           }

           // **********
           // если точка, но у нее не заполнены все необходимые поля
           // **********
       if (!ServicePP.IsPartner) 
       {
           int pointId= ServicePP.getPointID();

           if (pointId > 0 && !(ControllerName.Equals("partner", StringComparison.InvariantCultureIgnoreCase) && Array.IndexOf(actionNamesForPoint, ActionName) > -1))
           { 
               string stringOfNotFilledProperties = ServicePP.CheckForPointWhatPropertiesAreNotFilled(pointId);
               if (stringOfNotFilledProperties != String.Empty)
           {
               filterContext.Controller.TempData["ErrorsEmptyFields"] = stringOfNotFilledProperties;

               filterContext.Result = new RedirectToRouteResult(
                          new RouteValueDictionary { { "controller", "Partner" }, { "action", "Edit" },{"PointId",pointId} });
               return;
           }
           }
       }

       if (!ServicePP.ConditionsAreAccepted && !actionNamesAgreement.Any(p => p.Equals(ActionName, StringComparison.OrdinalIgnoreCase)) && !(Array.IndexOf(joined, ActionName) > -1))
       {
           filterContext.Result = new RedirectToRouteResult(
                             new RouteValueDictionary { { "controller", "Partner" }, { "action", "UserAgreement" }});
       }
       
         }
        }}


         public void OnActionExecuted(ActionExecutedContext filterContext)
         {
           
         }
    }

    // нужен для проверки необходимости ввода ключевого склова для отдельных Actions

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CheckPassThroughKeyword : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string ControllerName = filterContext.RouteData.Values["Controller"].ToString();
            string ActionName = filterContext.RouteData.Values["Action"].ToString();

            
            using (var ServicePP = new ServicePartnerPoint())
            {
                if (!filterContext.IsChildAction && filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
                {
                    if (!ServicePP.CheckIfCanPassTrough(String.Empty))

                      filterContext.Result = new RedirectToRouteResult(
                                    new RouteValueDictionary { { "controller", "Partner" }, { "action", "KeyWordEnter" }, { "ReturnUrl", filterContext.HttpContext.Request.RawUrl} });
                            return;
                        }
                    }
            }
        


        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }



}