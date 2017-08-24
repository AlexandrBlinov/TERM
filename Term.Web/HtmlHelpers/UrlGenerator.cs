using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Term.Web.Utils
{
    public static class UrlGenerator
    {

        private static string getLetter(char sign)
        {
            var corresp = new Dictionary<char, string>() { { 'а', "a" }, { 'б', "b" }, { 'в', "v" }, { 'г', "g" },
            {'е',"e"},{'ё',"yo"},{'ж',"zh"},{'з',"z"},{'и',"i"},{'й',"i"},{'к',"k"},{'л',"l"},{'м',"m"},{'н',"n"},{'о',"o"},{'п',"p"},{'р',"r"},{'с',"s"},{'т',"t"},{'у',"u"},{'ф',"f"},
            {'х',"kh"},{'ч',"ch"},{'ш',"sh"},{'щ',"sch"},{'ъ',""},{'ы',"y"},{'ь',""},{'э',"e"},{'ю',"u"},{'я',"ya"} };

            string result;
            if (!corresp.TryGetValue(sign, out result)) result = sign.ToString(); return result;
        }

        static public string GetUrlByTitle(string title)
        {
            title = Regex.Replace(title, @"[,.\s\/\)\(,]+", "-").ToLower().TrimEnd(new[] { '.', '-' });
            var arr = title.ToCharArray().Select(p => getLetter(p)).ToArray();
            return string.Join("", arr);
        }

        public static string Generate(params string[] segments)
        {
            var result = new StringBuilder();
            char slash = '/';

            foreach (var segment in segments)
            {
                if (segment != null)
                {
                    var str = segment.TrimEnd(slash).TrimStart(slash);

                    if (result.Length > 0) result.Append(slash);
                    result.Append(str);
                }
            }

            return result.ToString().ToLower();

        }


    }
}
