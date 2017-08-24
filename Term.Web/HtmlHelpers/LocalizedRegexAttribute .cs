using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using Term.Web.Views.Resources;

namespace Term.Web.HtmlHelpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizedRegexAttribute : RegularExpressionAttribute
    {

        static LocalizedRegexAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (LocalizedRegexAttribute),
                typeof (RegularExpressionAttributeAdapter));
        }

        public LocalizedRegexAttribute(string regularExpression, string errorMessageResourceName,
            Type errorMessageResourceType)
            : base(regularExpression)
        {
            ErrorMessageResourceType = errorMessageResourceType;
            ErrorMessageResourceName = errorMessageResourceName;
        }

        /*  private static string LoadRegex(string key)
        {
            var resourceManager = new ResourceManager(typeof(CartAndOrders));
            return resourceManager.GetString(key);
        } */
    }
}