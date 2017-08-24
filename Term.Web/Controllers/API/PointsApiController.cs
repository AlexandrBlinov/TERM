using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Yst.Context;
using YstProject.Services;
using Term.DAL;
using Term.Web.Filters;
using YstTerm.Models;


namespace Term.Web.Controllers.API
{

    /// <summary>
    /// Handles points of clients
    /// </summary>
     [AdminHashAuth]
    public class PointsApiController : ApiController
    {
        AppDbContext _dbContext;
        
            public PointsApiController ():this(new AppDbContext())
	      {

	      }
            public PointsApiController (AppDbContext db)
	    {
                _dbContext=db;
	    }
        //
        // GET api/pointsapi
        //  
        public IEnumerable<PartnerPoint> Get()
        {
            if (Request.Headers.Contains(Defaults.Hash) && Request.Headers.GetValues(Defaults.Hash).FirstOrDefault() == ConfigurationManager.AppSettings[Defaults.Hash])
               
            {     _dbContext.Configuration.ProxyCreationEnabled = false;
                    return _dbContext.Set<PartnerPoint>().ToList(); }
                           
                 return  Enumerable.Empty<PartnerPoint>();
        }

        /// <summary>
        /// Updates partner point from 1S if data is correct
        /// model.ContactFIO
        /// </summary>
        /// <param name="?"></param>

        [HttpPost]
       public int UpdatePoint([FromBody]PartnerPoint modeldto) {
       
          
            if (modeldto.PartnerPointId>0)
            {
            var point=_dbContext.PartnerPoints.FirstOrDefault(p=>p.PartnerPointId==modeldto.PartnerPointId);

                if (point!=null)
                {
                  point.ContactFIO=modeldto.ContactFIO;
                  point.Address=modeldto.Address;
                  if (point.DaysToDepartment>1) point.DaysToDepartment=modeldto.DaysToDepartment;
                  if (point.DaysToMainDepartment > 1) point.DaysToMainDepartment=modeldto.DaysToMainDepartment;
                  if (point.DepartmentId>1)   point.DepartmentId=modeldto.DepartmentId;
                  point.SaleDirection=modeldto.SaleDirection;
                  point.PhoneNumber=modeldto.PhoneNumber;
                      
                }
                _dbContext.SaveChanges();
                return 0;
                }
            return -1;     
            }

        /// <summary>
        /// Возвращает мини картинку (200 x height) партнерской точки 
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetPictureOfPoint(string pointId)
        {
            int fixed_WidthOrHeight = 200;
            var httpResponseMessage = new HttpResponseMessage();

            var request = HttpContext.Current.Request;
            
            var domainpath=String.Format("{0}{1}{2}{3}",request.Url.Scheme , System.Uri.SchemeDelimiter , request.Url.Host , request.Url.Port==80?"":":"+request.Url.Port.ToString());

            string pointIdUrl = "/Content/pointphotos/Point" + pointId + ".jpg";
            string imgNotFoundUrl = ConfigurationManager.AppSettings["PathToImageNotFound"];

            string appPath = HttpContext.Current.Request.ApplicationPath;
            string physicalPath = HttpContext.Current.Request.MapPath(appPath);
            
            
            byte[] fileContents;
            Bitmap image;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    WebRequest wr = WebRequest.Create(domainpath+pointIdUrl);

                    HttpWebResponse response = (HttpWebResponse)(await wr.GetResponseAsync());
                    image = (Bitmap)Bitmap.FromStream(response.GetResponseStream());

                    var curHeight = image.Height;
                    var curWidth = image.Width;
                    int newHeight, newWidth;

                    if (curHeight >= curWidth)
                    {
                        newHeight = fixed_WidthOrHeight;
                        newWidth = curWidth * fixed_WidthOrHeight / curHeight;
                    }
                    else
                    {
                        newWidth = fixed_WidthOrHeight;
                        newHeight = curHeight * fixed_WidthOrHeight / curWidth;
                    }

                    Bitmap finalBmp = new Bitmap((int)newWidth, (int)newHeight);
                    using (var g = Graphics.FromImage(finalBmp))
                    {
                        g.InterpolationMode = InterpolationMode.High;
                        g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                        g.DrawImage(image, 0, 0, newWidth, newHeight);

                    }
                    await Task.Factory.StartNew(() => finalBmp.Save(memoryStream, ImageFormat.Jpeg));
                    fileContents = memoryStream.ToArray();
                    
                }
                catch (WebException ex)
                {
                    
                    
                    var path = Path.Combine(physicalPath, imgNotFoundUrl);
                    var imagenf = Bitmap.FromFile(path);
                    Bitmap finalBmp = new Bitmap((int)fixed_WidthOrHeight, (int)151);
                    using (var g = Graphics.FromImage(finalBmp))
                    {
                        g.InterpolationMode = InterpolationMode.High;
                        g.FillRectangle(Brushes.White, 0, 0, fixed_WidthOrHeight, 151);
                        g.DrawImage(imagenf, 0, 0, fixed_WidthOrHeight, 151);

                    }
                    finalBmp.Save(memoryStream, ImageFormat.Jpeg);
                    fileContents = memoryStream.ToArray();
                }

                httpResponseMessage.Content = new ByteArrayContent(memoryStream.ToArray());

                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
            }


            return httpResponseMessage;
        } 
    }
}
