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
    // tiledata for tiles
    // this data is used for savedata, and for loading / generating maps mostly
    public class TileData
    {
        public int TextureIndex { get; set; } = 0;
        public Vector2 TileIndex { get; set; } = new Vector2(0,0);
        public Vector2 Position { get; set; } = new Vector2(0,0);
    }

    public class Tile
    {
        // this tile's respective tiledata (will match tile properties)
        public TileData TileData { get; set; }
        // index of the tile (0-MapWidth,0-MapHeight)
        public Vector2 TileIndex { get; set; }

        // the texture of this tile
        public Texture2D Texture { get; set; }

        // is the tile interactable, is it hovered, and the hover properties
        public bool IsInteractable { get; set; } = false;
        public bool IsHovered { get; set; } = false;
        public Texture2D OutlineTexture { get; set; }
        public bool ShowOutline { get; set; } = false;

        // tile position
        public Vector2 Position { get; set; } = new Vector2(0, 0);

        // center point of the tile (used later for npcs moving tile to tile)
        public Vector2 CenterPoint
        {
            get { return Position + new Vector2(16, 12); }
        }

        // scale to draw the tile at
        public Vector2 Scale { get; set; } = new Vector2(1,1);

        // tile constructor, pass a gamecontent manager and tiledata to load from
        public Tile(GameContent content_, TileData tileData_)
        {
            Position = tileData_.Position;
            TileIndex = tileData_.TileIndex;
            // get the texture to render from the gamecontent manager using the TextureIndex from the tiledata
            Texture = content_.GetTileTexture(tileData_.TextureIndex);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera)
        {
            // update tile?
        }

        public void Draw(GameTime gameTime_, SpriteBatch spriteBatch_)
        {
            spriteBatch_.Draw(Texture, position: Position, scale: Scale, layerDepth: 0.4f);
            // draw outline?
        }
    }
}
