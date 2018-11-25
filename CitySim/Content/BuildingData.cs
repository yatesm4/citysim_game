using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Objects;

namespace CitySim.Content
{
    static class BuildingData
    {
        public static Dictionary<int, Building> Dict_BuildingFromObjectID => new Dictionary<int, Building>()
        {
            {1, Building.LogCabin()},
            {2, Building.Farm()},
            {3, Residence.LowHouse()},
            {4, Building.Quarry()},
            {5, Building.PowerLine()},
            {6, Building.Windmill()},
            {7, Residence.MedHouse()},
            {8, Residence.EliteHouse()},
            {9, Building.Watermill()},
            {10, Building.TownHall()},
            {11, Building.Road() }
        };

        public static Dictionary<int, Building> Dict_BuildingKeys => new Dictionary<int, Building>()
        {
            // Residential Building Btn Icon Id + Building Constructor
            {100, Residence.LowHouse()},
            {101, Residence.MedHouse()},
            {102, Residence.EliteHouse()},

            // Resource Building Btn Icon Id + Building Constructor
            {200, Building.TownHall()},
            {201, Building.Farm()},
            {202, Building.LogCabin()},
            {203, Building.Quarry()},
            {204, Building.PowerLine()},
            {205, Building.Windmill()},
            {206, Building.Watermill()},

            // Decoration Building Btn Icon Id + Building Constructor
            {300, Building.Road()}
        };

        /// <summary>
        /// Provides a dictionary with Building, Resource object ids.
        /// </summary>
        public static Dictionary<int, List<int>> Dict_BuildingResourceLinkKeys => new Dictionary<int, List<int>>()
        {
            {1, new List<int>() {2}}, // log cabin -> tree
            {2, new List<int>() {10}}, // farm -> farmland
            {4, new List<int>() {4, 5, 6}}, // quarry  -> ore(s)
            //{9, new List<int>() {3} } // watermill -> water
        };

        /// <summary>
        /// Get name of resource from resource object ID
        /// </summary>
        public static Dictionary<int, string> Dic_ResourceNameKeys => new Dictionary<int, string>()
        {
            {2, "Wood"},
            {10, "Food"},
            {4, "Stone"},
            {5, "Coal"},
            {6, "Iron"},
            {3, "Water" }
        };

        /// <summary>
        /// From a resource's object ID, get the name and amount to be harvested per cell
        /// </summary>
        public static Dictionary<int, Object[]> Dic_ResourceCollectionKeys => new Dictionary<int, object[]>()
        {
            {2, new Object[] {"Wood", 1}},
            {10, new Object[] {"Food", 5}},
            {4, new Object[] {"Stone", 2}},
            {5, new Object[] {"Coal", 2}},
            {6, new Object[] {"Iron", 2}},
            {3, new object[] {"Water", 0} }
        };

        public static bool ValidBuilding(TileObject obj)
        {
            return ValidObj(obj) && obj.TypeId == 2;
        }

        public static bool ValidResource(TileObject obj)
        {
            return ValidObj(obj) && obj.TypeId == 1;
        }

        public static bool ValidObj(TileObject obj)
        {
            return obj != null && obj.ObjectId > 0;
        }
    }
}
