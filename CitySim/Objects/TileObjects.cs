using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CitySim.Objects
{
    public class TileObject
    {
        // identifiers
        public int Id { get; set; } = 0;
        public int TypeId { get; set; } = 0;
        public int ObjectId { get; set; } = 0;

        // properties
        //-------------------------------------

        // set texture index to 3 (grass) by default
        public int TextureIndex { get; set; } = 1;

        // how many cycles has this object completed
        public int CyclesCompleted { get; set; } = 0;
        // how long should each cycle take
        public float CycleTime { get; set; } = 10.0f;
        // time passed in cycle
        public float Time { get; set; } = 0;

        #region MAINTENANCE COSTS
        // cost(s) per cycle
        public int GoldCost { get; set; } = 0;
        public int WoodCost { get; set; } = 0;
        public int CoalCost { get; set; } = 0;
        public int IronCost { get; set; } = 0;
        public int StoneCost { get; set; } = 0;
        public int WorkersCost { get; set; } = 0;
        public int EnergyCost { get; set; } = 0;
        public int FoodCost { get; set; } = 0;
        #endregion

        #region CONSTRUCTION COSTS
        // cost on purchase (up front)
        public int GoldUpfront { get; set; } = 0;
        public int WoodUpfront { get; set; } = 0;
        public int CoalUpfront { get; set; } = 0;
        public int IronUpfront { get; set; } = 0;
        public int StoneUpfront { get; set; } = 0;
        public int WorkersUpfront { get; set; } = 0;
        public int EnergyUpfront { get; set; } = 0;
        public int FoodUpfront { get; set; } = 0;
        #endregion

        #region OUTPUTS
        // outputs(s) per cycle
        public int GoldOutput { get; set; } = 0;
        public int WoodOutput { get; set; } = 0;
        public int CoalOutput { get; set; } = 0;
        public int IronOutput { get; set; } = 0;
        public int StoneOutput { get; set; } = 0;
        public int WorkersOutput { get; set; } = 0;
        public int EnergyOutput { get; set; } = 0;
        public int FoodOutput { get; set; } = 0;
        #endregion
    }

    public class Building : TileObject
    {
        public string Name { get; set; } = "Base Building";

        public int Range { get; private set; } = 1;

        // building constructor
        // - takes list of settings for tileobject
        // - takes list of costs (ints)
        // - takes list of outputs (ints)
        // - takes list of construction / on-purchase fees (int)
        public Building(List<float> settings_, List<int> costs_, List<int> outputs_, List<int> upfronts_)
        {
            // set tileobject properties
            Id = (int)settings_[0];
            TypeId = (int)settings_[1];
            ObjectId = (int)settings_[2];
            TextureIndex = (int) settings_[3];
            CycleTime = settings_[4];
            Range = (int)settings_[5];

            // set costs per cycle
            GoldCost = costs_[0];
            WoodCost = costs_[1];
            CoalCost = costs_[2];
            IronCost = costs_[3];
            StoneCost = costs_[4];
            WorkersCost = costs_[5];
            EnergyCost = costs_[6];
            FoodCost = costs_[7];

            // set outputs per cycle
            GoldOutput = outputs_[0];
            WoodOutput = outputs_[1];
            CoalOutput = outputs_[2];
            IronOutput = outputs_[3];
            StoneOutput = outputs_[4];
            WorkersOutput = outputs_[5];
            EnergyOutput = outputs_[6];
            FoodOutput = outputs_[7];

            // set construction/on-purchase fees
            GoldUpfront = upfronts_[0];
            WoodUpfront = upfronts_[1];
            CoalUpfront = upfronts_[2];
            IronUpfront = upfronts_[3];
            StoneUpfront = upfronts_[4];
            WorkersUpfront = upfronts_[5];
            EnergyUpfront = upfronts_[6];
            FoodUpfront = upfronts_[7];
        }

        // construct a log cabin
        public static Building LogCabin()
        {
            var settings = new List<float>()
            {
                201, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                1, // object id: 1 = log cabin
                14, // texture indnex
                30, // cycle time: 30 seconds
                3, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                10, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                5, // workers
                1, // energy
                0 // food
            };

            var outputs = new List<int>()
            {
                0, // gold
                10, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                40, // gold
                15, // woood
                0, // coal
                0, // iron
                0, // stone
                5, // workers
                1, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Log Cabin"
            };
        }

        // construct a log cabin
        public static Building Farm()
        {
            var settings = new List<float>()
            {
                202, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                2, // object id: 2 = log cabin
                12, // texture indnex
                30, // cycle time: 30 seconds
                3, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                30, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                10, // workers
                2, // energy
                0 // food
            };

            var outputs = new List<int>()
            {
                0, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                0, // energy
                30, // food
            };

            var upfronts = new List<int>()
            {
                100, // gold
                40, // woood
                0, // coal
                0, // iron
                0, // stone
                10, // workers
                2, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Farm"
            };
        }

        // construct a low level house
        public static Building LowHouse()
        {
            var settings = new List<float>()
            {
                203, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                3, // object id: 3 = low level house
                11, // texture index
                30, // cycle time: 30 seconds
                2, 
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                0, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                1, // energy
                5 // food
            };

            var outputs = new List<int>()
            {
                10, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                2, // workers
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                20, // gold
                10, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                1, // energy
                5 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Low Residential Home"
            };
        }

        public static Building Quarry()
        {
            var settings = new List<float>()
            {
                204, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                4, // object id: 1 = log cabin
                15, // texture indnex
                30, // cycle time: 30 seconds
                3, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                20, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                15, // workers
                2, // energy
                0 // food
            };

            var outputs = new List<int>()
            {
                0, // gold
                0, // wood
                2, // coal
                2, // iron
                2, // stone
                0, // workers
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                80, // gold
                50, // woood
                0, // coal
                0, // iron
                0, // stone
                15, // workers
                2, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Quarry"
            };
        }

        public static Building PowerLine()
        {
            var settings = new List<float>()
            {
                205, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                5, // object id: 1 = log cabin
                16, // texture indnex
                30, // cycle time: 30 seconds
                4, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                15, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                5, // workers
                0, // energy
                0 // food
            };

            var outputs = new List<int>()
            {
                0, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                30, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                50, // gold
                30, // woood
                0, // coal
                10, // iron
                30, // stone
                5, // workers
                0, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Power Line"
            };
        }

        // construct a town hall
        public static Building TownHall()
        {
            var settings = new List<float>()
            {
                210, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                10, // object id: 10 = town hall
                10, // texture indnex
                30, // cycle time: 60 seconds
                5, 
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                0, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                5, // energy
                0 // food
            };

            var outputs = new List<int>()
            {
                15, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                20, // workers
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                500, // gold
                250, // woood
                0, // coal
                100, // iron
                250, // stone
                0, // workers
                5, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Town Hall"
            };
        }

        public void Update(GameTime gameTime)
        {

        }
    }

    public class Resource : TileObject
    {
        #region OUTPUTS
        // outputs(s) per cycle
        public int GoldOutput { get; set; } = 0;
        public int WoodOutput { get; set; } = 0;
        public int CoalOutput { get; set; } = 0;
        public int IronOutput { get; set; } = 0;
        public int StoneOutput { get; set; } = 0;
        public int WorkersOutput { get; set; } = 0;
        public int EnergyOutput { get; set; } = 0;
        public int FoodOutput { get; set; } = 0;
        #endregion

        // building constructor
        // - takes list of costs (ints)
        public Resource(List<float> settings_, List<int> outputs_)
        {
            // set tileobject properties
            Id = (int)settings_[0];
            TypeId = (int)settings_[1];
            ObjectId = (int)settings_[2];
            TextureIndex = (int)settings_[3];
            CycleTime = settings_[4];

            // set outputs
            GoldOutput = outputs_[0];
            WoodOutput = outputs_[1];
            CoalOutput = outputs_[2];
            IronOutput = outputs_[3];
            StoneOutput = outputs_[4];
            WorkersOutput = outputs_[5];
            EnergyOutput = outputs_[6];
            FoodOutput = outputs_[7];
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
