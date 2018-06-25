using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitySim;
using CitySim.Content;
using CitySim.Objects;
using CitySim.States;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Newtonsoft.Json;

namespace CitySim.States
{
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

        #region INVENTORY

        public Inventory PlayerInventory { get; set; }

        #endregion

        #endregion

        #region METHODS

        #region CONSTRUCTOR
        // construct state
        public GameState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            // create new gamecontent instance
            _gameContent = new GameContent(content);

            // save graphics device
            _graphicsDevice = graphicsDevice;

            LoadLoadingScreen();

            // mapLoaded = false, until a map is succesfully loaded in LoadMap()
            bool mapLoaded = false;
            Console.WriteLine($"Loading game...");

            if (_remainingLoad < 100) _remainingLoad = 100;
            Task.Run(() => LoadGame());

            // load (mouse) cursor content
            _cursorTexture = _gameContent.GetUiTexture(4);

            // create camera instance and set its position to mid map
            _camera = new Camera(graphicsDevice);
        }

        // constructor for new game
        public GameState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content, bool newgame) : base(game, graphicsDevice, content)
        {
            // create new gamecontent instance
            _gameContent = new GameContent(content);

            // save graphics device
            _graphicsDevice = graphicsDevice;

            LoadLoadingScreen();

            // mapLoaded = false, until a map is succesfully loaded in LoadMap()
            bool mapLoaded = false;
            Console.WriteLine($"Starting new game...");

            if (_remainingLoad < 100) _remainingLoad = 100;
            Task.Run(() => GenerateMap());

            // load (mouse) cursor content
            _cursorTexture = _gameContent.GetUiTexture(4);

            // create camera instance and set its position to mid map
            _camera = new Camera(graphicsDevice);
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
        }
        #endregion

        #region HANDLE MAP DATA

        public async void LoadGame()
        {
            await LoadMap();

            LoadingText = $"Wrapping things up...";

            _camera.Position = _currentMap.Tiles[25, 25].Position;

            // initialize player's inventory (currently, these are the default values being passed so fucking deal with it)
            PlayerInventory = new Inventory()
            {
                Gold = 500,
                Wood = 100,
                Coal = 50,
                Iron = 20,
                Food = 50,
                Workers = 10,
                Energy = 10
            };

            _remainingLoad -= 100;
        }

        public async Task<bool> LoadMap()
        {
            // array to hold tiles
            Tile[,] tileArr_ = new Tile[50, 50];
            string data = null;

            try
            {
                // try to read map data from data_map.json file
                using (var streamReader = new System.IO.StreamReader($"data_map.json"))
                {
                    data = streamReader.ReadToEnd();
                }
                // if the data read isn't null or empty, load the map | else, throw exception
                if (string.IsNullOrEmpty(data).Equals(true))
                {
                    throw new NotSupportedException("Error Reading Map Data: Data is empty.");
                }
                else
                {
                    Console.WriteLine($"Loading map...");

                    // deserialize (TileData) json to List of TileData
                    List<TileData> tdList_ = JsonConvert.DeserializeObject<List<TileData>>(data);

                    // for each TileData loaded
                    foreach (TileData t in tdList_)
                    {
                        // get x and y index
                        var x = (int)t.TileIndex.X;
                        var y = (int)t.TileIndex.Y;

                        // create new tile and pass gamecontent instance and tiledata
                        tileArr_[x, y] = new Tile(_gameContent, _graphicsDevice, t);
                    }
                }

                // create new map instance from loaded data
                _currentMap = new Map(tileArr_, 50, 50, 34, 100, _gameContent);
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Loading Map Data: {e.Message}.");
                return false;
            }
        }

        // save map data
        public void SaveMap()
        {
            // create list to hold tile data
            List<TileData> newData = new List<TileData>();

            // for each tile in current map,
            foreach(Tile t in _currentMap.Tiles)
            {
                // add its tile data to list
                newData.Add(t.TileData);
            }

            // backup old map data first
            try
            {
                // change filename to backup format
                System.IO.File.Move($"data_map.json", "data_map_backup.json");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error backing up previous map data: " + e.Message);
            }

            // get current data_map file
            using (var streamWriter = new System.IO.StreamWriter($"data_map.json"))
            {
                // overwrite data with list of tiledata
                streamWriter.WriteLine(JsonConvert.SerializeObject(newData, Formatting.Indented));
            }
            Console.WriteLine("Finished Saving Map.");
        }

        // generate map
        public async void GenerateMap()
        {
            LoadingText = $"Generating map...";

            LoadingText = $"...";

            Console.WriteLine(LoadingText);
            // create list to hold tile data
            List<TileData> tileData = new List<TileData>();

            // loop through and generate each tile in the map (default them to grass/empty) (50, 50 is default now)
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    // calculate position based on tile dimensions and row index (Width / 2 = 17, Height = 9 (Middle Offset))
                    // the position is calculated so that the tile will be placed in a fashion that it will render isometrically
                    // this means rendering tiles side by side, but also connecting them by offsetting the x and y with each row
                    // so that the "diamond" shape of each tile fits together snug
                    var position = new Vector2(x * 17 - y * 17, x * 9 + y * 9);

                    // set tile and (inner) object data
                    var td = new TileData
                    {
                        TileIndex = new Vector2(x, y),
                        Position = position,
                        Object = new TileObject()
                    };

                    // add tiledata to list
                    tileData.Add(td);
                }
            }

            _remainingLoad -= 10;

            LoadingText = $"Filling lakes with water...";

            // loop through tiles and generate unique map
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    // this function will run a chance roll on the tile for spawning resources / terrain
                    // and will also do the same for adjacent tiles
                    RunTileAndAdjacentsForWater(tileData, x, y);
                }
            }
            _remainingLoad -= 20;

            LoadingText = $"Growing some trees...";
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    RunTileAndAdjacentsForTrees(tileData, x, y);
                }
            }
            _remainingLoad -= 20;

            LoadingText = $"Creating ores...";
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    RunTileAndAdjacentsForOre(tileData, x, y, 5, 4);
                }
            }
            _remainingLoad -= 20;
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    RunTileAndAdjacentsForOre(tileData, x, y, 6, 5);
                }
            }
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    RunTileAndAdjacentsForOre(tileData, x, y, 7, 6);
                }
            }
            _remainingLoad -= 20;

            LoadingText = $"Cleaning up map...";
            // run through tiles and check if 50% of surrounding tiles are water, if so - fill in the middle
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    var index = new Vector2(x,y);
                    var tile = from t in tileData where t.TileIndex == index select t;
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

                        var adj_tiles = from tiles in tileData where tiles.TileIndex == dir select tiles;
                        if (adj_tiles.Any())
                        {
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

            // get data_map.json file
            using (var streamWriter = new System.IO.StreamWriter($"data_map.json"))
            {
                // write tiledata to file (overwrite any)
                streamWriter.WriteLine(JsonConvert.SerializeObject(tileData, Formatting.Indented));
            }
            Console.WriteLine("Map finished generating.");

            await LoadMap();

            LoadingText = $"Wrapping things up...";

            _camera.Position = _currentMap.Tiles[25, 25].Position;

            // initialize player's inventory (currently, these are the default values being passed so fucking deal with it)
            PlayerInventory = new Inventory()
            {
                Gold = 500,
                Wood = 100,
                Coal = 50,
                Iron = 20,
                Food = 50,
                Workers = 10,
                Energy = 10
            };

            _remainingLoad -= 10;
        }

        public void RunTileAndAdjacentsForWater(List<TileData> tileData, int x, int y)
        {
            var index = new Vector2(x, y);
            var td = from a in tileData where a.TileIndex == index select a;
            if (!td.Any()) return; // theres no tile so just skip
            foreach (var t in td)
            {
                // if the tile is already water - dont even bother
                if (t.TerrainId.Equals(2))
                {
                    return;
                }

                // if the tile is already a resource/building - dont even bother
                if (t.Object.TypeId.Equals(1) || t.Object.TypeId.Equals(2))
                {
                    return;
                }

                int i = _rndGen.Next(0, 1000000);

                int adj_water_tiles = 0;

                // check to see if any adjacent tiles are already water
                for (int o = 0; o < 4; o++)
                {
                    Vector2 dir = new Vector2(0,0);
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

                    var adj_tiles = from tiles in tileData where tiles.TileIndex == dir select tiles;
                    if (adj_tiles.Any())
                    {
                        foreach (var adj_tile in adj_tiles)
                        {
                            if (adj_tile.TerrainId.Equals(2))
                                adj_water_tiles++;
                        }
                    }
                }
                // if random chance
                if (i > (adj_water_tiles > 0 ? (adj_water_tiles < 2 ? 700000 : (adj_water_tiles < 3 ? 800000 : 900000)) : 990000))
                {
                    // generate water tile
                    t.TerrainId = 2;
                    t.Object = new TileObject()
                    {
                        Id = Convert.ToInt32($"{index.X}{index.Y}"),
                        TypeId = 1,
                        ObjectId = 3,
                        TextureIndex = 4
                    };
                    // for each adjacent direction
                    for (var loop_dir = 0; loop_dir < 4; loop_dir++)
                    {
                        Vector2 ref_tile = new Vector2(0, 0);
                        var adj_chance = 0.0f;
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
                        RunTileAndAdjacentsForWater(tileData, (int)ref_tile.X, (int)ref_tile.Y);
                    }
                }
            }
        }

        public void RunTileAndAdjacentsForTrees(List<TileData> tileData, int x, int y)
        {
            var index = new Vector2(x, y);
            var td = from a in tileData where a.TileIndex == index select a;
            if (!td.Any()) return; // theres no tile so just skip
            foreach (var t in td)
            {
                // if the tile is already water - dont even bother
                if (t.TerrainId.Equals(2))
                {
                    return;
                }

                // if the tile is already a resource - dont even bother
                if (t.Object.TypeId.Equals(1) || t.Object.TypeId.Equals(2))
                {
                    return;
                }

                int i = _rndGen.Next(0, 1000000);

                int adj_tree_tiles = 0;

                // check to see if any adjacent tiles are already water
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

                    var adj_tiles = from tiles in tileData where tiles.TileIndex == dir select tiles;
                    if (adj_tiles.Any())
                    {
                        adj_tree_tiles += adj_tiles.Count(adj_tile => adj_tile.Object.TypeId.Equals(1) && (adj_tile.Object.ObjectId.Equals(1) || adj_tile.Object.ObjectId.Equals(2)));
                    }
                }

                // if random chance
                if (i > (adj_tree_tiles > 0 ? (adj_tree_tiles < 2 ? 700000 : (adj_tree_tiles < 3 ? 800000 : 900000)) : 990000))
                {
                    // generate tree tile
                    t.TerrainId = 0;
                    t.Object = new TileObject()
                    {
                        Id = Convert.ToInt32($"{index.X}{index.Y}"),
                        TypeId = 1,
                        ObjectId = 2,
                        TextureIndex = 9
                    };
                    // for each adjacent direction
                    for (var loop_dir = 0; loop_dir < 4; loop_dir++)
                    {
                        Vector2 ref_tile = new Vector2(0, 0);
                        var adj_chance = 0.0f;
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
                        RunTileAndAdjacentsForTrees(tileData, (int)ref_tile.X, (int)ref_tile.Y);
                    }
                }
            }
        }

        public void RunTileAndAdjacentsForOre(List<TileData> tileData, int x, int y, int textureid, int objectid)
        {
            var index = new Vector2(x, y);
            var td = from a in tileData where a.TileIndex == index select a;
            if (!td.Any()) return; // theres no tile so just skip
            foreach (var t in td)
            {
                // if the tile is already water - dont even bother
                if (t.TerrainId.Equals(2))
                {
                    return;
                }

                // if the tile is already a resource - dont even bother
                if (t.Object.TypeId.Equals(1) || t.Object.TypeId.Equals(2))
                {
                    return;
                }

                int i = _rndGen.Next(0, 1000000);

                int adj_stone_tiles = 0;

                // check to see if any adjacent tiles are already water
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

                    var adj_tiles = from tiles in tileData where tiles.TileIndex == dir select tiles;
                    if (adj_tiles.Any())
                    {
                        adj_stone_tiles += adj_tiles.Count(adj_tile => adj_tile.Object.TypeId.Equals(1) && adj_tile.Object.ObjectId.Equals(objectid));
                    }
                }

                // if random chance
                if (i > (adj_stone_tiles > 0 ? (adj_stone_tiles < 2 ? 900000 : (adj_stone_tiles < 3 ? 950000 : 975000)) : 995000))
                {
                    // generate tree tile
                    t.TerrainId = 0; // add random chance for dirt or grass?
                    t.Object = new TileObject()
                    {
                        Id = Convert.ToInt32($"{index.X}{index.Y}"),
                        TypeId = 1,
                        ObjectId = objectid,
                        TextureIndex = textureid
                    };
                    // for each adjacent direction
                    for (var loop_dir = 0; loop_dir < 4; loop_dir++)
                    {
                        Vector2 ref_tile = new Vector2(0, 0);
                        var adj_chance = 0.0f;
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
                        RunTileAndAdjacentsForOre(tileData, (int)ref_tile.X, (int)ref_tile.Y, textureid, objectid);
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
            }
            else
            {
                // game state is still loading
            }
        }

        // post update (called after update)
        public override void PostUpdate(GameTime gameTime)
        {
            if (IsLoaded)
            {
                // game state is loaded

                // get keyboard state
                var keyboardState = Keyboard.GetState();

                // update map and camera
                _currentMap.Update(gameTime, keyboardState, _camera);
                _camera.Update(gameTime);
            }
            else
            {
                // game state is still loading
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
                // on escape, go back to menu state
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
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

                _currentMap.Draw(gameTime, spriteBatch);

                spriteBatch.End();

                //---------------------------------------------------------

                spriteBatch.Begin();
                // draw UI / HUD here 
                spriteBatch.Draw(_cursorTexture, mp, Color.White);

                spriteBatch.End();
            }
            else
            {
                // game state hasnt finished loading
                
                // most of whats drawn in here is strictly UI so only one spritebatch should be needed
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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

                spriteBatch.DrawString(_gameContent.GetFont(1), LoadingText, dimensions, Color.Black, 0.0f, new Vector2(x,y), 1.0f, SpriteEffects.None, 1.0f);

                Console.WriteLine(LoadProgress);

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
