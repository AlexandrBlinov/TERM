using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Yst.ViewModels;
using YstTerm.Models;
using System.Data.SqlClient;
using PagedList;
using Term.DAL;
using System.Web.Mvc;
using Yst.Utils;
using System.Threading.Tasks;
using Term.Web.Views.Resources;


namespace YstProject.Services
{
    public class ServicePartnerPoint : BaseService, IDisposable
    {
        private bool _allocDBContext = false;

        public ServicePartnerPoint()
            : this(new AppDbContext())
        {
            _allocDBContext = true;
        }

        public ServicePartnerPoint(AppDbContext dbcontext)
            : base(dbcontext) { }

        public bool CheckIfOrdersExist(int PointId)
        {
            var order = DbContext.Set<Order>().FirstOrDefault(o => o.PointId == PointId);
            return (order != null);
        }

        public string getKeyWord(int Id)
        {
            return DbContext.Set<PartnerPoint>().Where(p => p.PartnerPointId == Id).Select(u => u.KeyWord).FirstOrDefault();
        }



       // условия приняты
        public bool ConditionsAreAccepted
        {
            get
            {

                var caA = _context.Session["ConditionsAreAccepted"];
                int pointId = getPointID();
                if (pointId < 0)
                {
                    _context.Session["ConditionsAreAccepted"] = false;
                    return false;
                }
                if (caA == null)
                {
                    PartnerPoint pp = getPartnerPointById(pointId);
                    _context.Session["ConditionsAreAccepted"] = pp.ConditionsAreAccepted;
                    return pp.ConditionsAreAccepted;
                }
                else

                    return (bool)caA;
            }
        }


        public void AcceptConditions(int pointId)
        {

            var pp = DbContext.Set<PartnerPoint>().FirstOrDefault(p => p.PartnerPointId == pointId);
            pp.ConditionsAreAccepted = true;
            _context.Session["ConditionsAreAccepted"] = true;
            DbContext.SaveChanges();
        }


        


        public PartnerPointDTOForList[] GetAllPointsByPartnerId(string partnerId)
        {
            IList<PartnerPointDTOForList> partnerPoints =
                DbContext.PartnerPoints.Where(p => p.PartnerId == partnerId).Select(p => new PartnerPointDTOForList { PartnerPointId = p.PartnerPointId, PartnerId = p.PartnerId, ContactFIO = p.ContactFIO, PhoneNumber = p.PhoneNumber, Address = p.Address, IsLocked = false, InternalName = p.InternalName}).ToList();

            int PointId = getPointID();
            var Points = partnerPoints.OrderByDescending(p => p.PartnerPointId == PointId).ToArray();

            foreach (var point in Points)
            {
                var user = _userManager.FindByName(String.Format("Point{0}", point.PartnerPointId));

                if (user != null)
                    point.IsLocked = _userManager.GetLockoutEnabled(user.Id);

            }

            return Points;

        }

        // получить все точки головного терминала
        public SelectList GetPointNamesByPartner(string partnerId, Func<PartnerPoint, string> func)
        {

            return new SelectList(DbContext.PartnerPoints.Where(p => p.PartnerId == partnerId).ToList().Select(p => new { Id = p.PartnerPointId, Name = func(p) }), "Id", "Name");

        }


        // вычислить число дней до подразделения
       virtual public int GetDaysToDepartment(int PointId, int DepartmentId)
        {
            PartnerPoint pp = DbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == PointId);

            if (DepartmentId == Defaults.MainDepartment)
            {
                return pp.DaysToMainDepartment;
            }
            // else               
            return pp.DaysToDepartment;


        }

        // проверить можно ли пройти с учетом ключевого слова
        public bool CheckIfCanPassTrough(string KeyWord)
        {

            if (_context.Session[Defaults.KeyWord] != null)
                return true;

            /*   if (String.IsNullOrEmpty(getPartnerId()))
                   return true;
               */

            int PointId = getPointID();

            /*  if (PointId > 0 && !String.IsNullOrEmpty(CheckForPointWhatPropertiesAreNotFilled(PointId)))
                  return true;
              */

            string KeyWordFromDB = getKeyWord(PointId);

            if (String.IsNullOrEmpty(KeyWordFromDB) || KeyWord.CompareTo(KeyWordFromDB) == 0)
            {
                _context.Session[Defaults.KeyWord] = "OK";
                return true;
            }

            return false;

        }

        // апдейт пошлины и налогов для головного терминала
        public void UpdatePartnerIfNeeded(PointSettingsContainer psc)
        {
            
            Partner partner=this.Partner;

            if (partner != null && partner.IsForeign)
                if (partner.CustomDutyVal != psc.CustomDutyVal || partner.VatVal != psc.VatVal)
                {
                    partner.CustomDutyVal = psc.CustomDutyVal;

                    partner.VatVal = psc.VatVal;
                    DbContext.SaveChanges();
                }
        }

        // апдейт ценовых настроек
        public bool UpdatePricingRules(PointSettingsContainer psc, ref string message)
        {
            string Username = _context.User.Identity.Name;

            bool success = true;
            bool isownPoint = true;

            string PartnerId = GetPartnerId();

            // редактировать может только головной терминал (IsPartner=true) или сама точка 
            if (!(getPointID() == psc.PointId || (IsPartner && GetPartnerIdByPointId(psc.PointId) == PartnerId)))
            { success = false; message = "Попытка изменения чужих данных"; return success; }

            // --определяем вид правил которые изменяем
            var priceFor = PriceListFor.Client;


            // если партнер и не его точка, то 1
            // если партнер и его точка или точка и ее настройки, то 2 

            if (IsPartner && getPointID() != psc.PointId)
            {
                priceFor = PriceListFor.Point;
                // --определили
                isownPoint = false;

            }
            try
            {
                var id = psc.PointId;
                var pricingrules = psc.pricingrules;


                var partnerPoint = getPartnerPointById(psc.PointId);//_dbcontext.PartnerPoints.FirstOrDefault(pp => pp.PartnerPointId == id);
                if (IsPartner && isownPoint)
                {
                    Partner p = this.Partner;
                    p.Culture = psc.Language;
                }
                if (partnerPoint != null)
                {
                    //MyMapper<PartnerPoint>.CopyObjProperties(partnerPoint, psc, "PhoneNumber,ContactFIO,Address,DaysToMainDepartment,DepartmentId,KeyWord,ConditionsAreAccepted,Country,CompanyName,SaleDirection");

                    // Если есть цифры в тел. номере if (!String.IsNullOrEmpty(psc.PhoneNumber))
                    /*partnerPoint.PhoneNumber = psc.PhoneNumber;
                     * 
                             
                     */
                    partnerPoint.DaysToMainDepartment = psc.DaysToMainDepartment;
                    partnerPoint.DaysToDepartment = psc.DaysToDepartment ?? 0;
                    partnerPoint.DepartmentId = psc.DepartmentId;
                    if (isownPoint)
                    {
                        partnerPoint.PhoneNumber = psc.PhoneNumber;
                        partnerPoint.ContactFIO = psc.ContactFIO;
                        partnerPoint.Address = psc.Address;
                        partnerPoint.KeyWord = psc.KeyWord;
                        partnerPoint.Country = psc.Country;
                        partnerPoint.CompanyName = psc.CompanyName;
                        partnerPoint.SaleDirection = psc.SaleDirection;
                        partnerPoint.WebSite = psc.WebSite;
                        partnerPoint.Email = psc.Email;
                        partnerPoint.Language = psc.Language;
                        partnerPoint.AddressForDelivery = psc.AddressForDelivery;
                        partnerPoint.LatLng = psc.LatLng.Trim(new Char[] { '(', ')'});
                    //    partnerPoint.CustomDutyVal = psc.VatVal;
                     //   partnerPoint.VatVal = psc.VatVal;
                    }
                    // string PartnerId = _dbcontext.Set<ApplicationUser>().Where(u => u.UserName == Username && u.IsPartner).Select(u => u.PartnerId).FirstOrDefault() ?? String.Empty;


                }


                var oldrules = DbContext.PartnerPriceRules.Where(pp => pp.PartnerPointId == id && pp.PriceInOut == priceFor);
                var user = _userManager.FindByName(_context.User.Identity.Name);
                foreach (var rule in oldrules)
                {
                    DbContext.PartnerPriceRules.Remove(rule);

                }
                user.Email = psc.Email;
                DbContext.SaveChanges();

                if (pricingrules != null)
                {
                    // var NewPricingRules = pricingrules.Where(p => p.PriceType == "base" || p.PriceType == "zakup" || p.PriceType == "dont_show_price" || p.PriceType == "dont_show_producer").ToArray();
                    PriceTypeEnum[] availArray = { PriceTypeEnum.Zakup, PriceTypeEnum.Base,PriceTypeEnum.Recommend,  PriceTypeEnum.Dont_Show_Price, PriceTypeEnum.Dont_Show_Producer };

                    var NewPricingRules = pricingrules.Where((s) => availArray.Contains(s.PType)).ToArray();

                    var allProducersInRules = NewPricingRules.Select(p => p.ProducerId);
                    if (allProducersInRules.Count() != allProducersInRules.Distinct().Count())
                    {
                        success = false;
                        message = "Есть дубли в правилах";
                        return success;

                    }

                    foreach (var rule in NewPricingRules)
                    {
                        var newrule = new PartnerPriceRule
                        {
                            PartnerPointId = id,
                            PriceInOut = priceFor,
                            Discount = rule.Discount,
                            PriceType = rule.PriceType,
                            ProducerId = rule.ProducerId,
                            PType = rule.PType
                        };

                        DbContext.PartnerPriceRules.Add(newrule);
                    }
                    DbContext.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                success = false;
                message = exc.Message;
                ExceptionUtility.LogException(exc, "UpdatePricingRules");
            }



            return success;
        }



        /// <summary>
        // Получить ценовые настройки для точки в зависимости от PriceListFor.Client/Point
        /// </summary>
        /// <param name="ppdto"> Модель</param>
        /// <param name="pointId">Точка</param>
        /// <param name="priceFor">Цена для точки или клиента</param>
        public void GetPricingRules(PartnerPointBase ppdto, int pointId, PriceListFor priceFor)
        {
            
            string sqltext = @"SELECT Producers.ProducerId ProducerId, Producers.Name Name,  IsNULL(PartnerPriceRules.PriceType,'zakup') PriceType, 
            IsNull(PType,1) PType,
            IsNull(PartnerPriceRules.Discount,0) Discount from Producers 
            LEFT JOIN (SELECT ProducerId, Discount,PriceType,PType FROM PartnerPriceRules WHERE PartnerPointId={1} AND PriceInOut={2})PartnerPriceRules
            ON Producers.ProducerId=PartnerPriceRules.ProducerId
            WHERE Producers.ProductType={0} AND Producers.Active=1  ORDER BY Producers.Name";


           /* ppdto.PricingRulesDisks = (from producers in DbContext.Producers.Where(producer => producer.Active && producer.ProductType == ProductType.Disk)
                                      from pprules in DbContext.PartnerPriceRules.Where(pprule => pprule.ProducerId == producers.ProducerId && pprule.PartnerPointId == pointId && pprule.PriceType == priceFor.ToString()).DefaultIfEmpty()
                                      select new PartnerPriceRuleDTO
                                      {
                                          ProducerId = producers.ProducerId,
                                          Name = producers.Name,
                                          PriceType = pprules.PriceType ?? "zakup",
                                          Discount = pprules != null ? pprules.Discount : 0,
                                          PType = pprules.PType
                                      }).ToList();
          */



            ppdto.PricingRulesDisks = DbContext.Database.SqlQuery<PartnerPriceRuleDTO>(sqltext, ProductType.Disk, pointId, (int)priceFor).ToList();

            ppdto.PricingRulesTyres = DbContext.Database.SqlQuery<PartnerPriceRuleDTO>(sqltext, ProductType.Tyre, pointId, (int)priceFor).ToList();

            ppdto.PricingRulesBat = DbContext.Database.SqlQuery<PartnerPriceRuleDTO>(sqltext, ProductType.Akb, pointId, (int)priceFor).ToList();

            ppdto.PricingRulesAcc = DbContext.Database.SqlQuery<PartnerPriceRuleDTO>(sqltext, ProductType.Acc, pointId, (int)priceFor).ToList();



        }





        // returns PointId>0 if successfull, otherwise -1
        // создать стороннюю партнерскую точку
        public async Task<int> CreatePartnerPoint(PointSettingsContainer model, string partnerId)
        {

            bool IsPointForTerminal = (getPointID() == -1);

            string password = PasswordUtility.CreatePassword(8);
            PartnerPoint pp = new PartnerPoint()
            {
                PartnerId = partnerId,
                Password = password,
                Address = "",
                ContactFIO = model.ContactFIO,
                PhoneNumber = "",
                DaysToDepartment = model.DaysToDepartment ?? -1,
                DaysToMainDepartment = model.DaysToMainDepartment,
                DepartmentId = model.DepartmentId,
                KeyWord = "",
                InternalName = GetInternalNameToCreateForChild(partnerId),


            };

            try
            {
                DbContext.PartnerPoints.Add(pp);

                DbContext.SaveChanges();

                foreach (var rule in model.pricingrules)
                {
                    var newrule = new PartnerPriceRule
                    {
                        PartnerPointId = pp.PartnerPointId,
                        // PriceInOut = IsPointForTerminal ? PriceListFor.Client : PriceListFor.Point, //stub
                        PriceInOut = PriceListFor.Point, //stub
                        Discount = rule.Discount,
                        PriceType = rule.PriceType,
                        ProducerId = rule.ProducerId,
                        PType = rule.PType
                    };

                    DbContext.PartnerPriceRules.Add(newrule);
                }
                DbContext.SaveChanges();

                // у головной терминала нет своей точки
                // if (getPointID() == -1)
                if (IsPointForTerminal)
                { // запишем ID точки в профиль пользователя 
                    var user = await _userManager.FindByNameAsync(_context.User.Identity.Name);
                    user.PartnerPointId = pp.PartnerPointId;
                    DbContext.SaveChanges();
                    return user.PartnerPointId ?? -1; // error if -1
                }
                else     // Создаем новый логин ТОЛЬКО если уже есть партнерская точка для текущего пользователя
                {
                    var user = new ApplicationUser
                    {
                        UserName = pp.Name,
                        PartnerId = pp.PartnerId,
                        IsPartner = false,
                        PartnerPointId = pp.PartnerPointId
                    }; // key user = key point
                    string pwd = password;

                    if (_userManager.FindByName(user.UserName) == null)
                    {
                        var iresult = await _userManager.CreateAsync(user, pwd);
                        if (iresult.Succeeded)
                            return pp.PartnerPointId; // success
                        else
                        {  // if not created user, then remove PartnerPoint
                            DbContext.PartnerPoints.Remove(pp);
                            DbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exc ) {
                ExceptionUtility.LogException(exc, "CreatePartnerPoint");
                return -1; }

            return pp.PartnerPointId;

        }

        private string GetInternalNameToCreateForChild(string partnerId)
        {
            int countOfPoints = DbContext.Set<PartnerPoint>().Where(p => p.PartnerId == partnerId).Count();

            if (countOfPoints == 0) throw new Exception("нет точки у головного терминала " + partnerId);
            // первая точка - головной терминал, значит всего точек = count-1. Значит следующая точка = count.
            //return String.Format("Точка {0}", countOfPoints);
            return String.Format(Settings.Point + " {0}", countOfPoints);

        }

        public async Task<bool> DeletePartnerPoint(int pointId)
        {


            PartnerPoint pp = DbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);

            if (pp == null) return false;

            var user = await _userManager.FindByNameAsync(pp.Name);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }
            else
            {
                try
                {
                    DbContext.PartnerPoints.Remove(pp);
                    DbContext.SaveChanges();
                }
                catch (Exception exc)
                {
                    ExceptionUtility.LogException(exc, "DeletePartnerPoint");
                }
            }

            return true;

        }

        public async Task<bool> Disable(int pointId, bool action)
        {


            PartnerPoint pp = DbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);

            if (pp == null) return false;

            var user = _userManager.FindByName(pp.Name);

            if (user == null)
            {
                return false;
            }
            var result = await _userManager.SetLockoutEnabledAsync(user.Id, !action);

            return result.Succeeded;

        }



        internal int CreatePartnerOwnPoint(PointSettingsContainer psc, string PartnerId)
        {
            

            PartnerPoint pp = new PartnerPoint
            {
                PartnerId = PartnerId,
                Password = String.Empty,
                Address = psc.Address,
                CompanyName = psc.CompanyName,
                Country = psc.Country,
                ContactFIO = psc.ContactFIO,
                PhoneNumber = psc.PhoneNumber,
                ConditionsAreAccepted = false,
                SaleDirection = psc.SaleDirection,
                DaysToDepartment = psc.DaysToDepartment ?? -1,
                DaysToMainDepartment = psc.DaysToMainDepartment,
                DepartmentId = psc.DepartmentId,
                KeyWord = psc.KeyWord,
                Language = psc.Language,
                LatLng = psc.LatLng.Trim(new Char[] { '(', ')' }),
                AddressForDelivery = psc.AddressForDelivery,
                WebSite = psc.WebSite,
                Email = psc.Email,
                PartnerPriceRules = psc.pricingrules.Select(p => new PartnerPriceRule { PriceInOut = PriceListFor.Client, PType = p.PType, ProducerId = p.ProducerId, Discount = p.Discount }).ToList(),
                InternalName = Settings.TerminalClient
            };
            
            var user = _userManager.FindByName(_context.User.Identity.Name);
            try
            {
                DbContext.PartnerPoints.Add(pp);

                // записали точку
                DbContext.SaveChanges();

                // записали идентификатор точки в профиль пользователя
                user.PartnerPointId = pp.PartnerPointId;
                user.Email = pp.Email;
                DbContext.SaveChanges();
                // записали культуру пользователя
                Partner p = this.Partner;
                p.Culture = psc.Language;
                DbContext.SaveChanges();
            }
            catch(Exception exc)
            {
                ExceptionUtility.LogException(exc, "CreatePartnerOwnPoint");
                return -1; }

            return user.PartnerPointId ?? -1; // error if -1
        }

        public void Dispose()
        {
            if (_allocDBContext)
                DbContext.Dispose();

        }
    }
}