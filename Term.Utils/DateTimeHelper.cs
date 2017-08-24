using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Term.Utils
{
    /// <summary>
    /// Transforms DateTime? to short localized format for views
    /// </summary>
   public static class DateTimeHelper
    {
       public static string ToShortDateFormat(this DateTime? helper)
       {
           return helper.HasValue ? ((DateTime)helper).ToShortDateString() : String.Empty;
       }

       public static string ValueForPaginator(this DateTime? element)
       {
           if (element.HasValue) return ((DateTime)element).ToString("dd-MM-yyyy");
           return String.Empty;
       }

       /// <summary>
       /// Проверка текущий день выходной
       /// </summary>
       /// <param name="startingDate"></param>
       /// <returns></returns>
       private static bool IsDayOff(DateTime startingDate)
       {
           return (startingDate.DayOfWeek == DayOfWeek.Saturday || startingDate.DayOfWeek == DayOfWeek.Sunday);
       }

        /// <summary>
       /// Добавляет к дате число дней с учетом выходных
       /// </summary>
       /// <param name="startingDate">начальная дата </param>
       /// <param name="days">число дней</param>
       /// <returns></returns>
       /// 
       /// 
        public static DateTime AddDaysWithoutDaysOff(DateTime startingDate, int days)
        {
            for (int i = 0; i < days; i++)
            {
                startingDate=startingDate.AddDays(1);

                if (IsDayOff(startingDate)) startingDate = startingDate.AddDays(1);

                if (IsDayOff(startingDate)) startingDate = startingDate.AddDays(1);
            }

            return startingDate;
        }


    }
}
