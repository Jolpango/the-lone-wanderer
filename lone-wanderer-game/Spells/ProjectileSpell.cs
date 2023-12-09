﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            sprite.Play("flying");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = Position + direction * speed;
        }
    }
}
