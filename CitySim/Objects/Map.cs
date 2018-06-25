using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitySim.Content;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.Objects
{
    public class Map
    {
        // array to hold tiles
        private Tile[,] _tiles;
        public Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
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
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera)
        {
            // for each tile in each row
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    // update tile
                    Tiles[x, y].Update(gameTime, keyboardState, camera);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    Tiles[x,y].Draw(gameTime,spriteBatch);
                }
            }

            
        }
    }
}
