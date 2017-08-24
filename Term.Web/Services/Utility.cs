using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Term.DAL;
using Yst.ViewModels;
using Yst.Context;
using System.Text;
using System.Configuration;
using YstTerm.Models;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Yst.Utils
    
{
    public static class DisplayNameAttrLocator
    {
        /// <summary>
        /// Получить список свойств которые не заполнены
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="PropertyNames"></param>
        /// <returns></returns>
        public static IList<string> GetPropsNotFilled(Object obj, params string[] propertyNames)
        {

            List<string> propsNotFilled = new List<string>();

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //var i=default(properties[0].PropertyType);
            var objType = obj.GetType();

            foreach (var propName in propertyNames)
            {
                PropertyInfo propInfo = objType.GetProperty(propName);
                if (propInfo != null)
                {
                    var defaultValue = propInfo.PropertyType.IsValueType ? Activator.CreateInstance(propInfo.PropertyType) : null;
                    var value = propInfo.GetValue(obj, null);


                    if ((propInfo.PropertyType == typeof(string) && String.IsNullOrEmpty(value as String))||
                        (propInfo.PropertyType.IsValueType && defaultValue.Equals(value)) ||
                    !propInfo.PropertyType.IsValueType && value == null)
                    {
                        Attribute attr = propInfo.GetCustomAttribute(typeof(DisplayNameAttribute));
                        if (attr != null)
                            propsNotFilled.Add(((DisplayNameAttribute)(attr)).DisplayName);
                        else
                            propsNotFilled.Add(propInfo.Name);
                    }

                }
            }

            return propsNotFilled;
        } 
    }

    public static class PropertyInfoExtensions
    {
        public static int PropertyOrder(this PropertyInfo propInfo)
        {
            int output;
            var orderAttr = (DisplayAttribute)propInfo.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
            output = orderAttr != null ? orderAttr.Order : Int32.MaxValue;
            return output;
        }

        
    }

    # region mapper

    /// <summary>
    /// Класс копирует значения свойств одного объекта в другие
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class PropsMyMapper<T> where T : class, new()
    {

        public static T CopyObjProperties(Object source, params string[] namesOfProperties)
        {
            var dest = new T();

            var PropertiesList = GetProperties(namesOfProperties);


            foreach (PropertyInfo destPI in PropertiesList)
            {
                PropertyInfo sourcePI = source.GetType().GetProperty(destPI.Name);

                if (sourcePI == null)
                {
                    continue;
                }
                if (!destPI.CanWrite)
                {
                    continue;
                }
                if (destPI.GetSetMethod(true) != null && destPI.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((destPI.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!destPI.PropertyType.IsAssignableFrom(sourcePI.PropertyType))
                {
                    continue;
                }
                if (sourcePI.PropertyType == destPI.PropertyType && sourcePI.Name == destPI.Name)
                    destPI.SetValue(dest, sourcePI.GetValue(source, null), null);
            }
            return dest;
        }


        private static List<PropertyInfo> GetProperties(params string[] namesOfProperties)
        {
            string[] namesofProps = { };

            if (namesOfProperties.Length > 1)
                namesofProps = namesOfProperties;
            else if (namesOfProperties.Length == 1)
                namesofProps = namesOfProperties[0].Split(new char[] { ',', ';' });

            var PropertiesList = new List<PropertyInfo>();

            if (namesofProps.Length > 0)
                namesofProps.ToList().ForEach(p =>
                {
                    PropertyInfo propertyFound = typeof(T).GetProperty(p);
                    if (propertyFound != null)
                        PropertiesList.Add(propertyFound);

                });

            else // zero

                PropertiesList = typeof(T).GetProperties().ToList();


            return PropertiesList;
        }


        public static void CopyObjProperties(Object dest, Object source, params string[] namesOfProperties)
        {

            var PropertiesList = GetProperties(namesOfProperties);



            foreach (PropertyInfo destPI in PropertiesList)
            {
                PropertyInfo sourcePI = source.GetType().GetProperty(destPI.Name);

                if (sourcePI == null)
                {
                    continue;
                }
                if (!destPI.CanWrite)
                {
                    continue;
                }
                if (destPI.GetSetMethod(true) != null && destPI.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((destPI.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!destPI.PropertyType.IsAssignableFrom(sourcePI.PropertyType))
                {
                    continue;
                }
                if (sourcePI.PropertyType == destPI.PropertyType && sourcePI.Name == destPI.Name)
                    destPI.SetValue(dest, sourcePI.GetValue(source, null), null);
            }
        }
    }

    #endregion

    public static class PasswordUtility

    {   

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
     

    }
}