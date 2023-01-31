using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWPMapParser.Source
{
    internal class SourceMap
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public List<SourceLayer> Layers { get; set; }
    }
}
