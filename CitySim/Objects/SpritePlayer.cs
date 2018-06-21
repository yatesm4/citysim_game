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
        // sprite being played by this player
        public Sprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        private Sprite _sprite;

        // frame index of sprite being played
        public int FrameIndex
        {
            get { return _frameIndex; }
            set { _frameIndex = value; }
        }

        private int _frameIndex;

        // time of animation-playback
        private float _time;

        // sprite origin (render origin)
        public Vector2 Origin
        {
            get { return new Vector2(Sprite.FrameWidth / 2.0f, Sprite.FrameHeight); }
        }

        // custom origin (optional)
        public Vector2 CustomOrigin
        {
            get { return _customOrigin; }
            set { _customOrigin = value; }
        }

        private Vector2 _customOrigin;

        // sprite scale
        public float Scale { get; set; }


        // funtion to play sprite
        public void PlaySprite(Sprite sprite_)
        {
            // if argument sprite is already playing, do nothing
            if (Sprite == sprite_)
                return;

            // set sprite and reset frame index and playback time
            this._sprite = sprite_;
            this._frameIndex = 0;
            this._time = 0.0f;

            // set scale if nothing
            if (Scale.Equals(null) || Scale.Equals(0.0f))
            {
                Scale = 1.0f;
            }

        }

        // draw the sprite player
        public void Draw(GameTime gameTime_, SpriteBatch spriteBatch_, Vector2 position_, SpriteEffects spriteEffects_)
        {
            // if no sprite
            if (_sprite is null)
                throw new NotSupportedException("No sprite is loaded -> no sprite can be played.");

            // get current time of playback
            _time += (float) gameTime_.ElapsedGameTime.TotalSeconds;

            // while time is less than the time in a frame
            while (_time > Sprite.FrameTime)
            {
                // essentially, we are actually increasing the timer but the logic is reverse for this math
                _time -= Sprite.FrameTime;

                // if sprite isnt still
                if (Sprite.IsStill is false)
                {
                    // if sprite is looping
                    if (Sprite.IsLooping)
                    {
                        // go to next frame (or first frame)
                        _frameIndex = (_frameIndex + 1) % Sprite.FrameCount;
                    }
                    else
                    {
                        // if not looping, go till last frame
                        FrameIndex = Math.Min(_frameIndex + 1, Sprite.FrameCount - 1);
                    }
                }
            }

            // calculate source rectangle (area in spritesheet) to render based on frame index and frame dimensions
            Rectangle source = new Rectangle(FrameIndex * Sprite.Texture.Height, 0, Sprite.Texture.Height, Sprite.Texture.Height);

            // draw the sprite with the according properties
            spriteBatch_.Draw(Sprite.Texture, position_, source, Color.White, 0.0f, (CustomOrigin.Equals(null)||CustomOrigin.Equals(Vector2.Zero)) ? Origin : CustomOrigin, Scale, spriteEffects_, 0.0f);
        }
    }
}
