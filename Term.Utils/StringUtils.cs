using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term.Utils
{





    /// <summary>
    /// helper class for wrking with strings
    /// </summary>
    public static class StringUtils

    {
        private static readonly double EtMinValue = -30;
        private static Char space = ' ';
        public static double ToDouble(this string str)
        {
            if (string.IsNullOrEmpty(str)) return EtMinValue;

            double d;

            if (!double.TryParse(str.Replace('.', ','), out d)) return EtMinValue;

            return d;
        }


        public static string ProductIdTo7Symbols(this int productId) { return productId.ToString().PadLeft(7, '0'); }


        public static int GetProductId(string code)
        {
            return Int32.Parse(code.TrimStart('0'));
        }

        public static string GetColourFromName(string name)
        {
            string[] splitName = name.Split(space);
            string color = name.Split(space).LastOrDefault().ToLower();
            if (color == "insert") color = String.Concat(splitName[6], "_", splitName[7]).ToLower();
            if (color == "(bk+y)") color = splitName[6].ToLower();
            return color ?? color.ToUpper();
        }


        /// <summary>
        /// Формирует строку диапазона дней , пр-р: 1-5 дн.
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public static string PrepareRangeDays(int startDay, int endDay)
        {

            if (startDay > 0 && startDay == endDay) return String.Format("{0} ", startDay);

            if (startDay > 0 && endDay > 0 && startDay != endDay) return String.Format("{0}-{1}  ", startDay, endDay);

            return String.Empty;
        }

        /// <summary>
        /// Возвращаем последнюю цифру из диапазона, т.е.: 2-3 =3, 2=2
        /// </summary>
        /// <param name="range"> диапазон пр-р 2-3</param>
        /// <returns></returns>
        public static string GetLastDayFromRange(string range)
        {
            Char separator = '-';
            return range.Split(separator).Last();
        }


        /// <summary>
        /// Функция возвращает список guid  из строки разделенной точкой с запятой
        /// </summary>
        /// <param name="ordersFromSuppliers"></param>
        /// <returns></returns>
        public static IList<Guid> GetArrayOfGuidsFromString(string ordersFromSuppliers)
        {
            var resultOfGuids  = new List<Guid>();

            if (String.IsNullOrEmpty(ordersFromSuppliers)) return resultOfGuids;

            var stringGuids =ordersFromSuppliers.Split(';');
            foreach (var stringGuid in stringGuids)
            {
                Guid result;
                if (Guid.TryParse(stringGuid, out result)) resultOfGuids.Add(result);
            }
            return resultOfGuids;
        }

        /// <summary>
        /// Получить инициалы ФИО Иванов И.И.
        /// </summary>
        /// <param name="Fio"></param>
        /// <returns></returns>
        public  static string GetFioInitials(string Fio)
        {            
            Char space = ' ';
            string[] words = Fio.TrimStart(space).TrimEnd(space).Split(space);
            if (words.Length != 3) return Fio;
           return words[0]+space+ words[1][0] + "." + words[2][0] + ".";

        }
        
    }
}
