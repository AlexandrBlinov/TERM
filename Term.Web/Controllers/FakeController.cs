using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Yst.Context;
using YstProject.Services;
using Yst.Terminal.FakeViewModels;
using YstProject.Models;
using File = Term.DAL.File;
using Term.Web.Models;

#if DEBUG
namespace Term.Web.Controllers
{
    [Authorize]
    public class FakeController : Controller
    {
        private AppDbContext _dbContext = new AppDbContext();

        private MtsLocationsContext _dbMtsContext = new MtsLocationsContext();
        //
        // GET: /Fake/ 3+2
        //


        public ActionResult Create()
        {
            var skvs = new PersistedKeyValueStorage();
            

            skvs.Set("test.int", (int)123);
            skvs.Set("test.boolean", true);
            skvs.Set("test.datetime", DateTime.Now);
            skvs.Set("test.string", "asdfsdsdf");

            skvs.Set("season.disk.steel.begindate", new DateTime(2016,02,25));

            return Content("");
        }


        public ActionResult Details(Guid guid)
        {
         // GET: /Fake/central changes from other user
            IEnumerable<Term.DAL.File> files = _dbContext.Files.Where(p => p.GuidIn1S == guid).ToList();
            ViewBag.GuidIn1S = guid;      
            // test 3 changes  
            return View(files);


        }


        public ActionResult Index()
        {
            var lr= _dbMtsContext.LocationsRecords.First();

            return Content($"{ lr.Latitude}");

        }


        public ActionResult Test(int id)
        {
            
            return Content(id.ToString());
        }

        public ActionResult GetOrder(Guid guid)
        {

            var orders = _dbContext.Orders.Where(p => p.GuidIn1S == guid).Include(i => i.OrderDetails.Select(p=>p.Product)).Include(p=>p.OrderDetails).ToArray();

            string str = "";

            foreach (var item in orders[0].OrderDetails)
            {
                //foreach (var item in items)
                {
                    str += item.Product.ProductId.ToString();
                }
           
            }

            return Content(str /*orders[0].OrderDetails.Count.ToString()*/);
        }



        [OutputCache(Duration = 36000, VaryByParam = "id")]

        public  async Task <ActionResult> ShowImage(int id)
        {
            var file = await _dbContext.Files.FirstAsync(p => p.Id == id);


            var stream = new MemoryStream(file.Content);

            return new FileStreamResult(stream, file.ContentType);

            

        }


        public async Task<ActionResult> Upload(Guid guid)
        {

            var files = await _dbContext.Files.Where(p => p.GuidIn1S == guid).ToListAsync();
            ViewBag.GuidIn1S = guid;

            return View(files);

            
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(File model, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0 && upload.ContentLength <= Defaults.MaxUploadedFileSize)
                    {
                        var file = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            GuidIn1S = model.GuidIn1S,
                            Content = new System.IO.BinaryReader(upload.InputStream).ReadBytes(upload.ContentLength),
                            ContentType = upload.ContentType
                            
                        };


                        _dbContext.Files.Add(file);
                        _dbContext.SaveChanges();

                        return RedirectToAction("Upload", "Fake",new{ guid = model.GuidIn1S});
                    }
                    ModelState.AddModelError("", @"Неверный формат  или превышение макс. объема файла");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", @"Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(model);
        }

/*
        public ActionResult Index()
        {
            return View();
        }
        */
        [HttpPost]
        public ActionResult Index(FakeSecondViewModel model)
        {

            return View();
        }


        public ActionResult DoError()
        {
            throw new NotImplementedException("Something goes wrong");
           
        }
        
     /*   public ActionResult PassToForm() {

            return View();
        } */

        //[HttpPost]
        public ActionResult PassToForm(Yst.Terminal.FakeViewModels.FakeFormModel1 model)
        {

            return View(model);
        }

        [HttpPost]
        public ActionResult PassImage()
        {
            
            var file = new File
            {
                FileName = Request.Headers["FileName"],
                GuidIn1S = Guid.Parse(Request.Headers["GUID"]),
                Content = new System.IO.BinaryReader(Request.InputStream).ReadBytes((int)Request.InputStream.Length),
                ContentType = "image/jpeg"

            };
            

            _dbContext.Files.Add(file);
            _dbContext.SaveChanges();
         
            return  new HttpStatusCodeResult(200);
            
        }



        
        public ActionResult ShowModel()
        {

            return View(new PurchaseReturnDto());
        }

        [HttpPost]
        public ActionResult ShowModel( PurchaseReturnDto model)
        {
            if (Request.IsAjaxRequest()) return PartialView("_ClaimItems", model);


            return View(model);
      
        }


    }}

#endif