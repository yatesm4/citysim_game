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
        private string LINE = "###########################################################################";

        private bool _debug { get; set; } = false;

        private Random _rndGen { get; set; } = new Random();

        private GameContent _gameContent { get; set; }

        private KeyboardState _previousKeyboardState { get; set; }

        private Map _currentMap { get; set; }

        private Camera _camera { get; set; }

        private MouseState _previousMouseState { get; set; }

        private bool _firstTake { get; set; } = true;

        public GameState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _gameContent = new GameContent(content);

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

            _camera = new Camera(graphicsDevice);
            _camera.Position = _currentMap.Tiles[25, 25].Position;
        }

        public bool LoadMap()
        {
            Tile[,] tileArr_ = new Tile[50, 50];
            string data = null;

            try
            {
                using (var streamReader = new System.IO.StreamReader($"data_map.json"))
                {
                    data = streamReader.ReadToEnd();
                }
                // if the data read isn't null or empty, load the map | else, load the next map
                if (string.IsNullOrEmpty(data).Equals(true))
                {
                    throw new NotSupportedException("Error Reading Map Data: Data is empty.");
                }
                else
                {
                    Console.WriteLine($"Loading map...");
                    List<TileData> tdList_ = JsonConvert.DeserializeObject<List<TileData>>(data);

                    foreach (TileData t in tdList_)
                    {
                        var x = (int)t.TileIndex.X;
                        var y = (int)t.TileIndex.Y;

                        tileArr_[x, y] = new Tile(_gameContent, t);
                        tileArr_[x, y].OutlineTexture = _gameContent.GetTileTexture(1);
                        tileArr_[x, y].TileData = t;
                    }
                }

                _currentMap = new Map(tileArr_, 50, 50, 64, 64, _gameContent);
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Loading Map Data: {e.Message}.");
                return false;
            }
        }

        public void SaveMap()
        {
            List<TileData> newData = new List<TileData>();
            foreach(Tile t in _currentMap.Tiles)
            {
                newData.Add(t.TileData);
            }

            // backup old map data first
            try
            {
                System.IO.File.Move($"data_map.json", "data_map_backup.json");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error backing up previous map data: " + e.Message);
            }

            using (var streamWriter = new System.IO.StreamWriter($"data_map.json"))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(newData, Formatting.Indented));
            }
            Console.WriteLine("Finished Saving Map.");
        }

        public void GenerateMap()
        {
            Console.WriteLine($"Generating map...");
            List<TileData> tileData = new List<TileData>();
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    var position = new Vector2(x * 17 - y * 17, x * 9 + y * 9);
                    var td = new TileData
                    {
                        TileIndex = new Vector2(x, y),
                        Position = position,
                        TextureIndex = 3
                    };
                    tileData.Add(td);
                }
            }
            using (var streamWriter = new System.IO.StreamWriter($"data_map.json"))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(tileData, Formatting.Indented));
            }
            Console.WriteLine("Map finished generating.");
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            HandleInput(gameTime, keyboardState);

            _previousKeyboardState = keyboardState;
        }

        public void HandleInput(GameTime gameTime, KeyboardState keyboardState)
        {
            var ms = Mouse.GetState();

            if (_firstTake.Equals(true))
            {
                _firstTake = false;
            }

            float shift = (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                ? 3.5f
                : 1f;

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                // on escape, go back to menu state
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            } else if (keyboardState.IsKeyUp(Keys.LeftControl) && _previousKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (_debug.Equals(true))
                {
                    Console.WriteLine($"Closing Debug Menu...");
                    _debug = false;
                } else
                {
                    Console.WriteLine($"Starting Debug Menu...");
                    _debug = true;
                }
            }

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

            if (ms.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
            {
                // check for camera zoom > 0 ?
                _camera.Zoom -= 0.2f;
            }
            else if (ms.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
            {
                _camera.Zoom += 0.2f;
            }

            _previousKeyboardState = keyboardState;
            _previousMouseState = ms;
        }

        public override void PostUpdate(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            _currentMap.Update(gameTime, keyboardState, _camera);
            _camera.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
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
