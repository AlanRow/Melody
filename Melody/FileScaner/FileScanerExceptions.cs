using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileScaner.Exceptions
{
    class FileReadingException : Exception 
    {
        public FileReadingException(string message) : base(message) {}
    }

    class FileCompressedException : Exception 
    {
        public FileCompressedException(string message) : base(message) { }
    }
}
