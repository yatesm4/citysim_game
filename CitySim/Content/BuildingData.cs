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
                    {7, Building.MedHouse() },
                    {8, Building.EliteHouse() },
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
                    { 200, Building.TownHall() },
                    { 100, Building.LowHouse() },
                    { 201, Building.Farm() },
                    { 202, Building.LogCabin() },
                    { 203, Building.Quarry() },
                    { 204, Building.PowerLine() },
                    { 205, Building.Windmill() },
                    { 101, Building.MedHouse() },
                    { 102, Building.EliteHouse() },
                    { 300, Building.Road_Left() },
                    { 301, Building.Road_Right() },
                    { 302, Building.Road_4_Way_Intersection() },
                    { 303, Building.Road_T_Intersection_1() },
                    { 304, Building.Road_T_Intersection_2() },
                    { 305, Building.Road_T_Intersection_3() },
                    { 306, Building.Road_T_Intersection_4() }
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
