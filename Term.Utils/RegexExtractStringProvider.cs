using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Term.DAL;

namespace Term.Utils
{

    /// <summary>
    /// Класс для упрощения работы с регулярными выражениями
    /// </summary>
    public static class RegexExtractStringProvider
    {
        /// <summary>
        /// получаем n строку которая удовлетворяет паттерну (первая имеет номер=1)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string GetMatchedStringByOrder(string source,string pattern,int order)
        {
            var m = Regex.Match(source, pattern);
                if (m.Length > 0 && m.Groups.Count > order-1)
                    return m.Groups[m.Groups.Count - order].ToString();
                return String.Empty;
        }

        /// <summary>
        /// получаем все строки которые удовлетворяют паттерну
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>

        public static string[] GetArrayOfMathedStrings(string source, string pattern)
        {
            var result=new List<string>();
            var m = Regex.Match(source, pattern);
            if (m.Length > 0 && m.Groups.Count > 0)
                for (int i = 1; i < m.Groups.Count; i++) result.Add(m.Groups[i].ToString());

                
            return result.ToArray();
        }


        /// <summary>
        /// Возврат параметров для любого типоразмера в зависимости от вида товара
        /// </summary>
        /// <param name="tiporazmer"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        public static string[] GetParametersFromTyporazmer(string tiporazmer, ProductType productType)
        {
            string pattern = @"^(.*)\/(.*)\s*R(.*)$";
            if (productType==ProductType.Disk)  pattern = @"^(.*)x(.*)\/(.*)x(.*)\s+ET(.*)\s+D(.*)$";

            return GetParametersFromTiporazmer(tiporazmer,pattern);
                
        }



        private static string[] GetParametersFromTiporazmer(string tiporazmer, string pattern)
        {
            tiporazmer = Regex.Replace(tiporazmer, @"\s+", " ");
         //   string pattern = @"^(.*)x(.*)\/(.*)x(.*)\s+ET(.*)\s+D(.*)$";
            RegexOptions regexOptions = RegexOptions.None;
            Regex regex = new Regex(pattern,  regexOptions);
            string inputData = tiporazmer.Trim();
            string[] result = regex.Split(inputData).Where(res=>!String.IsNullOrEmpty(res)).ToArray();
            return result;
                       
            
        }

        

    }
}