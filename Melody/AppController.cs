using FileScaner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    class AppController
    {
        private WAVFile file;

        public void ReadFile(string path)
        {
            try
            {
                file = new WAVFile(path);
            } catch (FileScaner.Exceptions.FileReadingException ex)
            {
                throw new AudioFileReadingException(String.Format("File reading was failed: {0}", ex.Message));
            } catch (FileScaner.Exceptions.FileCompressedException ex)
            {
                throw new AudioFileReadingException("This application doesnt support compressed files data");
            } catch (FormatException)
            {
                throw new AudioFileReadingException("File has bad format");
            }
        }
    }
}
