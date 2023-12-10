

using LoneWandererGame.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LoneWandererGame.Spells
{
    public class MeleeSpell : Spell
    {
        public Vector2 Direction { get; set; }
        public Player Player { get; set; }
        public MeleeSpell(string name, string icon, string asset, Vector2 origin, Vector2 direction, Player player)
            : base(name, icon, asset, origin)
        {
            Direction = new Vector2(player.LastXDirection, 0);
            Direction.Normalize();
            Player = player;
        }
        public override void LoadContent(ContentManager content)
        {

            base.LoadContent(content);
            if (Direction.X > 0)
            {
                sprite.Play("right", () => Timer = -1);
            }
            else
            {
                sprite.Play("left", () => Timer = -1);
            }
            Position += Direction * sprite.GetBoundingRectangle(new MonoGame.Extended.Transform2()).Width / 2;
            SoundEffect.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = Player.Position;
            Position += Direction * sprite.GetBoundingRectangle(new MonoGame.Extended.Transform2()).Width / 2;
        }
    }
}
