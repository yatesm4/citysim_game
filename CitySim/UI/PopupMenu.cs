using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CitySim.Content;
using CitySim.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.UI
{
    public class BuildingMenu : CellMenu
    {
        public List<Texture2D> BuildingTextures { get; set; } = new List<Texture2D>();
        public Vector2 BuildingIndexes { get; set; } = new Vector2(10, 14);

        public BuildingMenu(Texture2D texture, SpriteFont font, Texture2D cellTexture, GameContent content) : base(texture, font, cellTexture)
        {
            _texture = texture;
            _cellTexture = cellTexture;
            _font = font;
            PenColor = Color.Black;
            HoverColor = Color.DarkGray;

            for (int i = (int) BuildingIndexes.X; i < (int) BuildingIndexes.Y; i++)
            {
                BuildingTextures.Add(content.GetTileTexture(i));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (_isHovering)
            {
                color = HoverColor;
            }

            // draw pop up menu here
            if (IsActive is true)
            {
                spriteBatch.Draw(_texture, Rectangle, color);
                var initCellPos = Position + new Vector2(_cellSpacer, _cellSpacer);
                var b = 0;
                for (int i = 0; i < CellRows; i++)
                {
                    var CellRowPos = initCellPos;
                    CellRowPos += new Vector2(0, _cellTexture.Height * i);
                    CellRowPos += new Vector2(0, _cellSpacer * i);
                    for (int j = 0; j < CellCols; j++)
                    {
                        var cellPosition = CellRowPos + new Vector2(j * _cellTexture.Width, 0) +
                                           new Vector2(j * _cellSpacer, 0);
                        var cellRectangle = new Rectangle((int)cellPosition.X, (int)cellPosition.Y, _cellTexture.Width, _cellTexture.Height);
                        try
                        {
                            throw new Exception();
                            //spriteBatch.Draw(BuildingTextures[b], new Rectangle((int)cellPosition.X, (int)cellPosition.Y, BuildingTextures[b].Width, BuildingTextures[b].Height), Color.White);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error drawing building texture in BuildingMenu: " + e.Message);
                            spriteBatch.Draw(_cellTexture, cellRectangle, new Color(rnd.Next(byte.MaxValue + 1), rnd.Next(byte.MaxValue + 1), rnd.Next(byte.MaxValue + 1)));
                        }
                        b++;
                    }
                }
            }
        }
    }

    public class CellMenu : PopupMenu
    {
        protected Random rnd = new Random();

        public static int CellCols = 3;
        public static int CellRows = 5;

        protected Texture2D _cellTexture;

        protected Vector2 _cellDimensions => new Vector2(_cellTexture.Width, _cellTexture.Height);

        protected int _cellSpacer => (_texture.Width - (_cellTexture.Width * 3)) / 4;

        public CellMenu(Texture2D texture, SpriteFont font, Texture2D cellTexture) : base(texture, font)
        {
            _texture = texture;
            _cellTexture = cellTexture;
            _font = font;
            PenColor = Color.Black;
            HoverColor = Color.DarkGray;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (_isHovering)
            {
                color = HoverColor;
            }

            // draw pop up menu here
            if (IsActive is true)
            {
                spriteBatch.Draw(_texture, Rectangle, color);
                var initCellPos = Position + new Vector2(_cellSpacer, _cellSpacer);
                for (int i = 0; i < CellRows; i++)
                {
                    var CellRowPos = initCellPos;
                    CellRowPos += new Vector2(0, _cellTexture.Height * i);
                    CellRowPos += new Vector2(0, _cellSpacer * i);
                    for (int j = 0; j < CellCols; j++)
                    {
                        var cellPosition = CellRowPos + new Vector2(j * _cellTexture.Width, 0) +
                                           new Vector2(j * _cellSpacer, 0);
                        var cellRectangle = new Rectangle((int)cellPosition.X, (int)cellPosition.Y, _cellTexture.Width, _cellTexture.Height);
                        spriteBatch.Draw(_cellTexture, cellRectangle, new Color(rnd.Next(byte.MaxValue + 1), rnd.Next(byte.MaxValue + 1), rnd.Next(byte.MaxValue + 1)));
                    }
                }
            }
        }
    }

    public class PopupMenu : Component
    {
        protected MouseState _currentMouse;
        protected MouseState _previousMouse;

        public bool IsActive { get; set; } = false;

        protected bool _isHovering;

        protected Texture2D _texture;
        protected SpriteFont _font;

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColor { get; set; }

        public Color HoverColor { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public PopupMenu(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            PenColor = Color.Black;
            HoverColor = Color.DarkGray;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (_isHovering)
            {
                color = HoverColor;
            }

            // draw pop up menu here
            if (IsActive is true)
            {
                spriteBatch.Draw(_texture, Rectangle, color);
            }
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            // update pop up menu here

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
