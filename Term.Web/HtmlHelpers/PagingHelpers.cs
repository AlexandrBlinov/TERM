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
         /*   tag.AddCssClass("mw104"); */
            tag.MergeAttribute("style", "color:green;");
            tag.InnerHtml = EnumDescriptionProvider.GetDescription(enumValue);

            return MvcHtmlString.Create(tag.ToString());

          
        }

        /*
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
        */

         public static MvcHtmlString DateFormat(this HtmlHelper html,
           DateTime? sourceDate)
         {
             if (sourceDate == null)
             { return MvcHtmlString.Empty; }
             else
             { return MvcHtmlString.Create(String.Format("{0:d}", sourceDate)); }
         }

       
    }
}