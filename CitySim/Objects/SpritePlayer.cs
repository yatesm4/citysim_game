using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CitySim.Objects
{
    public struct SpritePlayer
    {
        public Sprite Sprite
        {
            get { return _sprite; }
        }

        private Sprite _sprite;

        public int FrameIndex
        {
            get { return _frameIndex; }
            set { _frameIndex = value; }
        }

        private int _frameIndex;

        private float _time;

        public Vector2 Origin
        {
            get { return new Vector2(Sprite.FrameWidth / 2.0f, Sprite.FrameHeight); }
        }

        public Vector2 CustomOrigin
        {
            get { return _customOrigin; }
            set { _customOrigin = value; }
        }

        private Vector2 _customOrigin;

        public float Scale { get; set; }

        public void PlaySprite(Sprite sprite_)
        {
            if (Sprite == sprite_)
                return;

            this._sprite = sprite_;
            this._frameIndex = 0;
            this._time = 0.0f;

            if (Scale.Equals(null) || Scale.Equals(0.0f))
            {
                Scale = 1.0f;
            }

        }

        public void Draw(GameTime gameTime_, SpriteBatch spriteBatch_, Vector2 position_, SpriteEffects spriteEffects_)
        {
            if (Sprite is null)
                throw new NotSupportedException("No sprite is loaded -> no sprite can be played.");

            _time += (float) gameTime_.ElapsedGameTime.TotalSeconds;
            while (_time > Sprite.FrameTime)
            {
                _time -= Sprite.FrameTime;

                if (Sprite.IsStill is false)
                {
                    if (Sprite.IsLooping)
                    {
                        _frameIndex = (_frameIndex + 1) % Sprite.FrameCount;
                    }
                    else
                    {
                        FrameIndex = Math.Min(_frameIndex + 1, Sprite.FrameCount - 1);
                    }
                }
            }

            Rectangle source = new Rectangle(FrameIndex * Sprite.Texture.Height, 0, Sprite.Texture.Height, Sprite.Texture.Height);

            spriteBatch_.Draw(Sprite.Texture, position_, source, Color.White, 0.0f, (CustomOrigin.Equals(null)||CustomOrigin.Equals(Vector2.Zero)) ? Origin : CustomOrigin, Scale, spriteEffects_, 0.0f);
        }
    }
}
