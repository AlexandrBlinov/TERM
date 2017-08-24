using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace YstProject.Services
{
    public class UrlChecker
    {
        public static async Task<bool> CheckIfUrlExistsAsync(string url)
        {
            return await RemoteUrlSuccessAsync(url);
        }
        private static async Task<bool> RemoteUrlSuccessAsync(string url)
        {
            return await Task.Factory.StartNew<bool>(() =>
            {
                HttpWebResponse response = null;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    return true;

                }
                catch 
                {
                    return false;
                }
                finally
                {
                    // Don't forget to close your response.
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            }
          );

        }

        public static bool CheckIfUrlExists(string url)
        {
            return RemoteUrlSuccess(url);
        }
        private static bool RemoteUrlSuccess(string url)
        {


            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return true;

            }
            catch
            {
                return false;
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            } 
            
          

        }
    }
}