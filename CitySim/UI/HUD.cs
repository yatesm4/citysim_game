using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Content;
using CitySim.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.UI
{
    public class HUD : Component
    {
        public GameState State { get; set; }
        
        private GameContent _content { get; set; }

        private SpriteFont _font { get; set; }

        private MouseState _previousMouse { get; set; }
        private MouseState _currentMouse { get; set; }

        private Vector2 _displaySize { get; set; }

        private Texture2D _texture { get; set; }
        private Color _displayColor { get; set; } = Color.DarkSlateGray;
        private Color[] _displayColorData { get; set; }

        private Vector2 _position { get; set; }
        private Vector2 _scale { get; set; } = new Vector2(1, 1);

        private List<Component> _components = new List<Component>();

        public Rectangle DisplayRect => new Rectangle((int)_position.X, (int)_position.Y, (int)_displaySize.X, (int)_displaySize.Y);

        public HUD(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            int height = graphicsDevice_.Viewport.Height / 8;
            int width = (int)(graphicsDevice_.Viewport.Width - (graphicsDevice_.Viewport.Width * 0.2f));

            _position = new Vector2(graphicsDevice_.Viewport.Width * 0.1f, graphicsDevice_.Viewport.Width * 0.1f);
            _displaySize = new Vector2(width, height);

            Console.WriteLine("HUD created.");
            Console.WriteLine($"HUD Size: {_displaySize}");
            Console.WriteLine($"HUD Pos: {_position}");

            SetColorData(graphicsDevice_);
            _content = content_;
            _font = _content.GetFont(1);

            //LoadItemsMenu(graphicsDevice_);
        }

        public void SetColorData(GraphicsDevice graphicsDevice_)
        {
            _texture = new Texture2D(graphicsDevice_, (int)_displaySize.X, (int)_displaySize.Y);
            _displayColorData = new Color[(int)_displaySize.X * (int)_displaySize.Y];
            for (int i = 0; i < _displayColorData.Length; i++)
            {
                _displayColorData[i] = _displayColor;
            }
            _texture.SetData(_displayColorData);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DisplayRect, Color.White);
            foreach (Component c in _components)
            {
                c.Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Component c in _components)
            {
                c.Update(gameTime);
            }
        }
    }
}
