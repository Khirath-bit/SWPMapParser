using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWPMapParser.Destination
{
    internal class Tile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public List<TileEntity> TileEntities { get; set; } = new List<TileEntity>();
    }
}
