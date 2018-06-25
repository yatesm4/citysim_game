using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CitySim.Objects;

namespace CitySim.States
{
    public class SplashScreenState : State
    {
        // set splash-image sprite and sprite player
        private Sprite _sprSplash;
        private SpritePlayer _sprPlayer;

        // set animation-playback countdown (till end)
        private int _countdown = 200;

        // construct state
        public SplashScreenState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            // create sprite for splash-image and set loop to false
            _sprSplash = new Sprite(content.Load<Texture2D>("Sprites/Branding/YD_Logo"), 0.15f)
            {
                IsLooping = false
            };
            // set scale of sprite player and play splash-image sprite
            _sprPlayer.Scale = 3.0f;
            _sprPlayer.PlaySprite(_sprSplash);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // clear screen to black
            _graphicsDevice.Clear(Color.Wheat);

            // begin spriteBatch for state
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // draw sprite
            _sprPlayer.Draw(gameTime, spriteBatch, new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height), SpriteEffects.None);
            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // post update
        }

        public override void Update(GameTime gameTime)
        {
            // update
            if(_countdown > 0)
            {
                // subtract countdown
                _countdown--;
            }
            if (_countdown.Equals(0))
            {
                // change to main menu at end of countdown
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            }
        }
    }
}
