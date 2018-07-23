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
        private Color _displayColor { get; set; } = Color.AntiqueWhite;
        private Color[] _displayColorData { get; set; }

        private Vector2 _position { get; set; }
        private Vector2 _scale { get; set; } = new Vector2(1, 1);

        private List<Component> _components = new List<Component>();

        public Rectangle DisplayRect => new Rectangle((int)_position.X, (int)_position.Y, (int)_displaySize.X, (int)_displaySize.Y);
        public Rectangle BorderRect => new Rectangle((int)_position.X, (int)_position.Y - 5, (int)_displaySize.X, 5);

        /// <summary>
        /// An int array containing the IDs for icons to represent inventory items in the HUD.
        /// In order: Gold, Wood, Coal, Iron, Food, Energy, Workers
        /// </summary>
        public int[] HUDIconIDs { get; set; }

        public HUD(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            int height = (int)(graphicsDevice_.Viewport.Height * 0.1f);
            int width = (int)(graphicsDevice_.Viewport.Width);

            _position = new Vector2(0, graphicsDevice_.Viewport.Height - height);
            _displaySize = new Vector2(width, height);

            Console.WriteLine("HUD created.");
            Console.WriteLine($"HUD Size: {_displaySize}");
            Console.WriteLine($"HUD Pos: {_position}");

            SetColorData(graphicsDevice_);
            _content = content_;
            _font = _content.GetFont(1);

            HUDIconIDs = new int[]
            {
                5,6,7,8,9,10,11
            };

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
            spriteBatch.Draw(_texture, DisplayRect, Color.DarkOliveGreen);
            spriteBatch.Draw(_texture, BorderRect, Color.Black);

            foreach (Component c in _components)
            {
                c.Draw(gameTime, spriteBatch);
            }

            /**
             *      DRAW RESOURCE AND STATE DATA TO HUD
             **/
            if (State is null)
                return;

            string game_day = $"Day: {State.GSData.Day}";
            var game_day_origin = new Vector2(0, _font.MeasureString(game_day).Y / 2);
            var game_day_position = _position + new Vector2(5, 15);
            spriteBatch.DrawString(_font, game_day, game_day_position, Color.Black, 0.0f, game_day_origin, 1.5f, SpriteEffects.None, 1.0f);

            string game_year = $"Year: {State.GSData.Year}";
            var game_year_origin = new Vector2(0, _font.MeasureString(game_year).Y / 2);
            var game_year_position = game_day_position + new Vector2(0, _font.MeasureString(game_day).Y + 5);
            spriteBatch.DrawString(_font, game_year, game_year_position, Color.Black, 0.0f, game_year_origin, 1.5f, SpriteEffects.None, 1.0f);

            string version = "ALPHA VER 0.0";
            var version_origin = new Vector2(0, _font.MeasureString(version).Y / 2);
            var version_position = new Vector2(game_year_position.X, (DisplayRect.Y + DisplayRect.Height) - (_font.MeasureString(version).Y * 0.75f));
            spriteBatch.DrawString(_font, version, version_position, Color.Black, 0.0f, version_origin, 0.75f, SpriteEffects.None, 1.0f);

            var resource_vals = new int[]
            {
                State.GSData.PlayerInventory.Gold,
                State.GSData.PlayerInventory.Wood,
                State.GSData.PlayerInventory.Coal,
                State.GSData.PlayerInventory.Iron,
                State.GSData.PlayerInventory.Food,
                State.GSData.PlayerInventory.Energy,
                State.GSData.PlayerInventory.Workers
            };

            var icon_start_pos = new Vector2(game_day_position.X + (_font.MeasureString(game_day).X * 2.5f) + 15, _position.Y + 45);
            var icon_scale = new Vector2(2, 2);
            // display inventory icons in hud
            for(int i = 0; i < HUDIconIDs.Length; i++)
            {
                var icon_id = HUDIconIDs[i];
                var icon_texture = _content.GetUiTexture(icon_id);
                var icon_position = icon_start_pos + new Vector2(((icon_texture.Width * icon_scale.X) * i) + (i * 15), 0);
                var icon_rect = new Rectangle((int)icon_position.X, (int)icon_position.Y, (int)(icon_texture.Width * icon_scale.X), (int)(icon_texture.Height * icon_scale.X));
                var icon_origin = new Vector2(icon_rect.Width / 2, icon_rect.Height / 2);
                spriteBatch.Draw(icon_texture, destinationRectangle: icon_rect, color: Color.White, scale: icon_scale, origin: icon_origin);

                var resource_text = $"{resource_vals[i]}";
                var resource_text_origin = new Vector2(_font.MeasureString(resource_text).X / 2, _font.MeasureString(resource_text).Y / 2);
                var resource_text_pos = icon_position + new Vector2(-(icon_rect.Width / 2), 15);

                spriteBatch.DrawString(_font, resource_text, resource_text_pos, Color.Black, 0.0f, resource_text_origin, 1.5f, SpriteEffects.None, 1.0f);
            }
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            foreach (Component c in _components)
            {
                c.Update(gameTime, state);
            }

            // save gamestate
            State = state;
        }
    }
}
