using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Yst.ViewModels;
using YstTerm.Models;
using System.Text.RegularExpressions;
using Term.DAL;
using YstProject.Services;
using System.Configuration;

using Term.Utils;


namespace Term.Web.HtmlHelpers
{

    public static class MvcHtmlStringExtensions
    {

        private static readonly Regex OpeningTagPattern;

        static MvcHtmlStringExtensions()
        {
            OpeningTagPattern = new Regex("<[a-zA-Z]*");
        }

        public static MvcHtmlString DisabledIf(this MvcHtmlString controlHtml, bool isDisabled)
        {
            if (!isDisabled) return controlHtml;
            return
                new MvcHtmlString(OpeningTagPattern.Replace(controlHtml.ToString(),
                    x => string.Format("{0} disabled=\"disabled\"", x.Groups[0])));
        }

    }

    public static class ThumbnailHelper
    {
        const string IMAGE_TAG = "<img src=\"{0}\" alt=\"{1}\" {2} {3} />"; //0 - source, 1 - Alt tag, 2 - style, 3 - class

        public static string ThumbnailNew(this HtmlHelper htmlHelper, string productType, string file)
        {
            //file = file.Replace("~", "/");
            return string.Format("/Thumbnail/{0}/{1}", productType, file);

        }

        public static string Thumbnail(this HtmlHelper htmlHelper,  string file)
        {
            var result = $"/thlink/{file}.png";
            return result;

        }

        public static string FullPathToImage(this HtmlHelper htmlHelper, string productType, string file)
        {
          //  return string.Format("/Thumbnail/{0}/{1}", productType, file);
            file = file.Replace("~", "/");
            return ConfigurationManager.AppSettings["RemotePathToPictures"] + Defaults.PathToFullImages[productType] + file+".png";

        }

        public static string ThumbnailNew(this HtmlHelper htmlHelper, string file)
        {
            return string.Format("/Thumbnail/{0}",  file);
         }
       

        public static string Thumbnail(this HtmlHelper htmlHelper, string controller, string action, int w, int h, string file)
        {
            string location = ThumbnailLocation(htmlHelper, controller, action, w, h, file);

            //WC3 - ALT tag always needed so when not set then set to the filename.
            return string.Format(IMAGE_TAG, location, file, null, null);
        }

        public static string Thumbnail(this HtmlHelper htmlHelper, string controller, string action, int w, int h, string file, string altText = "")
        {
            string location = ThumbnailLocation(htmlHelper, controller, action, w, h, file);

            return string.Format(IMAGE_TAG, location, altText, null, null);
        }

        public static string Thumbnail(this HtmlHelper htmlHelper, string controller, string action, int w, int h, string file, string altText, IDictionary<string, object> htmlAttributes)
        {
            string location = string.Format("/{0}/{1}/{2}/{3}/{4}/{5}", controller, action, w, h, file);

            StringBuilder sb = new StringBuilder();
            string style = string.Empty;

            if (htmlAttributes != null && htmlAttributes.Count > 0)
            {
                BuildStyle(htmlAttributes, ref sb, ref style);
            }

            return string.Format(IMAGE_TAG, location, altText, sb, style);
        }

        private static void BuildStyle(IDictionary<string, object> htmlAttributes, ref StringBuilder sb, ref string style)
        {
            sb = new StringBuilder("style=\"");
            foreach (KeyValuePair<string, object> htmlAttribute in htmlAttributes)
            {
                if (htmlAttribute.Key == "class")
                    style = string.Format("class=\"{0}\"", htmlAttribute.Value);
                else
                    sb.Append(htmlAttribute.Key + ":" + htmlAttribute.Value + ";");
            }
            sb.Append("\"");
        }

        public static string ThumbnailLocation(this HtmlHelper htmlHelper, string controller, string action, int w, int h, string file)
        {
            return string.Format("/{0}/{1}/{2}/{3}/{4}", controller, action, w, h, file);

        }
    }

    public static class PagingHelpers
    {
        public static MvcHtmlString Round(this HtmlHelper html, decimal? price)
        {   
            return  MvcHtmlString.Create(   Math.Round(price??0, 0, MidpointRounding.AwayFromZero).ToString() );
        }

      /*  public static MvcHtmlString TrPriceRule(this HtmlHelper html, string PriceType, decimal Discount)
        {

            string result = String.Empty;
            
                result = @"<td><input class=""discount"" max="" 100"" min=""-100"" type=""number"" value=""{0}"" name=""base"" {1}>
                 <input class=""checkbox_del"" type=""checkbox"" name=""base"" {2} /> </td>
                <td class=""zakup""><input class=""discount"" max=""100"" min=""0"" type=""number"" value=""{3}"" name=""zakup"" {4} />
                <input class=""checkbox_del"" type=""checkbox"" name=""zakup"" {5} /> </td>
                <td><input class=""checkbox_del"" type=""checkbox"" name=""recommend"" {6} /> </td>
                <td><input class=""checkbox_del"" type=""checkbox"" name=""dont_show_price"" {7} /> </td>
                <td><input class=""checkbox_del"" type=""checkbox"" name=""dont_show_producer"" {8} /> </td>";

                switch (PriceType)
                {
                    case "base": return MvcHtmlString.Create(String.Format(result, Discount.ToString("0.##"), "", "checked", "5", "disabled", "", "", "", ""));
                case "zakup": return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", Discount.ToString("0.##"), "", "checked", "", "", ""));
                case "recommend": return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "checked", "", ""));
               // case "dont_show_price": return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "", "checked", ""));
                case "dont_show_producer": return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "", "", "checked"));
                default: return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "", "checked", ""));
                }
        }*/

        public static MvcHtmlString TrPriceRuleEnum(this HtmlHelper html, PriceTypeEnum PriceType, decimal Discount, bool RusUser, int ProducerId)
        {

            string result = String.Empty;

            if (RusUser)
            {
                result = @"
                <td>
                    <input class=""discount"" max="" 100"" min=""-100"" type=""number"" value=""{0}"" name=""base"" {1}>
                    <input id=""CheckBasePrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""base"" {2} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckBasePrice{9}""> </label> 
                </td>
                <td class=""zakup"">
                    <input class=""discount"" max=""100"" min=""0"" type=""number"" value=""{3}"" name=""zakup"" {4} />
                    <input id=""CheckZakupPrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""zakup"" {5} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckZakupPrice{9}""> </label> 
                </td>
                <td>
                    <input id=""CheckRecPrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""recommend"" {6} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckRecPrice{9}""> </label> 
                </td>
                <td>
                    <input id=""CheckDontShowPrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""dont_show_price"" {7} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckDontShowPrice{9}""> </label> 
                </td>
                <td>
                    <input id=""CheckDontShowProd{9}"" class=""checkbox_del"" type=""checkbox"" name=""dont_show_producer"" {8} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckDontShowProd{9}""> </label> 
                </td>";
            }
            else
            {
                result = @"
                <td class=""invisible"">
                    <input class=""discount"" max="" 100"" min=""-100"" type=""number"" value=""{0}"" name=""base"" {1}>
                    <input id=""CheckBasePrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""base"" {2} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckBasePrice{9}""> </label> 
                </td>
                <td class=""zakup"">
                    <input class=""discount"" max=""100"" min=""0"" type=""number"" value=""{3}"" name=""zakup"" {4} />
                    <input id=""CheckZakupPrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""zakup"" {5} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckZakupPrice{9}""> </label> 
                </td>
                <td class=""invisible"">
                    <input id=""CheckRecPrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""recommend"" {6} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckRecPrice{9}""> </label> 
                </td>
                <td>
                    <input id=""CheckDontShowPrice{9}"" class=""checkbox_del"" type=""checkbox"" name=""dont_show_price"" {7} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckDontShowPrice{9}""> </label> 
                </td>
                <td>
                    <input id=""CheckDontShowProd{9}"" class=""checkbox_del"" type=""checkbox"" name=""dont_show_producer"" {8} /> 
                    <label class=""checkbox_del-label no-select"" for=""CheckDontShowProd{9}""> </label> 
                </td>";
            }



            switch (PriceType)
            {
                case PriceTypeEnum.Base: return MvcHtmlString.Create(String.Format(result, Discount.ToString("0.##"), "", "checked", "5", "disabled", "", "", "", "", ProducerId));
                case PriceTypeEnum.Zakup: return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", Discount.ToString("0.##"), "", "checked", "", "", "", ProducerId));
                case PriceTypeEnum.Recommend: return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "checked", "", "", ProducerId));
                // case "dont_show_price": return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "", "checked", ""));
                case PriceTypeEnum.Dont_Show_Producer: return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "", "", "checked", ProducerId));
                default: return MvcHtmlString.Create(String.Format(result, "5", "disabled", "", "5", "disabled", "", "", "checked", "", ProducerId));
            }
        }


        public static MvcHtmlString SeasonOrderStatus(this HtmlHelper html, DateTime? DeliveryDate)
        {

            string result = String.Empty;

            if (DeliveryDate >= DateTime.Now)
            {
                result = @"<td style=""color:green"">Действующий</td>";
            }
            else
            {
                result = @"<td style=""color:grey"">Закрыт</td>";
            }

            return MvcHtmlString.Create(result);
        }

      /*  public static MvcHtmlString SeasonOrderCondition(this HtmlHelper html, SeasonOrderStatus Status)
        {

            string result = String.Empty;

            if (Status == SeasonOrderStatus.New)
            {
                result = @"<td style=""color:green"">Создан</td>";
            }
            if (Status == SeasonOrderStatus.Chancelled)
            {
                result = @"<td style=""color:red"">Отменён</td>";
            }

            return MvcHtmlString.Create(result);
        } */

       
           //return MvcHtmlString.Create( String.Format(result,"5","disabled",""));
            
            

           /*<td>
                    <input class="discount" max="100" min="-100" type="number" value="5" name="base" disabled>
                    <input class="checkbox_del" type="checkbox" name="base" />
                </td>
                <td>
                    <input class="discount" max="100" min="0" type="number" value="5" name="zakup" disabled>
                    <input class="checkbox_del" type="checkbox" name="zakup" />
                </td>
                <td><input class="checkbox_del" type="checkbox" name="recommend" checked /></td>
                <td>
                    <input class="checkbox_del" type="checkbox" name="dont_show_price" />
                </td>
                <td>
                    <input class="checkbox_del" type="checkbox" name="dont_show_producer" />
                </td> */
        
        
        

        public static MvcHtmlString UrlPagedAction(this HtmlHelper html, int i, Func<int, string> pageUrl)
        {
            return MvcHtmlString.Create(pageUrl(i)); 
        
        }

        /// <summary>
        /// Отображает картинку сезона
        /// </summary>
        /// <param name="html"></param>
        /// <param name="season">сезон (строка)</param>
        /// <returns></returns>
        public static MvcHtmlString SeasonImage(this HtmlHelper html, string season)
        {
            Dictionary<string,string> seasonImages = new Dictionary<string, string> {
            {"summer","season_summer"},
            {"winter","season_winter"},
            {"allseason","season_all"}};
            try
            {
                string value;
                if(    seasonImages.TryGetValue(season, out value))
            { 
                var tag = new TagBuilder("img");
                tag.MergeAttribute("src", String.Format("/Content/img/{0}.png", value));
                return MvcHtmlString.Create(tag.ToString());
                    
            }
            else return MvcHtmlString.Empty;
            }
            catch { return MvcHtmlString.Empty; }
            


        }

        public static MvcHtmlString OrderStatusWithColor(this HtmlHelper html, Enum enumValue )
        {
            TagBuilder tag = new TagBuilder("span");
            tag.AddCssClass("mw104");
            tag.MergeAttribute("style", "color:green;");
            tag.InnerHtml = EnumDescriptionProvider.GetDescription(enumValue);

            return MvcHtmlString.Create(tag.ToString());

           /* switch (Status)
            {
                case 3: return MvcHtmlString.Create("<td class=\"mw104\" style=\"color:red;\">Отменен</td>");
                case 2: return MvcHtmlString.Create("<td class=\"mw104\" style=\"color:green;\">Подтвержден</td>");
                case 4: return MvcHtmlString.Create("<td class=\"mw104\" style=\"color:blue;\">Отгружен</td>");
                default: return MvcHtmlString.Create("<td class=\"mw104\">" + Status + "</td>");
            }
            */
        }

         private static string getstrAddToUrl(string strAddToUrl,int page=1)
        {
            if (String.IsNullOrEmpty(strAddToUrl))
            {
                return ("?page=" + page.ToString());
            }
            else
            {
                return ("?" + strAddToUrl + "&page=" + page.ToString());
            }
            
        
        }

         public static MvcHtmlString DateFormat(this HtmlHelper html,
           DateTime? sourceDate)
         {
             if (sourceDate == null)
             { return MvcHtmlString.Empty; }
             else
             { return MvcHtmlString.Create(String.Format("{0:d}", sourceDate)); }
         }

        /*
        public static MvcHtmlString PageLinks(this HtmlHelper html,
          PagingInfo pagingInfo,
          Func<int, string> pageUrl
            //delegate string pageUrl(int inputNumber)
          )

        {
            if (pagingInfo.TotalPages <= 1)
                return (MvcHtmlString.Empty);


            var RequestQueryString = html.ViewContext.HttpContext.Request.QueryString;

            List<string> items = new List<string>();

            foreach (String name in RequestQueryString)
                if (name != "page")
                    items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(RequestQueryString[name])));

            String strAddToUrl = String.Join("&", items.ToArray());

    

            StringBuilder result = new StringBuilder();
            if (pagingInfo.CurrentPage > 1)
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1) + getstrAddToUrl(strAddToUrl,pagingInfo.CurrentPage - 1));
                tag.InnerHtml = "<<";
                result.Append(tag.ToString());
            }
           

            if (pagingInfo.TotalPages < 10)
            {
                for (int i = 1; i <= pagingInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                    tag.MergeAttribute("href", pageUrl(i) + getstrAddToUrl(strAddToUrl, i));
                    tag.InnerHtml = i.ToString();
                    if (i == pagingInfo.CurrentPage)
                        tag.AddCssClass("selected");
                    result.Append(tag.ToString());
                }
            }
            else // выводим  первую текущую и последнюю
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(1) + getstrAddToUrl(strAddToUrl, 1));
                tag.InnerHtml = "1";
                if ( pagingInfo.CurrentPage==1)
                    tag.AddCssClass("selected");
                result.Append(tag.ToString());

                if (Math.Max(pagingInfo.CurrentPage - 2, 1) > 1)
                    result.Append("<a>...</a>");

                for (int j = Math.Max(pagingInfo.CurrentPage -2, 1); j <= Math.Min(pagingInfo.CurrentPage + 2, pagingInfo.TotalPages); j++)
                    
                    if (j!= 1 && j!= pagingInfo.TotalPages)
                    {
                        tag = new TagBuilder("a"); // Construct an <a> tag
                        tag.MergeAttribute("href", pageUrl(j) + getstrAddToUrl(strAddToUrl, j));
                        tag.InnerHtml = j.ToString();
                        if (j==pagingInfo.CurrentPage)
                        tag.AddCssClass("selected");
                        result.Append(tag.ToString());

                    }

                if (Math.Max(pagingInfo.CurrentPage + 2, 1) < pagingInfo.TotalPages)
                    result.Append("<a>...</a>");

                tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(pagingInfo.TotalPages) + getstrAddToUrl(strAddToUrl, pagingInfo.TotalPages));
                tag.InnerHtml = pagingInfo.TotalPages.ToString();
                if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
                    tag.AddCssClass("selected");
                result.Append(tag.ToString());

            }

            if (pagingInfo.CurrentPage < pagingInfo.TotalPages)
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1) + getstrAddToUrl(strAddToUrl, pagingInfo.CurrentPage + 1));
                tag.InnerHtml = ">>";
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
        */

        /*
        public static MvcHtmlString PageButtons(this HtmlHelper html,
         PagingInfo pagingInfo,
         Func<int, string> pageUrl
            //delegate string pageUrl(int inputNumber)
         )
        {

            var RequestQueryString = html.ViewContext.HttpContext.Request.QueryString;

            List<string> items = new List<string>();

            foreach (String name in RequestQueryString)
                if (name != "page")
                    items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(RequestQueryString[name])));

            String strAddToUrl = String.Join("&", items.ToArray());



            StringBuilder result = new StringBuilder();
            if (pagingInfo.CurrentPage > 1)
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1) + getstrAddToUrl(strAddToUrl, pagingInfo.CurrentPage - 1));
                tag.InnerHtml = "<<";
                result.Append(tag.ToString());
            }


            if (pagingInfo.TotalPages < 10)
            {
                for (int i = 1; i <= pagingInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                    tag.MergeAttribute("href", pageUrl(i) + getstrAddToUrl(strAddToUrl, i));
                    tag.InnerHtml = i.ToString();
                    if (i == pagingInfo.CurrentPage)
                        tag.AddCssClass("selected");
                    result.Append(tag.ToString());
                }
            }
            else // выводим  первую текущую и последнюю
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(1) + getstrAddToUrl(strAddToUrl, 1));
                tag.InnerHtml = "1";
                if (pagingInfo.CurrentPage == 1)
                    tag.AddCssClass("selected");
                result.Append(tag.ToString());

                if (Math.Max(pagingInfo.CurrentPage - 2, 1) > 1)
                    result.Append("<a>...</a>");

                for (int j = Math.Max(pagingInfo.CurrentPage - 2, 1); j <= Math.Min(pagingInfo.CurrentPage + 2, pagingInfo.TotalPages); j++)

                    if (j != 1 && j != pagingInfo.TotalPages)
                    {
                        tag = new TagBuilder("a"); // Construct an <a> tag
                        tag.MergeAttribute("href", pageUrl(j) + getstrAddToUrl(strAddToUrl, j));
                        tag.InnerHtml = j.ToString();
                        if (j == pagingInfo.CurrentPage)
                            tag.AddCssClass("selected");
                        result.Append(tag.ToString());

                    }

                if (Math.Max(pagingInfo.CurrentPage + 2, 1) < pagingInfo.TotalPages)
                    result.Append("<a>...</a>");

                tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(pagingInfo.TotalPages) + getstrAddToUrl(strAddToUrl, pagingInfo.TotalPages));
                tag.InnerHtml = pagingInfo.TotalPages.ToString();
                if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
                    tag.AddCssClass("selected");
                result.Append(tag.ToString());

            }

            if (pagingInfo.CurrentPage < pagingInfo.TotalPages)
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1) + getstrAddToUrl(strAddToUrl, pagingInfo.CurrentPage + 1));
                tag.InnerHtml = ">>";
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString PageMyLinks(this HtmlHelper html,
            PagingInfo pagingInfo, Func<string,string> pageUrl            
            )
        {

           

            var RequestQueryString = html.ViewContext.HttpContext.Request.QueryString;

            List<string> items = new List<string>();

            foreach (String name in RequestQueryString)
                if (name != "page")
                    items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(RequestQueryString[name])));

            String  str = String.Join("&", items.ToArray());


            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a"); // Construct an <a> tag
                //tag.MergeAttribute("href", pageUrl(i.ToString())+"?"+str+"&page=" + i.ToString());

                if (str!="")
                { tag.MergeAttribute("href", pageUrl(i.ToString()) + "?" + str + "&page=" + i.ToString()); }
                else
                { tag.MergeAttribute("href", pageUrl(i.ToString()) + "?page=" + i.ToString()); }

                    tag.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                    tag.AddCssClass("selected");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }

         
            */
    }
}