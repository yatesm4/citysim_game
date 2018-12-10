using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CitySimAndroid.Content;
using CitySimAndroid.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CitySimAndroid.UI
{
    public class TopBar : Component
    {
        private GraphicsDevice _graphicsDevice;
        private GameContent _gameContent;

        private SpriteFont _font;

        private GameState _currentGameState;

        private TouchCollection _previousTouch;
        private TouchCollection _currentTouch;

        private Vector2 _displayDimensions { get; set; }
        private Rectangle _displayRectangle =>
            new Rectangle(0, 0, (int) _displayDimensions.X, (int) _displayDimensions.Y);

        private Vector2 _borderDimensions => new Vector2(_displayDimensions.X, 8);
        private Rectangle _borderRectangle =>
            new Rectangle(0, (int)_displayDimensions.Y, (int)_borderDimensions.X, (int)_borderDimensions.Y);

        private Color _displayColor => new Color(41, 46, 53);
        private Texture2D _displayTexture { get; set; }
        private Color _borderColor => Color.Black;
        private Texture2D _borderTexture { get; set; }
        private Texture2D _resourceAreaTexture { get; set; }

        private float _textScale = 1f;
        private Color _textColor { get; set; } = Color.White;

        private float _resourceIconTextureScale { get; set; } = 1f;

        private Rectangle _resourceIconsAreaRectangle =>
            new Rectangle(
                (int) (_displayDimensions.X / 4), // start 1/4 of width into top bar
                0, // start at 0 height
                (int) (_displayDimensions.X / 2), // as wide as half of top bar
                (int) _displayDimensions.Y); // as tall as top bar

        private Vector2 _resourceIconDisplayDimension =>
            new Vector2(
                _resourceIconsAreaRectangle.Width / _resourceIconIDs.Length,
                _displayDimensions.Y);

        private int[] _resourceIconIDs => new int[]
        {
            5,  // Gold
            6,  // Wood
            7,  // Coal
            8,  // Iron
            12, // Stone
            9,  // Food
            10, // Energy
            11  // Workers
        };

        private int[] _resourceVals => new int[]
        {
            _currentGameState.GSData.PlayerInventory.Gold,
            _currentGameState.GSData.PlayerInventory.Wood,
            _currentGameState.GSData.PlayerInventory.Coal,
            _currentGameState.GSData.PlayerInventory.Iron,
            _currentGameState.GSData.PlayerInventory.Stone,
            _currentGameState.GSData.PlayerInventory.Food,
            _currentGameState.GSData.PlayerInventory.Energy,
            _currentGameState.GSData.PlayerInventory.Workers
        };

        private List<TopBarIcon> _resourceTopBarIcons = new List<TopBarIcon>();

        public TopBar(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            // get required arguments
            _gameContent = content_;
            _graphicsDevice = graphicsDevice_;

            // set display dimensions
            _displayDimensions = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height * 0.09f);

            // calculate texture data(s)
            _displayTexture = new Texture2D(_graphicsDevice, (int) _displayDimensions.X, (int) _displayDimensions.Y);
            var o = new Color[(int) _displayDimensions.X * (int) _displayDimensions.Y];
            for (var i = 0; i < o.Length; i++) o[i] = _displayColor;
            _displayTexture.SetData(o);

            _borderTexture = new Texture2D(_graphicsDevice, (int)_borderDimensions.X, (int)_borderDimensions.Y);
            var e = new Color[(int)_borderDimensions.X * (int)_borderDimensions.Y];
            for (var i = 0; i < e.Length; i++) e[i] = _borderColor;
            _borderTexture.SetData(e);

            _resourceAreaTexture = _gameContent.GetUiTexture(29);
            _font = _gameContent.GetFont(1);

            // calculate icon rects
            var icon_start = new Vector2(_resourceIconsAreaRectangle.X, 0);
            for (var i = 0; i < _resourceIconIDs.Length; i++)
            {
                var icon_id = _resourceIconIDs[i];
                var icon_txt = _gameContent.GetUiTexture(icon_id);

                var icon_pos = icon_start + new Vector2(_resourceIconDisplayDimension.X * i, 0);

                var icon_rect = new Rectangle((int)icon_pos.X, (int)icon_pos.Y, (int)_resourceIconDisplayDimension.X,
                    (int)_resourceIconDisplayDimension.Y);
                var icon_print_rect = new Rectangle(
                    (int)(icon_rect.X + ((icon_rect.Width / 2) - (icon_rect.Width / 8))),
                    (int)(icon_rect.Y + (icon_rect.Height / 7)),
                    (int)(icon_rect.Width / 4),
                    (int)((icon_rect.Width / 4) * icon_txt.Height / icon_txt.Width)
                );

                var text_origin = new Vector2(
                    icon_rect.X +
                        (icon_rect.Width / 2), 
                    icon_rect.Y +
                        (icon_rect.Height));

                _resourceTopBarIcons.Add(new TopBarIcon()
                {
                    Texture = icon_txt,
                    Rect = icon_print_rect,
                    Index = i,
                    TextOrigin = text_origin
                });
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_displayTexture, destinationRectangle: _displayRectangle, color: Color.White);
            spriteBatch.Draw(_resourceAreaTexture, destinationRectangle: _resourceIconsAreaRectangle, color: Color.White);

            foreach (var r in _resourceTopBarIcons)
            {
                spriteBatch.Draw(r.Texture, destinationRectangle: r.Rect, color: Color.White);
                var res_text = "0";
                try
                {
                    res_text = $"{_resourceVals[r.Index]}";
                }
                catch (Exception e)
                {
                    Log.Error("CitySim-TopBar", "Error loading resource value for top bar: " + e.Message);
                }
                var text_dim = new Vector2(
                    _font.MeasureString(res_text).X,
                    _font.MeasureString(res_text).Y);
                var text_orig = new Vector2(text_dim.X / 2, text_dim.Y / 2);
                var text_pos = r.TextOrigin + new Vector2(0, (-_font.MeasureString(res_text).Y));
                spriteBatch.DrawString(_font, res_text, text_pos, Color.Black, 0.0f, text_orig, new Vector2(_textScale, _textScale), SpriteEffects.None, 1.0f);
            }

            var date_str_pos = new Vector2(_displayRectangle.Width * 0.8f, _displayRectangle.Height / 2);
            var date_str = "Day 1, Year 1";
            try
            {
                date_str = $"Day {_currentGameState.GSData.Day}, Year {_currentGameState.GSData.Year}";
            }
            catch (Exception e)
            {
                Log.Error("CitySim-TopBar", "Error loading date/year value for top bar: " + e.Message);
            }
            var date_str_origin = new Vector2(0, _font.MeasureString(date_str).Y / 2);
            spriteBatch.DrawString(_font, date_str, date_str_pos, Color.White, 0f, date_str_origin, new Vector2(_textScale, _textScale), SpriteEffects.None, 1f);

            // draw bottom border
            spriteBatch.Draw(_borderTexture, destinationRectangle: _borderRectangle, color: Color.White);
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            _currentGameState = state;
        }
    }

    public class TopBarIcon
    {
        public Texture2D Texture;
        public Rectangle Rect;
        public int Index;
        public Vector2 TextOrigin;
    }
}