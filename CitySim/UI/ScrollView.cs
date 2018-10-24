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
    class ScrollView : Component
    {
        // current and prev mouse used to determine where user is clicking
        private MouseState _currentMouse;
        private MouseState _previousMouse;

        // respective state that this button belongs to (null in states that arent gamestates)
        private GameState _state;

        // font for the button
        private SpriteFont _font;

        // for when mouse is over the button
        private bool _isHovering;

        public bool IsHovering => _isHovering;

        private Texture2D _texture;

        // used to determine when clicked
        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public int ID { get; set; } = 0;

        // used for collision
        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

        public ScrollView(Texture2D texture)
        {
            _texture = texture;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = _isHovering.Equals(true) ? Color.Green : Color.White;

            // draw component here
            spriteBatch.Draw(_texture, Rectangle, color);
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update component here
            _state = state;
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            var mr = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = mr.Intersects(Rectangle);
        }
    }
}
