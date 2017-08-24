
<%@ WebHandler Language="C#" CodeBehind="partline.ashx.cs" Class="PartlineHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;
using Term.DAL;
using System.Linq;
using Term.Utils;

public class PartlineHandler : IHttpHandler
{
    int _pointId = 6101; // головная точка партлайна
    XMLService _service;
    private static string str_replica = "replica";
    private static string str_legeartis = "LegeArtis";
    static string specifier = "G29";
    static string aricle_pattern = @"(\w+)";
     
       public void ProcessRequest(HttpContext context)
        {
           var builder = new StringBuilder();
           using (var db = new AppDbContext())
           {
               using(var _service = new XMLService(db))
               {
                   var results = _service.getTyreForPriceList(_pointId);
                   
                   builder.Append("Код товара;Бренд;Модель;Наименование;Ширина;Высота;Диаметр;Остаток;Срок доставки;Цена;Фото").AppendLine();
                   Array.ForEach(results.ToArray(), delegate(YstProject.Services.PriceListTyreXml item)
                   {
                       var article_cut = String.IsNullOrEmpty(item.Article) ? String.Empty : Term.Utils.RegexExtractStringProvider.GetMatchedStringByOrder(item.Article, aricle_pattern, 1);
                       
                       if (article_cut!= String.Empty && !String.IsNullOrEmpty(item.ProducerName))
                       builder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", article_cut, item.ProducerName, item.ModelName, item.Name, item.Width, item.Height, item.Diametr, item.RestMain, (byte)1, (item.Price ?? 0).ToString(specifier),item.PathToPicture).AppendLine();
                   }
                   
           );
               }
           }
           context.Response.ContentType = "text/csv";
           // context.Response.ContentType = "text/plain";
           context.Response.AddHeader("Content-Disposition", "attachment;filename=tyres.csv");
           context.Response.Charset = "UTF-8";
           context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(builder.ToString());
        }
    
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}
