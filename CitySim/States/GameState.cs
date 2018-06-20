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
        // is game currently debugging?
        private bool _debug { get; set; } = false;

        // gamecontent manager - holds all sprites, effects, sounds
        private GameContent _gameContent { get; set; }

        private GraphicsDevice _graphicsDevice { get; set; }

        // current map rendering
        private Map _currentMap { get; set; }

        // game camera
        private Camera _camera { get; set; }

        // previous keyboard state (before current)
        private KeyboardState _previousKeyboardState { get; set; }

        // previous mouse state (before current)
        private MouseState _previousMouseState { get; set; }

        // first render?
        private bool _firstTake { get; set; } = true;

        // construct state
        public GameState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            // create new gamecontent instance
            _gameContent = new GameContent(content);

            // save graphics device
            _graphicsDevice = graphicsDevice;

            // mapLoaded = false, until a map is succesfully loaded in LoadMap()
            bool mapLoaded = false;
            Console.WriteLine($"Searching for map files...");

            while (mapLoaded.Equals(false))
            {
                if (LoadMap())
                {
                    mapLoaded = true;
                }
                else
                {
                    Console.WriteLine("No maps found, generating maps...");
                    GenerateMap();
                }
            }

            Console.WriteLine($"Map loaded.");

            // create camera instance and set its position to mid map
            _camera = new Camera(graphicsDevice);
            _camera.Position = _currentMap.Tiles[25, 25].Position;
        }

        public bool LoadMap()
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
                        // set outline texture
                        tileArr_[x, y].OutlineTexture = _gameContent.GetTileTexture(1);
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
        public void GenerateMap()
        {
            Console.WriteLine($"Generating map...");
            // create list to hold tile data
            List<TileData> tileData = new List<TileData>();

            // loop through each tile in the map (50, 50 is default now)
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    // calculate position based on tile dimensions and row index (Width / 2 = 17, Height = 9 (Middle Offset))
                    // the position is calculated so that the tile will be placed in a fashion that it will render isometrically
                    // this means rendering tiles side by side, but also connecting them by offsetting the x and y with each row
                    // so that the "diamond" shape of each tile fits together snug
                    var position = new Vector2(x * 17 - y * 17, x * 9 + y * 9);

                    // set tile data
                    var td = new TileData
                    {
                        TileIndex = new Vector2(x, y),
                        Position = position,
                        TextureIndex = 3
                    };

                    // add tiledata to list
                    tileData.Add(td);
                }
            }
            // get data_map.json file
            using (var streamWriter = new System.IO.StreamWriter($"data_map.json"))
            {
                // write tiledata to file (overwrite any)
                streamWriter.WriteLine(JsonConvert.SerializeObject(tileData, Formatting.Indented));
            }
            Console.WriteLine("Map finished generating.");
        }

        // update
        public override void Update(GameTime gameTime)
        {
            // get current keyboard state
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            // handle current state input (keyboard / mouse)
            HandleInput(gameTime, keyboardState, mouseState);

            // set previous keyboardstate = keyboardstate;
            _previousKeyboardState = keyboardState;
            _previousMouseState = Mouse.GetState();
        }

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

            if (mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
            {
                // check for camera zoom > 0 ?
                _camera.Zoom -= 0.2f;
            }
            else if (mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
            {
                _camera.Zoom += 0.2f;
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // get keyboard state
            var keyboardState = Keyboard.GetState();

            // update map and camera
            _currentMap.Update(gameTime, keyboardState, _camera);
            _camera.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TWO SPRITE BATCHES:
            // First batch is for the game itself, the map, npcs, all that live shit
            // Second batch is for UI and HUD rendering - separate from camera matrixes and all that ingame shit

            spriteBatch.Begin(_camera);

            // draw game here

            _currentMap.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            //---------------------------------------------------------

            spriteBatch.Begin();

            // draw UI / HUD here 

            spriteBatch.End();
        }
    }
}
