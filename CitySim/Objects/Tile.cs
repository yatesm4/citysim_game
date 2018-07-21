using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Content;
using CitySim.States;
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
        public int TerrainId { get; set; } = 0;
        public TileObject Object { get; set; }

        public bool IsVisible { get; set; } = false;
        public bool IsActive { get; set; } = false;
    }

    public class Tile
    {
        public GameContent Content { get; set; }
        public GraphicsDevice GraphicsDevice_ { get; set; }

        // debug texture for drawing hitbox (click area)
        public Texture2D DebugRect { get; set; }

        // this tile's respective tiledata (will match tile properties)
        public TileData TileData { get; set; }

        // object (building or resource) belong to this tile
        public TileObject Object { get; set; }

        // the type of terrain on this tile (0 = Grass, 1 = Dirt, 2 = Water)
        public int TerrainId { get; set; }

        // index of the tile (0-MapWidth,0-MapHeight)
        public Vector2 TileIndex { get; set; }

        // the texture of this tile
        public Texture2D Texture { get; set; }

        public Sprite Spr;
        public SpritePlayer SprPlayer;

        public Texture2D ObjectTexture { get; set; }

        // is the tile interactable, is it hovered, and the hover properties
        public bool IsInteractable { get; set; } = false;
        public bool IsHovered { get; set; } = false;
        public Color DrawColor { get; set; } = Color.White;

        public bool IsVisible { get; set; } = false;
        public bool IsActive { get; set; } = false;

        private MouseState _previousMouseState { get; set; }

        private GameState _gameState { get; set; }

        // used to determine when clicked
        public event EventHandler Click;

        // hitbox for mouse touch
        public Rectangle TouchHitbox
        {
            get { return new Rectangle((int) Position.X + 16, (int) Position.Y + (83 * 2), 18 * 2, 10 * 2); }
        }

        // tile position
        public Vector2 Position { get; set; } = new Vector2(0, 0);

        // center point of the tile (used later for npcs moving tile to tile)
        public Vector2 CenterPoint
        {
            get { return Position + new Vector2(16, 12); }
        }

        // scale to draw the tile at
        public Vector2 Scale { get; set; } = new Vector2(2,2);

        // tile constructor, pass a gamecontent manager and tiledata to load from
        public Tile(GameContent content_, GraphicsDevice graphicsDevice_, TileData tileData_)
        {
            Content = content_;
            GraphicsDevice_ = graphicsDevice_;

            Position = tileData_.Position;
            TileIndex = tileData_.TileIndex;
            TerrainId = tileData_.TerrainId;
            // set object from tiledata if not null, otherwise generate default tileobject
            Object = tileData_.Object ?? new TileObject();
            // get the texture to render from the gamecontent manager using the TextureIndex from the tile's tileobject - if not null, otherwise default to 3 (grass)

            int texture_for_terrain = 0;
            switch (TerrainId)
            {
                case 0:
                    texture_for_terrain = 1;
                    break;
                case 1:
                    texture_for_terrain = 2;
                    break;
                case 2:
                    texture_for_terrain = 4;
                    break;
            }

            Texture = content_.GetTileTexture(texture_for_terrain);

            IsActive = tileData_.IsActive;
            IsVisible = tileData_.IsVisible;

            // set DebugRect data (optional w debug options)
            DebugRect = new Texture2D(graphicsDevice_, 1, 1);
            DebugRect.SetData(new[] { Color.Red });

            // initialize previous mouse state w current mouse state (not really that big of a deal as it will only make one frame behave odd and that frame is over with before user even notices unless their pc is literally a piece of shit)
            _previousMouseState = Mouse.GetState();
        }

        public TileData GetTileData()
        {
            return TileData = new TileData()
            {
                TileIndex = this.TileIndex,
                Position = this.Position,
                TerrainId = this.TerrainId,
                IsActive = this.IsActive,
                IsVisible = this.IsVisible,
                Object = this.Object
            };
        }

        // update
        // - check for mouse hovering and click (select)
        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera camera, GameState state)
        {
            // update tile?

            var currentMouse = Mouse.GetState();

            // convert mouse screen position to world position
            var m_screenPosition = new Vector2(currentMouse.X, currentMouse.Y);
            var m_worldPosition = Vector2.Zero;
            camera.ToWorld(ref m_screenPosition, out m_worldPosition);

            // apply offset (why the fuck this is needed I absolutely do not know but I randomly fucking figured out this formula and somehow it works so for the love of fuck - do not change this until a superior solution is TESTED and delivered
            m_worldPosition.X -= camera.Width / 2f;
            m_worldPosition.Y -= camera.Height / 2f;

            //m_worldPosition.X += (GraphicsDevice_.Viewport.Width * 0.25f);
            //m_worldPosition.Y += (GraphicsDevice_.Viewport.Height * 0.25f);

            // get bounds for mouse world position
            var mouseRectangle = new Rectangle((int)m_worldPosition.X, (int)m_worldPosition.Y, 1, 1);

            IsHovered = false;

            // check if mouse bounds intersects with tile touchbox bounds
            if (mouseRectangle.Intersects(TouchHitbox))
            {
                IsHovered = true;
                state.CurrentlySelectedTile = this;
                //Console.WriteLine($"Hover:: Mp=>{currentMouse.Position.ToString()} :: Mwp=>{m_worldPosition.ToString()} :: Tp=>{Position.ToString()}");
                //Console.WriteLine($"Hovering Over Tile: {TileIndex.ToString()}");

                if (currentMouse.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }

            // save mouse state as previous mousestate for next update call
            _previousMouseState = currentMouse;

            _gameState = state;
        }



        // draw
        // - draw tile
        // - draw outline if selected
        public void Draw(GameTime gameTime_, SpriteBatch spriteBatch_)
        {
            // set draw color to orange red if hovered by mouse, otherwise draw normal color
            DrawColor = IsHovered && (_gameState.CurrentlySelectedTile == this) ? Color.OrangeRed : Color.White;
            // but set drawcolor to greyed out if not visible
            DrawColor = (IsVisible || IsActive) ? DrawColor : Color.DarkGray;

            spriteBatch_.Draw(Texture, position: Position, scale: Scale, layerDepth: 0.4f, color: DrawColor); 

            if (Object.TypeId != 0)
            {
                if (TerrainId.Equals(2))
                {
                    // terrain is water => object is water, so dont render anything over the tile
                }
                else
                {
                    try
                    {
                        spriteBatch_.Draw(Content.GetTileTexture(Object.TextureIndex), position: Position, scale: Scale, layerDepth: 0.4f, color: DrawColor);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error drawing object sprite: " + e.Message);
                    }
                }
            }

            // draw extras ?

            // if tile is hovered, draw debug box on tile
            //if(IsHovered)
            //    spriteBatch_.Draw(DebugRect, destinationRectangle: TouchHitbox, color: new Color(Color.White, 0.25f));

            //spriteBatch_.Draw(DebugRect, destinationRectangle: TouchHitbox, color: new Color(Color.White, 0.25f));
        }
    }
}
