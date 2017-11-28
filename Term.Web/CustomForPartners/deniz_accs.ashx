
<%@ WebHandler Language="C#" CodeBehind="deniz.ashx.cs" Class="DenizHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;
using Term.DAL;
using System.Linq;
using Term.Utils;

public class DenizHandler : IHttpHandler
{
    int _pointId = 6663; // головная точка партлайна
 
    static string specifier = "G29";
     
       public void ProcessRequest(HttpContext context)
        {
           var builder = new StringBuilder();
           using (var db = new AppDbContext())
           {
               using(var service = new XMLService(db))
               {
                   var results = service.getAccsForPriceList(_pointId);
                   builder.Append("Артикул;Бренд;Группа;Название;Вес;Объем;Остаток;Цена;Путь к картинке").AppendLine();
                           Array.ForEach(results.ToArray(), item =>
                       builder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8}", item.Article,  item.ProducerName, 
                       item.CategoryName, item.Name, item.Weight,item.Volume,item.RestMain, (item.Price??0).ToString(specifier),item.PathToPicture).AppendLine() 
                   );
               }
           }
           context.Response.ContentType = "text/csv";
           // context.Response.ContentType = "text/plain";
           context.Response.AddHeader("Content-Disposition", "attachment;filename=accs.csv");
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
