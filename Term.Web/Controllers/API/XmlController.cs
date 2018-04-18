using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Term.Web.Filters;
using Yst.Services;
using YstProject.Services;

namespace Term.Web.Controllers.API
{
    [ErrorIfPricesAreBeingUpdatedFilter]
    
    public class XmlController : ApiController
    {
        private readonly static string _resourcenotfound = "Resource not found";
        

        private ServicePartnerPoint _servicePartnerPoint = null;
        private XMLService _xmlService = null;
        
       

        public XmlController():this (new ServicePartnerPoint(),new XMLService())
        {       }

        public XmlController(ServicePartnerPoint servicePartnerPoint, XMLService xMLService)
        {
            // TODO: Complete member initialization
            this._servicePartnerPoint = servicePartnerPoint;
            this._xmlService = xMLService;
        }
     

        private void CheckPointId(int pointId)
        {
            if (pointId == -1)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(_resourcenotfound),

                };
                throw new HttpResponseException(resp);
            }
        }


        
        [HttpGet]
        [ActionName("accs")]
        [TrackUserApiAction]
        public IQueryable<PriceListAccXml> GetAcc(string id)
        {
            int pointId = _servicePartnerPoint.GetPointByUserID(id);
            CheckPointId(pointId);

            return _xmlService.getAccsForPriceList(pointId);
            
            
        }

        [HttpGet]
        [ActionName("akb")]
        [TrackUserApiAction]
        public IQueryable<PriceListAkbXml> GetAkb(string id)
        {
            int pointId = _servicePartnerPoint.GetPointByUserID(id);
            CheckPointId(pointId);
            

            return  _xmlService.getAkbForPriceList(pointId);
            

        }
        /// <summary>
        /// http://localhost:9090/api/xml/tyre/560563b2-c787-42be-9b0d-47c122580f4b
        /// </summary>
        /// <param name="id">id is uid of user- partnerpoint</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("tyre")]
        [TrackUserApiAction]
        public IQueryable<PriceListTyreXml> GetTyres(string id, int? typeofrests = 0)
        {
            int pointId = _servicePartnerPoint.GetPointByUserID(id);

            CheckPointId(pointId);
          

            var result = _xmlService.getTyreForPriceList(pointId);
             if (typeofrests != 0) result = result.Where(p => (p.RestOtherStock == 0 && (p.RestMain > 0 || p.RestEkb > 0 || p.RestMsk > 0 || p.RestRnd > 0 || p.RestSpb > 0)));
            return result;
        }

        /// <summary>
        /// http://localhost:9090/api/xml/disk/560563b2-c787-42be-9b0d-47c122580f4b
        /// </summary>
        /// <param name="id">id is uid of user- partnerpoint</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("disk")]
        [TrackUserApiAction]
        public IQueryable<PriceListDiskXml> GetDisks(string id)
        {
            int pointId = _servicePartnerPoint.GetPointByUserID(id);
           

               CheckPointId(pointId); 

            return _xmlService.getDiskForPriceList(pointId);
            

        }

    }
}
