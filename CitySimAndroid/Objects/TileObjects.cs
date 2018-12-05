using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;

namespace CitySimAndroid.Objects
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

        public bool RequiresRoad { get; private set; } = true;

        // building constructor
        // - takes list of settings for tileobject
        // - takes list of costs (ints)
        // - takes list of outputs (ints)
        // - takes list of construction / on-purchase fees (int)
        public Building(List<float> settings_, List<int> costs_, List<int> outputs_, List<int> upfronts_)
        {
            SetProperties(settings_, costs_, outputs_, upfronts_);
        }

        public void SetProperties(List<float> settings_, List<int> costs_, List<int> outputs_, List<int> upfronts_)
        {
            // set tileobject properties
            Id = (int)settings_[0];
            TypeId = (int)settings_[1];
            ObjectId = (int)settings_[2];
            TextureIndex = (int)settings_[3];
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

        #region GENERATE BUILDINGS FACTORY METHODS

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
                5, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                10, // workers
                1, // energy
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
                0, // food
            };

            var upfronts = new List<int>()
            {
                40, // gold
                0, // woood
                0, // coal
                0, // iron
                0, // stone
                10, // workers
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
                2, // Range (Visib/Active)
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
                5, // energy
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
                10, // food
            };

            var upfronts = new List<int>()
            {
                100, // gold
                40, // woood
                0, // coal
                0, // iron
                0, // stone
                15, // workers
                5, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Farm"
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
                2, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                7, // gold
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
                0, // food
            };

            var upfronts = new List<int>()
            {
                80, // gold
                30, // woood
                0, // coal
                0, // iron
                0, // stone
                10, // workers
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
                5, // object id: 5, powerline
                16, // texture indnex
                30, // cycle time: 30 seconds
                4, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                5, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                2, // workers
                1, // energy
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
                0, // food
            };

            var upfronts = new List<int>()
            {
                50, // gold
                30, // woood
                0, // coal
                0, // iron
                0, // stone
                2, // workers
                1, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Power Line",
                RequiresRoad = false
            };
        }

        public static Building Windmill()
        {
            var settings = new List<float>()
            {
                206, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                6, // object id: 6, Windmill
                17, // texture indnex
                30, // cycle time: 30 seconds
                2, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                10, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                10, // workers
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
                80, // gold
                30, // woood
                0, // coal
                5, // iron
                20, // stone
                10, // workers
                0, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Windmill",
                RequiresRoad = false
            };
        }

        public static Building Watermill()
        {
            var settings = new List<float>()
            {
                209, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                9, // object id: 9, watermill
                37, // texture indnex
                30, // cycle time: 30 seconds
                2, // Range (Visib/Active)
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                5, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                3, // workers
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
                15, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                50, // gold
                75, // woood
                0, // coal
                0, // iron
                0, // stone
                3, // workers
                0, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Watermill"
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
                10, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
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

        public static Building Road()
        {
            var settings = new List<float>()
            {
                211, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                11, // object id: 3 = low level house
                26, // texture index
                30, // cycle time: 30 seconds
                2,
            };

            // set costs per cycle
            var costs = new List<int>()
            {
                1, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
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
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                5, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                0, // energy
                0 // food
            };

            return new Building(settings, costs, outputs, upfronts)
            {
                Name = "Road",
                RequiresRoad = false
            };
        }

        #endregion

        public void Update(GameTime gameTime)
        {

        }
    }

    public class Resident
    {
        // 200 random names for use with civilians
        public static readonly List<string> RandomNameList = new List<string>()
        {
            "Jaron Chandler","Bryant Lloyd","Harley Ponce","Madalyn Huff","Semaj Pineda","Madden Smith","Samir Ellison","Kristian Burke","Jovanny Haley","Natalie Mccullough","Sage Sanchez","Madeline Saunders","Savanna Pearson","Mylie Walton","Kendrick Snyder","Julianne Franco","Jazlynn Cowan","Aisha Morales","Eden Reese","Jaida Stanley","Randy Nichols","Elianna Lowery","Patrick Sweeney","Kali Schwartz","Hamza Parks","Rolando Simpson","Brielle Frost","Rosemary Shea","Luciana Velazquez","Lillie Cordova","German Browning","Pierce Pham","Brendon Wheeler","Kaley Osborne","Augustus Nolan","Lia Salazar","Elvis Combs","Kayden Casey","Chasity Bullock","Frances Mcdonald","Leon Calderon","Elise Chung","Jairo Kramer","Efrain Cain","Shawn Sullivan","Ariana Hanson","Piper Morrow","Marely Oneill","Casey Joseph","Jacoby Finley","Henry Newman","Jase Garner","Anna Moore","Rowan Hardin","Denisse Flynn","Liam Raymond","Gia Weaver","Willie Reynolds","Eliana Holt","Rayne Anthony","Derrick Harrison","Georgia Moyer","Tommy Ward","Mekhi Tucker","Izabelle Bean","Krish Mcneil","Harmony Ewing","Jacqueline Newton","Sidney Leon","Taylor Mullins","Everett Hurley","Alexia Mckenzie","Josiah Proctor","Alexander Shaw","Davian Cross","Karma Snow","Benjamin Johnston","Paisley Merritt","Emmy Horne","Karter Cummings","Natalia Oliver","Lea Goodman","Yurem Cannon","Nathalie Schaefer","Kadyn Tapia","Jeremiah Park","Keshawn Dean","Nia Colon","Jordan Cobb","Alyson Byrd","Andrew Gardner","Olive Young","Aria Marshall","Haley Schultz","Ashlynn Padilla","Alondra Wilson","Mckenzie Moses","Isaac Dillon","Avery Guerrero","Savion Cline","Abbey Bond","Arabella Taylor","Mareli Alexander","Alison Michael","John Hartman","Darwin Johns","Averi Myers","Amber Thomas","Caden Curtis","Siena Bowman","Tate Woodard","Estrella Owens","Cooper Ibarra","Grady Joyce","Cassandra Harrell","Karli Martin","Kelvin Gomez","Brynn Hutchinson","Bryson Osborn","Asher Lin","Giuliana Wiley","Oliver Meza","Myles Daniel","Kassidy Dunn","Kash Stephens","Terrell Carson","Daphne Rhodes","Raphael Swanson","Chandler Fowler","Delilah Blackwell","Hazel Deleon","Samson Bauer","Kaitlyn Jefferson","Carsen Hicks","Jaydon Moody","Dixie Gamble","Kaydence Briggs","Marcus Jimenez","Davin Waller","Rylan Acevedo","Nyasia Herring","Adolfo Lucero","Brock Faulkner","Frankie Richard","Nevaeh Benson","Gilbert Mendoza","Amya Hensley","Elena Brooks","Quinton Hooper","Salvatore Downs","Kailey Lyons","Jordan Oconnor","Saniyah Ryan","Jaylynn Leonard","Ireland Porter","Sydney Buck","Brian Summers","Titus Sexton","Meghan Cruz","Tristian Miranda","Justine Dodson","Dominique Perkins","Giovani Contreras","Kyra Todd","Kiera Robbins","Madelyn Tate","Kaden Arellano","Aileen Stuart","Xiomara Waters","Owen Olson","Ellis Allison","Javon Atkinson","Ty Keith","Sebastian Ochoa","Audrey Hodges","Sage Carney","Joaquin Schmitt","Tianna Knapp","Jakobe Allen","Anabel Gaines","Ryker Berger","Reilly Tyler","Avah White","Clare Cameron","Naomi Wolfe","Julian Baker","Reed Hancock","Marianna Peterson","Heather Mcgee","Kasen Costa","Anya Hayes","Rebekah Wise","Dayton Ramsey","Julio Sloan","Edgar Hunter","Isabela Stout","Richard Adams","Ally Chaney","Mariela Meyers","Nash Savage"
        };

        public string Name { get; set; } = "Jeff";

        public int DaysAlive { get; set; } = 0;

        public float Education { get; set; } = 0f;

        public float Health { get; set; } = 1.0f;

        public float Happiness { get; set; } = 0f;
    }

    public class Residence : Building
    {
        public List<Resident> Residents = new List<Resident>();

        public Residence(List<float> settings_, List<int> costs_, List<int> outputs_, List<int> upfronts_) : base(settings_, costs_, outputs_, upfronts_)
        {
            // add resident objects to residence
            for (int i = 0; i < WorkersOutput; i++)
            {
                Residents.Add(new Resident());
            }
        }

        // construct a low level house
        public static Residence LowHouse()
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
                4 // food
            };

            var outputs = new List<int>()
            {
                6, // gold
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
                15, // gold
                10, // wood
                0, // coal
                0, // iron
                0, // stone
                0, // workers
                1, // energy
                4 // food
            };

            return new Residence(settings, costs, outputs, upfronts)
            {
                Name = "Low Residential Home"
            };
        }

        // construct a medium level house
        public static Residence MedHouse()
        {
            var settings = new List<float>()
            {
                207, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                7, // object id: 7, mid level house
                20, // texture index
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
                4, // energy
                12 // food
            };

            var outputs = new List<int>()
            {
                8, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                6, // workers
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                40, // gold
                20, // wood
                0, // coal
                0, // iron
                10, // stone
                0, // workers
                4, // energy
                12 // food
            };

            return new Residence(settings, costs, outputs, upfronts)
            {
                Name = "Med Residential Home"
            };
        }

        // construct a medium level house
        public static Residence EliteHouse()
        {
            var settings = new List<float>()
            {
                208, // id = random identifier i have yet to assess how to use
                2, // type id: 2 = building
                8, // object id: 8, elite level house
                23, // texture index
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
                10, // energy
                24 // food
            };

            var outputs = new List<int>()
            {
                15, // gold
                0, // wood
                0, // coal
                0, // iron
                0, // stone
                15, // workers
                0, // energy
                0, // food
            };

            var upfronts = new List<int>()
            {
                150, // gold
                100, // wood
                0, // coal
                50, // iron
                35, // stone
                0, // workers
                10, // energy
                24 // food
            };

            return new Residence(settings, costs, outputs, upfronts)
            {
                Name = "Elite Residential Home"
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

        public static TileObject Farmland()
        {
            return new TileObject
            {
                Id = 206,
                TypeId = 1,
                ObjectId = 10,
                TextureIndex = 13
            };
        }

        public void Update(GameTime gameTime)
        {

        }

        public static TileData Water()
        {
            return new TileData()
            {
                TerrainId = 2,
                Object = new TileObject()
                {
                    Id = 0,
                    TypeId = 1,
                    ObjectId = 3,
                    TextureIndex = 4
                }
            };
        }

        public static TileData Tree()
        {
            return new TileData()
            {
                TerrainId = 0,
                Object = new TileObject()
                {
                    Id = 0,
                    TypeId = 1,
                    ObjectId = 2,
                    TextureIndex = 9
                }
            };
        }

        public static TileData Ore(int obj_id, int txt_id)
        {
            return new TileData()
            {
                TerrainId = 1,
                Object = new TileObject()
                {
                    Id = 0,
                    TypeId = 1,
                    ObjectId = obj_id,
                    TextureIndex = txt_id
                }
            };
        }
    }
}