using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Resources;
using System.Web;
using Term.DAL.Resources;

namespace Term.CustomAttributes
{
    /// <summary>
    /// Проверяет валидность даты (дата должна быть в интервале с begin до begin+days)
    /// </summary>
    public class ValidateDateAttribute : ValidationAttribute
    {
        private DateTime _begin;
        private DateTime _end;
        public ValidateDateAttribute(int begin, int days)
        {
            _begin = StartOfDay(DateTime.Now).AddDays(begin);
            _end = StartOfDay(DateTime.Now).AddDays(days);

        }
        private DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            DateTime val = (DateTime)value;

            return val >= _begin && val <= _end;

        }

        public override string FormatErrorMessage(string name)
        {

            return string.Format(Resource.DateInterval + " {0} - {1}", _begin.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), _end.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }
    }
    /// <summary>
    /// Attribute ussed for multiculture strings
    /// </summary>

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class MultiCultureDescriptionAttribute : Attribute
    {
        private string _description;


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="resourseType"></param>
        /// <param name="description"></param>
        public MultiCultureDescriptionAttribute(Type resourseType, string description)
        {
            _description = new ResourceManager(resourseType).GetString(description);

        }

        public string Description { get { return _description; } set { _description = value; } }

        public override string ToString()
        {
            return _description;
        }
    }

   
    /// <summary>
    /// Класс аттрибутов только для русских названий (упрощенный)
    /// </summary>
    
        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public sealed class EnumDescriptionAttribute : Attribute
        {
         /*   public EnumDescriptionAttribute(string description)
            {
                //Description = description;
                string _resourceKey = description;
                ResourceManager _resource = new ResourceManager(typeof(Resource));
                Description = _resource.GetString(_resourceKey);
            }
            */

        public EnumDescriptionAttribute(string description)
        {   
            Description = description;
        }

        public string Description { get; private set; }
        } 
}