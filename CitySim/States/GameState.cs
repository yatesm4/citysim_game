using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitySim;
using CitySim.Content;
using CitySim.Objects;
using CitySim.States;
using CitySim.UI;

using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Newtonsoft.Json;

namespace CitySim.States
{
    public class GameStateData
    {
        // time data
        public int Day { get; set; } = 1;
        public int Year { get { return Day / 365; } }
        public float DayTime { get; set; } = 0f;

        // player inventory data
        public Inventory PlayerInventory { get; set; } = new Inventory();

        // map data
        public List<TileData> TileData { get; set; } = new List<TileData>();
    }

    public class GameState : State
    {
        #region PROPS

        #region SYSTEM & DEBUGGING
        // is game currently debugging?
        private bool _debug { get; set; } = false;
        private Random _rndGen = new Random();

        private GraphicsDevice _graphicsDevice { get; set; }
        #endregion

        #region LOADING

        // if the game is currently saving
        protected bool IsSaving = false;

        // how much of the load pool is remaining (gets subtracted from with every milestone)
        private int _remainingLoad = 100;

        public int LoadProgress
        {
            get { return 100 - _remainingLoad; }
        }

        // is the game loaded?
        public bool IsLoaded
        {
            get { return LoadProgress.Equals(100); }
        }

        // is the game currently loading?
        public bool IsLoading = false;

        public string LoadingText { get; set; } = "Loading Game...";

        public Rectangle LoadingBar { get; set; }

        public Texture2D LoadingTexture { get; set; }
        public Texture2D LoadingCellTexture { get; set; }

        public Texture2D LoadingScreen_MapCellTexture { get; set; }
        public Texture2D LoadingScreen_MapCellHighlightedTexture { get; set; }
        public Vector2 LoadingScreen_CurrentCell { get; set; }
        public List<Vector2> LoadingScreen_HighlightedCells { get; set; } = new List<Vector2>();

        #endregion

        #region GAME CONTENT
        // gamecontent manager - holds all sprites, effects, sounds
        private GameContent _gameContent { get; set; }

        // texture for mouse cursor
        private Texture2D _cursorTexture { get; set; }

        // font for writing text
        private SpriteFont _font { get; set; }
        #endregion

        #region MAP AND CAMERA
        // current map rendering
        private Map _currentMap { get; set; }

        public Map CurrentMap => _currentMap;

        // game camera
        private Camera _camera { get; set; }
        #endregion

        #region MOUSE & KEYBOARD STATES
        // previous keyboard state (before current)
        private KeyboardState _previousKeyboardState { get; set; }

        // previous mouse state (before current)
        private MouseState _previousMouseState { get; set; }
        #endregion

        #region EXTRA PROPERTIES
        // first render?
        private bool _firstTake { get; set; } = true;
        #endregion

        #region GAME STATE DATA

        public GameStateData GSData { get; set; }

        private const float _timeCycleDelay = 8; // seconds
        private float _remainingDelay = _timeCycleDelay;

        // some extra building information
        private Vector2 _townHallIndex = Vector2.Zero;

        public Tile CurrentlySelectedTile { get; set; }

        #endregion

        #region COMPONENTS
        public List<Component> Components { get; set; } = new List<Component>();

        public TileObject SelectedObject { get; set; } = new TileObject();

        public Tile CurrentlyHoveredTile { get; set; }
        #endregion

        #endregion

        #region MAP PROPS
        private int _mapBounds = 75;

        private List<TileData> _tileData { get; set; }

        // generation props
        private float _tileWidth = 17f;
        private float _tileHeight = 8.5f;
        private float _tileScale = 2f;
        #endregion

        #region METHODS

        #region CONSTRUCTOR
        public GameState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content, bool newgame) : base(game, graphicsDevice, content)
        {
            SharedConstruction(graphicsDevice, content);

            if(newgame is true)
            {
                Console.WriteLine($"Starting new game...");
                Task.Run(() => GenerateMap());
            } else
            {
                Console.WriteLine($"Loading previous game...");
                Task.Run(() => LoadGame());
            }

            // generate gamestate data for new game || load gamestate data from previous game
            InitGameStateData();
        }

        // a method that can be called from both constructors to run general startup logic
        public void SharedConstruction(GraphicsDevice graphicsDevice, ContentManager content)
        {
            // create new gamecontent instance
            _gameContent = new GameContent(content);

            // save graphics device
            _graphicsDevice = graphicsDevice;

            LoadLoadingScreen();

            if (_remainingLoad < 100) _remainingLoad = 100;

            // load (mouse) cursor content
            _cursorTexture = _gameContent.GetUiTexture(4);

            // create camera instance and set its position to mid map
            _camera = new Camera(graphicsDevice);
            //_camera.Zoom = 1.5f;

            // load the hud in a separate thread
            Task.Run(() => LoadHUD());
        }

        public void LoadLoadingScreen()
        {
            var loading_bar_dimensions = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 8);
            var loading_bar_location =
                new Vector2(_graphicsDevice.Viewport.Width / 4, (_graphicsDevice.Viewport.Height / 4) * 2.5f);

            LoadingBar = new Rectangle((int)loading_bar_location.X, (int)loading_bar_location.Y, (int)loading_bar_dimensions.X, (int)loading_bar_dimensions.Y);

            LoadingTexture = new Texture2D(_graphicsDevice, 1, 1);
            LoadingTexture.SetData(new[] { Color.DarkSlateGray });

            LoadingCellTexture = new Texture2D(_graphicsDevice, 1, 1);
            LoadingCellTexture.SetData(new[] { Color.LightCyan });

            LoadingScreen_MapCellTexture = new Texture2D(_graphicsDevice, 1, 1);
            LoadingScreen_MapCellTexture.SetData(new[] { Color.WhiteSmoke });
            LoadingScreen_MapCellHighlightedTexture = new Texture2D(_graphicsDevice, 1, 1);
            LoadingScreen_MapCellHighlightedTexture.SetData(new[] { Color.Red });
        }

        public async void LoadHUD()
        {
            Components.Add(new HUD(_graphicsDevice, _gameContent));
        }

        public void InitGameStateData()
        {
            GSData = new GameStateData();

            GSData.PlayerInventory = new Inventory();
        }
        #endregion

        #region HANDLE MAP DATA

        public async void LoadGame()
        {
            LoadingText = $"Loading map...";

            await LoadMap();

            LoadingText = $"Wrapping things up...";

            _remainingLoad -= 100;
        }

        public async Task<bool> LoadMap()
        {
            // array to hold tiles
            Tile[,] tileArr_ = new Tile[_mapBounds, _mapBounds];
            string data = null;

            try
            {
                LoadingText = $"Reading save files...";
                // try to read map data from data_map.json file
                using (var streamReader = new System.IO.StreamReader($"GAMEDATA.json"))
                {
                    data = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                // if the data read isn't null or empty, load the map | else, throw exception
                if (string.IsNullOrEmpty(data).Equals(true))
                {
                    LoadingText = $"Map data corrupted... one moment...";
                    throw new NotSupportedException("Error Reading Map Data: Data is empty.");
                }
                else
                {
                    Console.WriteLine($"Loading map...");

                    // deserialize data to gamestate data object
                    GameStateData loaded_GSData = JsonConvert.DeserializeObject<GameStateData>(data);
                    // set gamestate data
                    GSData = loaded_GSData;
                    GSData.PlayerInventory = loaded_GSData.PlayerInventory;
                    GSData.TileData = loaded_GSData.TileData;

                    // get tiledata from gamestate data to List of _tileData
                    List<TileData> tdList_ = loaded_GSData.TileData;

                    LoadingText = $"Putting things back together...";
                    // for each _tileData loaded
                    foreach (TileData t in tdList_)
                    {
                        // get x and y index
                        var x = (int)t.TileIndex.X;
                        var y = (int)t.TileIndex.Y;

                        // create new tile and pass gamecontent instance and _tileData
                        tileArr_[x, y] = new Tile(_gameContent, _graphicsDevice, t);
                        tileArr_[x, y].Click += Tile_OnClick;
                        if (tileArr_[x, y].Object.TypeId.Equals(2) && tileArr_[x, y].Object.ObjectId.Equals(10))
                            _camera.Position = tileArr_[x, y].Position;
                    }

                    Console.WriteLine("Map restored - tile count: " + tileArr_.Length);
                }

                LoadingText = $"Looping through map data completed...";
                // create new map instance from loaded data
                _currentMap = new Map(tileArr_, _mapBounds, _mapBounds, 34, 100, _gameContent);


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Loading Map Data: {e.Message}.");
                return false;
            }
        }

        // generate map
        public async void GenerateMap()
        {
            LoadingText = $"Generating map...";
            Console.WriteLine(LoadingText);

            // create list to hold tile data
            _tileData = new List<TileData>();

            // ------------------------------------------   GENERATE TILES  ------------------------------------------
            /**
             * Recursively generate all of the tiles of the map. At first, they will be made as empty tiles to be populated in the below logic.
             */
            for (var x = 0; x < _mapBounds; x++)
            {
                for (var y = 0; y < _mapBounds; y++)
                {

                    // calculate position based on tile dimensions and row index (Width / 2 = 17, Height = 9 (Middle Offset))
                    // the position is calculated so that the tile will be placed in a fashion that it will render isometrically
                    // this means rendering tiles side by side, but also connecting them by offsetting the x and y with each row
                    // so that the "diamond" shape of each tile fits together snug
                    var position = new Vector2(x * (_tileWidth * _tileScale) - y * (_tileWidth * _tileScale), x * (_tileHeight * _tileScale) + y * (_tileHeight * _tileScale));

                    // set tile and (inner) object data
                    var td = new TileData
                    {
                        TileIndex = new Vector2(x, y),
                        Position = position,
                        Object = new TileObject()
                    };

                    // add _tileData to list
                    _tileData.Add(td);
                }
            }
            LoadingScreen_HighlightedCells = new List<Vector2>();
            _remainingLoad -= 10;

            // ------------------------------------------   GENERATE WATER  ------------------------------------------
            LoadingText = $"Filling lakes with water...";
            _remainingLoad -= GenerateResource( Resource.Water(), new int[,] { {700000, 800000, 900000, 990000}, {1000000, 0, 0, 0} }, 20 );

            // ------------------------------------------   GENERATE TREES  ------------------------------------------
            LoadingText = $"Growing some trees...";
            _remainingLoad -= GenerateResource( Resource.Tree(), new int[,] { {700000, 800000, 900000, 990000}, {1000000, 0, 0, 0} }, 20 );

            // ------------------------------------------   GENERATE ORES   ------------------------------------------
            /**
             * At first, an array is made to hold the object and texture id's of the ores, and also a loading progress int is used as well to determine the progress of the loading bar after that resources is finished generating
             */
            LoadingText = $"Creating ores...";
            int[,] gen_ore_vals = new int[,] { {4, 5, 20}, {5, 6, 0}, {6, 7, 20} };
            for (int e = 0; e < 3; e++) { _remainingLoad -= GenerateResource( Resource.Ore(gen_ore_vals[e, 0], gen_ore_vals[e, 1]), new int[,] { {900000, 950000, 975000, 995000}, {1000000, 0, 0, 0} }, gen_ore_vals[e, 2] ); }

            // ------------------------------------------   CLEAN UP MAP    ------------------------------------------
            /**
             * For example, fill in water tiles so that splotches of random water tile placement fill in to look more like lakes - do similar cleaning up to other resources as well
             */
            LoadingText = $"Cleaning up map...";
            for (var x = 0; x < _mapBounds; x++)
            {
                for (var y = 0; y < _mapBounds; y++)
                {
                    var index = new Vector2(x,y);
                    var tile = from t in _tileData where t.TileIndex == index select t;
                    if (!tile.Any()) continue;
                    var tile_loaded = tile.FirstOrDefault();

                    // if the terrain / object is already set, skip
                    if (tile_loaded.TerrainId != 0) continue;
                    if (tile_loaded.Object.TypeId > 0) continue;

                    var matchingTiles = 0;
                    for (int o = 0; o < 4; o++)
                    {
                        Vector2 dir = new Vector2(0, 0);
                        switch (o)
                        {
                            case 0:
                                dir = index + new Vector2(1, 0);
                                break;
                            case 1:
                                dir = index + new Vector2(0, 1);
                                break;
                            case 2:
                                dir = index + new Vector2(-1, 0);
                                break;
                            case 3:
                                dir = index + new Vector2(0, -1);
                                break;
                        }

                        var adj_tiles = from tiles in _tileData where tiles.TileIndex == dir select tiles;
                        if (adj_tiles.Any())
                        {
                            LoadingScreen_CurrentCell = dir;
                            foreach (var adj_tile in adj_tiles)
                            {
                                // run check
                                if (adj_tile.TerrainId.Equals(2))
                                    matchingTiles++;
                            }
                        }
                    }

                    if (matchingTiles >= 2)
                    {
                        tile_loaded.TerrainId = 2;
                        tile_loaded.Object = new TileObject()
                        {
                            Id = Convert.ToInt32($"{x}{y}"),
                            TypeId = 1,
                            ObjectId = 3,
                            TextureIndex = 4
                        };
                    }
                }
            }

            // ------------------------------------------   GEN START AREA  ------------------------------------------ 
            var townHall = Building.TownHall();
            bool townHallMade = false;

            LoadingText = $"Picking a spot for you to start...";
            for (int x = 0; x < _mapBounds; x++)
            {
                for(int y = 0; y < _mapBounds; y++)
                {

                    if (townHallMade is true)
                        continue;

                    if (x > _mapBounds - 20 || x < 20 || y < 20 || y > _mapBounds - 20)
                        continue;

                    var sel_index = new Vector2(x, y);
                    var sel_tile = from t in _tileData where t.TileIndex == sel_index select t;
                    var found_tile = sel_tile.FirstOrDefault();

                    if (found_tile is null)
                        continue;

                    if (found_tile.TerrainId > 0 && found_tile.Object.TypeId > 0)
                        continue;

                    var adj_grass_tiles = 0;

                    // check to see if any adjacent tiles are grass
                    for (int o = 0; o < 4; o++)
                    {
                        Vector2 dir = new Vector2(0, 0);
                        switch (o)
                        {
                            case 0:
                                dir = sel_index + new Vector2(1, 0);
                                break;
                            case 1:
                                dir = sel_index + new Vector2(0, 1);
                                break;
                            case 2:
                                dir = sel_index + new Vector2(-1, 0);
                                break;
                            case 3:
                                dir = sel_index + new Vector2(0, -1);
                                break;
                        }

                        var adj_tiles = from tiles in _tileData where tiles.TileIndex == dir select tiles;
                        if (adj_tiles.Any())
                        {
                            adj_grass_tiles += adj_tiles.Count(adj_tile => adj_tile.Object.TypeId < 1 && adj_tile.Object.ObjectId < 1 && adj_tile.TerrainId < 1);
                        }
                    }

                    if (adj_grass_tiles > 3)
                    {
                        int i = _rndGen.Next(0, 1000000);
                        if (i > 900000)
                        {
                            int j = _rndGen.Next(0, 1000000);
                            if (j > 500000)
                            {
                                found_tile.Object = townHall;
                                townHallMade = true;
                                _townHallIndex = new Vector2(x, y);
                            }
                        }
                    }
                }
            }

            // sum of resources in starting area
            var starting_tree_cnt = 0;
            var starting_stone_cnt = 0;

            List<TileData> starting_area_tiles = new List<TileData>();

            // loop thru the tiles around the town hall and add them/ give them visibility
            LoadingText = $"Lighting up the area...";
            for (int x = ((int)_townHallIndex.X - townHall.Range); x < (_townHallIndex.X + (townHall.Range + 1)); x++)
            {
                for (int y = ((int)_townHallIndex.Y - townHall.Range); y < (_townHallIndex.Y + (townHall.Range + 1)); y++)
                {

                    var sel_index = new Vector2(x, y);
                    var sel_tile = from t in _tileData where t.TileIndex == sel_index select t;
                    var found_tile = sel_tile.FirstOrDefault();

                    if(!(found_tile is null))
                    {
                        // there is a tile in this index
                        found_tile.IsVisible = true;

                        if (found_tile.Object.ObjectId.Equals(Resource.Tree().Object.ObjectId) &&
                            found_tile.Object.TypeId.Equals(Resource.Tree().Object.TypeId)) starting_tree_cnt++;
                        else if (found_tile.Object.ObjectId.Equals(Resource.Ore(4, 5).Object.ObjectId) &&
                                 found_tile.Object.TypeId.Equals(Resource.Ore(4, 5).Object.TypeId)) starting_stone_cnt++;
                        else
                        {
                            if(!(found_tile.Object.ObjectId > 0 || found_tile.Object.TypeId > 0)) starting_area_tiles.Add(found_tile);
                        }
                    }
                }
            }

            if (starting_tree_cnt < 4)
            {
                var trees_to_spawn = 4 - starting_tree_cnt;
                while (trees_to_spawn > 0)
                {
                    var rnd_tile = starting_area_tiles.OrderBy(x => _rndGen.Next()).Take(1).FirstOrDefault();
                    var sel_index = new Vector2(rnd_tile.TileIndex.X, rnd_tile.TileIndex.Y);
                    var sel_tile = from t in _tileData where t.TileIndex == sel_index select t;
                    var found_tile = sel_tile.FirstOrDefault();
                    found_tile.TerrainId = 0;
                    found_tile.Object = Resource.Tree().Object;
                    trees_to_spawn--;
                }
            }

            if (starting_stone_cnt.Equals(0))
            {
                var stone_to_spawn = 3;
                var rnd_tiles = starting_area_tiles.OrderBy(x => _rndGen.Next()).Take(stone_to_spawn);
                foreach (var rnd_tile in rnd_tiles)
                {
                    var sel_index = new Vector2(rnd_tile.TileIndex.X, rnd_tile.TileIndex.Y);
                    var sel_tile = from t in _tileData where t.TileIndex == sel_index select t;
                    var found_tile = sel_tile.FirstOrDefault();
                    found_tile.TerrainId = 1;
                    found_tile.Object = Resource.Ore(4, 5).Object;
                }
            }

            // generate newgame data and set tiledata to generated map
            GameStateData newgame = new GameStateData(){ TileData = _tileData };

            // get data_map.json file & write _tileData to file (overwrite any)
            using (var streamWriter = new System.IO.StreamWriter($"GAMEDATA.json")){ streamWriter.WriteLine(JsonConvert.SerializeObject(newgame, Formatting.Indented)); }
            Console.WriteLine("Map finished generating.");

            // now, load the map
            await LoadMap();
            LoadingText = $"Wrapping things up...";

            // set the camera's initial position
            _camera.Position = _currentMap.Tiles[(int)_townHallIndex.X, (int)_townHallIndex.Y].Position + new Vector2(0, 150);

            // top off the remaining load
            _remainingLoad -= 10;
        }

        /// <summary>
        /// Generates a resource (recursively). Will loop through game tiles and populate them randomly with the specificed resource.
        /// </summary>
        /// <param name="gen_resource">The TileData to be applied to selected tiles. Primarily will use the supplied TileObject of the TileData.</param>
        /// <param name="gen_range_vals">The generation values for RNG spawning.</param>
        /// <param name="rem_load_reduc">The amount to be reduced from the remaining load percentage upon completion of the recursion call.</param>
        /// <returns></returns>
        public int GenerateResource(TileData gen_resource, int[,] gen_range_vals, int rem_load_reduc = 0)
        {
            for (var x = 0; x < _mapBounds; x++)
            {
                for (var y = 0; y < _mapBounds; y++)
                {
                    RecursiveResourceSpawn(x, y, gen_resource, gen_range_vals);
                }
            }
            return rem_load_reduc;
        }

        public void RecursiveResourceSpawn(int x, int y, TileData resource, int[,] range_vals)
        {
            var index = new Vector2(x, y);
            var td = from a in _tileData where a.TileIndex == index select a;
            if (!td.Any()) return; // theres no tile so just skip
            foreach (var t in td)
            {
                // (if the tile is water || if the tile is already a resource/building)- end
                if ((t.TerrainId.Equals(2)) || (t.Object.TypeId.Equals(1) || t.Object.TypeId.Equals(2)))
                {
                    return;
                }

                // rng resource chance
                int i = _rndGen.Next(0, range_vals[1,0]);

                // current amount of similar adjacent tiles
                int similar_adj_tiles = 0;

                // check to see if any adjacent tiles are already water
                for (int o = 0; o < 4; o++)
                {
                    // direction being checked
                    Vector2 dir = new Vector2(0, 0);

                    // for each dir (each in 4)
                    switch (o)
                    {
                        case 0:
                            dir = index + new Vector2(1, 0);
                            break;
                        case 1:
                            dir = index + new Vector2(0, 1);
                            break;
                        case 2:
                            dir = index + new Vector2(-1, 0);
                            break;
                        case 3:
                            dir = index + new Vector2(0, -1);
                            break;
                    }

                    // get tile where index matches the index of tile being faced
                    var adj_tiles = from tiles in _tileData where tiles.TileIndex == dir select tiles;
                    if (adj_tiles.Any())
                    {
                        // if resource being applied is water
                        if (resource.TerrainId.Equals(2))
                        {
                            similar_adj_tiles += adj_tiles.Count(adj_tile => adj_tile.TerrainId.Equals(2));
                        }
                        else
                        {
                            if (resource.Object.ObjectId.Equals(1) || resource.Object.ObjectId.Equals(2))
                            {
                                similar_adj_tiles += adj_tiles.Count(adj_tile => adj_tile.Object.TypeId.Equals(resource.Object.TypeId) && (adj_tile.Object.ObjectId.Equals(1) || adj_tile.Object.ObjectId.Equals(2)));
                            }
                            else
                            {
                                similar_adj_tiles += adj_tiles.Count(adj_tile => adj_tile.Object.TypeId.Equals(resource.Object.TypeId) && adj_tile.Object.ObjectId.Equals(resource.Object.ObjectId));
                            }
                        }
                    }
                }
                // if random chance
                if (i > (similar_adj_tiles > 0 ? (similar_adj_tiles  < 2 ? range_vals[0,0] : (similar_adj_tiles < 3 ? range_vals[0,1] : range_vals[0,2])) : range_vals[0,3]))
                {
                    // generate water tile
                    t.TerrainId = resource.TerrainId;
                    t.Object = new TileObject()
                    {
                        Id = Convert.ToInt32($"{index.X}{index.Y}"),
                        TypeId = resource.Object.TypeId,
                        ObjectId = resource.Object.ObjectId,
                        TextureIndex = resource.Object.TextureIndex
                    };
                    // for each adjacent direction
                    for (var loop_dir = 0; loop_dir < 4; loop_dir++)
                    {
                        Vector2 ref_tile = new Vector2(0, 0);
                        // set the tile index offset for the direction
                        switch (loop_dir)
                        {
                            case 0:
                                ref_tile = new Vector2(1, 0);
                                break;
                            case 1:
                                ref_tile = new Vector2(0, 1);
                                break;
                            case 2:
                                ref_tile = new Vector2(-1, 0);
                                break;
                            case 3:
                                ref_tile = new Vector2(0, -1);
                                break;
                        }
                        ref_tile = index += ref_tile;
                        RecursiveResourceSpawn((int)ref_tile.X, (int)ref_tile.Y, resource, range_vals);
                    }
                }
            }
        }
        #endregion

        #region UPDATE & POST UPDATE
        // update
        public override void Update(GameTime gameTime)
        {
            if (IsLoaded)
            {
                // game state is loaded

                // get current keyboard state
                var keyboardState = Keyboard.GetState();
                var mouseState = Mouse.GetState();

                // handle current state input (keyboard / mouse)
                HandleInput(gameTime, keyboardState, mouseState);

                // set previous keyboardstate = keyboardstate;
                _previousKeyboardState = keyboardState;
                _previousMouseState = Mouse.GetState();

                // update gamestate data
                var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _remainingDelay -= timer;
                if (_remainingDelay <= 0)
                {
                    Task.Run(() => UpdateGameState(gameTime));
                    _remainingDelay = _timeCycleDelay;
                }
            }
            else
            {
                // game state is still loading
            }
        }

        public async void UpdateGameState(GameTime gameTime)
        {
            // cycle finished
            Console.WriteLine($"Advancing cycle to day: {GSData.Day}");

            ProcessBuildings();

            GSData.Day += 1;
            var day = GSData.Day;

            if ((day % 10).Equals(0)) SaveGame();
        }

        public void ProcessBuildings()
        {
            // reset current ammount inv values
            GSData.PlayerInventory.Energy = 30;
            GSData.PlayerInventory.Workers = 20;
            GSData.PlayerInventory.Food = 20;

            foreach (var t in _currentMap.Tiles)
            {
                if (t.Object.ObjectId > 0)
                {
                    if (t.Object.TypeId.Equals(1))
                    {
                        // resouce tile
                    } else if (t.Object.TypeId.Equals(2))
                    {
                        // building tile
                        //var b = BuildingData.Dict_BuildingFromObjectID[t.Object.ObjectId];
                        GSData.PlayerInventory.Gold += t.Object.GoldCost + t.Object.GoldOutput;
                        GSData.PlayerInventory.Wood += t.Object.WoodCost + t.Object.WoodOutput;
                        GSData.PlayerInventory.Coal += t.Object.CoalCost + t.Object.CoalOutput;
                        GSData.PlayerInventory.Iron += t.Object.IronCost + t.Object.IronOutput;
                        GSData.PlayerInventory.Stone += t.Object.StoneCost + t.Object.StoneOutput;
                        GSData.PlayerInventory.Workers += t.Object.WorkersCost + t.Object.WorkersOutput;
                        GSData.PlayerInventory.Energy += t.Object.EnergyCost + t.Object.EnergyOutput;
                        GSData.PlayerInventory.Food += t.Object.FoodCost + t.Object.FoodCost;
                    }
                }
            }
        }

        public async void ExitGame()
        {
            // on escape, go back to menu state
            _remainingLoad = 100;
            SaveGame();
            Console.WriteLine("Exiting game...");
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        // save map data
        public async void SaveGame()
        {
            if (IsSaving.Equals(true)) return;

            // create list to hold tile data
            GSData.TileData = new List<TileData>();

            // for each tile in current map,
            foreach (Tile t in _currentMap.Tiles)
            {
                // add its tile data to list
                GSData.TileData.Add(t.GetTileData());
            }

            try
            {
                // delete previous backups
                System.IO.File.Delete("GAMEDATA_BACKUP.json");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // backup old map data first
            try
            {
                // change filename to backup format
                System.IO.File.Move($"GAMEDATA.json", "GAMEDATA_BACKUP.json");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error backing up previous map data: " + e.Message);
            }

            IsSaving = true;
            // get current data_map file
            using (var streamWriter = new System.IO.StreamWriter($"GAMEDATA.json"))
            {
                // overwrite data with list of _tileData
                streamWriter.WriteLine(JsonConvert.SerializeObject(GSData, Formatting.Indented));
                streamWriter.Close();
            }

            IsSaving = false;
            Console.WriteLine("Finished Saving Map.");
        }

        // post update (called after update)
        public override void PostUpdate(GameTime gameTime)
        {
            CurrentlyHoveredTile = null;

            if (IsLoaded)
            {
                // game state is loaded

                // get keyboard state
                var keyboardState = Keyboard.GetState();

                // update map and camera
                try
                {
                    _currentMap.Update(gameTime, keyboardState, _camera, this);
                } catch (Exception e)
                {
                    //Console.WriteLine("Error drawing map: " + e.Message);
                }

                if (!(CurrentlyHoveredTile is null))
                {
                    if (SelectedObject.ObjectId > 0)
                    {
                        var sel_obj = SelectedObject;
                        if (sel_obj.TypeId.Equals(2))
                        {
                            var bldg = (Building)SelectedObject;
                            var rng = bldg.Range;


                            for (int x = ((int)CurrentlyHoveredTile.TileIndex.X - rng); x < (CurrentlyHoveredTile.TileIndex.X + (rng + 1)); x++)
                            {
                                for (int y = ((int)CurrentlyHoveredTile.TileIndex.Y - rng); y < (CurrentlyHoveredTile.TileIndex.Y + (rng + 1)); y++)
                                {
                                    try
                                    {
                                        _currentMap.Tiles[x, y].IsGlowing = true;
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine($"Couldn't activate tile: {new Vector2(x, y)} | {exc.Message}");
                                    }
                                }
                            }
                        }
                    }
                }

                // also update ui components
                foreach (var c in Components)
                {
                    c.Update(gameTime, this);
                }

                _camera.Update(gameTime);
            }
            else
            {
                // game state is still loading
            }
        }

        private void Tile_OnClick(object sender, EventArgs e)
        {
            // get selected object and clear it
            var sel_obj = SelectedObject;
            SelectedObject = new TileObject();

            var prev_inv = new Inventory()
            {
                Gold = GSData.PlayerInventory.Gold,
                Wood = GSData.PlayerInventory.Wood,
                Coal = GSData.PlayerInventory.Coal,
                Iron = GSData.PlayerInventory.Iron,
                Workers = GSData.PlayerInventory.Workers,
                Energy = GSData.PlayerInventory.Energy,
                Food = GSData.PlayerInventory.Food
            };

            try
            {
                Tile t = (Tile)sender;
                Console.WriteLine($"Tile clicked: {t.TileIndex}");
                if (t.IsVisible.Equals(true))
                {
                    if (sel_obj.ObjectId > 0 && sel_obj.TypeId.Equals(2) && t.Object.ObjectId <= 0)
                    {
                        var obj = (Building)sel_obj;

                        if (BuildingData.Dict_BuildingResourceLinkKeys.ContainsKey(obj.ObjectId))
                        {
                            for (int x = ((int)t.TileIndex.X - obj.Range);
                                x < (t.TileIndex.X + (obj.Range + 1));
                                x++)
                            {
                                for (int y = ((int)t.TileIndex.Y - obj.Range);
                                    y < (t.TileIndex.Y + (obj.Range + 1));
                                    y++)
                                {
                                    if (_currentMap.Tiles[x, y].Object.ObjectId.Equals(obj.ObjectId) && _currentMap.Tiles[x, y].Object.TypeId.Equals(2))
                                        throw new Exception(
                                            "There is already a building of that type collecting resources in this area.");
                                }
                            }
                        }

                        // check balance to see if player can afford building
                        bool canBuild = true;
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("gold", obj.GoldUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("wood", obj.WoodUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("coal", obj.CoalUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("iron", obj.IronUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("stone", obj.StoneUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("workers", obj.WorkersUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("energy", obj.EnergyUpfront);
                        if (canBuild.Equals(true)) canBuild = GSData.PlayerInventory.RequestResource("food", obj.FoodUpfront);
                        if (canBuild.Equals(false)) throw new Exception("Can't afford to place!");

                        _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].Object = obj;
                        _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].ObjectTexture =
                            _gameContent.GetTileTexture(obj.TextureIndex);
                        _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].TileData.Object = obj;

                        for (int x = ((int)t.TileIndex.X - obj.Range); x < (t.TileIndex.X + (obj.Range + 1)); x++)
                        {
                            for (int y = ((int)t.TileIndex.Y - obj.Range); y < (t.TileIndex.Y + (obj.Range + 1)); y++)
                            {
                                if (obj.ObjectId.Equals(5))
                                {
                                    try
                                    {
                                        _currentMap.Tiles[x, y].IsVisible = true;
                                        _currentMap.Tiles[x, y].TileData.IsVisible = true;
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine($"Couldn't activate tile: {new Vector2(x, y)} | {exc.Message}");
                                    }
                                }

                                if (BuildingData.Dict_BuildingResourceLinkKeys.ContainsKey(obj.ObjectId))
                                {
                                    foreach (var k in BuildingData.Dict_BuildingResourceLinkKeys[obj.ObjectId])
                                    {
                                        if (_currentMap.Tiles[x, y].Object.ObjectId.Equals(k) && _currentMap.Tiles[x, y].Object.TypeId.Equals(1))
                                        {
                                            Console.WriteLine("Adding " + BuildingData.Dic_ResourceCollectionKeys[k][0]);
                                            switch (BuildingData.Dic_ResourceCollectionKeys[k][0])
                                            {
                                                case "Wood":
                                                    _currentMap.Tiles[(int) t.TileIndex.X, (int) t.TileIndex.Y].Object.WoodOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].TileData.Object.WoodOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    break;
                                                case "Food":
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].Object.FoodOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].TileData.Object.FoodOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    break;
                                                case "Stone":
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].Object.StoneOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].TileData.Object.StoneOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    break;
                                                case "Coal":
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].Object.CoalOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].TileData.Object.CoalOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    break;
                                                case "Iron":
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].Object.IronOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    _currentMap.Tiles[(int)t.TileIndex.X, (int)t.TileIndex.Y].TileData.Object.IronOutput += (int)BuildingData.Dic_ResourceCollectionKeys[k][1];
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (obj.ObjectId.Equals(2))
                        {
                            // is farm, apply crops around farm
                            for (int x = ((int)t.TileIndex.X - 1); x < (t.TileIndex.X + 2); x++)
                            {
                                for (int y = ((int)t.TileIndex.Y - 1); y < (t.TileIndex.Y + 2); y++)
                                {
                                    if (new Vector2(x, y).Equals(t.TileIndex))
                                    {
                                        continue;
                                    }

                                    if (_currentMap.Tiles[x, y].Object.ObjectId > 0)
                                    {
                                        continue;
                                    }

                                    if (_currentMap.Tiles[x, y].IsVisible)
                                    {
                                        try
                                        {
                                            var farmobj = Resource.Farmland();
                                            _currentMap.Tiles[x, y].Object = farmobj;
                                            _currentMap.Tiles[x, y].ObjectTexture =
                                                _gameContent.GetTileTexture(farmobj.TextureIndex);
                                            _currentMap.Tiles[x, y].TileData.Object = farmobj;
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(
                                                $"Couldn't activate tile: {new Vector2(x, y)} | {exc.Message}");
                                        }
                                    }
                                }
                            }
                        }

                        GSData.TileData = new List<TileData>();

                        // for each tile in current map,
                        foreach (Tile cmt in _currentMap.Tiles)
                        {
                            // add its tile data to list
                            GSData.TileData.Add(cmt.GetTileData());
                        }
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"Tile {new Vector2(t.TileIndex.X, t.TileIndex.Y)} is outside of the active area.");
                }
            }
            catch (Exception exception)
            {
                SelectedObject = sel_obj;
                GSData.PlayerInventory = prev_inv;
            }
        }
        #endregion

        #region HANDLE INPUTS
        public void HandleInput(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            // if first render
            if (_firstTake.Equals(true))
            {
                _firstTake = false;
            }

            // if shift is held down, set shift multiplier - else, set to 1
            float shift = (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                ? 3.5f
                : 1f;

            // if esc is held down, go back to main menu
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Console.WriteLine("ESCAPE clicked...");
                //Task.Run(() => ExitGame());
            }

            if (mouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed)
            {
                SelectedObject = new TileObject();
            }

            // if WASD, move camera accordingly and mutiply by shift multiplier

            if (keyboardState.IsKeyDown(Keys.A))
            {
                _camera.Position += new Vector2(-1, 0) * shift;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                _camera.Position += new Vector2(1, 0) * shift;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                _camera.Position += new Vector2(0, -1) * shift;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                _camera.Position += new Vector2(0, 1) * shift;
            }

            // if mouse scrollwheel value is different than last frames scroll wheel value,
            // zoom camera accordingly
            // REMOVED TEMPORARILY
            // TODO: PROPERLY CONVERT MOUSE SCREEN POS TO WORLD POS WHEN CAMERA IS ZOOMED, THEN RE-ADD SCROLLING
            /*
            if (mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
            {
                // check for camera zoom > 0 ?
                _camera.Zoom -= 0.2f;
            }
            else if (mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
            {
                _camera.Zoom += 0.2f;
            }
            */
        }
        #endregion

        #region DRAW
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var msp = Mouse.GetState().Position;
            var mp = new Vector2(msp.X, msp.Y);

            if (IsLoaded)
            {
                // game state is loaded

                // TWO SPRITE BATCHES:
                // First batch is for the game itself, the map, npcs, all that live shit
                // Second batch is for UI and HUD rendering - separate from camera matrixes and all that ingame shit

                spriteBatch.Begin(_camera, samplerState: SamplerState.PointClamp);

                // draw game here
                try
                {
                    _currentMap.Draw(gameTime, spriteBatch);
                } catch (Exception e)
                {
                    //Console.WriteLine("Error drawing map:" + e.Message);
                }

                spriteBatch.End();

                //---------------------------------------------------------

                spriteBatch.Begin();
                // draw UI / HUD here 
                foreach (var c in Components)
                {
                    c.Draw(gameTime, spriteBatch);
                }

                if (SelectedObject.ObjectId > 0)
                {

                    var txt = _gameContent.GetTileTexture(SelectedObject.TextureIndex);
                    var pos = mp - new Vector2((txt.Width * 2) / 2, (txt.Height * 2) - ((txt.Height * 2) * 0.25f));
                    var rct = new Rectangle((int)pos.X, (int)pos.Y, txt.Width * 2, txt.Height * 2);
                    spriteBatch.Draw(txt, destinationRectangle: rct, color: Color.White);

                    if (!(CurrentlyHoveredTile is null))
                    {
                        var t = CurrentlyHoveredTile;
                        var obj = (Building)SelectedObject;

                        if (BuildingData.Dict_BuildingResourceLinkKeys.ContainsKey(SelectedObject.ObjectId))
                        {
                            var count = 0;
                            var resource_ids = BuildingData.Dict_BuildingResourceLinkKeys[SelectedObject.ObjectId];
                            var counts = new Dictionary<int, int>();

                            foreach (var r in resource_ids)
                            {
                                counts.Add(r, 0);
                            }

                            try
                            {
                                for (int x = ((int) t.TileIndex.X - obj.Range);
                                    x < (t.TileIndex.X + (obj.Range + 1));
                                    x++)
                                {
                                    for (int y = ((int) t.TileIndex.Y - obj.Range);
                                        y < (t.TileIndex.Y + (obj.Range + 1));
                                        y++)
                                    {
                                        foreach (var r in resource_ids)
                                        {
                                            if (_currentMap.Tiles[x, y].Object.ObjectId.Equals(obj.ObjectId) && _currentMap.Tiles[x, y].Object.TypeId.Equals(2))
                                                throw new Exception(
                                                    "There is already a building of that type collecting resources in this area.");

                                            if (_currentMap.Tiles[x, y].Object.ObjectId.Equals(r))
                                            {
                                                counts[r] = counts[r] + 1;
                                            }
                                        }
                                    }
                                }

                                int indent = 0;
                                foreach (var r in resource_ids)
                                {
                                    var res_str = $"{BuildingData.Dic_ResourceNameKeys[r]}: {(counts[r] * (int)BuildingData.Dic_ResourceCollectionKeys[r][1])}";
                                    var str_x = ((_gameContent.GetFont(1).MeasureString(res_str).X * 1.3f) / 2);
                                    var str_y = ((_gameContent.GetFont(1).MeasureString(res_str).Y * 1.3f) / 2);

                                    spriteBatch.DrawString(_gameContent.GetFont(1), res_str, mp + new Vector2(0, -50 + (15 * indent)), Color.Black, 0.0f, new Vector2(str_x, str_y), 1.3f, SpriteEffects.None, 1.0f);
                                    indent++;
                                }
                            }
                            catch (Exception e)
                            {
                                var print_str = $"{e.Message}";
                                var str_x = ((_gameContent.GetFont(1).MeasureString(print_str).X) / 2);
                                var str_y = ((_gameContent.GetFont(1).MeasureString(print_str).Y) / 2);

                                spriteBatch.DrawString(_gameContent.GetFont(1), print_str, mp + new Vector2(0, -50), Color.Black, 0.0f, new Vector2(str_x, str_y), 1.0f, SpriteEffects.None, 1.0f);
                            }
                        }
                    }
                }

                // draw cursor over UI elements !!
                spriteBatch.Draw(_cursorTexture, mp, Color.White);

                spriteBatch.End();
            }
            else
            {
                // game state hasnt finished loading
                
                // most of whats drawn in here is strictly UI so only one spritebatch should be needed
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                // draw loading bar and text

                var scale = new Vector2
                {
                    X = _graphicsDevice.Viewport.Width,
                    Y = _graphicsDevice.Viewport.Height
                };

                var dimensions = new Vector2
                {
                    X = scale.X / 2,
                    Y = scale.Y / 2
                };

                var x = (_gameContent.GetFont(1).MeasureString(LoadingText).X / 2);
                var y = (_gameContent.GetFont(1).MeasureString(LoadingText).Y / 2);

                spriteBatch.DrawString(_gameContent.GetFont(1), LoadingText, dimensions, Color.White, 0.0f, new Vector2(x,y), 1.0f, SpriteEffects.None, 1.0f);

                spriteBatch.Draw(LoadingTexture, destinationRectangle: LoadingBar, color: Color.White);

                var cells = LoadProgress / 10;

                for (int i = 0; i < cells + 1; i++)
                {
                    var cellRectangle = new Rectangle(LoadingBar.X + (i * (LoadingBar.Width / 10)), LoadingBar.Y, LoadingBar.Width / 10, LoadingBar.Height);
                    spriteBatch.Draw(LoadingCellTexture, destinationRectangle: cellRectangle, color: Color.White);
                }

                spriteBatch.Draw(_cursorTexture, mp, Color.White);

                // draw loading screen here?

                spriteBatch.End();
            }
        }
        #endregion

        #endregion
    }
}                                                                                 