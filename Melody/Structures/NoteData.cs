using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    class NoteData
    {
        public readonly string Name;
        public readonly double Hz;

        public NoteData(string name, double hz)
        {
            Name = name;
            Hz = hz;
        }
    }
}
