
<%@ WebHandler Language="C#" CodeBehind="autodoc.ashx.cs" Class="ClientHandler" %>
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
    int _pointId = 3762; 
    private static string str_replica = "replica";
    private static string str_legeartis = "LegeArtis";
    static string specifier = "G29";
     
       public void ProcessRequest(HttpContext context)
        {
           var builder = new StringBuilder();
           using (var db = new AppDbContext())
           {
               using(var service = new XMLService(db))
               {
                   var results = service.getDiskForPriceList(_pointId);
                   builder.Append("Код товара;Бренд;Модель;Наименование;Цвет диска;Ширина;Диаметр;Отв.;PCD;ET;DIA;Вес;Объём;Остаток;Цена;Рек. розн. цена;Фото").AppendLine();
                           Array.ForEach(results.ToArray(), item =>
                       builder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16}", item.ProductId, item.ProducerName.Equals(str_replica, StringComparison.InvariantCultureIgnoreCase) ? str_legeartis : item.ProducerName, item.ModelName, item.Name, (item.Color ?? "").ToUpper(), item.Width, item.Diametr, item.Holes, item.PCD, item.ET, item.DIA, item.Weight, item.Volume, item.RestMain, (item.Price ?? 0).ToString(specifier), (item.Price2 ?? 0).ToString(specifier), item.PathToRemotePicture).AppendLine() 
                   );
               }
           }
           context.Response.ContentType = "text/csv";
           // context.Response.ContentType = "text/plain";
           context.Response.AddHeader("Content-Disposition", "attachment;filename=wheels.csv");
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
