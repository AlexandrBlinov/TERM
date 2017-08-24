using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Configuration;
using System.Threading.Tasks;
using YstProject.Services;

namespace Term.Web.Controllers
{
    /// <summary>
    /// Контроллер для отображения миниатюр (thumbnails)
    /// </summary>
    public class ThumbnailController : Controller
    {
      
     [OutputCache(CacheProfile = "ThumbnailCacheProfile", VaryByParam = "file")]
        public async Task<ActionResult> Index(string productType, string file)
        {
            
            var width = Defaults.Sizes[productType].Width; var height = Defaults.Sizes[productType].Height;

            string pathToThumbnails = HttpContext.Server.MapPath(HttpContext.Request.ApplicationPath) + ConfigurationManager.AppSettings["PathToThumbnail"];
            string fileStub = HttpContext.Server.MapPath(HttpContext.Request.ApplicationPath) + @"Content\img\ImageNotFound.png";

            if (String.IsNullOrEmpty(file)) return File(fileStub, "image/jpg");

           

            string remotefilename = String.Format("{0}.png", file);
            string localfilename = String.Format("{0}.png", file);
            

            var localFilePath = Path.Combine(pathToThumbnails, localfilename);
            

            Bitmap finalBmp = null, remoteImage = null;

            if (!System.IO.File.Exists(localFilePath))
            {
                try
                {

                    string url = ConfigurationManager.AppSettings["RemotePathToPictures"] + Defaults.PathToFullImages[productType] + remotefilename;
                 

                    WebRequest request = WebRequest.Create(url);

                    var response = (HttpWebResponse)(await request.GetResponseAsync());
                    remoteImage = (Bitmap)Bitmap.FromStream(response.GetResponseStream());

                    width = (uint)(height * remoteImage.Width / remoteImage.Height);

                    finalBmp = new Bitmap((int)width, (int)height);
                    using (var g = Graphics.FromImage(finalBmp))
                    {
                        g.InterpolationMode = InterpolationMode.High;
                        g.FillRectangle(Brushes.White, 0, 0, width, height);
                        g.DrawImage(remoteImage, 0, 0, width, height);

                    }
                    await Task.Factory.StartNew(() => finalBmp.Save(localFilePath, ImageFormat.Png));

                    return File(localFilePath, "image/jpg");
                }
                catch
                {
                    localFilePath = fileStub;
                }
                finally
                { if (finalBmp != null) finalBmp.Dispose();
                if (remoteImage != null) remoteImage.Dispose();
                
                }

           }


            return File(localFilePath, "image/jpg");
        }
        

    }
}
