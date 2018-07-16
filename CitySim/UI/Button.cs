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
    public class Button : Component
    {
        // current and prev mouse used to determine where user is clicking
        private MouseState _currentMouse;

        // font for the button
        private SpriteFont _font;

        // for when mouse is over the button
        private bool _isHovering;

        private MouseState _previousMouse;

        private Texture2D _texture;

        // used to determine when clicked
        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColor { get; set; }

        public Color HoverColor { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public bool IsFlipped { get; set; } = false;

        // used for collision
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public string Text { get; set; }

        public Button(Texture2D texture, SpriteFont font)
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

            if (IsFlipped.Equals(true))
            {
                spriteBatch.Draw(_texture, destinationRectangle: Rectangle, color: color, effects: SpriteEffects.FlipHorizontally);
            } else
            {
                spriteBatch.Draw(_texture, Rectangle, color);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (_font.MeasureString(Text).X / 2);
                var y = (_font.MeasureString(Text).Y / 2);

                Vector2 origin = new Vector2(x,y);

                spriteBatch.DrawString(_font, Text, new Vector2(Rectangle.X + (Rectangle.Width - Rectangle.Width / 2), Rectangle.Y + (Rectangle.Height - Rectangle.Height / 2)), PenColor, 0, origin, Scale, SpriteEffects.None, 1);
            }
        }

        public override void Update(GameTime gameTime, GameState state)
        {
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
