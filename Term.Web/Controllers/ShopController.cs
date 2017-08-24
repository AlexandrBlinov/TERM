using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YstTerm.Models;
using YstProject.Services;
using PagedList;

namespace YstProject.Controllers
{
    public class ShopController : BaseController
    {
        private ServicePartnerPoint _sPartnerPoint = null;

        protected ServicePartnerPoint ServicePP
        {
            get { return _sPartnerPoint ?? (_sPartnerPoint = new ServicePartnerPoint(DbContext)); }
        }

        public ActionResult Index(DisksPodborView pb, int page = 1, int ItemsPerPage = 50)
        {
            // входные параметры - PartnerId, PartnerPointId
            // сроки поставки от центрального и периферийного складов


            int PartnerPointId = ServicePP.getPartnerPointID(User.Identity.Name);

            pb.ItemsPerPage = ItemsPerPage;
            ServicePP.getDisks(pb, page, PartnerPointId);

           // pb.SearchResults.ToPagedList(10, 50);

            return View("index", pb);

        }

         public ActionResult Disks([Bind(Include = "Name, Article, ProducerId, Width,PCD,ET,DIA,Hole, diametr, DiskColor")] DisksPodborView pb, int page = 1, int ItemsPerPage = 50)
         {


             int PartnerPointId = ServicePP.getPartnerPointID(User.Identity.Name);

             ServicePP.getDisks(pb, page, PartnerPointId);

              return View("Disks", pb);
             
         }

    }
}
