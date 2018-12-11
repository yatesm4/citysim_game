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
using CitySimAndroid.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySimAndroid.States
{
    public class MenuState : State
    {
        private GameInstance _game;

        // list to hold all components in menu
        private List<Component> _components;

        // texture for mouse cursor
        private Texture2D _cursorTexture { get; set; }
        private Texture2D _backgroundTexture { get; set; }

        private int scroll_x = -50;
        private bool scroll_x_reverse = true;

        private int scroll_y = -200;
        private bool scroll_y_reverse = false;

        // construct state
        public MenuState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _game = game;

            // variables to hold button texture and font
            var buttonTexture = _content.Load<Texture2D>("Sprites/UI/UI_Button");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font_01");

            _backgroundTexture = _content.Load<Texture2D>("Sprites/Images/world_capture");

            #region CREATE BUTTONS
            // create buttons and set properties, and click event functions
            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2) + new Vector2(0, -500),
                Text = "New Game",
                HoverColor = Color.Green,
                Scale = new Vector2(4.0f, 4.0f)
            };
            newGameButton.Click += NewGameButton_Click;
            newGameButton.Position = newGameButton.Position + new Vector2(-(newGameButton.Rectangle.Width / 2), 0);

            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2) + new Vector2(0, -250),
                Text = "Load Game",
                HoverColor = Color.Yellow,
                Scale = new Vector2(4.0f, 4.0f)
            };
            loadGameButton.Click += LoadGameButton_Click;
            loadGameButton.Position = loadGameButton.Position + new Vector2(-(loadGameButton.Rectangle.Width / 2), 0);

            var editMapButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2),
                Text = "Edit Map",
                HoverColor = Color.Orange,
                Scale = new Vector2(4.0f, 4.0f)
            };
            editMapButton.Click += EditMapButton_Click;
            editMapButton.Position = editMapButton.Position + new Vector2(-(editMapButton.Rectangle.Width / 2), 0);

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2) + new Vector2(0, 250),
                Text = "Quit Game",
                HoverColor = Color.Red,
                Scale = new Vector2(4.0f, 4.0f)
            };
            quitGameButton.Click += QuitGameButton_Click;
            quitGameButton.Position = quitGameButton.Position + new Vector2(-(quitGameButton.Rectangle.Width / 2), 0);
            #endregion

            // add buttons to list of components
            _components = new List<Component>()
            {
                newGameButton,
                loadGameButton,
                editMapButton,
                quitGameButton
            };

            // set mouse position
            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

            // load (mouse) cursor content
            _cursorTexture = _content.Load<Texture2D>("Sprites/UI/UI_Cursor");
        }

        #region BUTTON CLICK METHODS
        // functions for button click events

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            Log.Info("CitySim",  "Quitting game...");
            _game.Exit();
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            // todo load game
            Log.Info("CitySim",  "Loading game...");
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content, false));
            // load previous game
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // todo new game
            Log.Info("CitySim",  "Starting new game...");
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content, true));
            // load new game
        }

        private void EditMapButton_Click(object sender, EventArgs e)
        {
            Log.Info("CitySim",  "Editing maps...");
            //_game.ChangeState(new EditMapsListState(_game, _graphicsDevice, _content));
            // load edit map
        }

        private void TestingButton_Click(object sender, EventArgs e)
        {
            Log.Info("CitySim",  "Loading testing / debug...");
            _game.ChangeState(new TestingState(_game, _graphicsDevice, _content));
        }
        #endregion

        // draw state
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _game.RenderScale);

            #region BG SCROLL LOGIC
            // do scroll math for background image
            if (scroll_x > -300 && scroll_x_reverse.Equals(false))
            {
                scroll_x--;
            }
            else
            {
                if (scroll_x_reverse.Equals(false))
                {
                    scroll_x_reverse = true;
                }
            }
            if (scroll_x_reverse.Equals(true) && scroll_x < 0)
            {
                scroll_x++;
            }
            else
            {
                if (scroll_x_reverse.Equals(true))
                {
                    scroll_x_reverse = false;
                }
            }

            if (scroll_y > -200 && scroll_y_reverse.Equals(false))
            {
                scroll_y--;
            }
            else
            {
                if (scroll_y_reverse.Equals(false))
                {
                    scroll_y_reverse = true;
                }
            }
            if (scroll_y_reverse.Equals(true) && scroll_y < 0)
            {
                scroll_y++;
            }
            else
            {
                if (scroll_y_reverse.Equals(true))
                {
                    scroll_y_reverse = false;
                }
            }
            #endregion

            // draw background
            var bg_pos = new Vector2(scroll_x, scroll_y);
            spriteBatch.Draw(_backgroundTexture, bg_pos, null, Color.LightBlue, 0.0f, new Vector2(0,0), 3.0f, SpriteEffects.None, 0.0f);

            // draw each component
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            //var msp = Mouse.GetState().Position;
            //var mp = new Vector2(msp.X, msp.Y);

            //spriteBatch.Draw(_cursorTexture, mp, Color.White);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if not needed
        }

        // update
        public override void Update(GameTime gameTime)
        {
            // update each component
            foreach (var component in _components)
                component.Update(gameTime, null);
        }
    }
}