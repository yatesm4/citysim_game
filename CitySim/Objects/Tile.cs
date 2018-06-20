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
        // debug texture for drawing hitbox (click area)
        public Texture2D DebugRect { get; set; }

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

        private MouseState _previousMouseState { get; set; }

        // used to determine when clicked
        public event EventHandler Click;

        public Rectangle TouchHitbox
        {
            get { return new Rectangle((int) Position.X + 8, (int) Position.Y + 83, 18, 10); }
        }

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
        public Tile(GameContent content_, GraphicsDevice graphicsDevice_, TileData tileData_)
        {
            Position = tileData_.Position;
            TileIndex = tileData_.TileIndex;
            // get the texture to render from the gamecontent manager using the TextureIndex from the tiledata
            Texture = content_.GetTileTexture(tileData_.TextureIndex);

            // set DebugRect data (optional w debug options)
            DebugRect = new Texture2D(graphicsDevice_, 1, 1);
            DebugRect.SetData(new[] { Color.Red });

            _previousMouseState = Mouse.GetState();
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera)
        {
            // update tile?

            var currentMouse = Mouse.GetState();

            var cb = camera.GetBounds();

            var m_screenPosition = new Vector2(cb.X + (currentMouse.X * ( 0 - camera.Zoom)), cb.Y - (currentMouse.Y * (0 - camera.Zoom)));
            var m_worldPosition = Vector2.Zero;

            camera.ToWorld(ref m_screenPosition, out m_worldPosition);

            var mouseRectangle = new Rectangle((int)m_worldPosition.X, (int)m_worldPosition.Y, 1, 1);

            IsHovered = false;

            if (mouseRectangle.Intersects(TouchHitbox))
            {
                IsHovered = true;
                Console.WriteLine("Mouse is hovering over tile.");

                if (currentMouse.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }

            _previousMouseState = currentMouse;
        }

        public void Draw(GameTime gameTime_, SpriteBatch spriteBatch_)
        {
            spriteBatch_.Draw(Texture, position: Position, scale: Scale, layerDepth: 0.4f);
            // draw outline?
            if(IsHovered)
                spriteBatch_.Draw(DebugRect, destinationRectangle: TouchHitbox, color: new Color(Color.White, 0.25f));
        }
    }
}
