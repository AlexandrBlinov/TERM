using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using Term.DAL;
using Yst.Context;
using Yst.ViewModels;

namespace Term.Web.Filters
{
    public class TrackUserApiActionAttribute : ActionFilterAttribute
    {
        
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            try
            {
                var lastword=actionExecutedContext.Request.RequestUri.AbsolutePath.Split('/').LastOrDefault();

                if (!String.IsNullOrEmpty(lastword))

              
                    using (var dbContext = new AppDbContext())
                    {
                        var user=dbContext.Set<ApplicationUser>().FirstOrDefault(u => u.Id == lastword);

                        var userLog = new DbUserActionLog { Date = DateTime.Now, UserName = user.UserName, UserAction = actionExecutedContext.Request.RequestUri.AbsolutePath };
                        
                        dbContext.Set<DbUserActionLog>().Add(userLog);

                        dbContext.SaveChanges();
                    }
                

            }
            finally { }
            base.OnActionExecuted(actionExecutedContext);
        }


    }
}