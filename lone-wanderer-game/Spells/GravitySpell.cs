

using LoneWandererGame.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;

namespace LoneWandererGame.Spells
{
    public class GravitySpell : Spell
    {
        public Vector2 Direction { get; set; }
        public Player Player { get; set; }
        public Vector2 Velocity;
        public float Force;
        public GravitySpell(string name, string icon, string asset, Vector2 origin, Player player, float speed, float forceMulti)
            : base(name, icon, asset, origin)
        {
            Direction = new Vector2(player.LastXDirection, 0);
            Direction.Normalize();
            Player = player;
            Force = speed * forceMulti;
        }
        public override void LoadContent(ContentManager content)
        {

            base.LoadContent(content);
            sprite.Play("flying");
            Direction = new Vector2(Direction.X, -3);
            SoundEffect.Play();
            Velocity = Direction * Force;
        }

        public override void Update(GameTime gameTime)
        {
            Velocity.Y *= 0.98f;
            Velocity.X *= 0.99f;
            Velocity.Y += gameTime.GetElapsedSeconds() * 20;
            Position += Velocity;
            base.Update(gameTime);
        }
    }
}
