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
    public class GamePad : Component
    {
        private bool _firstTake = false;

        private GameContent _content;

        private TouchCollection _currentTouch;
        private TouchCollection _previousTouch;

        private GameState _state;

        private GraphicsDevice _graphicsDevice;

        private bool _isHovering { get; set; } = false;
        public bool IsHovering => _isHovering;

        private Texture2D _texture;
        private Texture2D _textureRotated;

        private float _textureScale = 0.8f;
        private float _textureDisplayDimension => _texture.Width * _textureScale;

        private Color _color;

        private Vector2 _gamepadDisplayOrigin;

        // up button
        private Vector2 _btn1_position => new Vector2(_textureDisplayDimension + (_textureDisplayDimension / 4), _gamepadDisplayOrigin.Y + _textureDisplayDimension / 4);
        private Rectangle _btn1_rect => new Rectangle((int)_btn1_position.X, (int)_btn1_position.Y, (int)_textureDisplayDimension, (int)_textureDisplayDimension);
        private bool _btn1_pressed = false;

        // down button
        private Vector2 _btn2_position => new Vector2(_textureDisplayDimension + (_textureDisplayDimension / 4), _gamepadDisplayOrigin.Y +
            _textureDisplayDimension + (_textureDisplayDimension / 4));
        private Rectangle _btn2_rect => new Rectangle((int)_btn2_position.X, (int)_btn2_position.Y, (int)_textureDisplayDimension, (int)_textureDisplayDimension);
        private bool _btn2_pressed = false;

        // left button
        private Vector2 _btn3_position => new Vector2((_textureDisplayDimension / 4), _gamepadDisplayOrigin.Y +
                                                                                      ((_textureDisplayDimension / 4) * 3));
        private Rectangle _btn3_rect => new Rectangle((int) _btn3_position.X, (int) _btn3_position.Y,
            (int) _textureDisplayDimension, (int) _textureDisplayDimension);
        private bool _btn3_pressed = false;

        // right button
        private Vector2 _btn4_position => new Vector2((_textureDisplayDimension / 4) + (_textureDisplayDimension * 2), _gamepadDisplayOrigin.Y +
                                                                                                                       ((_textureDisplayDimension / 4) * 3));
        private Rectangle _btn4_rect => new Rectangle((int)_btn4_position.X, (int)_btn4_position.Y,
            (int)_textureDisplayDimension, (int)_textureDisplayDimension);
        private bool _btn4_pressed = false;

        // union rect
        public Rectangle GamePadUnionHitbox => Rectangle.Union(
            Rectangle.Union(_btn1_rect, _btn2_rect),
            Rectangle.Union(_btn3_rect, _btn4_rect));

        private Vector2 _btnScale { get; set; } = new Vector2(2f, 2f);

        public float CameraMoveSpeed = 10;

        public GamePad(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            _graphicsDevice = graphicsDevice_;
            _content = content_;
            _texture = _content.GetUiTexture(41);
            _textureRotated = _content.GetUiTexture(43);

            _gamepadDisplayOrigin = new Vector2(0, (int)((_graphicsDevice.Viewport.Height / 2) -
                                                    (_textureDisplayDimension * 1.25)));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, destinationRectangle: _btn1_rect, color: _btn1_pressed ? Color.DarkGray : Color.White, effects: SpriteEffects.None);

            spriteBatch.Draw(_texture, destinationRectangle: _btn2_rect, color: _btn2_pressed ? Color.DarkGray : Color.White, effects: SpriteEffects.FlipVertically);

            spriteBatch.Draw(_textureRotated, destinationRectangle: _btn3_rect, color: _btn3_pressed ? Color.DarkGray : Color.White, effects: SpriteEffects.FlipHorizontally);

            spriteBatch.Draw(_textureRotated, destinationRectangle: _btn4_rect, color: _btn4_pressed ? Color.DarkGray : Color.White, effects: SpriteEffects.None);

            // debug
            //spriteBatch.Draw(_texture, destinationRectangle: _gamepadDisplayRect, color: Color.Red, effects: SpriteEffects.None);
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update touch states
            _previousTouch = _currentTouch;
            _currentTouch = state.CurrentTouch;

            // resset button bool vals
            _btn1_pressed = false;
            _btn2_pressed = false;
            _btn3_pressed = false;
            _btn4_pressed = false;

            // for every touch
            foreach (var tl in _currentTouch)
            {
                // if pressing/touching
                if (tl.State == TouchLocationState.Moved || tl.State == TouchLocationState.Pressed)
                {
                    var trect = new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 1, 1);

                    if (trect.Intersects(_btn1_rect))
                    {
                        Log.Info("CitySim-UI", "Gamepad Button Touch Registered");
                        _btn1_pressed = true;
                        // reset gamestate camera movement
                        state.CameraIsMoving = false;
                        state.CameraDestination = Vector2.Zero;
                        // update camera position in gamestate
                        state.Camera.Position += new Vector2(0, -CameraMoveSpeed);
                    }
                    else if (trect.Intersects(_btn2_rect))
                    {
                        Log.Info("CitySim-UI", "Gamepad Button Touch Registered");
                        _btn2_pressed = true;
                        // reset gamestate camera movement
                        state.CameraIsMoving = false;
                        state.CameraDestination = Vector2.Zero;
                        // update camera position in gamestate
                        state.Camera.Position += new Vector2(0, CameraMoveSpeed);
                    }
                    else if (trect.Intersects(_btn3_rect))
                    {
                        Log.Info("CitySim-UI", "Gamepad Button Touch Registered");
                        _btn3_pressed = true;
                        // reset gamestate camera movement
                        state.CameraIsMoving = false;
                        state.CameraDestination = Vector2.Zero;
                        // update camera position in gamestate
                        state.Camera.Position += new Vector2(-CameraMoveSpeed, 0);
                    }
                    else if (trect.Intersects(_btn4_rect))
                    {
                        Log.Info("CitySim-UI", "Gamepad Button Touch Registered");
                        _btn4_pressed = true;
                        // reset gamestate camera movement
                        state.CameraIsMoving = false;
                        state.CameraDestination = Vector2.Zero;
                        // update camera position in gamestate
                        state.Camera.Position += new Vector2(CameraMoveSpeed, 0);
                    }
                }
            }
        }
    }
}