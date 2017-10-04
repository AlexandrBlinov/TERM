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


    public class NewsService : BaseService
    {
        public long getNewsId()
        {
            var partnerId = IsPartner ? Partner.PartnerId : CurrentPoint.PartnerId;
            var vipakb = _dbContext.PartnerPropertyValues.Where(p => p.PartnerId == partnerId && p.Name == "akbvip").FirstOrDefault();
            string sqltext = string.Empty;
            string Username = _context.User.Identity.Name;
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (vipakb != null && IsPartner)
            {
                sqltext = String.Format(@"SELECT * FROM News WHERE Id NOT IN
                (SELECT NewsId FROM NewsNotifications WHERE UserName = '{0}') AND Culture = '{1}' AND Active=1", Username, currentCulture);
            }
            else
            {
                sqltext = String.Format(@"SELECT * FROM News WHERE Id NOT IN
                (SELECT NewsId FROM NewsNotifications WHERE UserName = '{0}') AND Culture = '{1}' AND Active=1 and Id != 4", Username, currentCulture);
            }
            var News = _dbContext.News.SqlQuery(sqltext).ToList();
            long NewsId = 0;
            if (News.Count > 0)
                NewsId = News[0].Id;
            return NewsId;
        }
    }

}