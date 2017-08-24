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
    public class UserService  
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpContextBase _context;
         public UserService( AppDbContext dbContext, HttpContextBase context)
        {
            _dbContext=dbContext;
            _context = context;
        }

         public UserService() : this(new AppDbContext(), new HttpContextWrapper(HttpContext.Current)) { }

         public string getUserId()
         {
             string Username = _context.User.Identity.Name;
             var appUser = _dbContext.Set<ApplicationUser>().Where(u => u.UserName == Username).FirstOrDefault();
             return appUser.Id;
         }


    }
}