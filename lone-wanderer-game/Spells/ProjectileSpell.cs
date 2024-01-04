using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LoneWandererGame.Spells
{
    public class ProjectileSpell : Spell
    {
        private Vector2 direction;
        private float speed;
        public ProjectileSpell(string name, string icon, string asset, Vector2 origin, Vector2 direction, float speed)
            : base(name, icon, asset, origin)
        {
            this.direction = direction;
            this.speed = speed;
            this.Rotation = (float)(Math.Atan2(direction.Y, direction.X) / (2 * Math.PI));
        }

        public override Vector2 GetVelocity()
        {
            return speed * direction;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            sprite.Play("flying");
            SoundEffect.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = Position + direction * speed;
            if (ParticleEmitter is not null)
            {
                ParticleEmitter.Direction = direction;
                ParticleEmitter.Emit(Position, ParticleAmount ?? 1);
                ParticleEmitter.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Game1 game)
        {
            base.Draw(spriteBatch, game);
            if (ParticleEmitter is not null)
                ParticleEmitter.Draw(spriteBatch);
        }
    }
}
