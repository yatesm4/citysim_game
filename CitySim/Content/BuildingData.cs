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
        public static Dictionary<int, Building> Dict_BuildingFromObjectID
        {
            get
            {
                return new Dictionary<int, Building>()
                {
                    {1, Building.LogCabin() },
                    {2, Building.Farm() },
                    {3, Building.LowHouse() },
                    {4, Building.Quarry() },
                    {5, Building.PowerLine() },
                    {6, Building.Windmill() },
                    {10, Building.TownHall() }
                };
            }
        }

        public static Dictionary<int, Building> Dict_BuildingKeys
        {
            get
            {
                return new Dictionary<int, Building>()
                {
                    { 1, Building.TownHall() },
                    { 2, Building.LowHouse() },
                    { 3, Building.Farm() },
                    { 4, Building.LogCabin() },
                    { 5, Building.Quarry() },
                    { 6, Building.PowerLine() },
                    { 7, Building.Windmill() }
                };
            }
        }

        /// <summary>
        /// Provides a dictionary with Building, Resource object ids.
        /// </summary>
        public static Dictionary<int, List<int>> Dict_BuildingResourceLinkKeys
        {
            get
            {
                return new Dictionary<int, List<int>>()
                {
                    { 1, new List<int>(){2}}, // log cabin
                    { 2, new List<int>(){10}}, // farm
                    { 4, new List<int>(){4,5,6}} // quarry
                };
            }
        }

        public static Dictionary<int, string> Dic_ResourceNameKeys
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    { 2, "Wood" },
                    { 10, "Food" },
                    { 4, "Stone" },
                    { 5, "Coal" },
                    { 6, "Iron" }
                };
            }
        }

        public static Dictionary<int, Object[]> Dic_ResourceCollectionKeys
        {
            get
            {
                return new Dictionary<int, object[]>()
                {
                    { 2, new Object[]{"Wood", 2}},
                    { 10, new Object[]{"Food", 5}},
                    { 4, new Object[]{"Stone", 4}},
                    { 5, new Object[]{"Coal", 4}},
                    { 6, new Object[]{"Iron", 4}}
                };
            }
        }
    }
}
