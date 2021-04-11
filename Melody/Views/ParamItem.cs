using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Views
{
    public class ParamItem
    {
        public readonly string Label;

        public ParamItem(string label)
        {
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParamItem)
            {
                return (obj as ParamItem).Label == Label;
            }
            return false;
        }
    }
}
