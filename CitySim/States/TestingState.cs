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
    public class TestingState : State
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
        public TestingState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {

            // variables to hold button texture and font
            var buttonTexture = _content.Load<Texture2D>("Sprites/UI/UI_Button");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font_01");

            _backgroundTexture = _content.Load<Texture2D>("Sprites/Images/world_capture");

            // add buttons to list of components
            _components = new List<Component>();

            // set mouse position
            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

            // load (mouse) cursor content
            _cursorTexture = _content.Load<Texture2D>("Sprites/UI/UI_Cursor");
        }

        // draw state
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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
