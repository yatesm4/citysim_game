using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace CitySimAndroid.Content
{
    /// <summary>
    /// Class for storing dynamic content
    /// Uses t for generic type, types range from Texture2D to SoundEffect etc
    /// Uses Id to identify specific content data within lists
    /// Stores path for loading content from contentmanager
    /// Data is the actual content itself, loaded from the path. T is the generic type for the data
    /// </summary>
    /// <typeparam name="T">Generic Content Type</typeparam>
    public class ContentData<T>
    {
        public ContentData(int id, string path, ContentManager content)
        {
            Id = id;
            Path = path;
            Data = content.Load<T>(path);
        }

        public int Id { get; set; } = 0;
        public string Path { get; set; } = string.Empty;
        public T Data { get; set; } = default(T);

    }

    /// <summary>
    /// Content management class to store ContentData lists.
    /// Loads all textures needed within the game and can return specified content from lists
    /// </summary>
    public class GameContent
    {
        // where to load content from
        private ContentManager _content;

        // private lists to store contentdata
        private List<ContentData<Texture2D>> _uiTexturesList { get; set; } = new List<ContentData<Texture2D>>();
        private List<ContentData<SpriteFont>> _fontsList { get; set; } = new List<ContentData<SpriteFont>>();
        private List<ContentData<Texture2D>> _tileTexturesList { get; set; } = new List<ContentData<Texture2D>>();
        private List<ContentData<Song>> _soundEffectsList { get; set; } = new List<ContentData<Song>>();

        // list accessors
        public List<ContentData<Texture2D>> UiTextures
        {
            get { return _uiTexturesList; }
            set { _uiTexturesList = value; }
        }

        public List<ContentData<SpriteFont>> Fonts
        {
            get { return _fontsList; }
            set { _fontsList = value; }
        }

        public List<ContentData<Texture2D>> TileTextures
        {
            get { return _tileTexturesList; }
            set { _tileTexturesList = value; }
        }

        public List<ContentData<Song>> SoundEffects
        {
            get { return _soundEffectsList; }
            set { _soundEffectsList = value; }
        }

        // get specific content data based on an id from a content type category
        public Texture2D GetUiTexture(int id)
        {
            return (from a in UiTextures
                    where a.Id.Equals(id)
                    select a.Data).SingleOrDefault<Texture2D>();
        }

        public SpriteFont GetFont(int id)
        {
            return (from a in Fonts
                    where a.Id.Equals(id)
                    select a.Data).SingleOrDefault<SpriteFont>();
        }

        public Texture2D GetTileTexture(int id)
        {
            return (from a in TileTextures
                    where a.Id.Equals(id)
                    select a.Data).SingleOrDefault<Texture2D>();
        }

        public Song GetSoundEffect(int id)
        {
            return (from a in SoundEffects
                    where a.Id.Equals(id)
                    select a.Data).SingleOrDefault<Song>();
        }

        // constructor
        // takes a contentmanager as an arg
        // loads all textures within the game
        // TODO
        //  Pass id to only load specific textures (useful for menu state so it doesnt load game textures when it doesnt need to
        public GameContent(ContentManager content)
        {
            _content = content;

            LoadUITextures();
            LoadFonts();
            LoadTileTextures();
            LoadSoundEffects();
        }

        // load ui textures 
        public void LoadUITextures()
        {
            var i = 1;
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Arrow_Green", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Arrow_Black", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Cursor", _content));

            // inventory icons for hud
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Gold", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Wood", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Coal", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Iron", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Food", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Energy", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Workers", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Stone", _content));

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Blank", _content)); // 13
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_TownHall", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_House", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Farm", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Logs", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Ore", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Power", _content));
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Windmill", _content));// 20

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Small", _content)); // 21
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Small_Trash", _content)); // 22
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Delete", _content)); // 23

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_House_Med", _content)); // 24
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_House_Elite", _content)); // 25

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Small_House", _content)); // 26
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Small_Money", _content)); // 27
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Small_Tree", _content)); // 28

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Panel_ResourceBar", _content)); // 29

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Road_Left", _content)); // 30
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Road_Right", _content)); // 31
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_4_Way_Intersection", _content)); // 32
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_T_Intersection_1", _content)); // 33 
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_T_Intersection_2", _content)); // 34
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_T_Intersection_3", _content)); // 35
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_T_Intersection_4", _content)); // 36

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Panel_DisplayInfo", _content)); // 37
            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/UI_Button_Small_Close", _content)); // 38

            UiTextures.Add(new ContentData<Texture2D>(i++, "Sprites/UI/Icons/Icon_Cell_Watermill", _content)); // 39

            Log.Info("CitySim",  $"Ui Textures: {i}");
        }

        public void LoadFonts()
        {
            // total: 1
            var i = 1;
            Fonts.Add(new ContentData<SpriteFont>(i++, "Fonts/Font_01", _content));

            Log.Info("CitySim",  $"Fonts: {i}");
        }

        // load tile/tileset textures
        public void LoadTileTextures()
        {
            // Load Tile Effect Textures within negative range
            TileTextures.Add(new ContentData<Texture2D>(-1, "Sprites/Tiles/FX/Smoke/01-Anim", _content));

            // total: 12
            var i = 1;
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Grass", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Dirt", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Cement", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Water", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Stone", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Coal", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Iron", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Tree_Single_01", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Natural/Tree_Cluster_01", _content));

            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/TownHall/01", _content)); // 10
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/01", _content)); // 11
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Farm/01", _content));  // 12
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Farm/02", _content));  // 13
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Wood/01", _content));  // 14
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Ore/01", _content));   // 15
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Power/01", _content)); // 16
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Power/02", _content)); // 17

            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/02", _content)); // 18
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/03", _content)); // 19
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/04", _content)); // 20
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/05", _content)); // 21
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/06", _content)); // 22
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/07", _content)); // 23
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/08", _content)); // 24
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/09", _content)); // 25

            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/Left", _content)); // 26
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/Right", _content)); // 27
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/4_Way_Intersection", _content)); // 28
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/T_Intersection_1", _content)); // 29
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/T_Intersection_2", _content)); // 30
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/T_Intersection_3", _content)); // 31
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/T_Intersection_4", _content)); // 32
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/Corner_1", _content)); // 33
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/Corner_2", _content)); // 34
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/Corner_3", _content)); // 35 
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Roads/Corner_4", _content)); // 36

            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Watermill/01", _content)); // 37
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Watermill/01-Anim", _content)); // 38

            Log.Info("CitySim",  $"Tile Textures: {i}");
        }

        public Dictionary<int, int> Dict_CorrespondingAnimTextureID => new Dictionary<int, int>()
        {
            {37, 38}, // Watermill
        };

        // load sound effects
        public void LoadSoundEffects()
        {
            // total: 1
            var i = 1;
            SoundEffects.Add(new ContentData<Song>(i++, "Sounds/FX/Glimmer", _content));

            Log.Info("CitySim",  $"Sound Effects: {i}");
        }
    }
}