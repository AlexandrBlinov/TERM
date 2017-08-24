using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yst.Context;

using System.Data.Entity;
using Term.DAL;
using Term.Services;
using YstProject.Models;
using YstProject.Services;

using System.Threading.Tasks;

namespace Term.Web.Controllers.API
{
    /// <summary>
    /// Сервис Dpd :получить все города с областями, терминалы и стоимость доставки
    /// </summary>
    public class DpdApiController : ApiController
    {
        private readonly AppDbContext _dbContext;

        private readonly DPDGeography2Service _dpdService;
        private readonly DeliveryCostCalculatorService _costCalculatorService;

        private readonly auth _authdata = new auth
        {   
            clientKey = ConfigurationManager.AppSettings["DPDClientKey"],
            clientNumber = Int64.Parse(ConfigurationManager.AppSettings["DPDNumber"]),
        };

       

        

        public DpdApiController() : this(new AppDbContext(), new DPDGeography2Service(), new DeliveryCostCalculatorService()) { }
        public DpdApiController(AppDbContext dbContext, DPDGeography2Service dpdservice,DeliveryCostCalculatorService costCalculatorService)
        {
            _dbContext = dbContext;
            _dpdService = dpdservice;
            _costCalculatorService = costCalculatorService;
        }


        /// <summary>
        /// Получить города с доп информацией по которым есть тарифы доставки 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public IQueryable<CityWithRegionDto> Get(string term)
        {

            var allCityIds =
                _dbContext.Set<RateToMainCity>()
                    .Select(p => p.CityId)
                    .Union(_dbContext.Set<RateToAdditionalCity>().Select(p => p.CityId));

            return
            _dbContext.Set<City>()
                .Include(p => p.Region)
                .Where(city => city.Name.StartsWith(term) && allCityIds.Contains(city.Id))
                .Select(p => new CityWithRegionDto { Id=p.Id, DpdCode = p.DpdCode,Name = p.Name,Abbreviation = p.Abbreviation,RegionId = p.RegionId,RegionName=p.Region.Name}).OrderBy(p=>p.RegionName);

            


        }

        /// <summary>
        ///  Получить список терминалов по городу
        /// </summary>
        /// <param name="kladr"></param>
        /// <returns></returns>
        public IEnumerable<DpdTerminalDto> GetTerminalsByCity(string kladr)
        {

          //  var results=_dpdService.getTerminalsSelfDelivery2(_authdata);
            //return String.Format("{0}, {1} {2} {3}", CityName, StreetAbbr, Street, HouseNo);

            var results = _dbContext.Set<DpdTerminal>().Where(p => p.CityCode == kladr).OrderBy(p => p.Street).ToList().Select(p => new DpdTerminalDto { Name = p.TerminalName, Address = p.ToString(), Code = p.TerminalCode, Schedule = p.Schedule }).AsEnumerable();

            return results;
            
        }



        /// <summary>
        /// Получить стоимость доставки 
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="terminalOrAddress"></param>
        /// <returns></returns>
        public async Task<Object> GetCostAndTimeOfDelivery(string cityId, bool terminalOrAddress, Guid? orderGuid = null)
        {
            var cost = await _costCalculatorService.GetCostOfDelivery(cityId, terminalOrAddress, null, orderGuid);
            var deliveryTimeMin = await _costCalculatorService.GetNumberOfDaysOfDeliveryAsync(cityId, 0, orderGuid);
            var deliveryTimeMax = await _costCalculatorService.GetNumberOfDaysOfDeliveryAsync(cityId, 1, orderGuid);

            return new { Cost = cost, DeliveryTimeMin = deliveryTimeMin, DeliveryTimeMax = deliveryTimeMax };

        }

        /// <summary>
        /// Получить стоимость доставки 
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="terminalOrAddress"></param>
        /// <returns></returns>
        public async Task<int> GetCostOfDeliveryForThirdParty(int departmentId, string cityId, decimal weight, bool terminalOrAddress)
        {
            return  await _costCalculatorService.GetCostOfDeliveryForThirdParty(departmentId, cityId, weight, terminalOrAddress);

        }

        public DateTime GetDateTime()
        {
            return DateTime.Now;

        }

    }
}
