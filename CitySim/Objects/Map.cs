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
        private Tile[,] _tiles;
        public Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        int width, height, tw, th;

        private GameContent _content { get; set; }
        public GameContent Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private SpriteFont font;

        public Map(Tile[,] tiles_, int width_, int height_, int tx_, int ty_, GameContent content_)
        {
            Tiles = tiles_;
            width = width_;
            height = height_;
            tw = tx_;
            th = ty_;
            Content = content_;
            font = Content.GetFont(1);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera)
        {
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
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
                    // update tiles before draw?
                }
            }

            // draw each tile and sort based on depth
            List<Tile> tiles = Tiles.OfType<Tile>().ToList();
            List<Tile> sortedTiles = Tiles.OfType<Tile>().ToList().OrderBy(o => o.Position.Y).ToList();

            foreach (Tile t in sortedTiles)
            {
                t.Draw(gameTime, spriteBatch);
            }
        }
    }
}
