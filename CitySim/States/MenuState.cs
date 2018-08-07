using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitySim;
using CitySim.States;
using CitySim.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CitySim.States
{
    public class MenuState : State
    {
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

            // variables to hold button texture and font
            var buttonTexture = _content.Load<Texture2D>("Sprites/UI/UI_Button");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font_01");

            _backgroundTexture = _content.Load<Texture2D>("Sprites/Images/world_capture");

            // create buttons and set properties, and click event functions

            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2) + new Vector2(0, -100),
                Text = "New Game",
                HoverColor = Color.Red,
                Scale = new Vector2(0.8f, 0.8f)
            };
            newGameButton.Click += NewGameButton_Click;

            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2),
                Text = "Load Game",
                HoverColor = Color.Orange,
                Scale = new Vector2(0.8f, 0.8f)
            };
            loadGameButton.Click += LoadGameButton_Click;

            var editMapButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2) + new Vector2(0, 100),
                Text = "Edit Map",
                HoverColor = Color.Yellow,
                Scale = new Vector2(0.8f, 0.8f)
            };
            editMapButton.Click += EditMapButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2) + new Vector2(0, 200),
                Text = "Quit Game",
                HoverColor = Color.Green,
                Scale = new Vector2(0.8f, 0.8f)
            };
            quitGameButton.Click += QuitGameButton_Click;

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

        // functions for button click events

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Quitting game...");
            _game.Exit();
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            // todo load game
            Console.WriteLine("Loading game...");
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content, false));
            // load previous game
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // todo new game
            Console.WriteLine("Starting new game...");
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content, true));
            // load new game
        }

        private void EditMapButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Editing maps...");
            //_game.ChangeState(new EditMapsListState(_game, _graphicsDevice, _content));
            // load edit map
        }

        // draw state
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // do scroll math for background image
            if(scroll_x > -300 && scroll_x_reverse.Equals(false))
            {
                scroll_x--;
            } else
            {
                if(scroll_x_reverse.Equals(false))
                {
                    scroll_x_reverse = true;
                }
            }
            if(scroll_x_reverse.Equals(true) && scroll_x < 0)
            {
                scroll_x++;
            } else
            {
                if(scroll_x_reverse.Equals(true))
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

            // draw background
            spriteBatch.Draw(_backgroundTexture, new Vector2(scroll_x, scroll_y), Color.LightBlue);

            // draw each component
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            var msp = Mouse.GetState().Position;
            var mp = new Vector2(msp.X, msp.Y);
            // draw UI / HUD here 
            spriteBatch.Draw(_cursorTexture, mp, Color.White);

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
