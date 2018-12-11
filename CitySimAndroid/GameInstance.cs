using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CitySimAndroid.States;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CitySimAndroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameInstance : Game
    {
        GameAnalytics fpsCounter;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private State _currentState;
        private State _nextState;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private SoundEffect ClickSound;
        private SoundEffect DestroySound;

        protected const int TargetWidth = 480 * 3;
        protected const int TargetHeight = 270 * 3;
        public Matrix RenderScale;

        public GameInstance()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true
            };

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // uncomment if mouse visibility is needed
            //IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentState = new SplashScreenState(this, GraphicsDevice, Content);

            ClickSound = Content.Load<SoundEffect>("Sounds/FX/click");
            DestroySound = Content.Load<SoundEffect>("Sounds/FX/Poof");

            fpsCounter = new GameAnalytics(spriteBatch, Content);
            fpsCounter.LoadContent(Content);

            float scaleX = GraphicsDevice.Viewport.Width / TargetWidth;
            float scaleY = GraphicsDevice.Viewport.Height / TargetHeight;
            RenderScale = Matrix.CreateScale(new Vector3(scaleX, scaleY, 1));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (!(_currentState is MenuState))
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    if (_currentState is GameState state) Task.Run(() => state.SaveGame());
                    _nextState = new MenuState(this, GraphicsDevice, Content);
                }
            }

            if (_currentMouseState.LeftButton == ButtonState.Released &&
                _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                if (IsMouseInsideWindow())
                {
                    var mk_snd = true;
                    if (_currentState is GameState s) { if (!s.IsLoaded) mk_snd = false; }
                    if (mk_snd.Equals(true)) ClickSound.Play(0.2f, -0.3f, 0.0f);
                }
            }

            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
                if (_currentState is MenuState)
                {
                    MediaPlayer.Play(Content.Load<Song>("Sounds/Music/Bgm2"));
                    MediaPlayer.IsRepeating = true;
                }
                else if (_currentState is GameState cs)
                {
                    cs.ObjectDestroyed += GameState_ObjectDestroyed;
                }
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
        }

        private void GameState_ObjectDestroyed(object sender, EventArgs e)
        {
            DestroySound.Play(0.2f, -0.3f, 0.0f);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _currentState.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);

            fpsCounter.Draw(gameTime, spriteBatch);
        }

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        private bool IsMouseInsideWindow()
        {
            MouseState ms = Mouse.GetState();
            Point pos = new Point(ms.X, ms.Y);
            return GraphicsDevice.Viewport.Bounds.Contains(pos);
        }
    }

    public class GameAnalytics
    {
        private SpriteFont _font;
        private float _fps = 0;
        private float _totalTime;
        private float _displayFPS;

        public GameAnalytics(SpriteBatch batch, ContentManager content)
        {
            this._totalTime = 0f;
            this._displayFPS = 0f;
        }

        public void LoadContent(ContentManager content)
        {
            this._font = content.Load<SpriteFont>("Fonts/Font_01");
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _totalTime += elapsed;

            if (_totalTime >= 1000)
            {
                _displayFPS = _fps;
                _fps = 0;
                _totalTime = 0;
            }

            _fps++;

            var ver_str = "Ver: PRE-ALPHA 0.0";

            batch.Begin();
            batch.DrawString(this._font, ver_str, new Vector2(10, 10), Color.White);
            batch.DrawString(this._font, this._displayFPS.ToString() + " FPS", new Vector2(10, 10 + _font.MeasureString(ver_str).Y), Color.White);
            batch.End();
        }
    }
}