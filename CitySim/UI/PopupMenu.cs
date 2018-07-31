using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.UI
{
    public class PopupMenu : Component
    {
        private MouseState _currentMouse;
        private MouseState _previousMouse;

        public bool IsActive { get; set; } = false;

        private bool _isHovering;

        private Texture2D _texture;
        private SpriteFont _font;

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColor { get; set; }

        public Color HoverColor { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public PopupMenu(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            PenColor = Color.Black;
            HoverColor = Color.DarkGray;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (_isHovering)
            {
                color = HoverColor;
            }

            // draw pop up menu here
            if (IsActive is true)
            {
                spriteBatch.Draw(_texture, Rectangle, color);
            }
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update pop up menu here

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
