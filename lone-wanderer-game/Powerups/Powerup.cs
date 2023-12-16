using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace LoneWandererGame.Powerups
{
    public class Powerup
    {
        private Game1 game;
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Rectangle SourceRect { get; private set; }
        private AnimatedSprite sprite;
        private string color;
        public float ActiveTimer { get; private set; }
        public bool Active { get; set; }
        public int EffectAmount { get; set; }
        public int LightIndex { get; set; }

        public Powerup() {}

        public Powerup(Game1 game, Vector2 position, SpriteSheet spriteSheet, string color, float activeTime, int lightIndex = -1)
        {
            this.game = game;
            Position = position;
            this.color = color;

            sprite = new AnimatedSprite(spriteSheet);
            sprite.Depth = 0.25f;
            sprite.Play("animation0");

            ActiveTimer = activeTime;
            Active = false;
            EffectAmount = 0;
            LightIndex = lightIndex;
        }

        public RectangleF CollisionRectangle
        {
            get
            {
                return sprite.GetBoundingRectangle(Position, 0f, Vector2.One);
            }
        }
        public string Color
        {
            get
            {
                return color;
            }
        }

        public void LoadContent() {}

        public void Update(GameTime gameTime)
        {
            if (Active && ActiveTimer > 0.0f)
                ActiveTimer -= gameTime.GetElapsedSeconds();

            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Game1 game)
        {
            sprite.Draw(spriteBatch, Position, 0f, Vector2.One);
        }
    }
}
