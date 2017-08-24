using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Term.DAL;
using Term.Web.Services;
using YstProject.Services;
using YstTerm.Models;

namespace Term.Web.Controllers
{
    public class PodborAutoTyresDisksController : BaseController
    
    {
        private static readonly string _space = Defaults.Space, _invisible = Defaults.Invisible;

        private  readonly PodborTyreDiskService _podborTyreDiskService;

        public PodborAutoTyresDisksController(PodborTyreDiskService podborService)
        {
            _podborTyreDiskService = podborService;
        }

        public PodborAutoTyresDisksController() : this(new PodborTyreDiskService())
        {
                
        }

        //
        // GET: /PodborAutoTyresDisks/

        public ActionResult Index([Bind(Include = "brand, model, year, engine")]PodborAutoView podborModel)
        {

            podborModel.Brands = _podborTyreDiskService.GetBrands();

            podborModel.Models = _podborTyreDiskService.GetCars(podborModel.brand);

            podborModel.Years = _podborTyreDiskService.GetYears(podborModel.brand, podborModel.model);

            podborModel.Engines = _podborTyreDiskService.GetEngines(podborModel.brand, podborModel.model, podborModel.year);


            var isPartner = ServicePP.IsPartner;
            var SaleMode = ServicePP.IsSaleAvailable;

            ViewBag.SaleMode = SaleMode && isPartner ? _space : _invisible;



            bool AddStock = ServicePP.CanPartnerUseAdditionalStock();
            ViewBag.AddStock = AddStock;
            if (!ModelState.IsValid) throw new HttpException(404, "Not found");
            String[] pavParams = { podborModel.model, podborModel.brand, podborModel.engine };
            ViewBag.Length = 0;
            if (!pavParams.Any(p => String.IsNullOrEmpty(p)))
            {



                // var arr = pav.getJsonArray();
                var engine = podborModel.engine.Replace("~", "/");
                var arr = _podborTyreDiskService.GetResults(podborModel.brand, podborModel.model, podborModel.year, engine).ToArray();
                ViewBag.Bolts = arr[0].BoltsSize.Replace(',', '.');
                if (arr[0].IsBolts)
                    ViewBag.BoltsUrl = Url.Action("Others", "Home", new { @OthersType = "Bolts", @Folder = Defaults.WheelsBoltsFolder, @Width = arr[0].Size1, @Diametr = arr[0].Size2 });
                else
                    ViewBag.BoltsUrl = Url.Action("Others", "Home", new { @OthersType = "Bolts", @Folder = Defaults.WheelsNutsFolder, @Width = arr[0].Size1, @Diametr = arr[0].Size2 });

                ViewBag.tyres = arr.Where(p => p.ProductType == ProductType.Tyre).ToArray();
                ViewBag.disks = arr.Where(p => p.ProductType == ProductType.Disk).ToArray();

                /* ViewBag.tyresZavod = arr.Where(p => p.ProductType == ProductType.Tyre && p.Mode == 0).ToArray();

                ViewBag.disksZavod = arr.Where(p => p.ProductType == ProductType.Disk && p.Mode == 0).ToArray();
                ViewBag.tyresZamena = arr.Where(p => p.ProductType == ProductType.Tyre && (p.Mode == 1 || p.Mode == 2)).ToArray();
                ViewBag.disksZamena = arr.Where(p => p.ProductType == ProductType.Disk && (p.Mode == 1 || p.Mode == 2)).ToArray();*/
                ViewBag.Length = Math.Max(ViewBag.tyres.Length, ViewBag.disks.Length);

            }
            return View(podborModel);


        }

       
    }
}
