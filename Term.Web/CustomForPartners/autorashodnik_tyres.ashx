
<%@ WebHandler Language="C#" CodeBehind="autorashodnik.ashx.cs" Class="ClientHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;
using Term.DAL;
using System.Linq;
using Term.Utils;

public class ClientHandler : IHttpHandler
{
    int _pointId = 4906;
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
                   
                   builder.Append("Код товара;Артикул;Вид товара;Бренд;Модель;Наименование;Ширина;Высота;Диаметр;Вес;Объём;Индекс скорости;Индекс нагрузки;Сезон;Остаток;Цена;Рек. розн. цена;Фото").AppendLine();
                   Array.ForEach(results.ToArray(), delegate(YstProject.Services.PriceListTyreXml item)
                   {
                       var article_cut = String.IsNullOrEmpty(item.Article) ? String.Empty : Term.Utils.RegexExtractStringProvider.GetMatchedStringByOrder(item.Article, aricle_pattern, 1);
                       
                       if (article_cut!= String.Empty && !String.IsNullOrEmpty(item.ProducerName))
                       builder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17}",item.ProductIdTo7S, article_cut,item.ParentName, item.ProducerName, item.ModelName, item.Name, item.Width, item.Height, item.Diametr,item.Weight,item.Volume, item.SpeedIndex, item.LoadIndex,item.Season, item.RestMain, (item.Price ?? 0).ToString(specifier), (item.Price2 ?? 0).ToString(specifier),item.PathToPicture).AppendLine();
                   }
                   
           );
               }
           }
           context.Response.ContentType = "text/csv";
           //context.Response.ContentType = "text/plain";
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
