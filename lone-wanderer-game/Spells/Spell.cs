using LoneWandererGame.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System.Collections.Generic;

namespace LoneWandererGame.Spells
{
    public class Spell
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public string Asset { get; private set; }
        public string Sound { get; set; }
        public float Rotation { get; set; } = 0;
        public float Timer { get ; set; }
        public Vector2 Position { get; protected set; }
        protected AnimatedSprite sprite;
        public int Damage { get; set; }
        public List<BaseEnemy> HitEnemies { get; set; }
        public SoundEffect SoundEffect { get; set; }
        public Spell(string name, string icon, string asset, Vector2 origin)
        {
            Name = name;
            Icon = icon;
            Position = origin;
            Asset = asset;
            HitEnemies = new List<BaseEnemy>();
        }

        public virtual void LoadContent(ContentManager content)
        {
            var spriteSheet = content.Load<SpriteSheet>($"Sprites/SpellAnimations/{Asset}.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
            sprite.Depth = 0.26f;
            SoundEffect = content.Load<SoundEffect>($"Sounds/{Sound}");
        }
        public RectangleF CollisionRectangle
        { 
            get
            {
                return sprite.GetBoundingRectangle(Position, 0f, Vector2.One);
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            Timer -= gameTime.GetElapsedSeconds();
        } 
        public virtual void Draw(SpriteBatch spriteBatch, Game1 game)
        {
            //RectangleF playerRect = CollisionRectangle;
            //Texture2D tempTexture = new Texture2D(game.GraphicsDevice, (int)playerRect.Width, (int)playerRect.Height);
            //Color[] data = new Color[(int)playerRect.Width * (int)playerRect.Height];
            //for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
            //    tempTexture.SetData(data);
            //spriteBatch.Draw(tempTexture, new Vector2(CollisionRectangle.X, CollisionRectangle.Y), null, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.76f);
            sprite.Draw(spriteBatch, Position, Rotation, Vector2.One);
        }
    }
}
