using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using YstProject.Services;
using YstTerm.Models;

namespace Term.Web.Controllers
{
    public class PodborAkbController : BaseController
    {
        private readonly PodborAkbService _podborakbservice;

        public PodborAkbController()
            : this(new PodborAkbService())
        {
          
        }

        public PodborAkbController(PodborAkbService service)
        { _podborakbservice = service; }

        public ActionResult Index([Bind(Include = "brand, carModel, year, engine")]PodborAkbViewModel model)
        {
            var str = Request.Url.AbsoluteUri;
            str = HttpUtility.UrlDecode(str);

            var split = str.Split('?');
            if (split.Count() == 2)
            {
                var cardata = split[1].Split('&');
                if (cardata.Count() == 4)
                {
                    model.brand = cardata[0];
                    model.carModel = cardata[1];
                    model.year = Convert.ToInt32(cardata[2]);
                    model.engine = cardata[3];
                }
            }

            if (split.Count() == 3)
            {
                var cardata = split[1].Split('&');
                if (cardata.Count() == 4)
                {
                    model.brand = cardata[0];
                    model.carModel = cardata[1];
                    model.year = Convert.ToInt32(cardata[2]);
                    model.engine = cardata[3];
                }
                var param = split[2].Split('&');
                foreach (string item in param)
                {
                    var items = item.Split('&');
                    foreach (string part in items)
                    {
                        var parts = part.Split('=');
                        if (parts[0] == "volumes")
                        {
                            var volumes = parts[1].Split(',');
                            foreach (string volume in volumes)
                            {
                                model.SelectedVolumes.Add(int.Parse(volume));
                            }
                        }
                        if (parts[0] == "selectedsizes")
                        {
                            var selectedsizes = parts[1].Split(',');
                            foreach (string selectedsize in selectedsizes)
                            {
                                model.SelectedSz.Add(selectedsize);
                            }
                        }
                    }
                }
            }

            model.Brands = _podborakbservice.GetBrands();
            if (model.brand != null)  //ViewBag.Brands = _podborakbservice.GetBrands(); 
                model.Models = _podborakbservice.GetCars(model.brand);
            if (model.carModel != null)
                model.Years = _podborakbservice.GetYears(model.brand, model.carModel);
          //  if (model.year != null)
                model.Modifications = _podborakbservice.GetEngines(model.brand, model.carModel, (int)model.year);

            var arr = new object[] { model.brand, model.carModel, model.year, model.engine };

            if (arr.All(p => p != null))
            {
                model.ShowProps = true;

                // максимальный размер (куб)
                model.MaxSize = ((PodborAkbService)_podborakbservice).getMaxSize(model.brand, model.carModel, (int)model.year, model.engine);

                model.Capacities = ((PodborAkbService)_podborakbservice).getCapacitiesFromCube(model.brand, model.carModel, (int)model.year, model.engine, model.MaxSize);

                model.Connection = ((PodborAkbService)_podborakbservice).getConnection(model.brand, model.carModel, (int)model.year, model.engine);

                if (!String.IsNullOrEmpty(model.volumes))
                {
                    var volumes = model.volumes.Split(',');

                    foreach (var volume in volumes)
                        model.SelectedVolumes.Add(int.Parse(volume));

                }
                if (!String.IsNullOrEmpty(model.selectedsizes))
                {
                    foreach (var selectedsize in model.selectedsizes.Split(','))
                        model.SelectedSz.Add(selectedsize);
                }

                int PartnerPointId = ServicePP.getPointID();

                var podbor = new AkbPodborView { Page = 1, ItemsPerPage = 500, PriceMax = Defaults.PriceMaxRus };

                Products.GetAkbs(podbor, PartnerPointId);

                var allakbresults = podbor.SearchResults.Cast<AkbSearchResult>().ToList();

                model.AkbSearchResults = allakbresults.Where(p => model.Capacities.Any(cap => cap.ToString() == p.Capacity) && p.Width <= model.MaxSize.Width && p.Height <= model.MaxSize.Height && p.Length <= model.MaxSize.Length && p.Connection == model.Connection).ToList();
                model.Sizes = model.AkbSearchResults.Select(p => new Size3d { Height = p.Height, Length = p.Length, Width = p.Width }.ToString()).Distinct();

                if (model.SelectedVolumes.Any()) model.AkbSearchResults = model.AkbSearchResults.Where(p => model.SelectedVolumes.Any(fv => fv.ToString() == p.Capacity)).ToList();

                if (model.SelectedSz.Any()) model.AkbSearchResults = model.AkbSearchResults.Where(p => model.SelectedSz.Any(sz => sz == p.Size)).ToList();

                foreach (var item in model.Capacities)
                {
                    model.GroupedResults.Add(item,
                     allakbresults.Where(p => p.Capacity == item.ToString() && p.Width <= model.MaxSize.Width && p.Height <= model.MaxSize.Height && p.Length <= model.MaxSize.Length && p.Connection == model.Connection).ToList());

                }
            }

            return View(model);
        }

        [HttpGet]
        [ActionName("GetModels")]


        public JsonResult GetModels(string brand)
        {
            return Json(_podborakbservice.GetCars(brand), JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [ActionName("GetYears")]

        public JsonResult GetYears(string brand, string carModel)
        {
            return Json(_podborakbservice.GetYears(brand, carModel), JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        [ActionName("GetEngines")]
        public JsonResult GetEngines(string brand, string carModel, int year)
        {
            return Json(_podborakbservice.GetEngines(brand, carModel, year), JsonRequestBehavior.AllowGet);
        }

    }
}
