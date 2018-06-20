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
    /// <summary>
    /// Save Data for Tiles
    /// </summary>
    public class TileData
    {
        public int TextureIndex { get; set; } = 0;
        public Vector2 TileIndex { get; set; } = new Vector2(0,0);
        public Vector2 Position { get; set; } = new Vector2(0,0);
    }

    public class Tile
    {
        public TileData TileData { get; set; }
        public Vector2 TileIndex { get; set; }

        public Texture2D Texture { get; set; }

        public bool IsInteractable { get; set; } = false;
        public bool IsHovered { get; set; } = false;
        public Texture2D OutlineTexture { get; set; }
        public bool ShowOutline { get; set; } = false;

        public Vector2 Position { get; set; } = new Vector2(0, 0);

        public Vector2 CenterPoint
        {
            get { return Position + new Vector2(16, 12); }
        }

        public Vector2 Scale { get; set; } = new Vector2(1,1);

        public Tile(GameContent content_, TileData tileData_)
        {
            Position = tileData_.Position;
            TileIndex = tileData_.TileIndex;
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
