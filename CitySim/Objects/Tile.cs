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
        public Vector2 TileIndex { get; set; } = new Vector2(0,0);
        public Vector2 Position { get; set; } = new Vector2(0,0);
        public TileObject Object { get; set; }
    }

    public class Tile
    {
        // debug texture for drawing hitbox (click area)
        public Texture2D DebugRect { get; set; }

        // this tile's respective tiledata (will match tile properties)
        public TileData TileData { get; set; }

        public TileObject Object { get; set; }

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

        // hitbox for mouse touch
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
            // set object from tiledata if not null, otherwise generate default tileobject
            Object = tileData_.Object ?? new TileObject();
            // get the texture to render from the gamecontent manager using the TextureIndex from the tile's tileobject - if not null, otherwise default to 3 (grass)
            Texture = content_.GetTileTexture(Object?.TextureIndex ?? 3);

            // set DebugRect data (optional w debug options)
            DebugRect = new Texture2D(graphicsDevice_, 1, 1);
            DebugRect.SetData(new[] { Color.Red });

            // initialize previous mouse state w current mouse state (not really that big of a deal as it will only make one frame behave odd and that frame is over with before user even notices unless their pc is literally a piece of shit)
            _previousMouseState = Mouse.GetState();
        }

        // update
        // - check for mouse hovering and click (select)
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera)
        {
            // update tile?

            var currentMouse = Mouse.GetState();

            // convert mouse screen position to world position
            var m_screenPosition = new Vector2(currentMouse.X, currentMouse.Y);
            var m_worldPosition = Vector2.Zero;
            camera.ToWorld(ref m_screenPosition, out m_worldPosition);

            // apply offset (why the fuck this is needed I absolutely do not know but I randomly fucking figured out this formula and somehow it works so for the love of fuck - do not change this until a superior solution is TESTED and delivered
            m_worldPosition.X -= camera.Width / 2;
            m_worldPosition.Y -= camera.Height / 2;

            // get bounds for mouse world position
            var mouseRectangle = new Rectangle((int)m_worldPosition.X, (int)m_worldPosition.Y, 1, 1);

            IsHovered = false;

            // check if mouse bounds intersects with tile touchbox bounds
            if (mouseRectangle.Intersects(TouchHitbox))
            {
                IsHovered = true;
                Console.WriteLine($"Hover:: Mp=>{currentMouse.Position.ToString()} :: Mwp=>{m_worldPosition.ToString()} :: Tp=>{Position.ToString()}");

                if (currentMouse.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }

            // save mouse state as previous mousestate for next update call
            _previousMouseState = currentMouse;
        }

        // draw
        // - draw tile
        // - draw outline if selected
        public void Draw(GameTime gameTime_, SpriteBatch spriteBatch_)
        {
            spriteBatch_.Draw(Texture, position: Position, scale: Scale, layerDepth: 0.4f);
            
            // draw extras ?

            // if tile is hovered, draw debug box on tile
            if(IsHovered)
                spriteBatch_.Draw(DebugRect, destinationRectangle: TouchHitbox, color: new Color(Color.White, 0.25f));
        }
    }
}
