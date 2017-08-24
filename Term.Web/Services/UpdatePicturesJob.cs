using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Yst.Context;
using Term.DAL;
using System.Data.Entity;
using Term.Web.HtmlHelpers;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlTypes;
using Yst.Services;

namespace YstProject.Services
{

    [XmlType("item")]
    public class PictureItem
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Path;
    }

    class UpdateDisksPicturesJob : IJob
    {

        public async void Execute(IJobExecutionContext context)
        {
           

            using (var db = new AppDbContext())
            {

                db.Configuration.AutoDetectChangesEnabled = false;
               

                db.DbActionLogs.Add(new DbActionLogs ("updated pictures of disks start" ));
                db.SaveChanges();

                var dictPaths = new Dictionary<string, bool>();
                var dictProducts = new Dictionary<Product, string>();

                
                var disks = db.Set<Product>().Include(pr => pr.Producer).Include(m => m.Model).
                    Where(p => p.ProductType == ProductType.Disk && p.Producer.Active && p.ModelId != null && p.PathToRemotePicture == null).ToArray();

                int counter = 0;
                db.DbActionLogs.Add(new DbActionLogs(disks.Length.ToString()));
                foreach (var disk in disks)
                {
                  
                    
                    if (disk.Producer == null || disk.Model == null) continue;
                    string path = PictureUtility.GetPictureOfDisk(disk.ProducerId, disk.Model.Name, disk.Name, disk.ProductId);

                    if (String.Compare(disk.PathToRemotePicture, path) == 0) continue;

                   
                    if (dictPaths.ContainsKey(path)) dictProducts.Add(disk, dictPaths[path] ? path : null);
                    else
                    {
                        if (await UrlChecker.CheckIfUrlExistsAsync(path))
                        {
              
                            counter++;
                            dictPaths.Add(path, true);
                            dictProducts.Add(disk, path);
                        }
                        else
                        {
              
                            dictPaths.Add(path, false);
                            dictProducts.Add(disk, null);
                        }

                    }

                }
                

                var items = dictProducts.Where(p => p.Value != null).Select(p => new PictureItem { Id = p.Key.ProductId, Name = p.Key.Name, Path = p.Value }).ToArray();

                db.DbActionLogs.Add(new DbActionLogs ( "updated pictures of disks dict " +items.Length.ToString()));
                db.SaveChanges();

                if (items.Length == 0) return;
                

                using (var mem = new MemoryStream())
                {
                    try
                    {
                  
                        var serializer = new XmlSerializer(typeof(PictureItem[]), new XmlRootAttribute() { ElementName = "items" });
                  
                        serializer.Serialize(mem, items);
                  
                        mem.Seek(0, System.IO.SeekOrigin.Begin);

                        string errorMsg;
                  

                        var result = SPExecutor.Execute("spUpdatePicturesWithRemoteSource", new SqlParameter { ParameterName = "@xmlData", SqlDbType = SqlDbType.Xml, Direction = ParameterDirection.Input, Value = new SqlXml(mem) }, out errorMsg);
                  
                        var retRes = result != 0 ? errorMsg : "success updating";
                    
                       // db.DbActionLogs.Add(new DbActionLogs ( retRes));

                        
                        db.SaveChanges();
                    }
                    catch (Exception exc)
                    {
                        ILogger logger = new Logger();
                        logger.Error(exc.ToString());
                        db.DbActionLogs.Add(new DbActionLogs("error calling stored procedure"));
                        db.SaveChanges();
                      
                    }

                }
            }
        }
    }


    class UpdateTyresPicturesJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AppDbContext())
            {

                db.DbActionLogs.Add(new DbActionLogs("updated pictures of tyres start"));
                db.SaveChanges();


                var products = db.Set<Product>().Include(pr => pr.Producer).Include(m => m.Model).Where(p => p.ProductType == ProductType.Tyre && p.Producer.Active && p.ModelId != null && p.PathToRemotePicture == null
                && db.Set<RestOfProduct>().Any(rop => rop.ProductId == p.ProductId));

                int counter = 0;
                foreach (var product in products)
                {

                    var path = PictureUtility.GetPictureOfTyre(product.ModelId);
                    if (!String.IsNullOrEmpty(path))
                    {
                        if (UrlChecker.CheckIfUrlExists(path))
                        {

                            counter++;
                            product.PathToRemotePicture = path;
                        }
                    }


                }

                db.DbActionLogs.Add(new DbActionLogs("updated pictures of tyres end. Updated=" + counter.ToString()));
                db.SaveChanges();
            }
        }
    }

    class UpdateOthersPicturesJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (var db = new AppDbContext())
            {

                db.DbActionLogs.Add(new DbActionLogs("updated pictures of others start"));
                db.SaveChanges();


                var products = db.Set<Product>().Where(p => p.ProductType != ProductType.Tyre &&  p.ProductType != ProductType.Disk /*&& p.Producer.Active && p.ModelId != null*/ && p.PathToRemotePicture == null
                && db.Set<RestOfProduct>().Any(rop => rop.ProductId == p.ProductId));

                int counter = 0;
                foreach (var product in products)
                {
                    string path;   
                    if (product.ProductType == ProductType.Akb) path = PictureUtility.GetPictureOfAkb(product.ProductId);
                    else path = PictureUtility.GetPictureOfAcc(product.ProductId);
                    if (!String.IsNullOrEmpty(path))
                    {
                        if (UrlChecker.CheckIfUrlExists(path))
                        {
                            counter++;
                            product.PathToRemotePicture = path;
                        }
                    }
                    
                }

                db.DbActionLogs.Add(new DbActionLogs("updated pictures of others end. Updated=" + counter.ToString()));
                db.SaveChanges();
            }
        }
    }
}

   
    

