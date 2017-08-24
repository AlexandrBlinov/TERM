
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

                var results = service.getAccsForPriceList(_pointId);
                builder.Append("Код товара;Артикул;Наименование;Группа;Производитель;Вес;Объём;Остаток;Цена;Фото").AppendLine();
                Array.ForEach(results.ToArray(), item =>
            builder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", item.ProductId,item.Article,
            item.Name,item.CategoryName,item.ProducerName,item.Weight, item.Volume,
            item.RestMain, (item.Price??0).ToString(specifier), item.PathToPicture).AppendLine()
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
