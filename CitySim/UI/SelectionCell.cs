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
    public class SelectionCell : Component
    {
        public HorizontalScrollMenu ParentMenu { get; set; }

        private int _cellID = 0;

        private MouseState _currentMouse;
        private MouseState _previousMouse;

        private bool _isHovering = false;
        public bool Clicked { get; private set; }


        public Texture2D Texture { get; set; }
        public Texture2D ObjectTexture { get; set; }
        public int ObjectID { get; set; } = 1;
        public int ObjectTypeID { get; set; } = 0;
        public Texture2D HoverTexture { get; set; }

        public Color DisplayColor { get; set; } = Color.DarkGray;
        public Color[] DisplayColorData { get; set; }
        public Color HoverColor { get; set; } = Color.LightGray;
        public Color[] HoverColorData { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 ResetPosition { get; set; }
        private Vector2 _displaySize { get; set; }
        public Vector2 Scale { get; set; } = new Vector2(1, 1);

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)_displaySize.X, (int)_displaySize.Y);
            }
        }

        public SelectionCell(GraphicsDevice graphicsDevice_, Vector2 position_, Texture2D object_, int id_)
        {
            ObjectTexture = object_;
            _cellID = id_;
            _displaySize = new Vector2(32,32);
            Position = position_;
            ResetPosition = position_;
            SetColorData(graphicsDevice_);
        }

        public void ResetPos()
        {
            Position = ResetPosition;
        }

        public void SetColorData(GraphicsDevice graphicsDevice_)
        {
            Texture = new Texture2D(graphicsDevice_, (int)_displaySize.X, (int)_displaySize.Y);
            HoverTexture = new Texture2D(graphicsDevice_, (int)_displaySize.X, (int)_displaySize.Y);
            DisplayColorData = new Color[(int)_displaySize.X * (int)_displaySize.Y];
            HoverColorData = new Color[(int)_displaySize.X * (int)_displaySize.Y];
            for (int i = 0; i < DisplayColorData.Length; i++)
            {
                DisplayColorData[i] = DisplayColor;
                HoverColorData[i] = HoverColor;
            }
            Texture.SetData(DisplayColorData);
            HoverTexture.SetData(HoverColorData);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw here
            var txt = Texture;
            if (_isHovering.Equals(true))
            {
                spriteBatch.Draw(HoverTexture, Rectangle, Color.White);
            }
            spriteBatch.Draw(ObjectTexture, Rectangle, Color.White);
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update here
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Console.WriteLine($"Cell Clicked: {_cellID}");
                    ParentMenu.SelectedTexture = ObjectTexture;
                    ParentMenu.SelectedTextureTypeId = ObjectTypeID;
                    ParentMenu.SelectedTextureId = ObjectID;
                }
            }
        }
    }
}
