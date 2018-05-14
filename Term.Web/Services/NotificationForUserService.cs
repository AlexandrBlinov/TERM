using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Term.DAL;
using WebGrease.Css.Extensions;
using Yst.Context;
using Yst.ViewModels;
using YstProject.Services;

namespace Term.Web.Services
{
    /// <summary>
    /// Сервис для отправки сообщения пользователю
    /// </summary>
    public class NotificationForUserService
    {

        private readonly AppDbContext _dbContext;
        private readonly HttpContextBase _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;

         private readonly Expression<Func<NotificationForUser,string,bool>> predicate =(p, username) => p.UserName.Equals(username) && p.Status == StatusOfNotification.Unread;

        private readonly Expression<Func<NotificationForUser, bool>> predicate2 = (p) =>  p.Status == StatusOfNotification.Unread;

        public NotificationForUserService(AppDbContext dbContext, HttpContextBase httpContext)
        {
            _dbContext = dbContext;
            _httpContext = httpContext;
                _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
        }

        public NotificationForUserService():this (new AppDbContext(), new HttpContextWrapper(HttpContext.Current))
        {

        }

        
        
        /// <summary>
        /// Проверить есть ли активные уведомления для пользователя
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckIfExists(string username)
        {
            return _dbContext.NotificationsForUsers.Any( p => p.UserName.Equals(username) && p.Status == StatusOfNotification.Unread);
        }


        /// <summary>
        /// Получает первое сообщение или false если его нет
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public NotificationForUser GetFirst(string username)
        {
            return _dbContext.NotificationsForUsers.FirstOrDefault(p => p.UserName.Equals(username) && p.Status == StatusOfNotification.Unread);
        }


        public void AddIfNotExists(string username, string message)
        {
            if (!CheckIfExists(username))    Add(username,message);
        }

        /// <summary>
        /// Добавить уведомление
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        public void Add(string username, string message)
        {
            _dbContext.NotificationsForUsers.Add(new NotificationForUser
            {
                Date = DateTime.Now,
                Message = message,
                Status = StatusOfNotification.Unread,
                UserName = username
            });

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Добавить уведомление
        /// </summary>
        /// <param name="username"></param>
        
        public void DisableAll(string username)
        {
         var notifications=   _dbContext.NotificationsForUsers.Where(p=>p.UserName.Equals(username) && p.Status==StatusOfNotification.Unread);

         foreach (var notification in notifications)
         {
             notification.Status = StatusOfNotification.Read;
         }
            //notifications.ForEach(n=>n.Status=1);

            _dbContext.SaveChanges();
        }


        /// <summary>
        /// Добавить уведомление
        /// </summary>
        /// <param name="username"></param>

        public void DisableFirst(string username)
        {
            var notification =
                _dbContext.NotificationsForUsers.FirstOrDefault(p => p.UserName.Equals(username) && p.Status == StatusOfNotification.Unread);

            if (notification != null)

            {
                notification.Status = StatusOfNotification.Read;

                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Получить пользователя по точке
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public string GetUserFromPointId(int pointId)
        {
           var user = _userManager.Users.FirstOrDefault(p => p.PartnerPointId == pointId);
            return user?.UserName;
               
        }

        /// <summary>
        /// Получить пользователя по поставщику
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public string GetUserFromSupplierId(int supplierId)
        {
            var user = _userManager.Users.FirstOrDefault(p => p.SupplierId == supplierId);
               return  user?.UserName;  
        }



    }
}