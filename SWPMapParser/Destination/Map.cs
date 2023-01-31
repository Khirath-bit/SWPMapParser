using System;
using System.Collections.Generic;
using System.Linq;

namespace SWPMapParser.Destination
{
    internal class Map
    {
        public Map()
        {
            AddMapping();
        }

        public List<Tile> Tiles { get; set; } = new List<Tile>();

        public int Width { get; set; }

        public int Height { get; set; }

        public string Name { get; set; }

        public List<TypeMapping> TypeMapping { get; set; } = new();

        public List<OrientationMapping> OrientationMapping { get; set; } = new();

        private void AddMapping()
        {
            foreach (var enumVal in Enum.GetValues(typeof(TileEntryType)).Cast<TileEntryType>())
            {
                TypeMapping.Add(new TypeMapping { Type = enumVal.ToString(), Value = (int)enumVal});
            }

            foreach (var enumVal in Enum.GetValues(typeof(Orientation)).Cast<Orientation>())
            {
                OrientationMapping.Add(new OrientationMapping { Orientation = enumVal.ToString(), Value = (int)enumVal });
            }
        }
    }
}
