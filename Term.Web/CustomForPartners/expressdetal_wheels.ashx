
<%@ WebHandler Language="C#" CodeBehind="expressdetal.ashx.cs" Class="ExpressDetalHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;
using Term.DAL;
using System.Linq;
using Term.Utils;

public class ExpressDetalHandler : IHttpHandler
{
    int _pointId = 4944; // головная точка автодока
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
               using(var service = new XMLService(db))
               {
                   var results = service.getDiskForPriceList(_pointId);
                   builder.Append("Код товара;Бренд;Модель;Наименование;Цвет диска;Ширина;Диаметр;Отв.;PCD;ET;DIA;Остаток;Срок доставки;Цена").AppendLine();
                           Array.ForEach(results.ToArray(), item =>
                       builder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}", item.ProductId, item.ProducerName.Equals(str_replica, StringComparison.InvariantCultureIgnoreCase) ? str_legeartis : item.ProducerName, item.ModelName, item.Name, (item.Color??"").ToUpper(), item.Width, item.Diametr, item.Holes, item.PCD, item.ET, item.DIA, item.RestMain, (byte)1, (item.Price??0).ToString(specifier)).AppendLine() 
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
