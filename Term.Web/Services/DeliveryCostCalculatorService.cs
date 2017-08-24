using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Yst.Context;

using Term.DAL;
using Term.Soapmodels;

namespace YstProject.Services
{

    

    /// <summary>
    ///  Класс который считает стоимость доставки товаров в корзине 
    /// </summary>
    public class DeliveryCostCalculatorService
    {
        /// <summary>
        /// Модель для доставки dpd
        /// </summary>
        private class DpdDeliveryItem
        {
            public decimal Weight { get; set; }

            public int DepartmentId { get; set; }

        }

      

         private readonly AppDbContext _dbContext ;
         private readonly HttpContextBase _httpContext;
         private readonly BaseService _baseService;


        public DeliveryCostCalculatorService( AppDbContext dbContext, HttpContextBase httpContext,BaseService baseService)
        {
            _dbContext=dbContext;
            _httpContext = httpContext;
            _baseService = baseService;

        }

        public DeliveryCostCalculatorService() : this(new AppDbContext(), new HttpContextWrapper(HttpContext.Current), new BaseService()){}


        /// <summary>
        /// Получить стоимость доставки за товар
        /// </summary>
        /// <param name="model">Вес и подразделение-отправитель</param>
        /// <param name="record">тарифы до основного города</param>
        /// <param name="terminalOrAddress">до терминала=true, до адреса = false</param>
        /// <returns></returns>
        private decimal GetCostOfDpdDeliveryItem(DpdDeliveryItem model,RateToMainCity record,bool terminalOrAddress)
        {
            return GetCostOfDpdAllDelivery(model.DepartmentId, record, model.Weight, terminalOrAddress);
           

        }

        /// <summary>
        /// Полный рассчет стоимости доставки внутри города и между городами
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="record"></param>
        /// <param name="weight"></param>
        /// <param name="terminalOrAddress"></param>
        /// <returns></returns>
        private decimal GetCostOfDpdAllDelivery(int departmentId, RateToMainCity record, decimal weight, bool terminalOrAddress)
        {
            weight = Decimal.Ceiling(weight);
            decimal result;

            var cityCodes = new Dictionary<int, string>() { { 5, "76000001000" }, { 112, "78000000000" }, { 138, "66000001000" } };

            // если локальная доставка (внутри города)
            if (cityCodes.ContainsKey(departmentId) && cityCodes[departmentId].Equals(record.CityId))
            {
                if (weight <= 20) result = terminalOrAddress ? 150 : 200;
                else if (weight <= 40) result = terminalOrAddress ? 200 : 250;
                else
                {
                    result = (weight - 40)*(terminalOrAddress ? 5/2 : 4) + (terminalOrAddress ? 200 : 250);
                }
                    return result;
            }

            // доставка между городами
            if (weight <= 20)
            {
                if (departmentId != Defaults.DepartmentEkb) result = terminalOrAddress ? record.Rate20 : record.Rate20Door;
                else result = terminalOrAddress ? record.Rate20_Ekb : record.Rate20Door_Ekb;
            }
            else if (weight <= 40)
            {
                if (departmentId != Defaults.DepartmentEkb) result = terminalOrAddress ? record.Rate40 : record.Rate40Door;
                else result = terminalOrAddress ? record.Rate40_Ekb : record.Rate40Door_Ekb;

            }
            else
            { 
            // вес более 40 килограмм
            if (departmentId != Defaults.DepartmentEkb) result = (weight - 40) * (terminalOrAddress ? record.RatePlus1 : record.RatePlus1Door) + (terminalOrAddress ? record.Rate40 : record.Rate40Door);
            else result = (weight - 40) * (terminalOrAddress ? record.RatePlus1_Ekb : record.RatePlus1Door_Ekb) + (terminalOrAddress ? record.Rate40_Ekb : record.Rate40Door_Ekb);
            }
            return result * (decimal)Defaults.RateNDS;
        }


        /// <summary>
        /// Получить имя пользователя
        /// </summary>
        /// <returns></returns>
        private string GetUserName()
        {
          //  if (_httpContext != null && _httpContext.User.Identity != null && _httpContext.User.Identity.IsAuthenticated)
                return _httpContext.User.Identity.Name;
         //   else
          //      return null;

        }


        /// <summary>
        /// Получить стоимость доставки в основной город
        /// </summary>
        /// <param name="cityId">Город куда везем</param>
        /// <param name="terminalOrAddress">Терминал=true , адрес=false</param>
        /// <param name="departmentId">Отбирать только по этому подразделению</param>
        /// <returns></returns>
        public async Task<int> GetCostOfDelivery(string cityId, bool terminalOrAddress, int? departmentId = null, Guid? orderGuid = null)
        {


            if (cityId == null) throw new NullReferenceException("CityId  is null");

            double rateForDelivery = Defaults.RateForDelivery;
            try
            {
                rateForDelivery = (double)(_baseService.CurrentPoint.Partner.DeliveryProfitPercent.Value + 100) / 100;

            }
            catch
            {
                // ignored
            }

            decimal sum = 0;
            var username = GetUserName();
            var dpdItems = await _dbContext.Set<Cart>().Where(cart => cart.CartId == username && (departmentId == null || cart.DepartmentId == departmentId)).Include(p => p.Product).Select(p => new DpdDeliveryItem { DepartmentId = p.DepartmentId, Weight = p.Count * p.Product.Weight }).ToListAsync();

            if (orderGuid != null)
            {
                dpdItems = await _dbContext.Set<OrderDetail>().Where(cart => cart.GuidIn1S == orderGuid).Include(p => p.Product).Select(p => new DpdDeliveryItem { DepartmentId = p.Order.DepartmentId, Weight = p.Count * p.Product.Weight }).ToListAsync();
            }

            var record = await _dbContext.Set<RateToMainCity>().FindAsync(cityId);
            // если  это не основной город то ищем в городах
            if (record == null)
            {

                var additionalRateRecord = await _dbContext.Set<RateToAdditionalCity>().FindAsync(cityId);
                if (additionalRateRecord == null) throw new NullReferenceException("Can't find destination city in additional cities by cityId=" + cityId);
                sum = additionalRateRecord.Rate;
                cityId = additionalRateRecord.MainCityId;
                record = await _dbContext.Set<RateToMainCity>().FindAsync(cityId);
                if (record == null) throw new NullReferenceException("Can't find destination city in main by cityId=" + cityId);
            }


            sum += dpdItems.Sum(item => GetCostOfDpdDeliveryItem(item, record, terminalOrAddress));


            var cost = (int)(sum * (decimal)rateForDelivery);



            return cost;
            // return new CostAndDeliveryTime(cost,deliveryTime);
        }


        /// <summary>
        /// Получить число дней доставки
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="terminalOrAddress"></param>
        /// <param name="type">0- мин. кол-во дней, 1-макс. кол-во дней</param>
        /// <returns></returns>
        public async Task<int> GetNumberOfDaysOfDeliveryAsync(string cityId, int type, Guid? orderGuid = null)
        {
            if (cityId == null) throw new NullReferenceException("CityId  is null");

            var username = GetUserName();
            var dpdItems = await _dbContext.Set<Cart>().Where(cart => cart.CartId == username).Include(p => p.Product).Select(p => new DpdDeliveryItem { DepartmentId = p.DepartmentId, Weight = p.Count * p.Product.Weight }).ToListAsync();
            if (orderGuid != null)
            {
                dpdItems = await _dbContext.Set<OrderDetail>().Where(cart => cart.GuidIn1S == orderGuid).Include(p => p.Product).Select(p => new DpdDeliveryItem { DepartmentId = p.Order.DepartmentId, Weight = p.Count * p.Product.Weight }).ToListAsync();
            }
            if (type == 0) return dpdItems.Min(item => GetNumberOfDaysFromDepartmentToCity(item.DepartmentId, cityId));
            return dpdItems.Max(item => GetNumberOfDaysFromDepartmentToCity(item.DepartmentId, cityId));

        }

        


        /// <summary>
        /// Возвращает число дней доставки от подразделения до города
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public  int GetNumberOfDaysFromDepartmentToCity(int departmentId, string cityId)
        {
            var recordfound = _dbContext.Set<TimeOfDelivery>().FirstOrDefault(p => p.DepartmentId == departmentId && p.CityId == cityId);
            return (recordfound == null) ? -1 : recordfound.DaysTo;

        }


        /// <summary>
        /// Используется для вызовов извне
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="cityId"></param>
        /// <param name="weight"></param>
        /// <param name="terminalOrAddress"></param>
        /// <returns></returns>
        public async Task<int> GetCostOfDeliveryForThirdParty(int departmentId, string cityId, decimal weight, bool terminalOrAddress)
        {
            decimal sum = 0;

            var record = _dbContext.Set<RateToMainCity>().Find(cityId);
            // если  это не основной город то ищем в городах
            if (record == null)
            {

                var additionalRateRecord = await _dbContext.Set<RateToAdditionalCity>().FindAsync(cityId);
                if (additionalRateRecord == null) throw new NullReferenceException("Can't find destination city in additional cities by cityId=" + cityId);
                sum = additionalRateRecord.Rate;
                cityId = additionalRateRecord.MainCityId;
                record = await _dbContext.Set<RateToMainCity>().FindAsync(cityId);
                if (record == null) throw new NullReferenceException("Can't find destination city in main by cityId=" + cityId);
            }


            sum = GetCostOfDpdAllDelivery(departmentId, record, weight, terminalOrAddress);


            return (int)sum;
        }

        public async Task<string> GetCityByIdAsync(string cityId)
        {
            var cityFound=await _dbContext.Set<City>().FindAsync(cityId);

            if (cityFound != null) return cityFound.Name;
            return String.Empty;

        }

        public async Task<string> GetRegionByIdAsync(int RegionId)
        {
           var regionFound= await _dbContext.Set<Region>().FindAsync(RegionId);
           if (regionFound != null) return regionFound.Name;
           return String.Empty;
        }

        /// <summary>
        /// Подготовить строку для сохранения в заказ
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public async Task <string> PrepareDeliveryDataString(DeliveryInfo di)
        {
            string result;

            if (di==null) return null;
            
            if (di.RegionId==null || di.CityId==null) return null;
           var region= await GetRegionByIdAsync(Int32.Parse(di.RegionId));
           var city = await GetCityByIdAsync(di.CityId);

           if (di.TerminalOrAddress) result = String.Format("{0}, г.{1}, ({2}) {3}", region, city,di.TerminalCode, await GetTerminalAddressByCode(di.TerminalCode));
                else result = String.Format("{0}, г.{1}, {2}. {3}, д.{4}", region, city, di.StreetType, di.Street, di.House);

            return result;
        }

        /// <summary>
        /// Получить адрес терминала DPD по коду
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetTerminalAddressByCode(string code)
        {
            var terminalFound = await _dbContext.Set<DpdTerminal>().FindAsync(code);
            
            if (terminalFound!=null) return String.Format(" {0}. {1}, д.{2}", terminalFound.StreetAbbr, terminalFound.Street, terminalFound.HouseNo);

            return null;
        }
    }
}