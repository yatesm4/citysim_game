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
    public class DialogWindow : Component
    {
        private GraphicsDevice _graphicsDevice;
        private GameContent _gameContent;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private Rectangle _mouseRect =>
            new Rectangle(
                _currentMouseState.X,
                _currentMouseState.Y,
                1, 1
            );

        private Rectangle _prevMouseRect =>
            new Rectangle(
                _previousMouseState.X,
                _previousMouseState.Y,
                1, 1
            );

        private TouchCollection _currentTouch;
        private TouchCollection _previousTouch;

        private GameState _currentGameState;


        private bool _isHovering { get; set; } = false;
        public bool IsHovering => _isHovering;


        public event EventHandler Click;
        public event EventHandler Close;
        public event EventHandler Open;


        public Color TextColor { get; set; } = Color.Black;
        public Color HoverColor { get; set; } = Color.White;

        private Texture2D _texture { get; set; }
        public Texture2D Texture => _texture;
        private Texture2D _shadowTexture { get; set; }
        public Texture2D ShadowTexture => _shadowTexture;
        private Texture2D _topBarTexture { get; set; }
        public Texture2D TopBarTexture => _topBarTexture;
        private Texture2D _closeButtonTexture { get; set; }
        public Texture2D CloseButtonTexture => _closeButtonTexture;
        public Button CloseButton { get; set; }

        public Vector2 TopBarDimensions =>
            new Vector2(
                Rectangle.Width,
                (CloseButton?.Rectangle.Height ?? 32 * 5) / 2
            );
        public Rectangle TopBarRectangle =>
            new Rectangle(
                Rectangle.X,
                (int)(Rectangle.Y - TopBarDimensions.Y),
                (int)TopBarDimensions.X,
                (int)TopBarDimensions.Y
            );
        public Vector2 CloseButtonPosition =>
            new Vector2(
                (TopBarRectangle.X + TopBarRectangle.Width) - (CloseButton?.Rectangle.Width ?? 32 * 5f) - 20,
                (TopBarRectangle.Y + 20)
            );

        private SpriteFont _font { get; set; }

        public string Text { get; set; } = "No text defined.";
        public string[] TextRows => Text.Split(new[] { '\r', '\n' });
        public int MaxRowLength => TextRows.Select(r => r.Length).Concat(new[] { 0 }).Max();
        public Vector2 MaxTextDimensions()
        {
            var row = "";
            foreach (var r in TextRows) { if (r.Length > row.Length) row = r; }
            return _font.MeasureString(row);
        }

        public Vector2 TextScale { get; set; } = new Vector2(1.5f, 1.5f);

        public Vector2 TextStartPosition =>
            new Vector2(
                Position.X + (Dimensions.X / 2),
                Position.Y + (Padding)
            );

        public int Padding { get; set; } = 100;

        // Dimensions of DialogWindow
        public Vector2 Dimensions =>
            new Vector2(
                (MaxTextDimensions().X * TextScale.X) + (Padding * 2),
                ((MaxTextDimensions().Y * TextScale.Y) * TextRows.Length) + (Padding * 2)
            );

        // Dimensions of the black border displayed around the DialogWindow
        public Vector2 BorderDimensions =>
            new Vector2(
                Dimensions.X + 4,
                Dimensions.Y + 4
            );

        public Rectangle Rectangle =>
            new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)Dimensions.X,
                (int)Dimensions.Y
            );

        public Rectangle ShadowRectangle =>
            new Rectangle(
                (int)Position.X + 4,
                (int)TopBarRectangle.Y + 4,
                (int)Dimensions.X,
                (int)Dimensions.Y + (int)TopBarRectangle.Height
            );

        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public DialogWindow(string text_, Vector2 pos_, GraphicsDevice gd_, SpriteFont font_, GameContent content_, Texture2D txt_ = null)
        {
            // get required arguments from constructor
            Text = text_;
            _graphicsDevice = gd_;
            _font = font_;
            _gameContent = content_;

            // calculate texture of dialog window shadow
            _shadowTexture = new Texture2D(_graphicsDevice, (int)Dimensions.X, (int)Dimensions.Y);
            var u = new Color[(int)Dimensions.X * (int)Dimensions.Y];
            for (int i = 0; i < u.Length; i++)
            {
                u[i] = Color.Black;
            }
            _shadowTexture.SetData(u);

            // check if custom texture supplied
            if (txt_ != null)
            {
                _texture = txt_;
            }
            else
            {
                // calculate default texture for dialog window 
                _texture = new Texture2D(_graphicsDevice, (int)Dimensions.X, (int)Dimensions.Y);

                var o = new Color[(int)Dimensions.X * (int)Dimensions.Y];

                for (int i = 0; i < o.Length; i++)
                {
                    o[i] = Color.DarkKhaki;
                }
                _texture.SetData(o);
            }

            // Calculate position
            Position = new Vector2(
                (_graphicsDevice.Viewport.Width / 2) - (Rectangle.Width / 2),
                (_graphicsDevice.Viewport.Height / 2) - (Rectangle.Height / 2)
            );

            // get close button texture
            _closeButtonTexture = _gameContent.GetUiTexture(38);
            CloseButton = new Button(_closeButtonTexture, _font)
            {
                Position = CloseButtonPosition,
                HoverColor = Color.Red
            };
            CloseButton.CustomRect = new Rectangle(
                (int)CloseButton.Position.X,
                (int)CloseButton.Position.Y,
                _closeButtonTexture.Width * 5,
                _closeButtonTexture.Height * 5);
            CloseButton.Click += CloseButton_Click;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Console.Out.WriteLine("Closing dialog window...");
            Close?.Invoke(this, new EventArgs());
            Disposed = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Disposed) return;

            var k = TextRows;
            var g = MaxTextDimensions();
            spriteBatch.Draw(_shadowTexture, destinationRectangle: ShadowRectangle, color: Color.White);
            spriteBatch.Draw(_texture, destinationRectangle: Rectangle, color: Color.White);
            spriteBatch.Draw(_texture, destinationRectangle: TopBarRectangle, color: Color.White);
            CloseButton.Draw(gameTime, spriteBatch);
            if (!string.IsNullOrEmpty(Text))
            {
                int i = 0;
                foreach (var row in TextRows)
                {

                    // calculate text position and sheit
                    var txt_pos = TextStartPosition;
                    txt_pos += new Vector2(0, i * (_font.MeasureString(row).Y * TextScale.Y));

                    // middle of the text, betch
                    var txt_origin = new Vector2((_font.MeasureString(row).X) / 2,
                        (_font.MeasureString(row).Y) / 2);

                    spriteBatch.DrawString(_font, row, txt_pos, TextColor, 0f, txt_origin, TextScale,
                        SpriteEffects.None, 1f);

                    i++;
                }
            }
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            if (Disposed) return;

            // get mouse state
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _previousTouch = _currentTouch;
            _currentTouch = TouchPanel.GetState();

            if (TopBarRectangle.Contains(_mouseRect))
            {
                if (_currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    // get location offset of mouse states
                    var x = _mouseRect.X - _prevMouseRect.X;
                    var y = _mouseRect.Y - _prevMouseRect.Y;

                    Position += new Vector2(x, y);
                    CloseButton.Position += new Vector2(x, y);
                }
            }

            CloseButton.Update(gameTime, state);
        }
    }
}