using LoneWandererGame.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace LoneWandererGame
{
    public class SpellSelection
    {
        public SpellDefinition SpellDefinition { get; set; }
        public Vector2 Position { get; set; }
        private Texture2D texture;
        private Texture2D selectionBox;
        public Color Color = Color.White;
        public bool HasSpell = false;
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            }
        }
        public SpellSelection() { }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>($"Sprites/SpellIcons/{SpellDefinition.Icon}");
            selectionBox = content.Load<Texture2D>($"Sprites/spellselectionbox");
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Vector2 boxPos = new Vector2(Position.X - selectionBox.Width / 4, Position.Y - 50);
            spriteBatch.Draw(selectionBox, boxPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            drawStrings(spriteBatch, font);
        }

        private void drawStrings(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (HasSpell)
            {
                spriteBatch.DrawString(font, $"{SpellDefinition.Name} {SpellDefinition.CurrentLevel + 2}", new Vector2(Position.X, Position.Y + texture.Height), Color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                spriteBatch.DrawString(font, $"Multiplier:{SpellDefinition.LevelDefinitions[SpellDefinition.CurrentLevel].SpecialMultiplier} -> {SpellDefinition.LevelDefinitions[SpellDefinition.CurrentLevel + 1].SpecialMultiplier}", new Vector2(Position.X, Position.Y + texture.Height + 20), Color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                spriteBatch.DrawString(font, $"Damage:{SpellDefinition.LevelDefinitions[SpellDefinition.CurrentLevel].Damage} -> {SpellDefinition.LevelDefinitions[SpellDefinition.CurrentLevel + 1].Damage}", new Vector2(Position.X, Position.Y + texture.Height + 40), Color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            }
            else
            {
                spriteBatch.DrawString(font, $"{SpellDefinition.Name} {SpellDefinition.CurrentLevel + 1}", new Vector2(Position.X, Position.Y + texture.Height), Color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                spriteBatch.DrawString(font, $"Multiplier:{SpellDefinition.LevelDefinitions[SpellDefinition.CurrentLevel].SpecialMultiplier}", new Vector2(Position.X, Position.Y + texture.Height + 20), Color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                spriteBatch.DrawString(font, $"Damage:{SpellDefinition.LevelDefinitions[SpellDefinition.CurrentLevel].Damage}", new Vector2(Position.X, Position.Y + texture.Height + 40), Color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            }
        }
    }
}
