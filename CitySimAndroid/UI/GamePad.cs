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
using Microsoft.Xna.Framework.Input.Touch;

namespace CitySimAndroid.UI
{
    public class GamePad : Component
    {
        private TouchCollection _currentTouch;
        private TouchCollection _previousTouch;

        private GameState _state;

        private GraphicsDevice _graphicsDevice;

        private bool _isHovering { get; set; } = false;
        public bool IsHovering => _isHovering;

        private Texture2D _texture;

        private Color _color;

        private Vector2 _btn1_position { get; set; }

        private Vector2 _btn2_position { get; set; }
        private Vector2 _btn3_position { get; set; }
        private Vector2 _btn4_position { get; set; }

        private Vector2 _btnScale { get; set; } = new Vector2(2f, 2f);

        public GamePad(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            _graphicsDevice = graphicsDevice_;

            _texture = content_.GetUiTexture(40);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            throw new NotImplementedException();
        }
    }
}