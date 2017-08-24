using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace YstProject.Controllers
{
    public class Disp : IDisposable
    {
        private Stream str;

        public void Dispose()
        {
            Dispose(true);
        }

        ~Disp()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                str.Dispose();
            }
        }
    }
}