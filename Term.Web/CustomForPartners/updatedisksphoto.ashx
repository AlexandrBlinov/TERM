
<%@ WebHandler Language="C#" CodeBehind="updatedisksphoto.ashx.cs" Class="UpdatedisksphotoHandler" %>
using System;
using System.Web;
using Yst.Context;
using Yst.Services;
using System.Text;
using Term.DAL;
using System.Linq;
using Term.Utils;
using YstTerm.Models;
using System.Collections.Generic;
using System.Data.Entity;
using YstProject.Services;
using Term.Web.HtmlHelpers;
using System.IO;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

public class UpdatedisksphotoHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        using (var db = new AppDbContext())
        {

            db.DbActionLogs.Add(new DbActionLogs("updated pictures of tyres start"));
            db.SaveChanges();


            var products = db.Set<Product>().Include(pr => pr.Producer).Include(m => m.Model).Where(p => p.ProductType == ProductType.Tyre && p.Producer.Active && p.ModelId != null && p.PathToRemotePicture == null
            && db.Set<RestOfProduct>().Any(rop => rop.ProductId == p.ProductId));

            int counter = 0;
            foreach (var product in products)
            {

                var path =  PictureUtility.GetPictureOfTyre(product.ModelId);
                if (!String.IsNullOrEmpty(path))
                {
                    if (UrlChecker.CheckIfUrlExists(path))
                    {

                        counter++;
                        product.PathToRemotePicture = path;
                    }
                }
                

            }

            db.DbActionLogs.Add(new DbActionLogs("updated pictures of tyres end. Updated="+counter.ToString()));
            db.SaveChanges();
        }
    }



    public bool IsReusable {
        get {
            return false;
        }
    }
}
