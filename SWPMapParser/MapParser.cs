using Newtonsoft.Json;
using SWPMapParser.Destination;
using SWPMapParser.Source;
using System.Collections.Generic;
using System.IO;

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

            map.Height = sourceMap.Width;
            map.Width = sourceMap.Height;

            //Init tiles
            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                    map.Tiles.Add(new Tile { X = y, Y = x });

            foreach (var layer in sourceMap.Layers)
            {
                var tileEntries = Magic(layer.Data.ToArray());

                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        if (tileEntries[(map.Width - 1 - x) * map.Width + y].Type is TileEntryType.LaserBeam or < 0)
                            continue;

                        map.Tiles[x * map.Width + y].TileEntities.Add(tileEntries[(map.Width - 1 - x) * map.Width + y]);
                        //WALLS ARE DEFAULT SOUTH!!!!
                    }
                }
            }

            return map;
        }

        private static List<TileEntity> Magic(uint[] data)
        {

            var tiles = new List<TileEntity>();

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

                var type = (TileEntryType)(global_tile_id - (1 + 0)); //0 first id in tileset, has to be changed if the first id is a different one

                // Resolve the tile
                tiles.Add(new TileEntity
                {
                    Type = type, 
                    Orientation = (Orientation)ResolveOrientation(flipped_horizontally, flipped_vertically, flipped_diagonally, type)
                });
            }

            return tiles;
        }

        /// <summary>
        /// Idk, found out through trial and error
        /// </summary>
        private static int ResolveOrientation(bool flipped_horizontally, bool flipped_vertically, bool flipped_diagonally, TileEntryType type)
        {
            var wallOffset = 0;

            if(type == TileEntryType.Wand)
            {
                wallOffset = 2;
            }

            if (flipped_horizontally && flipped_vertically)
            {
                return (int)(Orientation.SOUTH + wallOffset) % 4;
            }
            else if (flipped_horizontally && flipped_diagonally)
            {
                return (int)(Orientation.EAST + wallOffset) % 4;
            }
            else if (flipped_vertically && flipped_diagonally)
            {
                return (int)(Orientation.WEST + wallOffset) % 4;
            }


            return (int)(Orientation.NORTH + wallOffset) % 4;
        }
    }
}
