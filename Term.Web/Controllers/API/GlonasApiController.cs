using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Term.Services;
using Term.Utils;
using Term.Web.Services;

namespace Term.Web.Controllers.API
{
    public class GlonasApiController : ApiController
    {
        private readonly GlonasService _gService;
        private readonly GoogleDistanceService _distanceService; 
        public GlonasApiController(GlonasService service, GoogleDistanceService distanceService)
        {
            _gService = service;
            _distanceService = distanceService;
        }

        public GlonasApiController():this(new GlonasService(),new GoogleDistanceService())
        {
        }



        /// <summary>
        /// Получаем ориентировочное время прибытия через документ или Google api
        /// </summary>
        /// <param name="saleguid"></param>

        public async Task<HttpResponseMessage> Get(Guid saleguid)
        {
            DateTime result;
            int lastNumber = 0;
            IList<Coordinates> list;

            var job = _gService.GetJobForSaleByGuid(saleguid);

            if (job == null) return 
                    new HttpResponseMessage { Content = new StringContent("job not found"),
                        StatusCode = HttpStatusCode.InternalServerError };


            if (String.IsNullOrEmpty(job.Driver))
                return new HttpResponseMessage
                {
                    Content = new StringContent("driver name is empty"),
                    StatusCode = HttpStatusCode.InternalServerError
                };

            string drivername = StringUtils.GetFioInitials(job.Driver);

            var coords = _gService.GetDriverCoordinates(drivername);

            var orderGuid = _gService.GetGuidOfOrder(saleguid) ?? Guid.Empty;
            var currentDetail = _gService.GetDetailForOrderOfJob(job, orderGuid);

            if (_gService.GetIfAnyItemsOfJobDelivered(job))
            {


                if (currentDetail.IsDelivered) return new HttpResponseMessage
                {
                    Content = new StringContent("sale already delivered"),
                    StatusCode = HttpStatusCode.InternalServerError
                };

                lastNumber = _gService.GetLastQueueNumberOfDelivered(job);

                if (lastNumber >= currentDetail.NumberOfQueue)
                    return new HttpResponseMessage
                    {
                        Content = new StringContent("missing order of numbers"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };

                // список координат с последней пройденной по текущую
                list = _gService.GetListOfCoordinates(job, lastNumber, currentDetail.NumberOfQueue);


            }
            else

            //  с первой точки по текущую (lastNumber=0)
            {
                list = _gService.GetListOfCoordinates(job, 0, currentDetail.NumberOfQueue);
            }
                        

            string dest = String.Join("|", list);

            int secondsOnWay = await _distanceService.GetDurationInSeconds(coords.ToString(), dest);
            int secondsToUnload = _gService.GetTimeToUnload(job, lastNumber, currentDetail.NumberOfQueue);

            result = DateTime.Now.AddSeconds(secondsOnWay + secondsToUnload);

            return new HttpResponseMessage
            {
                
                Content = new JsonContent(new
                {
                    DriverLongitude = coords.Longitude,
                    DriverLatitude = coords.Latitude,
                    RequestDate = _gService.RequestDate.ToDateTime(),
                    Fio = _gService.DriverFio,
                    Contents = result.ToDateTime() //return exception
                }),

                StatusCode = HttpStatusCode.OK
            };


        }


    }
}
