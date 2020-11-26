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
}
