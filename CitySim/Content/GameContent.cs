using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CitySim.Content
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
        private List<ContentData<SoundEffect>> _soundEffectsList { get; set; } = new List<ContentData<SoundEffect>>();

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

        public List<ContentData<SoundEffect>> SoundEffects
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

        public SoundEffect GetSoundEffect(int id)
        {
            return (from a in SoundEffects
                where a.Id.Equals(id)
                select a.Data).SingleOrDefault<SoundEffect>();
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


            Console.WriteLine($"Ui Textures: {i}");
        }

        public void LoadFonts()
        {
            // total: 1
            var i = 1;
            Fonts.Add(new ContentData<SpriteFont>(i++, "Fonts/Font_01", _content));

            Console.WriteLine($"Fonts: {i}");
        }

        // load tile/tileset textures
        public void LoadTileTextures()
        {
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

            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/TownHall/01", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/House/01", _content));
            TileTextures.Add(new ContentData<Texture2D>(i++, "Sprites/Tiles/Buildings/Wood/01", _content));

            Console.WriteLine($"Tile Textures: {i}");
        }

        // load sound effects
        public void LoadSoundEffects()
        {
            // total: 1
            var i = 1;
            //SoundEffects.Add(new ContentData<SoundEffect>(i++, "Sounds/Effects/footstep", _content));

            Console.WriteLine($"Sound Effects: {i}");
        }
    }

}
