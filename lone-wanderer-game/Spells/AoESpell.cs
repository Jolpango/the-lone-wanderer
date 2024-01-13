using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.Spells
{
    public class AoESpell : Spell
    {
        public AoESpell(string name, string icon, string asset, Vector2 origin)
            : base(name, icon, asset, origin) { }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            sprite.Play("attack", () => Timer = -1);
            SoundEffect.Play();
            if (ParticleEmitter is not null)
            {
                ParticleEmitter.Emit(Position, ParticleAmount ?? 1);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Game1 game)
        {
            if (ParticleEmitter is not null)
                ParticleEmitter.Draw(spriteBatch);
#if DEBUG
            spriteBatch.DrawRectangle(sprite.GetBoundingRectangle(Position, 0, Vector2.One), Color.Red, 1, 1);
#endif
        }
    }
}
