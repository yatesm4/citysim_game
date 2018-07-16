using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CitySim.Objects;
using CitySim.States;

namespace CitySim.UI
{
    public class GridCell : Component
    {
        private MouseState _previousMouse;
        private MouseState _currentMouse;
        private bool _isHovering;

        public TileData TileData { get; set; }

        private Vector2 _cellSize = new Vector2(8, 8);

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColor { get; set; } = Color.Black;

        public Color CellColor { get; set; } = Color.Green;
        public Color[] CellColorData { get; set; }

        public Color HoverColor { get; set; } = Color.LightBlue;
        public Color[] HoverColorData { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1, 1);

        public Texture2D Texture { get; set; }
        public Texture2D HoverTexture { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)_cellSize.X, (int)_cellSize.Y);
            }
        }

        public GridCell(Color cellColor_, GraphicsDevice graphicsDevice_)
        {
            CellColor = cellColor_;
            SetColorData(graphicsDevice_);
        }

        public void SetColorData(GraphicsDevice graphicsDevice_)
        {
            Texture = new Texture2D(graphicsDevice_, (int)_cellSize.X, (int)_cellSize.Y);
            HoverTexture = new Texture2D(graphicsDevice_, (int)_cellSize.X, (int)_cellSize.Y);
            CellColorData = new Color[(int)_cellSize.X * (int)_cellSize.Y];
            HoverColorData = new Color[(int)_cellSize.X * (int)_cellSize.Y];
            for(int i = 0; i < CellColorData.Length; i++)
            {
                CellColorData[i] = CellColor;
                HoverColorData[i] = HoverColor;
            }
            Texture.SetData(CellColorData);
            HoverTexture.SetData(HoverColorData);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw
            var txt = Texture;

            if (_isHovering.Equals(true))
            {
                txt = HoverTexture;
            }

            spriteBatch.Draw(txt, Rectangle, Color.White);
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
