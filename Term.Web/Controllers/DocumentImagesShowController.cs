using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yst.Context;
using YstProject.Services;

namespace Term.Web.Controllers
{
    public class DocumentImagesShowController : Controller
    {
        readonly AppDbContext _dbContext;
        public DocumentImagesShowController() : this(new AppDbContext()) { }
        public DocumentImagesShowController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



       // [OutputCache(Duration = 36000, VaryByParam = "id")]
        public async Task<ActionResult> GetById(int id)
        {
            var file = await _dbContext.Files.FirstAsync(p => p.Id == id);


            var stream = new MemoryStream(file.Content);

            return new FileStreamResult(stream, file.ContentType);
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Term.DAL.File model, HttpPostedFileBase upload)
        {
            try
            {
                if (upload != null && upload.ContentLength > 0 && upload.ContentLength <= Defaults.MaxUploadedFileSize)
                {
                    var file = new Term.DAL.File
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        GuidIn1S = model.GuidIn1S,
                        Content = new System.IO.BinaryReader(upload.InputStream).ReadBytes(upload.ContentLength),
                        ContentType = upload.ContentType

                    };


                    _dbContext.Files.Add(file);
                    _dbContext.SaveChanges();

                    return RedirectToAction("Details", "Orders", new { guid = model.GuidIn1S });
                }

                throw new FileFormatException("Неверный формат  или превышение макс. размера файла");

            }
            catch 
            {
                
                throw;

            }

        }

       
       // [ChildActionOnly]
        public ActionResult GetImagesByDocGuid(Guid guid)
        {
            
            var files = _dbContext.Files.Where(p => p.GuidIn1S == guid).ToList();
            ViewBag.GuidIn1S = guid;
            ViewBag.FromAction = ControllerContext.IsChildAction;

            if (ViewBag.FromAction) return PartialView(files);

            if (files.Count == 0) throw new HttpException(404, "Scans are absent");

            return View(files);
        }

        
        
    }
}
