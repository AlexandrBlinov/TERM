using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Term.DAL;
using Term.Web.Services;
using Yst.Context;
using YstProject.Services;

namespace Term.Web.Controllers
{
    public class NotificationsApiController : ApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly NotificationForUserService _service;
        private readonly HttpContextBase _context;


        public NotificationsApiController() : this(new AppDbContext(), 
            new NotificationForUserService(), 
            new HttpContextWrapper(System.Web.HttpContext.Current)) { }


         public NotificationsApiController(AppDbContext db, NotificationForUserService service, HttpContextBase context)
	    {
                _dbContext=db;
                _service = service;
                _context = context;

	    }

        private string Username { get { return _context.User.Identity.Name; }}

        /// <summary>
        /// Проверяет есть ли уведомления
        /// </summary>
        /// <returns></returns>
         [HttpGet]
        public bool CheckIfExists()
        {
            return _service.CheckIfExists(Username);
        }

         [HttpGet]
         public NotificationForUser GetFirst()
         {
             return _service.GetFirst(Username);
         }

        /// <summary>
        /// Удаляет все уведомления
        /// </summary>
      
        [HttpPost]
        public void DisableAll()
        {
             _service.DisableAll(Username);

        }
    }
}
