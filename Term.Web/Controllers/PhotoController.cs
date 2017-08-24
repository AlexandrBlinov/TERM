using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yst.Context;
using YstTerm.Models;

namespace Term.Web.Controllers
{
    public class PhotoController : BaseController
    {
        readonly AppDbContext _dbContext;
        public PhotoController() : this(new AppDbContext()) { }
        public PhotoController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Newsmaker")]
        public ActionResult Index(PhotoViewModel model)
        {
            ViewBag.CountPhoto = model.NamePhoto != String.Empty ? _dbContext.PhotoForProducts.Count(p => p.NamePhoto == model.NamePhoto) : 0;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Newsmaker")]
        public ActionResult UploadImg(string namePhoto, int number, HttpPostedFileBase upload)
        {
            try
            {
                var photo = new Term.DAL.PhotoForProducts
                {
                    NamePhoto = namePhoto,
                    NumberPhoto = number,
                    ContentType = upload.ContentType,
                    Photo = new System.IO.BinaryReader(upload.InputStream).ReadBytes(upload.ContentLength)
                };
                _dbContext.PhotoForProducts.Add(photo);
                _dbContext.SaveChanges();
                var model = new Term.DAL.PhotoForProducts
                {
                    NamePhoto = namePhoto,
                    NumberPhoto = number
                };
                return RedirectToAction("Index", "Photo", model);
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex);
                return RedirectToAction("Index", "Photo");
            }
        }

        public async Task<ActionResult> GetProductPhoto(string namePhoto, int number)
        {
            var photo = await _dbContext.PhotoForProducts.FirstAsync(p => p.NamePhoto == namePhoto && p.NumberPhoto == number);
            var stream = new MemoryStream();
            stream = new MemoryStream(photo.Photo);
            var contentType = photo.ContentType ?? "img/jpeg";

            /*Если нужно будет сжимать фото
            
             var remoteImage = (Bitmap)Bitmap.FromStream(stream);
            var finalBmp = new Bitmap(80, 80);
            using (var g = Graphics.FromImage(finalBmp))
            {
            g.InterpolationMode = InterpolationMode.High;
            g.FillRectangle(Brushes.White, 0, 0, 80, 80);
            g.DrawImage(remoteImage, 0, 0, 80, 80);

            }
            var stream2 = new MemoryStream();

            finalBmp.Save(stream2, System.Drawing.Imaging.ImageFormat.Bmp);
            Byte[] bytes = stream2.ToArray();
            var stream3 = new MemoryStream(bytes);*/
            return new FileStreamResult(stream, contentType);
        }

        [Authorize(Roles = "Newsmaker")]
        public async Task<ActionResult> RemoveProductPhoto(string namePhoto, int number)
        {
            var photo = await _dbContext.PhotoForProducts.FirstAsync(p => p.NamePhoto == namePhoto && p.NumberPhoto == number);
            try
            {
                _dbContext.PhotoForProducts.Remove(photo);
                _dbContext.SaveChanges();
                var model = new Term.DAL.PhotoForProducts
                {
                    NamePhoto = namePhoto,
                    NumberPhoto = number
                };
                
                return RedirectToAction("Index", "Photo", model);
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex);
                return RedirectToAction("Index", "Photo");
            }
        }
    }
}