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
                    { 6, Building.PowerLine() }
                };
            }
        }
    }
}
