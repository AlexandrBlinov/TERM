using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Yst.Context;
using Yst.Utils;
using Yst.ViewModels;
using Term.DAL;
using YstTerm.Models;
using System.Data.Entity;

namespace YstProject.Services
{
     public class BaseService
    {
        protected AppDbContext _dbContext = null;
        protected UserManager<ApplicationUser> _userManager = null;

        protected readonly HttpContextBase _context;

        private  Partner _partner;

        private  ApplicationUser _user;


        public BaseService() : this(new AppDbContext()) { }

        public BaseService(AppDbContext dbcontext)
        {
            _dbContext = dbcontext;
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));

            _context = new HttpContextWrapper(HttpContext.Current);
        }

        protected AppDbContext DbContext
        {
            get { return _dbContext ?? (_dbContext = new AppDbContext()); }
        }

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public ApplicationUser CurrentUser
        {
            get
            {
                return _user ?? (_user = _dbContext.Set<ApplicationUser>().FirstOrDefault(u => u.UserName == _context.User.Identity.Name));
            }
        }

        /// <summary>
        /// Текущий партнер
        /// </summary>
        public Partner Partner
        {
            get
            {
                if (_partner != null) return _partner;              
                if (CurrentUser != null)
                {
                    string partnerId = CurrentUser.IsPartner ? CurrentUser.PartnerId : null;
              
                    if (partnerId == null) return null;

                    return (_partner = _dbContext.Set<Partner>().FirstOrDefault(p => p.PartnerId == partnerId));

                }
                return null;
            }
        }

       

        // Получить код контрагента для текущего пользователя при условии что он партнер
        public string GetPartnerId() => Partner?.PartnerId;



        /// <summary>
        ///  Может ли пользователь использовать сервис доставки DPD
        /// </summary>
        public bool CanUserUseDpdDelivery => Partner != null && !Partner.IsForeign && Partner.HasDpdContract;
        

        /// <summary>
        /// Новый способ получить текущую точку
        /// </summary>
        public PartnerPoint CurrentPoint
        {
           get {

                //var pointId = (int)CurrentUser.PartnerPointId;
                int? pointId = CurrentUser.PartnerPointId ?? null;
                if (pointId == null) return null;
                return _dbContext.Set<PartnerPoint>().Include(p=>p.Partner).FirstOrDefault(point => point.PartnerPointId == pointId);

           }
        
        }

        /// <summary>
        /// Возвращает партнера или точку по ID партнера
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Partner GetPartnerById(string id)=> _dbContext.Set<Partner>().FirstOrDefault(p => p.PartnerId == id);
              
        

        public bool IsForeignPartner => Partner != null && Partner.IsForeign;

        // признак,  может  ли использовать дополнительный склад
        public bool CanPartnerUseAdditionalStock()
        {
            if (IsPartner) return !IsForeignPartner;

            
            // int PointID = getPointID();
           // int PointID = CurrentPoint.PartnerPointId;
            //string PartnerID = GetPartnerByPointId(PointID);
            Partner partner = GetPartnerById(CurrentUser.PartnerId);

            return !partner.IsForeign;


           
        }

        //доступны ли акции партнёру
        public bool IsSaleAvailable => IsPartner && this.Partner.IsSale;
       

        //Проверить, что текущий пользователь - партнер, а не точка 
        public bool IsPartner => Partner != null;
      

        // Получить идентификатор точки для текущего пользователя
        public int getPointID() => CurrentUser.PartnerPointId??-1;
       
                
        // Получить точку по Id
        public PartnerPoint getPartnerPointById(int Id) => _dbContext.Set<PartnerPoint>().FirstOrDefault(p => p.PartnerPointId == Id);
        
        
        /// <summary>
        /// Получить код контрагента из кода точки
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public string GetPartnerIdByPointId(int pointId)
        {
            string partnerId = _dbContext.Set<ApplicationUser>().Where(u => u.PartnerPointId == pointId).Select(u => u.PartnerId).FirstOrDefault();          

            if (partnerId == null) throw new NullReferenceException("PartnerId not found for given PointId=" + pointId.ToString());

            return partnerId;

        }

        // все нужные свойства заполнены
        public string CheckForPointWhatPropertiesAreNotFilled(int pointId)
        {
            PartnerPoint pp = _dbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);
            //String[] pavParams = {pp.Address,pp.CompanyName,pp.ContactFIO,pp.Country,pp.PhoneNumber};


            IList<string> list = DisplayNameAttrLocator.GetPropsNotFilled(pp, "Address", "CompanyName", "ContactFIO", "Country", "PhoneNumber");
            return String.Join(",", list.ToArray());


        }

        public int GetPointByUserID(string UserId)
        {
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                var appUser = _dbContext.Set<ApplicationUser>().Where(u => u.Id == UserId).FirstOrDefault();
                if (appUser != null)
                    if (appUser.IsPartner) return appUser.PartnerPointId ?? -1;

            }
            return -1;
        }



    }
}