using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace LoneWandererGame.Spells
{
    public class PiercingSpell : Spell
    {
        private Vector2 direction;
        private float speed;
        public PiercingSpell(string name, string icon, string asset, Vector2 origin, Vector2 direction, float speed)
            : base(name, icon, asset, origin)
        {
            this.direction = direction;
            this.speed = speed;
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
        }
    }
}
