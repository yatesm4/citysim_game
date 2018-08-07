using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Content;
using CitySim.Objects;
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

        public int ID { get; set; } = 0;

        // enabling this extra property will lock the button if the resources needed arent available for the object provided by the button
        public bool ResourceLocked = false;
        public bool Locked = false;

        // used for collision
        public Rectangle Rectangle
        {
            get
            {
                return CustomRect.IsEmpty ? new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height) : CustomRect;
            }
        }

        public Rectangle CustomRect { get; set; } = new Rectangle();

        public string Text { get; set; }

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

            if (ResourceLocked.Equals(true))
            {
                var prev_inv = new Inventory()
                {
                    Gold = state.GSData.PlayerInventory.Gold,
                    Wood = state.GSData.PlayerInventory.Wood,
                    Coal = state.GSData.PlayerInventory.Coal,
                    Iron = state.GSData.PlayerInventory.Iron,
                    Workers = state.GSData.PlayerInventory.Workers,
                    Energy = state.GSData.PlayerInventory.Energy,
                    Food = state.GSData.PlayerInventory.Food
                };

                if (BuildingData.Dict_BuildingKeys.ContainsKey(ID))
                {
                    var b = BuildingData.Dict_BuildingKeys[ID];
                    bool canBuild = true;
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("gold", b.GoldCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("wood", b.WoodCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("coal", b.CoalCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("iron", b.IronCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("stone", b.StoneCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("workers", b.WorkersCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("energy", b.EnergyCost);
                    if (canBuild.Equals(true)) canBuild = state.GSData.PlayerInventory.RequestResource("food", b.FoodCost);
                    Locked = !canBuild;
                    Console.WriteLine($"{b.Name} Button Avail: {Locked.ToString()}");
                }
                else
                {
                    Locked = true;
                }
                state.GSData.PlayerInventory = prev_inv;
            }

            _isHovering = false;

            if (Locked.Equals(false))
            {
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
}
