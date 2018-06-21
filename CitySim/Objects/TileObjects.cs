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
    }

    public abstract class Building : TileObject
    {
        #region COSTS
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
        // - takes list of settings for tileobject
        // - takes list of costs (ints)
        // - takes list of outputs (ints)
        public Building(List<float> settings_, List<int> costs_, List<int> outputs_)
        {
            // set tileobject properties
            Id = (int)settings_[0];
            TypeId = (int)settings_[1];
            ObjectId = (int)settings_[2];
            TextureIndex = (int) settings_[3];
            CycleTime = settings_[4];

            // set costs
            GoldCost = costs_[0];
            WoodCost = costs_[1];
            CoalCost = costs_[2];
            IronCost = costs_[3];
            StoneCost = costs_[4];
            WorkersCost = costs_[5];
            EnergyCost = costs_[6];
            FoodCost = costs_[7];

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
