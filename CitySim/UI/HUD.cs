using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySim.Content;
using CitySim.Objects;
using CitySim.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CitySim.UI
{
    public class HUD : Component
    {
        public GameState State { get; set; }

        public IncomeReport CurrentIncomeReport => State.CurrentMap.GetIncomeReport;
        
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

        protected Vector2 SelectionCells_BuildingIndexes => new Vector2(13, 21); // Y is max + 1
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

        protected Vector2 ResourceCell_Dimension => new Vector2(ResourceBar_Dimensions.X / 8, ResourceBar_Dimensions.Y);

        private string _baseString = "WORKERS: ";
        private string _baseString_PerTurnRevenue = "-000";
        private string _baseString_Header = "RESOURCE  COST LOSS GAIN";
        protected Vector2 Tooltip_Dimensions => new Vector2(_font.MeasureString(_baseString).X, (_font.MeasureString(_baseString).Y + 10) * 8);
        protected Vector2 ExtraTooltip_Dimensions => new Vector2(_font.MeasureString(_baseString_PerTurnRevenue).X, _font.MeasureString(_baseString_PerTurnRevenue).Y + 10);
        protected Texture2D Tooltip_Texture { get; set; }

        /// <summary>
        /// An int array containing the IDs for icons to represent inventory items in the HUD.
        /// In order: Gold, Wood, Coal, Iron, Food, Energy, Workers
        /// </summary>
        public int[] HUDIconIDs { get; set; }

        public HUD(GraphicsDevice graphicsDevice_, GameContent content_)
        {
            _graphicsDevice = graphicsDevice_;

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
                5,6,7,8,12,9,10,11
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
            Tooltip_Texture = new Texture2D(graphicsDevice_, (int)Tooltip_Dimensions.X, (int)Tooltip_Dimensions.Y);
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
            _currentMouse = Mouse.GetState();
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            var show_spec_resource_toolip = false;
            var show_spec_resource_tooltip_id = 0;

            spriteBatch.Draw(_texture, DisplayRect, Color.DarkKhaki);
            spriteBatch.Draw(_texture, BorderRect, Color.Black);

            spriteBatch.Draw(SelectionCellsSection_Texture, SelectionCellsSection_Rectangle, Color.DarkKhaki);
            spriteBatch.Draw(InfographicsSection_Texture, InfographicsSection_Rectangle, Color.DarkOliveGreen);
            spriteBatch.Draw(ResourceBar_Texture, ResourceBar_Rectangle, Color.DarkSlateGray);

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
                State.GSData.PlayerInventory.Stone,
                State.GSData.PlayerInventory.Food,
                State.GSData.PlayerInventory.Energy,
                State.GSData.PlayerInventory.Workers
            };

            var resource_string_vals = new string[]
            {
                "Gold", "Wood", "Coal", "Iron", "Stone", "Food", "Energy", "Workers"
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

                if (icon_rect.Contains(_currentMouse.Position))
                {
                    show_spec_resource_toolip = true;
                    show_spec_resource_tooltip_id = i;
                }

                var resource_text = $"{resource_vals[i]}";
                var resource_text_origin = new Vector2(_font.MeasureString(resource_text).X / 2, _font.MeasureString(resource_text).Y / 2);
                var resource_text_pos = icon_position + new Vector2(icon_rect.Width * 0.5f, -(icon_rect.Height * 0.25f));

                spriteBatch.DrawString(_font, resource_text, resource_text_pos, Color.White, 0.0f, resource_text_origin, 1.0f, SpriteEffects.None, 1.0f);
            }
            #endregion

            var show_tooltip = false;
            Button ref_b = null;

            foreach (Component c in _components)
            {
                c.Draw(gameTime, spriteBatch);
                if (c is Button b)
                {
                    if (b.IsHovering)
                    {
                        if (b.ResourceLocked)
                        {
                            if (BuildingData.Dict_BuildingKeys.ContainsKey(b.ID))
                            {
                                show_tooltip = true;
                                ref_b = b;
                            }
                        }
                    }
                }
            }

            if (show_tooltip)
            {
                var b = ref_b;
                var prev_inv = new Inventory()
                {
                    Gold = State.GSData.PlayerInventory.Gold,
                    Wood = State.GSData.PlayerInventory.Wood,
                    Coal = State.GSData.PlayerInventory.Coal,
                    Iron = State.GSData.PlayerInventory.Iron,
                    Stone = State.GSData.PlayerInventory.Stone,
                    Workers = State.GSData.PlayerInventory.Workers,
                    Energy = State.GSData.PlayerInventory.Energy,
                    Food = State.GSData.PlayerInventory.Food
                };
                var bld = BuildingData.Dict_BuildingKeys[b.ID];
                var cost_vals = new Dictionary<int, int>()
                                {
                                    {0, bld.GoldUpfront},
                                    {1, bld.WoodUpfront},
                                    {2, bld.CoalUpfront},
                                    {3, bld.IronUpfront},
                                    {4, bld.StoneUpfront },
                                    {5, bld.WorkersUpfront},
                                    {6, bld.EnergyUpfront},
                                    {7, bld.FoodUpfront}
                                };
                var per_turn_cost_vals = new Dictionary<int, int>()
                {
                    {0, bld.GoldCost},
                    {1, bld.WoodCost},
                    {2, bld.CoalCost},
                    {3, bld.IronCost},
                    {4, bld.StoneCost},
                    {5, bld.WorkersCost},
                    {6, bld.EnergyCost},
                    {7, bld.FoodCost}
                };
                var per_turn_gain_vals = new Dictionary<int, int>()
                {
                    {0, bld.GoldOutput},
                    {1, bld.WoodOutput},
                    {2, bld.CoalOutput},
                    {3, bld.IronOutput},
                    {4, bld.StoneOutput},
                    {5, bld.WorkersOutput},
                    {6, bld.EnergyOutput},
                    {7, bld.FoodOutput}
                };
                var str_vals = new Dictionary<int, string>()
                                {
                                    {0, "gold"},
                                    {1, "wood"},
                                    {2, "coal"},
                                    {3, "iron"},
                                    {4, "stone" },
                                    {5, "workers"},
                                    {6, "energy"},
                                    {7, "food"}
                                };
                var bool_vals = new Dictionary<int, bool>()
                                {
                                    {0, false},
                                    {1, false},
                                    {2, false},
                                    {3, false},
                                    {4, false},
                                    {5, false},
                                    {6, false},
                                    {7, false }
                                };
                foreach (var d in cost_vals)
                {
                    bool_vals[d.Key] = State.GSData.PlayerInventory.RequestResource(str_vals[d.Key], d.Value);
                }
                // reset player inventory
                State.GSData.PlayerInventory = prev_inv;

                // set dimensions of tooltip
                var tooltip_rect = new Rectangle((int)_currentMouse.X, (int)(_currentMouse.Y - Tooltip_Dimensions.Y),
                    (int)Tooltip_Dimensions.X, (int)Tooltip_Dimensions.Y);

                // re position the tooltip if it is showing outside of the window
                if ((_currentMouse.X + tooltip_rect.Width + (ExtraTooltip_Dimensions.X * 3)) > _graphicsDevice.Viewport.Width)
                {
                    tooltip_rect.X = (int)(_graphicsDevice.Viewport.Width - (tooltip_rect.Width + (ExtraTooltip_Dimensions.X * 3)));
                }
                else if ((_currentMouse.X + tooltip_rect.Width + (ExtraTooltip_Dimensions.X * 3)) < 0)
                {
                    tooltip_rect.X = 0;
                }
                if ((_currentMouse.Y - (tooltip_rect.Height + ExtraTooltip_Dimensions.Y)) < 0)
                {
                    tooltip_rect.Y = 0;
                }
                else if ((_currentMouse.Y - (tooltip_rect.Height + ExtraTooltip_Dimensions.Y)) > _graphicsDevice.Viewport.Height)
                {
                    tooltip_rect.Y = _graphicsDevice.Viewport.Height - tooltip_rect.Height;
                }

                // set color data of tooltip
                int res_strt = 0;
                var dta = new Color[(int)Tooltip_Dimensions.X * (int)Tooltip_Dimensions.Y];
                for (int o = 0; o < dta.Length; o++)
                {
                    dta[o] = Color.White;
                }
                Tooltip_Texture.SetData(dta);

                // draw tooltip
                spriteBatch.Draw(Tooltip_Texture, destinationRectangle: tooltip_rect, color: Color.LightSlateGray);

                // draw resource strings in tooltip
                var complete_str = "";
                foreach (var val in str_vals)
                {
                    var res_str = $"{val.Value}: ";
                    complete_str += res_str;

                    var res_pos = new Vector2(tooltip_rect.X, ((tooltip_rect.Y + Tooltip_Dimensions.Y) - ((_font.MeasureString(_baseString).Y + 10) * res_strt) - 5));
                    var origin = new Vector2(0, _font.MeasureString(res_str).Y);
                    spriteBatch.DrawString(_font, res_str, res_pos, (bool_vals[val.Key] ? Color.Black : Color.DarkRed), 0, origin, new Vector2(1, 1), SpriteEffects.None, 1);

                    // draw extra rec for display of per turn revenue for resource
                    var xtra_dimen = new Rectangle((int)tooltip_rect.X + (int)Tooltip_Dimensions.X, (int)((tooltip_rect.Y + tooltip_rect.Height) - (ExtraTooltip_Dimensions.Y * (res_strt + 1))), (int)ExtraTooltip_Dimensions.X * 3, (int)ExtraTooltip_Dimensions.Y);
                    spriteBatch.Draw(Tooltip_Texture, destinationRectangle: xtra_dimen, color: Color.LightGray);

                    string[] rev_strs = { $"-{cost_vals[val.Key]}", $"-{per_turn_cost_vals[val.Key]}", $"+{per_turn_gain_vals[val.Key]}" };
                    for (int i = 0; i < rev_strs.Length; i++)
                    {
                        var rev_str_pos = new Vector2(xtra_dimen.X + (i * (xtra_dimen.Width / 3)), res_pos.Y);
                        var rev_str_origin = new Vector2(0, _font.MeasureString(_baseString_PerTurnRevenue).Y);

                        var friendly_string = "";
                        var offset_cnt = _baseString_PerTurnRevenue.Length - rev_strs[i].Length;
                        for (int j = 0; j < offset_cnt; j++)
                        {
                            friendly_string += " ";
                        }
                        friendly_string += rev_strs[i];

                        spriteBatch.DrawString(_font, (rev_strs[i].Equals("-0") || rev_strs[i].Equals("+0") ? "" : friendly_string), rev_str_pos, (i.Equals(0) ? (bool_vals[val.Key] ? Color.Black : Color.Red) : (i.Equals(1) ? Color.Red : Color.Green)), 0, rev_str_origin, new Vector2(1, 1), SpriteEffects.None, 1);
                    }
                    res_strt++;
                }

                var xtra_tooltip_rect = new Rectangle((int)tooltip_rect.X, (int)((tooltip_rect.Y + tooltip_rect.Height) - (Tooltip_Dimensions.Y + ExtraTooltip_Dimensions.Y)),
                    (int)Tooltip_Dimensions.X + (int)(ExtraTooltip_Dimensions.X * 3), (int)ExtraTooltip_Dimensions.Y);
                spriteBatch.Draw(Tooltip_Texture, destinationRectangle: xtra_tooltip_rect, color: Color.DarkSlateGray);

                var head_str_pos = new Vector2(xtra_tooltip_rect.X, xtra_tooltip_rect.Y + xtra_tooltip_rect.Height);
                var head_str_origin = new Vector2(0, _font.MeasureString(_baseString_Header).Y);
                spriteBatch.DrawString(_font, _baseString_Header, head_str_pos, Color.White, 0f, head_str_origin, new Vector2(0.9f, 0.9f), SpriteEffects.None, 1);
            }

            if (show_spec_resource_toolip)
            {
                // build string
                var spec_res_tooltip_txt = $"{resource_string_vals[show_spec_resource_tooltip_id]}'s current amount is {resource_vals[show_spec_resource_tooltip_id]} ({CurrentIncomeReport.TotalGain[show_spec_resource_tooltip_id]}-{CurrentIncomeReport.TotalLoss[show_spec_resource_tooltip_id]})";

                // calculate dimensions
                var spec_res_tooltip_dimens = new Vector2(_font.MeasureString(spec_res_tooltip_txt).X, _font.MeasureString(spec_res_tooltip_txt).Y);
                var spec_res_tooltip_rect = new Rectangle((int)_currentMouse.X, (int)(_currentMouse.Y - spec_res_tooltip_dimens.Y),
                    (int)spec_res_tooltip_dimens.X, (int)spec_res_tooltip_dimens.Y);
                spec_res_tooltip_rect.X = _currentMouse.X - (spec_res_tooltip_rect.Width / 2);
                Vector2 spec_res_tooltip_txt_pos = new Vector2(spec_res_tooltip_rect.X + (spec_res_tooltip_rect.Width / 2), spec_res_tooltip_rect.Y + _font.MeasureString(spec_res_tooltip_txt).Y);

                // draw tooltip and string
                spriteBatch.Draw(Tooltip_Texture, destinationRectangle: spec_res_tooltip_rect, color: Color.LightGray);
                spriteBatch.DrawString(_font, spec_res_tooltip_txt, spec_res_tooltip_txt_pos, Color.Black, 0, new Vector2(_font.MeasureString(spec_res_tooltip_txt).X / 2, _font.MeasureString(spec_res_tooltip_txt).Y), new Vector2(0.9f, 0.9f), SpriteEffects.None, 1);
            }
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
