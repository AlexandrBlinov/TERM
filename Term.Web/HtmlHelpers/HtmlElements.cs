using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using YstProject.Services;

namespace Term.Web.HtmlHelpers
{
    public static class HtmlElements
    
    {

        static string specifierToFormatPrice = "G29";

       
        /// <summary>
        /// Checkbox - удаляем вспомогательные элементы hidden , которые мешают кастомизации
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString BasicCheckBoxFor<T>(this HtmlHelper<T> html,
                                                 Expression<Func<T, bool>> expression,
                                                 object htmlAttributes = null)
        {

            var result = html.CheckBoxFor(expression, htmlAttributes).ToString();
            const string pattern = @"<input name=""[^""]+"" type=""hidden"" value=""false"" />";
            var single = Regex.Replace(result, pattern, "");
            return MvcHtmlString.Create(single);
        }


        /// <summary>
        /// Отображает знак рубля или доллара в зависимости от культуры
        /// </summary>
        /// <param name="html"></param>
        /// <param name="price"></param>
        /// <param name="classImg"></param>
        /// <param name="classSpan"></param>
        /// <returns></returns>
        public static MvcHtmlString Price(this HtmlHelper html, decimal price, string classImg = "data-table-price-label", string classSpan = "data-table-price",string spanId=null)

        {   
            var culture =System.Threading.Thread.CurrentThread.CurrentUICulture;


            //id=details-sum
            //class=details-price-one
            TagBuilder span = new TagBuilder("span");
            if (spanId != null) span.GenerateId(spanId);
            span.AddCssClass(classSpan);
            span.SetInnerText(price.ToString(CultureInfo.InvariantCulture));

            TagBuilder img = new TagBuilder("img");
            img.AddCssClass(classImg);
            

            string result;
            
            if (culture.Name != Defaults.Culture_RU)
            {
                
                img.MergeAttribute("src", ConfigurationManager.AppSettings["PathToUsdPicture"]);
                img.MergeAttribute("alt", ConfigurationManager.AppSettings["PathToUsdPicture"]);
                result = img.ToString() + span.ToString();
                
            }
            else
            {
                img.MergeAttribute("src", ConfigurationManager.AppSettings["PathToRubPicture"]);
                img.MergeAttribute("alt", ConfigurationManager.AppSettings["PathToUsdPicture"]);
                result = span.ToString() + img.ToString();
            
            }

            return new MvcHtmlString(result);
            
                                
        }
            
                                
        /// <summary>
        /// Отображение цены (згнак рубля и доллара - спец символы)
        /// </summary>
        /// <param name="html"></param>
        /// <param name="price"></param>
        /// <param name="classImg"></param>
        /// <param name="classSpan"></param>
        /// <param name="spanId"></param>
        /// <returns></returns>

        public static MvcHtmlString Price2(this HtmlHelper html, decimal price, string classImg = "data-table-price-label", string classSpan = "data-table-price", string spanId = null)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            var dollar = @"&#36;";
            var rouble = @"&#8381;";

            //id=details-sum
            //class=details-price-one
            TagBuilder span = new TagBuilder("span");
            if (spanId != null) span.GenerateId(spanId);
            span.AddCssClass(classSpan);
            //  span.SetInnerText(price.ToString(CultureInfo.InvariantCulture));
            //span.SetInnerText(price.ToString(CultureInfo.InvariantCulture) + " &#8381;");


            if (culture.Name != Defaults.Culture_RU)
                span.InnerHtml = String.Format("{0}  {1}", dollar, price.ToString(CultureInfo.InvariantCulture));

            else
                span.InnerHtml = String.Format("{0}  {1}",  price.ToString(CultureInfo.InvariantCulture),rouble);
            

            string    result = span.ToString();
            

            return new MvcHtmlString(result);


        }


        public static MvcHtmlString Price3(this HtmlHelper html, decimal price, string classImg = "data-table-price-label", string classSpan = "data-table-price", string spanId = null)
        {

            

            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            var signdollar = @"&#36;";

            //id=details-sum
            //class=details-price-one
            var span = new TagBuilder("span");
            if (spanId != null) span.GenerateId(spanId);
            span.AddCssClass(classSpan);

            if (culture.Name == Defaults.Culture_RU) { 
            var spanRouble = new TagBuilder("span");
            spanRouble.AddCssClass("rouble");
            spanRouble.InnerHtml = "P";

            span.InnerHtml=  price.ToString(specifierToFormatPrice) + spanRouble.ToString();

            }
            else
            {
                var spanDollar = new TagBuilder("span");
                spanDollar.AddCssClass("dollar");
                spanDollar.InnerHtml = signdollar;
                             
                span.InnerHtml = spanDollar.ToString()+" "+price.ToString(CultureInfo.InvariantCulture) ;
            }  

            string result = span.ToString();
            

            return new MvcHtmlString(result);


        }

        public static SelectList ListItemsPerPage(int maxValue, int step)
        {
            var items = new List<SelectListItem>();
            for (var i = step; i <= maxValue; i = i + step)
            {
                var item = new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                };
                items.Add(item);
            }
            return new SelectList(items, "Value", "Text", 1);
        }

    }
}