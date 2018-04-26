using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using YstTerm.Models;


namespace YstProject.Models
{

    public class DateTimeModelBinder : DefaultModelBinder
    {
        private string[] _customDateFormats = { "dd.M.yyyy", "dd/M/yyyy", "dd-M-yyyy" };


        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);


            if (String.IsNullOrEmpty(value.AttemptedValue)) return null;

            if (DateTime.TryParseExact(value.AttemptedValue, _customDateFormats, CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out DateTime dateValue))
                return dateValue;
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Passed date is in incorrect format");
                return value.AttemptedValue;
            }

        }
    }

    /// <summary>
    /// Привязывем к модели SeasonCartViewModelBinder , чтобы  для поля  public DisplaySeasonCart? Display принимался null, а не первое значение по умолчанию
    /// </summary>
    public class SeasonCartViewModelBinder : DefaultModelBinder {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            SeasonCartViewModel model = (SeasonCartViewModel)base.BindModel(controllerContext, bindingContext);

           var val = bindingContext.ValueProvider.GetValue("Display");

           if (val == null) model.Display = null;

           else
           {
               DisplaySeasonCart currentDisplay;

               if (Enum.TryParse(val.AttemptedValue, out currentDisplay)) model.Display = currentDisplay;
           }
             
            return model;
        }
    }

    public static class ParamsCollectionProvider
    {
        public static string[] GetCollectionOfParametersFromRouteTable(string name)
        {
            var parameters = new List<string>();
            string path = ((System.Web.Routing.Route)(System.Web.Routing.RouteTable.Routes[name])).Url;
            string pattern = @"\{\w+\}";

            foreach (Match m in Regex.Matches(path, pattern))
                parameters.Add(Regex.Replace(m.ToString(), @"[\{\}]", ""));

            return parameters.ToArray();
        }

    }

    public static class DefaultValueProvider<TClass>
    {

        public static object GetDefaultValue(string propName) //where TResult : struct
        {
            DefaultValueAttribute attr;
            var pageInfo = typeof(TClass).GetProperty(propName);
            if (pageInfo != null && pageInfo.IsDefined(typeof(DefaultValueAttribute), false))
            {
                attr = (DefaultValueAttribute)Attribute.GetCustomAttribute(pageInfo, typeof(DefaultValueAttribute));

                return attr.Value ?? null;
            }
            throw new NullReferenceException("Property " + propName + " has not DefaultValueAttribute in class " + typeof(TClass).Name);

        }

    }

    /// <summary>
    /// Base class to map all props to variables
    /// </summary>
    /// <typeparam name="TModel"></typeparam>

    public class CommonPodborViewModelBinder<TModel> : DefaultModelBinder where TModel : CommonPodborView
    {
        private string _route = string.Empty;
        public CommonPodborViewModelBinder() : base() { }
        public CommonPodborViewModelBinder(string route)
            : base()
        {
            _route = route;

        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            var model = (TModel)base.BindModel(controllerContext, bindingContext);

            if (model == null) throw new Exception("Model is not convertible to CommonPodborView");
            else
            {
                if (model.Page == default(int)) model.Page = (int)DefaultValueProvider<CommonPodborView>.GetDefaultValue("Page"); 
                if (model.ItemsPerPage == default(int)) model.ItemsPerPage = (int)DefaultValueProvider<CommonPodborView>.GetDefaultValue("ItemsPerPage");
              //  if (model.DisplayView == null) model.DisplayView = Display.Table;
             
                ProcessModel(bindingContext, model);

                return model;
            }
        }

        protected virtual void ProcessModel(ModelBindingContext bindingContext, TModel model)
        {
            if (!String.IsNullOrEmpty(_route))
            {
                string[] arrToNull = ParamsCollectionProvider.GetCollectionOfParametersFromRouteTable(_route);
                string no_filter = "all";
                for (int i = 0; i < arrToNull.Length; i++)
                    if (bindingContext.ValueProvider.GetValue(arrToNull[i]).AttemptedValue.CompareTo(no_filter) == 0)
                    {
                        var propInfo = model.GetType().GetProperty(arrToNull[i]);
                        propInfo.SetValue(model, null);
                    }
            }
        }


    }

    public class TyresModelBinder : CommonPodborViewModelBinder<TyresPodborView>
    {

        public TyresModelBinder() : base("Tyres") { }

    }

    public class DisksModelBinder : CommonPodborViewModelBinder<DisksPodborView>
    {
        public DisksModelBinder() : base("Disks") { }


    }

    public class AkbModelBinder : CommonPodborViewModelBinder<AkbPodborView>
    {
        public AkbModelBinder() : base("Akb") { }

    }

    public class AccModelBinder : CommonPodborViewModelBinder<AccPodborView>
    {
        public AccModelBinder() : base("Acc") { }

    }

    public class EnumModelBinder<T> : IModelBinder where T:struct
    {
        private T DefaultValue { get; set; }
        public EnumModelBinder(T defaultValue)
        {
            DefaultValue = defaultValue;
        }
        public EnumModelBinder()
        {
            
            DefaultValue =(T) Enum.Parse(typeof(T), "0");
        }

        #region IModelBinder Members
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            
            return bindingContext.ValueProvider.GetValue(bindingContext.ModelName) == null
            ? DefaultValue
            : GetEnumValue(DefaultValue, bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue);
        }

        #endregion

        public static T GetEnumValue<T>(T defaultValue, string value)
        {
            T enumType = defaultValue;

            if ((!String.IsNullOrEmpty(value)) && (Contains(typeof(T), value)))
                enumType = (T)Enum.Parse(typeof(T), value, true);

            return enumType;
        }
        public static bool Contains(Type enumType, string value)
        {
            return Enum.GetNames(enumType).Contains(value, StringComparer.OrdinalIgnoreCase);
        }
    }

    
}

   
