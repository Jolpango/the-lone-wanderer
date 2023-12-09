using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Sprites;
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
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
