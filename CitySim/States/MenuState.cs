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

namespace CitySim.States
{
    public class MenuState : State
    {
        private List<Component> _components;
        
        public MenuState(GameInstance game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTexture = _content.Load<Texture2D>("Sprites/UI/UI_Button");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font_01");

            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(50, 50),
                Text = "New Game",
                HoverColor = Color.Red,
                Scale = new Vector2(0.8f, 0.8f)
            };
            newGameButton.Click += NewGameButton_Click;

            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(50, 150),
                Text = "Load Game",
                HoverColor = Color.Orange,
                Scale = new Vector2(0.8f, 0.8f)
            };
            loadGameButton.Click += LoadGameButton_Click;

            var editMapButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(50, 250),
                Text = "Edit Map",
                HoverColor = Color.Yellow,
                Scale = new Vector2(0.8f, 0.8f)
            };
            editMapButton.Click += EditMapButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(50, 350),
                Text = "Quit Game",
                HoverColor = Color.Green,
                Scale = new Vector2(0.8f, 0.8f)
            };
            quitGameButton.Click += QuitGameButton_Click;

            _components = new List<Component>()
            {
                newGameButton,
                loadGameButton,
                editMapButton,
                quitGameButton
            };

            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Quitting game...");
            _game.Exit();
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            // todo load game
            Console.WriteLine("Loading game...");
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // todo new game
            Console.WriteLine("Starting new game...");
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            // load new state
        }

        private void EditMapButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Editing maps...");
            //_game.ChangeState(new EditMapsListState(_game, _graphicsDevice, _content));
            // load edit map
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if not needed
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
    }
}
