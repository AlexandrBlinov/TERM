using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Term.Services
{
    /// <summary>
    /// 
    /// </summary>
 public  class GoogleDistanceService
 {
     //https://maps.googleapis.com/maps/api/distancematrix/json?destinations=55.752121,37.617664&origins=57.636927,39.8228632&key=AIzaSyBd_7Xjo_N5QSRRmzTHERgZmX3QO9gK6eM&language=ru&unit=metric&departure_time=now


     private static readonly string GoogleApiKey = "AIzaSyBd_7Xjo_N5QSRRmzTHERgZmX3QO9gK6eM";
        public  async Task<int> GetDurationInSeconds(string origin, string destination)
        {
            int result = 0;
            string url = @"https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" +
              origin + "&destinations=" + destination +
              "&key="+GoogleApiKey+
              "&mode=driving&language=ru&unit=metric";

            
            var request = (HttpWebRequest)WebRequest.Create(url);
                        
            try
            {
                using (var response = await request.GetResponseAsync())
                {            
                    using (var dataStream =  response.GetResponseStream())
                    {
                        result = GetTotalValueFromXml(dataStream, "duration");
                    }
                }

            }
            catch { }

            return result;
        }

     /// <summary>
     /// Получает итоговую сумму после парсинга xml в зависимости от параметра (duration/ distance)
     /// </summary>
     /// <param name="stream"></param>
        /// <param name="parameterName">duration/distance</param>
     /// <returns></returns>
     public static  int GetTotalValueFromXml(Stream stream,string parameterName)
     {
         var xdoc = XDocument.Load(stream);

         var result = xdoc.Elements("DistanceMatrixResponse").Elements("status").Any(el => el.Value == "OK");

         if (!result) return 0;
         
          var duration =   xdoc.Descendants("element")
                 .Elements(parameterName)
                 .Elements("value")
                 .Sum(el => int.Parse(el.Value));

         return duration;
     }

        public static int GetTotalValueFromXml(string  stream, string parameterName)
        {
            var xdoc = XDocument.Load(stream);

            var result = xdoc.Elements("DistanceMatrixResponse").Elements("status").Any(el => el.Value == "OK");

            if (!result) return 0;

            var duration = xdoc.Descendants("element")
                   .Elements(parameterName)
                   .Elements("value")
                   .Sum(el => int.Parse(el.Value));

            return duration;
        }
    }
}
