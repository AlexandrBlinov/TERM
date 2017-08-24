using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YstProject.Services
{
    public class FileFormatErrorException : Exception
    {
        public FileFormatErrorException()
        {
        }

        public FileFormatErrorException(string message)
            : base(message)
        {
        }

    }

    public class FileOperationErrorException : Exception
    {
        public FileOperationErrorException()
        {
        }

        public FileOperationErrorException(string message)
            : base(message)
        {
        }

    }
}