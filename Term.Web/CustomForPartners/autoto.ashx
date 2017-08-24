
<%@ WebHandler Language="C#" CodeBehind="autoto.ashx.cs" Class="AutotoHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;
using Term.DAL;
using System.Linq;
using Term.Utils;

public class AutotoHandler : IHttpHandler
{
    int _pointId = 3342; // головная точка автодока
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
                   var resultst = _service.getTyreForPriceList(_pointId);
                   
                   builder.Append("Код товара;Бренд;Фото;ШтрихКод").AppendLine();
                   Array.ForEach(resultst.ToArray(), delegate(YstProject.Services.PriceListTyreXml item)
                   {
                       var article_cut = String.IsNullOrEmpty(item.Article)? String.Empty: Term.Utils.RegexExtractStringProvider.GetMatchedStringByOrder(item.Article, aricle_pattern,1);

                       if (article_cut != String.Empty && !String.IsNullOrEmpty(item.ProducerName))
                           builder.AppendFormat("{0};{1};{2}; ", article_cut, item.ProducerName, item.PathToPicture).AppendLine();
                   });
                   
                  var  resultsd = _service.getDiskForPriceList(_pointId);

                  foreach (var item in resultsd.AsEnumerable())
                  {
                      if (!String.IsNullOrEmpty(item.PathToRemotePicture)&& !String.IsNullOrEmpty(item.Barcode))
                      builder.AppendFormat("{0};{1};{2};{3}", item.ProductId,
                          item.ProducerName.Equals(str_replica, StringComparison.InvariantCultureIgnoreCase)
                              ? str_legeartis
                              : item.ProducerName, item.PathToRemotePicture, item.Barcode).AppendLine();
                  }
                           
                   
                        var resultacs = _service.getAccsForPriceList(_pointId);

                   foreach (var item in resultacs.AsEnumerable())
                   {
                       if (!String.IsNullOrEmpty(item.PathToPicture) && !String.IsNullOrEmpty(item.Barcode))
                   builder.AppendFormat("{0};{1};{2};{3}", item.ProductId, item.ProducerName, item.PathToPicture, item.Barcode).AppendLine();    
                   }
                   
                    
                   
          
               }
           }
           context.Response.ContentType = "text/csv";
           context.Response.ContentType = "text/plain";
         //  context.Response.AddHeader("Content-Disposition", "attachment;filename=products.csv");
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
