using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Yst.Context;
//using Yst.Filters;
using Yst.Services;
using Yst.ViewModels;
using YstIdentity.Models;
using YstProject.Services;
using Term.DAL;
using Term.Web.Filters;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace Term.Web.Controllers.API
{
    [AdminHashAuth]
    public class PartnerApiController : ApiController
    {
        private readonly AppDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;
        protected UserManager<ApplicationUser> Manager
        {
            get { return _userManager ?? (_userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext))); }

        }
       
        protected IDbSet<Partner> Partners
        {
            get { return _dbContext.Partners; }

        }


        

        public PartnerApiController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PartnerApiController():this(new AppDbContext())
        {
        }
        // GET api/partnerapi

        /*
         public IEnumerable<Partner> Get()
         {
             return Partners;

         }

         public Partner GetOne(string id)
         {

             Partner partner = Partners.First(p => p.PartnerId == id);
           return (partner);
         }

     */


        [HttpPost]
        [ActionName("create")]
    //      [AdminHashAuth]
        /// Create user and partner from xml
         // http://localhost:9090/api/partnerapi/create?username=test9&password=ghdff3
         // <Partner xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
        //<PartnerId>П003415</PartnerId>
        //<Name>Центр Сервиса</Name> 
        //<FullName>ООО «Центр Сервиса»</FullName> 
        //<ContactFIO>Родионов Александр Александрович</ContactFIO> 
        //<Address>160000, г.Вологда. ул Сергея Орлова д 2.помещение 14</Address> 
        //<PhoneNumber>89005399046</PhoneNumber> 
        //<INN>352533232</INN> 
        //<KPP>352501001</KPP>
        //<Culture>en-US</Culture>
        //</Partner>
 
 
        public async Task<HttpResponseMessage> Post([FromBody]Partner partnerDto, [FromUri]string username, [FromUri] string password)
        {
            
            HttpResponseMessage msg =null;

            Debug.Assert(partnerDto!=null);
            if (username == null || password==null)
                 return Request.CreateResponse(HttpStatusCode.InternalServerError, "Can't create user. Username or password are empty");

            if (ModelState.IsValid)
            {
                
                var appUser = new ApplicationUser
                {
                    UserName = username,
                    DepartmentId = Defaults.MainDepartment,
                    PartnerId = partnerDto.PartnerId,
                    IsPartner = true,
                    
                };

                
                if (await Manager.FindByNameAsync(appUser.UserName) == null)
                {
                    IdentityResult adminresult = null;
                    adminresult = await Manager.CreateAsync(appUser, password);

                    if (adminresult.Succeeded && !Partners.Any(p => p.PartnerId == partnerDto.PartnerId))
                    {
                        try
                        {
                            _dbContext.Partners.Add(partnerDto);
                            _dbContext.SaveChanges();
                        }
                        catch (Exception exc) { return Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message); }

                        msg = Request.CreateResponse(HttpStatusCode.OK);
                      //  msg.Headers.Location = new Uri(Url.Link("DefaultApi", new { controller = "PartnerApi", action = "GetOne", id = partnerDto.PartnerId }));
                       

                        return msg;
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Can't create user " + adminresult.Errors.ToString());

                    }
                }
                else
                {
                    if (!Partners.Any(p => p.PartnerId == partnerDto.PartnerId))
                    {
                        try
                        {
                            _dbContext.Partners.Add(partnerDto);
                            _dbContext.SaveChanges();
                            
                        }
                        catch (Exception exc) { return Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message); }
                    }
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, String.Format("User with name {0} exists", username)); ;
                }
            }
            return  Request.CreateResponse(HttpStatusCode.InternalServerError, "Model is invalid"); ;
        }

        /// <summary>
        /// Загрузка остатков - резервов для клиентов
        /// lapenkov_vi:9090/api/partnerapi/importpartnerproperties
        /// </summary>
        /// <returns></returns>
        [ActionName("ImportPartnerProperties")]
        [HttpPost]
        public async Task<HttpResponseMessage> ImportPartnerProperties()
        {
            string errorMsg;

            var stream = await Request.Content.ReadAsStreamAsync();


            int result = SPExecutor.Execute("spImportPartnerProperties", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(stream) }, out errorMsg);

            if (result != 0) return new HttpResponseMessage {StatusCode=HttpStatusCode.InternalServerError, Content = new StringContent($"Error:{errorMsg}") };

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent($"{result}") };


        }


        [HttpPost]
        [ActionName("delete")]
       [AdminHashAuth]
        public async Task<HttpResponseMessage> Delete(string Id)
        { IdentityResult adminresult = null;
           // var partner = _partners.GetOne(p => p.PartnerId == Id);

             if (!Partners.Any(p => p.PartnerId == Id)) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Partner id is not found");
            var appUser = _dbContext.Users.FirstOrDefault(p => p.PartnerId == Id);
            if (appUser == null) throw new ArgumentNullException("appUser");


            if (await Manager.FindByNameAsync(appUser.UserName) == null) Request.CreateErrorResponse(HttpStatusCode.NotFound, "Username not found");
            adminresult = await Manager.SetLockoutEnabledAsync(appUser.Id, true);

            if (adminresult.Succeeded)
            return Request.CreateResponse(HttpStatusCode.OK);
            else
              return  Request.CreateResponse(HttpStatusCode.InternalServerError, "Can't disable user " + appUser.UserName);
        }

       
      

    }

  
}
