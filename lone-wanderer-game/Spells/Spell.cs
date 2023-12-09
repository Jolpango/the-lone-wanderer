﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace LoneWandererGame.Spells
{
    public class Spell
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public string Asset { get; private set; }
        public Vector2 Position { get; protected set; }
        protected AnimatedSprite sprite;
        public int Damage { get; protected set; }
        public Spell(string name, string icon, string asset, Vector2 origin)
        {
            Name = name;
            Icon = icon;
            Position = origin;
            Asset = asset;
        }
        public virtual void LoadContent(ContentManager content)
        {
            var spriteSheet = content.Load<SpriteSheet>($"Sprites/SpellAnimations/{Asset}.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
        }
        public Rectangle CollisionRectangle
        { 
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 64, 64);
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position, 0, Vector2.One);
        }
    }
}