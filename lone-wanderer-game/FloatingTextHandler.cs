using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame
{
    public class FloatingText
    {
        public Vector2 position;
        public string text;
        public float timer = 1.0f;
        public SpriteFont font;
        public float alpha = 1.0f;

        public FloatingText(string text, Vector2 position, SpriteFont font)
        {
            this.text = text;
            this.position = position;
            this.font = font;
        }

        public void Update(GameTime gameTime)
        {
            timer -= gameTime.GetElapsedSeconds();
            position.Y -= gameTime.GetElapsedSeconds() * 100f;
            alpha -= gameTime.GetElapsedSeconds();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, position, Color.White, 0, Vector2.Zero, alpha, SpriteEffects.None, 1.0f);
        }
    }
    public class FloatingTextHandler
    {
        public List<FloatingText> texts;
        private Game1 game;

        public FloatingTextHandler(Game1 game)
        {
            this.game = game;
            texts = new List<FloatingText>();
        }
        public void Update(GameTime gameTime)
        {
            List<FloatingText> toRemove = new List<FloatingText>();
            foreach(FloatingText text in texts)
            {
                text.Update(gameTime);
                if (text.timer <= 0.0f)
                {
                    toRemove.Add(text);
                }
            }
            foreach (FloatingText text in toRemove)
            {
                texts.Remove(text);
            }
        }

        public void AddText(string text, Vector2 position)
        {
            this.texts.Add(new FloatingText(text, position, game.RegularFont));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(FloatingText text in texts)
            {
                text.Draw(spriteBatch);
            }
        }
    }
}
