using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.Context;
using Yst.ViewModels;

namespace YstProject.Services
{
    /// <summary>
    /// UserService class handles user functions in identity
    /// </summary>
    public class NewsService
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpContextBase _context;
        public NewsService(AppDbContext dbContext, HttpContextBase context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public NewsService() : this(new AppDbContext(), new HttpContextWrapper(HttpContext.Current)) { }

        public long getNewsId()
        {
            string Username = _context.User.Identity.Name;
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            String sqltext = String.Format(@"SELECT * FROM News WHERE  Id NOT IN
            (SELECT NewsId FROM NewsNotifications WHERE UserName = '{0}') AND Culture = '{1}' AND Active=1", Username, currentCulture);

            var News = _dbContext.News.SqlQuery(sqltext).ToList();
            long NewsId = 0;
            if (News.Count > 0)
                NewsId = News[0].Id;
            return NewsId;
        }


    }
}