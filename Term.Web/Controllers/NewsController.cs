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
using YstTerm.Models;

namespace Term.Web.Controllers
{
    public class NewsController : BaseController
    {
        readonly AppDbContext _dbContext;
        public NewsController() : this(new AppDbContext()) { }
        public NewsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Newsmaker")]
        public ActionResult NewsList(NewsViewModel model)
        {
            model.News = _dbContext.News.Where(p => p.Id != 0).Select(p => new NewsViewModel
            {
                Id = p.Id,
                NewsName = p.NewsName,
                NewsText = p.NewsText,
                DatePublish = p.DatePublish
            }).ToList();
            return View(model);
        }

        [Authorize(Roles = "Newsmaker")]
        public ActionResult GetNewsDetails(long Id)
        {
           // var news = new Term.DAL.News();
           var news = _dbContext.News.FirstOrDefault(o => o.Id == Id);
            var model = new NewsViewModel
            {
                Id = news.Id,
                NewsName = news.NewsName,
                NewsText = news.NewsText,
                Culture = news.Culture,
                Active = news.Active,
                DatePublish = news.DatePublish,
                ContentType = news.ContentType,
                PreviewImg = news.PreviewImg
            };
            return View(model);
        }


        public async Task<ActionResult> GetNewsImg(long Id, int ImgType)
        {
            var news = await _dbContext.News.FirstAsync(o => o.Id == Id);
            var stream = new MemoryStream();
            var contentType = "img/jpeg";
            if (ImgType == Defaults.NewsPreviewPhoto && news.PreviewImg != null && news.PreviewImg.Length > 0)
            {
                stream = new MemoryStream(news.PreviewImg);
                contentType = news.ContentType;
            }
            if (ImgType == Defaults.NewsMainPhoto && news.MainImg != null && news.MainImg.Length > 0)
            {
                stream = new MemoryStream(news.MainImg);
                contentType = news.ContentType;
            }
            return new FileStreamResult(stream, contentType);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Newsmaker")]
        public async Task<ActionResult> Create(NewsViewModel model)
        {
            try
            {
                var news = new Term.DAL.News
                {
                    NewsName = model.NewsName,
                    NewsText = model.NewsText,
                    Culture = model.Culture,
                    DatePublish = DateTime.Now
                };
                _dbContext.News.Add(news);
               await _dbContext.SaveChangesAsync();
                return RedirectToAction("NewsList", "News");
            }
            catch
            {
                return RedirectToAction("NewsList", "News");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NewsViewModel model, long Id, bool Active)
        {
            
            var news = _dbContext.News.FirstOrDefault(o => o.Id == Id);
            
            news.NewsName = model.NewsName;
            news.NewsText = model.NewsText;
            news.Culture = model.Culture;
            news.Active = Active;
            _dbContext.SaveChanges();
            return RedirectToAction("GetNewsDetails", "News", new { Id = Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImg(long Id, int ImgType, HttpPostedFileBase upload)
        {
            try
            {
                var news = new Term.DAL.News();
                news = _dbContext.News.FirstOrDefault(o => o.Id == Id);
                news.ContentType = upload.ContentType;
                if (ImgType == Defaults.NewsPreviewPhoto)
                    news.PreviewImg = new System.IO.BinaryReader(upload.InputStream).ReadBytes(upload.ContentLength);
                if (ImgType == Defaults.NewsMainPhoto)
                    news.MainImg = new System.IO.BinaryReader(upload.InputStream).ReadBytes(upload.ContentLength);

                _dbContext.SaveChanges();
                return RedirectToAction("GetNewsDetails", "News", new { Id = Id });
            }
            catch 
            {
                return RedirectToAction("NewsList", "News");
            }
        }
        //Далее контроллеры для юзеров
        public ActionResult Index(NewsViewModel model)
        {
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            model.News = _dbContext.News.Where(p => p.Active && p.Culture == currentCulture).Select(p => new NewsViewModel
            {
                Id = p.Id,
                NewsName = p.NewsName,
                NewsText = p.NewsText,
                DatePublish = p.DatePublish
            }).ToList();
            return View(model);
        }

        public ActionResult Details(long Id)
        {
         
           var news = _dbContext.News.FirstOrDefault(o => o.Id == Id);

           if (news == null) return HttpNotFound("News Not Found");

            var model = new NewsViewModel
            {
                Id = news.Id,
                NewsName = news.NewsName,
                NewsText = news.NewsText,
                Culture = news.Culture,
                Active = news.Active,
                DatePublish = news.DatePublish
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult AddNotifications(long Id)
        {
            bool success = true;
            var notif = new Term.DAL.NewsNotifications
            {
                UserName = User.Identity.Name,
                NewsId = Id
            };
            _dbContext.NewsNotifications.Add(notif);
            _dbContext.SaveChanges();
            return Json(success);
        }
    }
}