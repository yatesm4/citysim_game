using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Content;
using CitySim.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.UI
{
    public class HUD : Component
    {
        public GameState State { get; set; }
        
        protected GameContent _content { get; set; }

        protected GraphicsDevice _graphicsDevice { get; set; }

        protected SpriteFont _font { get; set; }

        protected MouseState _previousMouse { get; set; }
        protected MouseState _currentMouse { get; set; }

        protected Vector2 _displaySize { get; set; }

        protected Texture2D _texture { get; set; }
        protected Color _displayColor { get; set; } = Color.WhiteSmoke;
        protected Color[] _displayColorData { get; set; }

        protected Vector2 _buttonSize { get; set; }

        protected Vector2 _position { get; set; }
        protected Vector2 _scale { get; set; } = new Vector2(1, 1);

        protected List<Component> _components = new List<Component>();

        public Rectangle DisplayRect => new Rectangle((int)_position.X, (int)_position.Y, (int)_displaySize.X, (int)_displaySize.Y);
        public Rectangle BorderRect => new Rectangle((int)_position.X, (int)_position.Y - 5, (int)_displaySize.X, 5);

        // PROPERTIES FOR SECTIONS WITHIN UI

        // SELECTION CELLS (RIGHT MENU)
        protected Vector2 SelectionCellsSection_GridSpacePercents => new Vector2(0.9f, 0.1f);
        protected Vector2 SelectionCellsSection_Dimensions => new Vector2(_displaySize.X * 0.4f, _displaySize.Y);
        protected Rectangle SelectionCellsSection_Rectangle => new Rectangle((int)(_position.X + (_displaySize.X * 0.6)), (int)_position.Y, (int)SelectionCellsSection_Dimensions.X, (int)SelectionCellsSection_Dimensions.Y);
        protected Texture2D SelectionCellsSection_Texture { get; set; }

        protected Vector2 SelectionCell_GridDimensions => new Vector2(6,3);
        protected Vector2 SelectionCell_Dimensions => new Vector2((SelectionCellsSection_Dimensions.X * SelectionCellsSection_GridSpacePercents.X) / SelectionCell_GridDimensions.X,
            (SelectionCellsSection_Dimensions.Y * SelectionCellsSection_GridSpacePercents.X) / SelectionCell_GridDimensions.Y);
        protected Texture2D SelectionCell_Texture { get; set; }

        protected Vector2 SelectionCell_Spacing => new Vector2((SelectionCellsSection_Dimensions.X * SelectionCellsSection_GridSpacePercents.Y) /
                                                               (SelectionCell_GridDimensions.X + 1),
            (SelectionCellsSection_Dimensions.Y * SelectionCellsSection_GridSpacePercents.Y) /
            (SelectionCell_GridDimensions.Y + 1));

        protected Vector2 SelectionCells_BuildingIndexes => new Vector2(12, 19); // Y is max + 1
        protected List<Texture2D> SelectionCells_BuildingTextures { get; set; } = new List<Texture2D>();

        // INFOGRAPHIC SECTION (MIDDLE MENU)
        protected Vector2 InfographicsSection_Dimensions => new Vector2(_displaySize.X * 0.3f, _displaySize.Y);
        protected Rectangle InfographicsSection_Rectangle => new Rectangle((int)(_position.X + (_displaySize.X * 0.3)), (int)_position.Y, (int)InfographicsSection_Dimensions.X, (int)InfographicsSection_Dimensions.Y);
        protected Texture2D InfographicsSection_Texture { get; set; }

        protected Vector2 ResourceBar_Dimensions => new Vector2(InfographicsSection_Dimensions.X * 0.9f,
            InfographicsSection_Dimensions.Y * 0.3f);

        protected Vector2 ResourceBar_Position => new Vector2(InfographicsSection_Rectangle.X +
                                                              ((InfographicsSection_Rectangle.Width * 0.1f) / 2),
            (InfographicsSection_Rectangle.Y + InfographicsSection_Rectangle.Height) - (ResourceBar_Dimensions.Y + (InfographicsSection_Rectangle.Height * 0.1f)));
        protected Rectangle ResourceBar_Rectangle => new Rectangle((int)ResourceBar_Position.X, (int)ResourceBar_Position.Y, (int)ResourceBar_Dimensions.X, (int)ResourceBar_Dimensions.Y);
        protected Texture2D ResourceBar_Texture { get; set; }

        protected Vector2 ResourceCell_Dimension => new Vector2(ResourceBar_Dimensions.X / 7, ResourceBar_Dimensions.Y);

        /// <summary>
        /// An int array containing the IDs for icons to represent inventory items in the HUD.
        /// In order: Gold, Wood, Coal, Iron, Food, Energy, Workers
        /// </summary>
        public int[] HUDIconIDs { get; set; }

        public HUD(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            int height = (int)(graphicsDevice_.Viewport.Height * 0.25f);
            int width = (int)(graphicsDevice_.Viewport.Width);

            _position = new Vector2(0, graphicsDevice_.Viewport.Height - height);
            _displaySize = new Vector2(width, height);
            _buttonSize = new Vector2(30, height * 0.8f);

            Console.WriteLine("HUD created.");
            Console.WriteLine($"HUD Size: {_displaySize}");
            Console.WriteLine($"HUD Pos: {_position}");

            _content = content_;
            _font = _content.GetFont(1);

            HUDIconIDs = new int[]
            {
                5,6,7,8,9,10,11
            };

            SetColorData(graphicsDevice_);

            for (int i = (int)SelectionCells_BuildingIndexes.X; i < SelectionCells_BuildingIndexes.Y; i++)
            {
                SelectionCells_BuildingTextures.Add(content_.GetUiTexture(i));
            }

            if (SelectionCells_BuildingTextures.Count <
                (SelectionCell_GridDimensions.X * SelectionCell_GridDimensions.Y))
            {
                var dif = (SelectionCell_GridDimensions.X * SelectionCell_GridDimensions.Y) -
                          SelectionCells_BuildingTextures.Count;
                for (int i = 0; i < dif + 1; i++)
                {
                    SelectionCells_BuildingTextures.Add(SelectionCells_BuildingTextures[0]);
                }
            }

            SetSelectionCellButtons();
        }

        public void SetColorData(GraphicsDevice graphicsDevice_)
        {
            _texture = new Texture2D(graphicsDevice_, (int)_displaySize.X, (int)_displaySize.Y);
            _displayColorData = new Color[(int)_displaySize.X * (int)_displaySize.Y];
            for (int i = 0; i < _displayColorData.Length; i++)
            {
                _displayColorData[i] = _displayColor;
            }
            _texture.SetData(_displayColorData);

            SelectionCellsSection_Texture = new Texture2D(graphicsDevice_, (int)SelectionCellsSection_Dimensions.X, (int)SelectionCellsSection_Dimensions.Y);
            var scst_data =
                new Color[(int) SelectionCellsSection_Dimensions.X * (int) SelectionCellsSection_Dimensions.Y];
            for (int i = 0; i < scst_data.Length; i++)
            {
                scst_data[i] = Color.White;
            }
            SelectionCellsSection_Texture.SetData(scst_data);

            InfographicsSection_Texture = new Texture2D(graphicsDevice_, (int)InfographicsSection_Dimensions.X, (int)InfographicsSection_Dimensions.Y);
            var ist_data =
                new Color[(int)InfographicsSection_Dimensions.X * (int)InfographicsSection_Dimensions.Y];
            for (int i = 0; i < ist_data.Length; i++)
            {
                ist_data[i] = Color.White;
            }
            InfographicsSection_Texture.SetData(ist_data);

            ResourceBar_Texture = new Texture2D(graphicsDevice_, (int)ResourceBar_Dimensions.X, (int)ResourceBar_Dimensions.Y);
            var rbt_data =
                new Color[(int)ResourceBar_Dimensions.X * (int)ResourceBar_Dimensions.Y];
            for (int i = 0; i < rbt_data.Length; i++)
            {
                rbt_data[i] = Color.White;
            }
            ResourceBar_Texture.SetData(rbt_data);

            SelectionCell_Texture = new Texture2D(graphicsDevice_, (int)SelectionCell_Dimensions.X, (int)SelectionCell_Dimensions.Y);
            var sct_data = new Color[(int)SelectionCell_Dimensions.X * (int)SelectionCell_Dimensions.Y];
            for (int i = 0; i < sct_data.Length; i++)
            {
                sct_data[i] = Color.Beige;
            }
            SelectionCell_Texture.SetData(sct_data);
        }

        public void SetSelectionCellButtons()
        {
            var initCellPos = new Vector2(SelectionCellsSection_Rectangle.X, SelectionCellsSection_Rectangle.Y) +
                              new Vector2(SelectionCell_Spacing.X, SelectionCell_Spacing.Y);
            var b = 1;
            for (int i = 0; i < (SelectionCell_GridDimensions.Y); i++)
            {
                var cellRowPos = initCellPos;
                cellRowPos += new Vector2(0, SelectionCell_Dimensions.Y * i) + new Vector2(0, SelectionCell_Spacing.Y * i);
                for (int j = 0; j < (SelectionCell_GridDimensions.X); j++)
                {
                    var cellColPos = cellRowPos + new Vector2(j * SelectionCell_Dimensions.X, 0) + new Vector2(j * SelectionCell_Spacing.X, 0);
                    var cellRect = new Rectangle((int)cellColPos.X, (int)cellColPos.Y, (int)SelectionCell_Dimensions.X, (int)SelectionCell_Dimensions.Y);
                    var btn = new Button(SelectionCells_BuildingTextures[0], _font, cellRect);
                    btn = new Button(SelectionCells_BuildingTextures[b], _font, cellRect)
                    {
                        Position = cellColPos,
                        HoverColor = Color.Yellow,
                        ResourceLocked = true
                    };
                    btn.ID = b;
                    btn.Click += BldgBtn_OnClick;
                    _components.Add(btn);
                    // increment which building is currently drawn
                    b++;
                }
            }
        }

        private void BldgBtn_OnClick(object sender, EventArgs e)
        {
            if (sender is Button convSender)
            {
                Console.WriteLine($"Button clicked: {BuildingData.Dict_BuildingKeys[convSender.ID].Name}");
                State.SelectedObject = BuildingData.Dict_BuildingKeys[convSender.ID];
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DisplayRect, Color.DarkKhaki);
            spriteBatch.Draw(_texture, BorderRect, Color.Black);

            spriteBatch.Draw(SelectionCellsSection_Texture, SelectionCellsSection_Rectangle, Color.DarkKhaki);
            spriteBatch.Draw(InfographicsSection_Texture, InfographicsSection_Rectangle, Color.DarkOliveGreen);
            spriteBatch.Draw(ResourceBar_Texture, ResourceBar_Rectangle, Color.DarkSlateGray);

            foreach (Component c in _components)
            {
                c.Draw(gameTime, spriteBatch);
            }

            /**
             *      DRAW RESOURCE AND STATE DATA TO HUD
             **/
            if (State is null)
                return;

            #region DRAW RESOURCES
            string game_day = $"Day: {State.GSData.Day}";
            var game_day_origin = new Vector2(0, _font.MeasureString(game_day).Y / 2);
            var game_day_position = _position + new Vector2(5, 15);
            spriteBatch.DrawString(_font, game_day, game_day_position, Color.Black, 0.0f, game_day_origin, 1.5f, SpriteEffects.None, 1.0f);

            string game_year = $"Year: {State.GSData.Year}";
            var game_year_origin = new Vector2(0, _font.MeasureString(game_year).Y / 2);
            var game_year_position = game_day_position + new Vector2(0, _font.MeasureString(game_day).Y + 5);
            spriteBatch.DrawString(_font, game_year, game_year_position, Color.Black, 0.0f, game_year_origin, 1.5f, SpriteEffects.None, 1.0f);

            string version = "ALPHA VER 0.0";
            var version_origin = new Vector2(0, _font.MeasureString(version).Y / 2);
            var version_position = new Vector2(game_year_position.X, (DisplayRect.Y + DisplayRect.Height) - (_font.MeasureString(version).Y * 0.75f));
            spriteBatch.DrawString(_font, version, version_position, Color.Black, 0.0f, version_origin, 0.75f, SpriteEffects.None, 1.0f);

            var resource_vals = new int[]
            {
                State.GSData.PlayerInventory.Gold,
                State.GSData.PlayerInventory.Wood,
                State.GSData.PlayerInventory.Coal,
                State.GSData.PlayerInventory.Iron,
                State.GSData.PlayerInventory.Food,
                State.GSData.PlayerInventory.Energy,
                State.GSData.PlayerInventory.Workers
            };

            var icon_start_pos = new Vector2(ResourceBar_Rectangle.X, ResourceBar_Rectangle.Y);
            var icon_scale = new Vector2(2, 2);
            // display inventory icons in hud
            for(int i = 0; i < HUDIconIDs.Length; i++)
            {
                var icon_id = HUDIconIDs[i];
                var icon_texture = _content.GetUiTexture(icon_id);
                var icon_position = icon_start_pos + new Vector2(ResourceCell_Dimension.X * i, 0);
                icon_position += new Vector2(ResourceCell_Dimension.X * 0.3f, ResourceCell_Dimension.Y * 0.3f);
                var icon_rect = new Rectangle((int)icon_position.X, (int)icon_position.Y, (int) (ResourceCell_Dimension
                        .X * 0.6f), (int)
                    (ResourceCell_Dimension.Y * 0.6f));
                spriteBatch.Draw(icon_texture, icon_rect, Color.White);

                var resource_text = $"{resource_vals[i]}";
                var resource_text_origin = new Vector2(_font.MeasureString(resource_text).X / 2, _font.MeasureString(resource_text).Y / 2);
                var resource_text_pos = icon_position + new Vector2(icon_rect.Width * 0.5f, -(icon_rect.Height * 0.25f));

                spriteBatch.DrawString(_font, resource_text, resource_text_pos, Color.White, 0.0f, resource_text_origin, 1.0f, SpriteEffects.None, 1.0f);
            }
            #endregion
        }

        public override void Update(GameTime gameTime, GameState state)
        {
            foreach (Component c in _components)
            {
                c.Update(gameTime, state);
            }

            // save gamestate
            State = state;
        }
    }
}
