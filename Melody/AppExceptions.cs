using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    class AudioFileReadingException : Exception
    {
        public AudioFileReadingException(string message) : base(message) { }
    }
    
    class FileNotLoadedException : Exception
    {
        public FileNotLoadedException(string message) : base(message) { }
    }

    class SpectrumNotCalculatedException : Exception
    {
        public SpectrumNotCalculatedException(string message) : base(message) { }
    }
}
