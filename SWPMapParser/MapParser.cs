using Newtonsoft.Json;
using SWPMapParser.Destination;
using SWPMapParser.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace SWPMapParser
{
    internal static class MapParser
    {
        static uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
        static uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
        static uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;
        static uint ROTATED_HEXAGONAL_120_FLAG = 0x10000000;

        public static Map Parse(string name, string file)
        {
            var map = new Map
            {
                Name = name
            };

            var sourceMap = JsonConvert.DeserializeObject<SourceMap>(File.ReadAllText(file));

            map.Width = sourceMap.Width;
            map.Height = sourceMap.Height;

            //Init tiles
            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                    map.Tiles.Add(new Tile { X = x, Y = y });

            foreach (var layer in sourceMap.Layers)
            {
                var tileEntries = Magic(layer.Data.ToArray());

                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        if (tileEntries[x * map.Width + y].Type is TileEntryType.LaserBeam or <0)
                            continue;

                        map.Tiles[x * map.Width + y].TileEntries.Add(tileEntries[x * map.Width + y]);
                    }
                }
            }

            return map;
        }

        private static List<TileEntry> Magic(uint[] data)
        {

            var tiles = new List<TileEntry>();

            for (uint i = 0; i < data.Length; i++)
            {
                uint global_tile_id = data[i];

                // Read out the flags
                bool flipped_horizontally = (global_tile_id & FLIPPED_HORIZONTALLY_FLAG) > 0;
                bool flipped_vertically = (global_tile_id & FLIPPED_VERTICALLY_FLAG) > 0;
                bool flipped_diagonally = (global_tile_id & FLIPPED_DIAGONALLY_FLAG) > 0;
                bool rotated_hex120 = (global_tile_id & ROTATED_HEXAGONAL_120_FLAG) > 0;

                // Clear all four flags
                global_tile_id &= ~(FLIPPED_HORIZONTALLY_FLAG |
                                    FLIPPED_VERTICALLY_FLAG |
                                    FLIPPED_DIAGONALLY_FLAG |
                                    ROTATED_HEXAGONAL_120_FLAG);

                // Resolve the tile
                tiles.Add(new TileEntry
                {
                    Type = (TileEntryType)(global_tile_id - (1 + 0)), //0 first id in tileset, has to be changed if the first id is a different one
                    Orientation = ResolveOrientation(flipped_horizontally, flipped_vertically, flipped_diagonally)
                });
            }

            return tiles;
        }

        /// <summary>
        /// Idk, found out through trial and error
        /// </summary>
        private static Orientation ResolveOrientation(bool flipped_horizontally, bool flipped_vertically, bool flipped_diagonally)
        {
            if (flipped_horizontally && flipped_vertically)
            {
                return Orientation.TopToBottom;
            }
            else if (flipped_horizontally && flipped_diagonally)
            {
                return Orientation.LeftToRight;
            }
            else if (flipped_vertically && flipped_diagonally)
            {
                return Orientation.RightToLeft;
            }


            return Orientation.BottomToTop;
        }
    }
}
