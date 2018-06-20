using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace CitySim.Objects
{
    public class Sprite
    {
        public Texture2D Texture
        {
            get { return _texture; }
        }

        private Texture2D _texture;

        public float FrameTime
        {
            get { return _frameTime; }
        }

        private float _frameTime = 1f;

        public bool IsLooping
        {
            get { return _isLooping; }
            set { _isLooping = value; }
        }

        private bool _isLooping = true;

        public bool IsStill
        {
            get { return _isStill; }
            set { _isStill = value; }
        }

        private bool _isStill = false;

        public bool IsAnimation
        {
            get { return _isAnimation; }
            set { _isAnimation = value; }
        }

        private bool _isAnimation = true;

        public int FrameWidth
        {
            get { return IsAnimation ? Texture.Height : Texture.Width; }
        }

        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        public int FrameCount
        {
            get { return IsAnimation ? Texture.Width / FrameWidth : 0; }
        }

        public Sprite(Texture2D texture_, float frameTime_)
        {
            this._texture = texture_;
            this._frameTime = frameTime_;
        }

        public Sprite(Texture2D texture_, bool isAnimation_)
        {
            this._texture = texture_;
            this._isAnimation = isAnimation_;
        }
    }
}
