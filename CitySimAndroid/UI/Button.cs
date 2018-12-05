using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CitySimAndroid.Content;
using CitySimAndroid.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace CitySimAndroid.UI
{
    public class Button : Component
    {
        // current and prev mouse used to determine where user is clicking
        private MouseState _currentMouse;

        private TouchCollection _currentTouch;
        private TouchCollection _previousTouch;

        private GameState _state;

        // font for the button
        private SpriteFont _font;

        // for when mouse is over the button
        private bool _isHovering;

        public bool IsHovering => _isHovering;

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

        public bool IsSelected { get; set; } = false;

        public int ID { get; set; } = 0;

        // enabling this extra property will lock the button if the resources needed arent available for the object provided by the button
        public bool ResourceLocked = false;
        public bool Locked = false;

        private string _baseString = "+0 +0 +0 +0 +0 +0 +0";

        // used for collision
        public Rectangle Rectangle =>
            CustomRect.IsEmpty
                ? new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (!string.IsNullOrEmpty(Text)
                        ? (((int)(_font.MeasureString(Text).X * Scale.X) + TextPadding_X) > _texture.Width
                            ? (int)(_font.MeasureString(Text).X * Scale.X) + TextPadding_X
                            : _texture.Width)
                        : _texture.Width),
                    (!string.IsNullOrEmpty(Text)
                        ? (((int)(_font.MeasureString(Text).Y * Scale.Y) + TextPadding_Y) > _texture.Height
                            ? (int)(_font.MeasureString(Text).Y * Scale.Y) + TextPadding_Y
                            : _texture.Height)
                        : _texture.Height))
                : CustomRect;

        public Rectangle CustomRect { get; set; } = new Rectangle();

        public string Text { get; set; }

        public int TextPadding_X = 100;
        public int TextPadding_Y = 50;

        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            PenColor = Color.Black;
            HoverColor = Color.DarkGray;
        }

        public Button(Texture2D texture, SpriteFont font, Rectangle customRect)
        {
            _texture = texture;
            CustomRect = customRect;
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

            if (Locked.Equals(true))
            {
                color = Color.DarkGray;
            }

            if (IsSelected.Equals(true))
            {
                color = Color.DarkGray;
            }

            if (IsFlipped.Equals(true))
            {
                spriteBatch.Draw(_texture, destinationRectangle: Rectangle, color: color, effects: SpriteEffects.FlipHorizontally);
            }
            else
            {
                spriteBatch.Draw(_texture, Rectangle, color);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                // get vals for origin
                var x = (_font.MeasureString(Text).X / 2);
                var y = (_font.MeasureString(Text).Y / 2);

                Vector2 origin = new Vector2(x, y);

                spriteBatch.DrawString(_font,
                    Text,
                    new Vector2(Rectangle.X + (Rectangle.Width - Rectangle.Width / 2),
                        Rectangle.Y + (Rectangle.Height - Rectangle.Height / 2)),
                    PenColor,
                    0,
                    origin,
                    Scale, 
                    SpriteEffects.None,
                    1);
            }


        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update state (gamestate)
            _state = state;

            // update mousestate (deprecated for mobile)
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            // update touch state
            _previousTouch = _currentTouch;
            _currentTouch = TouchPanel.GetState();

            if (ResourceLocked.Equals(true))
            {
                if (BuildingData.Dict_BuildingKeys.ContainsKey(ID))
                {
                    var b = BuildingData.Dict_BuildingKeys[ID];
                    bool canBuild = true;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Gold >= b.GoldUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Wood >= b.WoodUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Coal >= b.CoalUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Iron >= b.IronUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Stone >= b.StoneUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Workers >= b.WorkersUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Energy >= b.EnergyUpfront;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.Food >= b.FoodUpfront;
                    Locked = !canBuild;
                }
                else
                {
                    Locked = true;
                }
            }

            _isHovering = false;

            /*
            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (Locked.Equals(false))
                {
                    if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                    {
                        Click?.Invoke(this, new EventArgs());
                    }
                }
            }
            */

            foreach (TouchLocation tl in _currentTouch)
            {
                // get touch position
                var tl_pos = tl.Position;

                // construct rect to represent touch area
                var tl_rect = new Rectangle((int)tl_pos.X, (int)tl_pos.Y, 1, 1);

                if (tl.State != TouchLocationState.Pressed) continue;

                if (tl_rect.Intersects(Rectangle)) Click?.Invoke(this, new EventArgs());
            }
        }
    }
}