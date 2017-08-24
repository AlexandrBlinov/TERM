using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace YstProject.Services
{
    public class FileUploaderService
    {
        private static readonly string relativePath = "App_Data";
        private readonly HttpContextBase _httpContext;
        private string _fullPathToFile = null;

        public FileUploaderService():this(new HttpContextWrapper(HttpContext.Current))
        {

        }
        public FileUploaderService(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public string FullPathToFile { get { return _fullPathToFile; } }

        public bool SaveFileFromUpload(HttpPostedFileBase file, string fileFormat)
        {

            var filename = Guid.NewGuid().ToString();
            filename = String.Format("{0}.{1}", filename, fileFormat);

            if (file != null && file.ContentLength > 0 && file.ContentLength <= Defaults.MaxUploadedFileSize)

                try
                {

                    _fullPathToFile = Path.Combine(_httpContext.Server.MapPath(_httpContext.Request.ApplicationPath), relativePath, filename);
                    file.SaveAs(_fullPathToFile);

                }
                catch (Exception exc)
                {
                    ExceptionUtility.LogException(exc, "save file to app dir");
                    throw;

                }
            else
            {
                throw new FileFormatErrorException("file size is too large");
            }
            return true;

        }
        public void DeleteUploadedFile()
        {
            var path = FullPathToFile;
            if (path != null)
                try
                {
                    System.IO.File.Delete(path);

                }
                catch (Exception e)
                {
                    ExceptionUtility.LogException(e, "Upload xlsx file");
                    throw;
                }

        }
    }
}