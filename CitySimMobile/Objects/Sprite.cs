using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Graphics;

namespace CitySimMobile.Objects
{
    public class Sprite
    {
        // sprite's texture
        public Texture2D Texture
        {
            get { return _texture; }
        }

        private Texture2D _texture;

        // sprite's frametime (how long each frame)
        public float FrameTime
        {
            get { return _frameTime; }
        }

        private float _frameTime = 1f;

        // is the sprite looping?
        public bool IsLooping
        {
            get { return _isLooping; }
            set { _isLooping = value; }
        }

        private bool _isLooping = true;

        // is the sprite still (not animated)
        public bool IsStill
        {
            get { return _isStill; }
            set { _isStill = value; }
        }

        private bool _isStill = false;

        // is the sprite an animation ?
        public bool IsAnimation
        {
            get { return _isAnimation; }
            set { _isAnimation = value; }
        }

        private bool _isAnimation = true;

        // sprite's frame width - if an animation, return the height (width is multiple frames, height will be same as width for each frame)
        public int FrameWidth
        {
            get { return IsAnimation ? (CustomFrameWidth > 0 ? CustomFrameWidth : Texture.Height) : Texture.Width; }
        }

        public int CustomFrameWidth { get; set; } = 0;

        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        // return the frame count if an animation (width / framewidth = how many frames)
        public int FrameCount
        {
            get { return IsAnimation ? Texture.Width / FrameWidth : 0; }
        }

        // construct sprite
        public Sprite(Texture2D texture_, float frameTime_)
        {
            this._texture = texture_;
            this._frameTime = frameTime_;
        }

        // construct sprite (set animation to false using this constructor and not worry about frametime)
        public Sprite(Texture2D texture_, bool isAnimation_)
        {
            this._texture = texture_;
            this._isAnimation = isAnimation_;
            this._isStill = true;
        }
    }
}