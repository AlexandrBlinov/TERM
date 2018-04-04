using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;
using Yst.Context;
using System.Data.Entity;
using Term.Utils;
using Term.Web.Models;
using System.Globalization;

namespace Term.Web.Services
{
    /// <summary>
    /// Координаты для Google api
    /// </summary>
    public struct Coordinates
    {
        public Coordinates(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString() =>
       $" {Latitude.ToString("#.#######", CultureInfo.InvariantCulture)},{Longitude.ToString("#.#######", CultureInfo.InvariantCulture)}";
      
    }

    

    /*
     *  Класс для рассчета времени прибытия и отображения точек
     */ 
    public class GlonasService
    {
        private static Coordinates _defCoordinates = new Coordinates(57.657997, 39.83897);
        private struct DriverData
        {

            public string Fio { get; set; }
            public DateTime RequestDate { get; set; }
        }

        private readonly AppDbContext _dbContext;
        private MtsLocationsContext _dbMtsContext;
        private readonly HttpContextBase _httpContext;

        private DriverData _driverdata;
        

        public GlonasService():this (new AppDbContext(),new MtsLocationsContext(), new HttpContextWrapper(HttpContext.Current))
        {

        }
        public GlonasService(AppDbContext dbContext, MtsLocationsContext mts, HttpContextBase httpContext)
        {
            _dbContext = dbContext;
            _dbMtsContext = mts;
            _httpContext = httpContext;
           // _record = null;
        }


        /// <summary>
        /// Получить guid заказа или ничего если нет
        /// </summary>
        /// <param name="saleguid"></param>
        /// <returns></returns>
        public Guid? GetGuidOfOrder(Guid saleguid)
        {
            return _dbContext.Set<Sale>().FirstOrDefault(s => s.GuidIn1S == saleguid)?.GuidOfOrderIn1S;
        }

        /*
         *  По guid реализации находит задание на отгрузку в которых есть эта реализация
         */
        public JobForShipment GetJobForSaleByGuid(Guid saleguid)
        {  

            Guid? guidOfOrder = GetGuidOfOrder(saleguid);
            if (guidOfOrder == null) throw new NullReferenceException("sale not found");

            return _dbContext.Set<JobForShipment>().FirstOrDefault(d => d.Details.Any(detail => detail.GuidOfOrderIn1S == guidOfOrder));
          
        }

        /*
         *  Проверить, есть ли хотя бы одно доставленное 
         */ 
        public bool GetIfAnyItemsOfJobDelivered( JobForShipment job) => job.Details.Any(detail => detail.IsDelivered);

        /// <summary>
        /// Получает последний номер очереди доставленного
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public int GetLastQueueNumberOfDelivered(JobForShipment job) =>   job.Details.Where(detail => detail.IsDelivered).Max(n => n.NumberOfQueue);


        /// <summary>
        /// Получает первый номер очереди
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public int GetFirstQueueNumber(JobForShipment job) => job.Details.Min(n => n.NumberOfQueue);


        /*
         * Получить детали задания
         */
        public IList<JobForShipmentDetail> GetDetailsOfJob (JobForShipment job ) => job.Details.ToList();

       
        /// <summary>
        /// Получить текущую запись по заданию на отгрузку для данного заказа 
        /// </summary>
        /// <returns></returns>
        public JobForShipmentDetail GetDetailForOrderOfJob(JobForShipment job,Guid orderGuid)
        {

            return job.Details.FirstOrDefault(detail => detail.GuidOfOrderIn1S == orderGuid);
        }

        /// <summary>
        /// Получить список координат 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="initQueue"></param>
        /// <param name="lastQueue"></param>
        /// <returns></returns>
       public IList<Coordinates> GetListOfCoordinates(JobForShipment job, int initQueue, int lastQueue, bool fromYar=false)
        {
            var list = new List<Coordinates>();
            
          var listOfSelectedItems = job.Details.Where(p => p.NumberOfQueue > initQueue  && p.NumberOfQueue <= lastQueue).OrderBy(detail => detail.NumberOfQueue).ToList(); // .Distinct(detail=>detail.Latitude);

            foreach (var item in listOfSelectedItems)
            {
                var coord = new Coordinates (item.Latitude, item.Longitude);

                if (!list.Contains(coord)) list.Add(coord);
                
            }
            return list;
        }



        /// <summary>
        ///     
        /// </summary>
        /// <param name="driverFio"></param>
        /// <returns></returns>
        public Coordinates GetDriverCoordinates(string driverFio)
        {
           driverFio= StringUtils.GetFioInitials(driverFio);

         var subscriberId=   _dbMtsContext.Subscribers.ToList().FirstOrDefault(p => StringUtils.GetFioInitials(p.FIO) == driverFio)?.SubscriberId;
           
            if (subscriberId == null) throw new NullReferenceException("driver is not found by name");

         var record  = _dbMtsContext.LocationsRecords.Where(p => p.SubscriberId == subscriberId).OrderByDescending(p=>p.RequestDate).FirstOrDefault();

            _driverdata.Fio = driverFio;
            _driverdata.RequestDate = record.RequestDate;

            if (record == null) throw new NullReferenceException($"driver's {subscriberId} coordinates  are not found ");
            return new Coordinates(record.Latitude, record.Longitude);

        }

        public string DriverFio => _driverdata.Fio;
        public DateTime RequestDate => _driverdata.RequestDate;

        /// <summary>
        /// Время разгрузки в секундах
        /// </summary>
        /// <param name="job"></param>
        /// <param name="initQueue"></param>
        /// <param name="lastQueue"></param>
        /// <returns></returns>

        public int GetTimeToUnload(JobForShipment job, int initQueue, int lastQueue)
        {
            var listOfSelectedItems = job.Details.Where(p => p.NumberOfQueue > initQueue && p.NumberOfQueue < lastQueue).OrderBy(detail => detail.NumberOfQueue).ToList(); // .Distinct(detail=>detail.Latitude);

            int result = 0;
            foreach (var item in listOfSelectedItems)
            {
                result += item.PlanUnloadTime;

            }
            return result;

        }

    }
}