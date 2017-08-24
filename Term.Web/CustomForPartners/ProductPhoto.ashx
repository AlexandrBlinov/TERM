<%@ WebHandler Language="C#" CodeBehind="ProductPhoto.ashx.cs" Class="ProductPhotoHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;

public class ProductPhotoHandler : IHttpHandler
{


    public void ProcessRequest(HttpContext context)
    {
        var builder = new StringBuilder();
        var result = String.Empty;
        var productId = Convert.ToInt32(context.Request.QueryString["ProductId"].ToString());
        using (var db = new AppDbContext())
        {

            try
            {
                using(var service = new ProductService(db))
                {
                    result = service.GetProduct(productId).PathToRemotePicture;
                }
                byte[] imageData;
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    imageData = client.DownloadData(result);
                }
                if (result!=String.Empty)
                {
                    context.Response.ContentType = "image/png";
                    context.Response.OutputStream.Write(imageData, 0, imageData.Length);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            catch
            {
                if (result == String.Empty)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Product not found.");
                }
            }
        }
    }


    public bool IsReusable {
        get {
            return false;
        }
    }
}
