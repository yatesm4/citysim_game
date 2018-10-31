using System.Threading.Tasks;
using CitySim.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CitySim
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameInstance : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private State _currentState;
        private State _nextState;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private SoundEffect ClickSound;

        public GameInstance()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 480 * 3;
            graphics.PreferredBackBufferHeight = 270 * 3;

            Content.RootDirectory = "Content";

            this.Window.AllowUserResizing = true;
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentState = new SplashScreenState(this, GraphicsDevice, Content);

            ClickSound = Content.Load<SoundEffect>("Sounds/FX/Click");
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
                    if(mk_snd.Equals(true)) ClickSound.Play(0.2f, -0.3f, 0.0f);
                }
            }

            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
                if (_currentState is MenuState)
                {
                    //MediaPlayer.Play(Content.Load<Song>("Sounds/Music/Bgm2"));
                    //MediaPlayer.IsRepeating = true;
                }
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
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
}
