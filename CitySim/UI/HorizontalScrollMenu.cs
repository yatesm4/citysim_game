using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CitySim.Objects;
using CitySim.States;

namespace CitySim.UI
{
    public class HorizontalScrollMenu : Component
    {
        public GameContent _content { get; set; }
        public SpriteFont _font { get; set; }

        private bool _isHovering { get; set; } = false;

        private MouseState _previousMouse;
        private MouseState _currentMouse;

        private Vector2 _displaySize { get; set; }

        public Texture2D Texture { get; set; }
        public Texture2D SelectedTexture { get; set; }
        public int SelectedTextureId { get; set; } = 1;
        public int SelectedTextureTypeId { get; set; } = 0;
        public Color DisplayColor { get; set; } = Color.DarkGray;
        public Color[] DisplayColorData { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = new Vector2(1, 1);

        public Button[] Buttons = new Button[2];
        public List<Component> Components = new List<Component>();
        public List<ContentData<Texture2D>> Items = new List<ContentData<Texture2D>>();
        public SelectionCell[] SelectionCells;
        public Vector2 SelectionIndex = new Vector2(0, 7);

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.X, (int)_displaySize.X, (int)_displaySize.Y);
            }
        }

        public Rectangle PreviewRectangle
        {
            get
            {
                return new Rectangle((int)Position.X + (int)_displaySize.X + 8, (int)Position.Y + 8, 32, 32);
            }
        }

        public HorizontalScrollMenu(GraphicsDevice graphicsDevice_, GameContent content_, Vector2 size_, Vector2 position_, List<ContentData<Texture2D>> items_)
        {
            Position = position_;
            _displaySize = size_;
            Items = items_;
            _content = content_;
            _font = _content.GetFont(1);
            SetColorData(graphicsDevice_);
            LoadSelections(graphicsDevice_, _content);
            LoadButtons(graphicsDevice_, _content);
        }

        public void SetColorData(GraphicsDevice graphicsDevice_)
        {
            Texture = new Texture2D(graphicsDevice_, (int)_displaySize.X, (int)_displaySize.Y);
            DisplayColorData = new Color[(int)_displaySize.X * (int)_displaySize.Y];
            for (int i = 0; i < DisplayColorData.Length; i++)
            {
                DisplayColorData[i] = DisplayColor;
            }
            Texture.SetData(DisplayColorData);
        }

        public void LoadSelections(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            SelectionCells = new SelectionCell[Items.Count];
            Console.WriteLine($"Loading selections for debug menu: {Items.Count} items");
            for(int i = 0; i < Items.Count; i++)
            {
                SelectionCells[i] = new SelectionCell(graphicsDevice_, new Vector2(Position.X + 48, Position.Y + 8), Items[i].Data, i + 1)
                {
                    ParentMenu = this,
                    ObjectID = Items[i].Id
                };
            }
            SelectedTexture = SelectionCells[1].ObjectTexture;
        }

        public void LoadButtons(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            Buttons[0] = new Button(content_.GetUiTexture(7), content_.GetFont(1))
            {
                Position = new Vector2(Rectangle.X + 8, Rectangle.Y + 8)
            };
            Buttons[0].Click += delegate
            {
                if(SelectionIndex.X > 0)
                {
                    SelectionIndex -= new Vector2(1, 1);
                }
            };
            Buttons[1] = new Button(content_.GetUiTexture(7), content_.GetFont(1))
            {
                Position = new Vector2((Rectangle.X + 40) + (32 * 7) + (8 * 8), Rectangle.Y + 8),
                IsFlipped = true
            };
            Buttons[1].Click += delegate
            {
                if (SelectionIndex.Y < Items.Count)
                {
                    SelectionIndex += new Vector2(1, 1);
                }
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color.White);
            foreach(Button b in Buttons)
            {
                b.Draw(gameTime, spriteBatch);
            }
            var j = 1;
            for(int i = (int)SelectionIndex.X; i < SelectionIndex.Y; i++)
            {
                SelectionCells[i].ResetPos();
                SelectionCells[i].Position = new Vector2(Rectangle.X + (j * 40) + 8, Rectangle.Y + 8);
                SelectionCells[i].Draw(gameTime, spriteBatch);
                j++;
            }
            spriteBatch.Draw(SelectedTexture, PreviewRectangle, Color.White);
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            foreach(Button b in Buttons)
            {
                b.Update(gameTime, state);
            }
            for (int i = (int)SelectionIndex.X; i < SelectionIndex.Y; i++)
            {
                SelectionCells[i].Update(gameTime, state);
            }
        }
    }
}
