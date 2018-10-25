using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitySim.Content;
using CitySim.States;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.Objects
{
    public class IncomeReport
    {
        public int TotalGoldLoss = 0;
        public int TotalWoodLoss = 0;
        public int TotalCoalLoss = 0;
        public int TotalIronLoss = 0;
        public int TotalStoneLoss = 0;
        public int TotalWorkersLoss = 0;
        public int TotalEnergyLoss = 0;
        public int TotalFoodLoss = 0;

        public int[] TotalLoss => new int[]
        {
            TotalGoldLoss,
            TotalWoodLoss,
            TotalCoalLoss,
            TotalIronLoss,
            TotalStoneLoss,
            TotalFoodLoss,
            TotalEnergyLoss,
            TotalWorkersLoss
        };

        public int TotalGoldGain = 0;
        public int TotalWoodGain = 0;
        public int TotalCoalGain = 0;
        public int TotalIronGain = 0;
        public int TotalStoneGain = 0;
        public int TotalWorkersGain = 0;
        public int TotalEnergyGain = 0;
        public int TotalFoodGain = 0;

        public int[] TotalGain => new int[]
        {
            TotalGoldGain,
            TotalWoodGain,
            TotalCoalGain,
            TotalIronGain,
            TotalStoneGain,
            TotalFoodGain,
            TotalEnergyGain,
            TotalWorkersGain
        };

        public int TotalGoldRevenue = 0;
        public int TotalWoodRevenue = 0;
        public int TotalCoalRevenue = 0;
        public int TotalIronRevenue = 0;
        public int TotalStoneRevenue = 0;
        public int TotalWorkersRevenue = 0;
        public int TotalEnergyRevenue = 0;
        public int TotalFoodRevenue = 0;

        public int[] TotalRevenue => new int[]
        {
            TotalGoldRevenue,
            TotalWoodRevenue,
            TotalCoalRevenue,
            TotalIronRevenue,
            TotalStoneRevenue,
            TotalFoodRevenue,
            TotalEnergyRevenue,
            TotalWorkersRevenue
        };
    }

    public class Map
    {
        // array to hold tiles
        private Tile[,] _tiles;
        public Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        /// <summary>
        /// Generate a report of the per-turn revenue (sum of a resource loss + gain per turn)
        /// </summary>
        public IncomeReport GetIncomeReport
        {
            get
            {
                var r = new IncomeReport();
                foreach (var t in Tiles)
                {
                    r.TotalGoldLoss += t.Object.GoldCost;
                    r.TotalWoodLoss += t.Object.WoodCost;
                    r.TotalCoalLoss += t.Object.CoalCost;
                    r.TotalIronLoss += t.Object.IronCost;
                    r.TotalStoneLoss += t.Object.StoneCost;
                    r.TotalWorkersLoss += t.Object.WorkersCost;
                    r.TotalEnergyLoss += t.Object.EnergyCost;
                    //r.TotalFoodLoss += t.Object.FoodCost;

                    r.TotalGoldGain += t.Object.GoldOutput;
                    r.TotalWoodGain += t.Object.WoodOutput;
                    r.TotalCoalGain += t.Object.CoalOutput;
                    r.TotalIronGain += t.Object.IronOutput;
                    r.TotalStoneGain += t.Object.StoneOutput;
                    r.TotalWorkersGain += t.Object.WorkersOutput;
                    r.TotalEnergyGain += t.Object.EnergyOutput;
                    r.TotalFoodGain += t.Object.FoodOutput;
                }

                r.TotalWorkersGain += 20;
                r.TotalEnergyGain += 30;
                r.TotalFoodGain += 110;

                r.TotalGoldRevenue =  r.TotalGoldGain - r.TotalGoldLoss;
                r.TotalWoodRevenue = r.TotalWoodGain - r.TotalWoodLoss;
                r.TotalCoalRevenue = r.TotalCoalGain - r.TotalCoalLoss;
                r.TotalIronRevenue = r.TotalIronGain - r.TotalIronLoss;
                r.TotalStoneRevenue = r.TotalStoneGain - r.TotalStoneLoss;
                r.TotalWorkersRevenue = r.TotalWorkersGain - r.TotalWorkersLoss;
                r.TotalEnergyRevenue = r.TotalEnergyGain - r.TotalEnergyLoss;

                r.TotalFoodLoss = (r.TotalWorkersRevenue * 2);
                r.TotalFoodRevenue = r.TotalFoodGain - (r.TotalWorkersRevenue * 2);

                return r;
            }
        }

        // tile dimensions (NOT USED CURRENTLY)
        int width, height, tw, th;

        // game content manager
        private GameContent _content { get; set; }
        public GameContent Content
        {
            get { return _content; }
            set { _content = value; }
        }

        // default font
        private SpriteFont font;

        // construct map
        public Map(Tile[,] tiles_, int width_, int height_, int tx_, int ty_, GameContent content_)
        {
            // set tiles
            Tiles = tiles_;
            // set map width and height
            width = width_;
            height = height_;
            // set tile width and height
            tw = tx_;
            th = ty_;
            // set gamecontent manager
            Content = content_;
            // set font
            font = Content.GetFont(1);
        }
        
        // update map
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera, GameState state)
        {
            // for each tile in each row
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // update tile
                    Tiles[x, y].Update(gameTime, keyboardState, camera, state);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tiles[x,y].Draw(gameTime,spriteBatch);
                }
            }

            
        }
    }
}
