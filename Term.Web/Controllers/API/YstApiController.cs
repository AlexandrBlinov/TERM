using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using System.Web;
using YstProject.Services;
using YstTerm.Models;
using Yst.Services;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using System.Text.RegularExpressions;
using Yst.ViewModels;
using Term.DAL;
using Term.Web.Services;
using Yst.Context;


namespace Term.Web.Controllers.API
{
    public class YstApiController : ApiController, IDisposable
    {

        private PodborTyreDiskService _podborService = null;

        private readonly AppDbContext _dbContext;
        /*  private ProductService Products
          {
              get { return _productService ?? (_productService = new ProductService()); }

          } */

        public YstApiController() : this(new AppDbContext(), new PodborTyreDiskService())
        {

        }
        public YstApiController(AppDbContext dbContext, PodborTyreDiskService service)
        {
            _dbContext = dbContext;
            _podborService = service;
        }



        [HttpPost]
        [ActionName("uploadfile")]
        public bool UploadFile()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                var httpPostedFileName = HttpContext.Current.Request["UploadedImageName"];
                var name = User.Identity.Name;
                if (httpPostedFile != null)
                {
                    // Validate the uploaded image(optional)
                    try
                    {

                        String pathToPointPhotos = ConfigurationManager.AppSettings["PathToPointPhotos"];
                        //  String fullPathToDirectory = String.Concat(HttpContext.Server.MapPath(HttpContext.Request.ApplicationPath), ImportDirectory);

                        // Get the complete file path
                        // var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/pointphotos"), httpPostedFileName + ".jpg");
                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(pathToPointPhotos), httpPostedFileName + ".jpg");

                        // Save the uploaded file to "UploadedFiles" folder
                        httpPostedFile.SaveAs(fileSavePath);

                    }
                    catch { return false; }
                }
                return true;
            }
            return false;
        }






    }


}

