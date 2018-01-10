using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Term.Services;
using Yst.ViewModels;
using Yst.Context;
using System.Net;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using YstProject.Services;

namespace Term.Web.Controllers
{
    public class ReportsController : Controller
    {
        readonly AppDbContext _dbContext;
        private ServiceTerminal _ws;
        protected ServiceTerminal WS
        {
            get
            {
                return _ws ?? (_ws =
                new ServiceTerminal
                {
                    PreAuthenticate = true,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginWS"],
                ConfigurationManager.AppSettings["PasswordWS"])
                });
            }
        }
        public ReportsController() : this(new AppDbContext()) { }
        public ReportsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult> Freeman(ReportModel model)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            model.PartnerId = Defaults.FreemanCode;
            if (model.BeginDate != null)
            {
                var start = model.BeginDate ?? DateTime.Now;
                var stop = model.EndDate ?? DateTime.Now;
                model.ReturnItems = await Task.Run(() => WS.ReturnOfDefectiveReport(model.PartnerId, start, stop));
            }
            return View(model);
        }

        public async Task<ActionResult> FreemanWheelsTest(ReportModel model)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            model.PartnerId = Defaults.FreemanCode;
            if (model.BeginDate != null)
            {
                var start = model.BeginDate ?? DateTime.Now;
                var stop = model.EndDate ?? DateTime.Now;
                model.ReturnWheelsTest = await Task.Run(() => WS.WheelsTestReport(model.PartnerId, start, stop));
            }
            return View(model);
        }

        public ActionResult VideoTests(int productId)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Defaults.VideoLabFTP);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginVideoLab"], ConfigurationManager.AppSettings["PasswordVideoLab"]);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            List<string> directories = new List<string>();

            string line = streamReader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                if (line.Contains(productId.ToString())) directories.Add(line);
                line = streamReader.ReadLine();
            }
            ViewBag.Files = directories;
            streamReader.Close();
            return View();
        }

        public async Task Download(string name)
        {

            var downloadedData = new byte[0];

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Defaults.VideoLabFTP + name);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["LoginVideoLab"], ConfigurationManager.AppSettings["PasswordVideoLab"]);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                //Streams
                FtpWebResponse response = request.GetResponse() as FtpWebResponse;
                Stream reader = response.GetResponseStream();

                //Download to memory
                //Note: adjust the streams here to download directly to the hard drive
                MemoryStream memStream = new MemoryStream();
                byte[] buffer = new byte[1024]; //downloads in chuncks

                while (true)
                {

                    //Try to read the data
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        //Write the downloaded data
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }

                //Convert the downloaded stream to a byte array
                downloadedData = memStream.ToArray();

                //Clean up
                memStream.Close();
            }
            catch (Exception)
            {

            }
            Response.ContentType = "video/x-ms-wmv";
            Response.BinaryWrite(downloadedData);
        }

        public ActionResult Details(string claimnumber, int productId)
        {
            var model = new ClaimsViewWithDetails();
            var num = Convert.ToInt32(claimnumber.Remove(1, 1));
            model.Claim = _dbContext.Claims.Where(p => p.NumberIn1S == num).FirstOrDefault();
            if (model.Claim != null)
                model.ClaimDetails = _dbContext.ClaimsDetails.Where(p => p.GuidIn1S == model.Claim.GuidIn1S && p.ProductId == productId).ToList();

            return View(model);
        }
    }
}
