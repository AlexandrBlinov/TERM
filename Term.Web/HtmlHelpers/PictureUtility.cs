using Term.Web.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yst.Context;
//using Yst.Commons;
using Yst.Services;
using Term.DAL;
using System.Globalization;
using YstProject.Services;

namespace Term.Web.HtmlHelpers
{
    public static class PictureUtility
    {
              
        private static string[] _cargoWheelsProducers = Defaults.CargoWheelsProducers.Split(Defaults.CommaSign);
        static string _fullRemotePathToImage = ConfigurationManager.AppSettings["RemotePathToPictures"];

        public static MvcHtmlString Thumbnail(this HtmlHelper htmlHelper, Product product)
        {
            if (product == null) throw new NullReferenceException("product passed to picture can't be null");
            string url = GetThumbnailOfProduct(product);
            return MvcHtmlString.Create(url);
        }


        public static MvcHtmlString RemotePicture(this HtmlHelper htmlHelper, int productId)
        {

            return  MvcHtmlString.Create(GetRemotePictureByProductId(productId));
            
        }


        public static MvcHtmlString RemotePicture(this HtmlHelper htmlHelper, Product product)
        
        {
            if (product==null) throw new NullReferenceException("product passed to picture can't be null");
        string url =GetRemotePictureOfProduct(product);
        return MvcHtmlString.Create(url);
        }

       
        public static string getRemoteFileName( string producer, string file)
        {
            string str_replica = "replica";
            string str_legeartis = "legeartis"; 

            String filenameremote;
         if (String.Compare(producer,"all",StringComparison.InvariantCultureIgnoreCase)==0)
            filenameremote = String.Format("{0}.png", file.ToLower());
         else
         {
             producer= producer.ToLower();
             if (producer.Contains(str_replica)) producer = str_legeartis;
            // if (String.Compare(producer, str_replica, true) == 0) producer = str_legeartis;
            filenameremote = String.Format("{1}/{0}.png", file.ToLower(),producer);

         
            }
         return filenameremote;
        }

        /// <summary>
        /// Gets picture by product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static string GetRemotePictureByProductId(int productId)
        {

            using (var db = new AppDbContext())
            {
                var product = db.Set<Product>().First(p => p.ProductId == productId);

                return GetRemotePictureOfProduct(product);
            }
        }


        /// <summary>
        /// Gets picture by product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>

        private static string GetRemotePictureOfProduct(Product product)
        {
            if (product == null) return String.Empty;
            switch (product.ProductType)
            {
                case ProductType.Tyre: { return GetPictureOfTyre(product.ModelId); }
                case ProductType.Disk: {
                    if (product.Producer == null || product.Model == null) return String.Empty;
                    
                        return GetPictureOfDisk(product.ProducerId, product.Model.Name, product.Name, product.ProductId);
                    
                }
                case ProductType.Akb: { return GetPictureOfAkb(product.ProductId); }
                case ProductType.Acc: { return GetPictureOfAcc(product.ProductId); }
                

            }
            return String.Empty;
        }

        public static string GetPictureOfTyre(int? modelId)
        {
            string tires = "tires";
            if (modelId.HasValue)
            {
                return UrlGenerator.Generate(_fullRemotePathToImage, tires, String.Concat(modelId.ToString().PadLeft(5, '0'),".png"));

            }
            return String.Empty;
        }
        /// <summary>
        /// Картинку товара на детальной странице
        /// </summary>
        /// <param name="producerId"></param>
        /// <param name="modelName"></param>
        /// <param name="name"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static string GetPictureOfDisk(int? producerId, string modelName, string name, int productId)
        {
           

            string wheels = "allwheels";
            if (producerId != null && _cargoWheelsProducers.Contains(producerId.ToString()) )
            {
                var url = UrlGenerator.Generate(_fullRemotePathToImage, wheels, String.Concat(productId.ToString(), ".png"));

                return url;
            }
            string color_pattern = @"\sD[\d.]+\s(.*)";
            //string legeartis = "legeartis";
            
            //string str_replica = "replica";

            string color = String.Empty;
            Match m = Regex.Match(name, color_pattern, RegexOptions.IgnoreCase);

            if (m.Length > 0 && m.Groups.Count > 1) color = m.Groups[1].ToString();
            color = color.Replace(' ', '_');

            //if (name.ToLower().Contains(legeartis) || producer.ToLower().Contains(str_replica)) producer = legeartis;
            var path = UrlGenerator.Generate(_fullRemotePathToImage, wheels, String.Concat(modelName, "_", color, ".png"));

            return path;
        }



        /// <summary>
        /// Получает путь к миниатюрной картинке диска
        /// </summary>
        /// <param name="producerName"></param>
        /// <param name="modelName"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string GetPictureOfThumbnailForDisk(int? producerId, string modelName, string Name, int productId)
        {
            if (producerId != null && _cargoWheelsProducers.Contains(producerId.ToString()) )
            {
                return productId.ToString() ?? string.Empty;
            }
            modelName =modelName?? String.Empty;
            
            modelName = modelName.ToLower().Trim(new Char[] {' ', '_'});
            
              string pattern = @"[\.\s\(\)\:\+]";
              string replacement = "_";
               string name = Regex.Replace(modelName, pattern, replacement);
            

              if (name.Contains("concept")) name = "_" + name;
              if (name.Contains("model_forged")) name = name.Replace("_", " ");

              string color_pattern = @"\sD[\d.]+\s(.*)";
              string color = String.Empty;
              Match m = Regex.Match(Name, color_pattern, RegexOptions.IgnoreCase);
              if (m.Length > 0 && m.Groups.Count > 1) color = m.Groups[1].ToString();
              color = color.Replace(' ', '_').ToLowerInvariant();
            //  return String.Concat(producer, "~", name, "_", color); 
              return String.Concat(name, "_", color); 
        }

        

       
        public static string GetPictureOfAkb(int productId)
        {

            return String.Format("{0}batteries/{1}.png", _fullRemotePathToImage, productId.ToString().PadLeft(7, '0'));
        }

        public static string GetPictureOfAcc(int productId)
        {
            return String.Format("{0}accessories/{1}.png", _fullRemotePathToImage, productId.ToString().PadLeft(7, '0'));

        }

        public static string GetThumbnailOfProduct(Product product)
        {
            
            switch (product.ProductType)
            {
                case ProductType.Tyre:
                    if (product.ModelId != null)
                        return String.Format("/thumbnail/tyre/{0}", product.ModelId.ToString().PadLeft(5, '0'));
                    else return String.Format("/thumbnail/tyre/{0}", product.ProductId);
                case ProductType.Disk:
                    if (product.Model!=null)
                    return String.Format("/thumbnail/disk/{0}", GetPictureOfThumbnailForDisk(product.ProducerId, product.Model.Name, product.Name, product.ProductId));
                    else
                    {
                        return String.Format("/thumbnail/disk/{0}", product.ProductId);
                    }

                
                case ProductType.Akb: return String.Format("/thumbnail/akb/{0}", product.ProductId);
                case ProductType.Acc: return String.Format("/thumbnail/acc/{0}", product.ProductId);
                default: return String.Format("/thumbnail/others/{0}", product.ProductId);
            }
        }

    }
}
