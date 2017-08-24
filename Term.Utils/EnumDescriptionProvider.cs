using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Term.CustomAttributes;

namespace Term.Utils
{

   
    /// <summary>
    /// Helper class to works with enums and dropdpwns
    /// </summary>
    

   public static class EnumDescriptionProvider
    {

       public static IEnumerable<SelectListItem> GetSelectListFromEnum<TEnum>()
       {
           return (Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(
               enu => new SelectListItem { Value = enu.ToString(), Text = GetMultiCultureDescription(enu.ToString(), typeof(TEnum)) }));
          
       }

      

        public static string GetDescription(Enum enumValue)
        {
            var enumType = enumValue.GetType();

            var enumValueName = Enum.GetName(enumType, enumValue);
            if (enumValueName == null)
            {
                return "Unknown";
            }

            var enumField = enumType.GetField(enumValueName);
            if (enumField == null)
            {
                return enumValueName;
            }

            var userTextAttribute = enumField.GetCustomAttributes(typeof(MultiCultureDescriptionAttribute), false).FirstOrDefault();
            if (userTextAttribute == null)
            {
                return enumValueName;
            }

            return ((MultiCultureDescriptionAttribute)userTextAttribute).Description;
        }

        /// <summary>
        /// Used for multiculture decriptions
        /// Get resource key in current culture
        ///  [MultiCultureDescription(typeof(OrderStatus), "Confirmed")] where OrderStatus is Resource file, Confirmed is resource key
        ///    Confirmed = 2,
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        /// 

        public static string GetMultiCultureDescription(string enumValue,Type enumtype)
        {
            var fi = enumtype.GetField(enumValue);
            var userTextAttribute = fi.GetCustomAttributes(typeof(MultiCultureDescriptionAttribute), false).First();

            return ((MultiCultureDescriptionAttribute)userTextAttribute).Description;
        } 

        public static string GetMultiCultureDescription(Enum enumValue)
        {
            var enumType = enumValue.GetType();

            var enumValueName = Enum.GetName(enumType, enumValue);
            if (enumValueName == null)
            {
                return "Unknown";
            }

            var enumField = enumType.GetField(enumValueName);
            if (enumField == null)
            {
                return enumValueName;
            }

            var userTextAttribute = enumField.GetCustomAttributes(typeof(MultiCultureDescriptionAttribute), false).FirstOrDefault();
            if (userTextAttribute == null)
            {
                return enumValueName;
            }

            return ((MultiCultureDescriptionAttribute)userTextAttribute).Description;
        } 
    }
}
