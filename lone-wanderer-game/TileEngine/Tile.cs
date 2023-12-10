using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoneWandererGame.TileEngines
{
    public class Tile
    {
        private Game1 game;
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Rectangle SourceRect { get; private set; }
        public float Depth { get; private set; }
        public bool Empty { get; private set; }

        public Tile()
        {
            Empty = true;
        }

        public Tile(Game1 game, Texture2D texture, Vector2 position, Rectangle sourceRect, float depth)
        {
            this.game = game;
            Texture = texture;
            Position = position;
            SourceRect = sourceRect;
            Depth = depth;
            Empty = false;
        }

        public void Draw(Vector2 cameraPosition)
        {
            Vector2 diff = cameraPosition - Position;
            float l = diff.Length();
            if (l < 500)
                game.SpriteBatch.Draw(Texture, Position, SourceRect, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, Depth);
        }

        public Rectangle GetCollisionRectangle()
        {
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                SourceRect.Width,
                SourceRect.Height
            );
        }
    }
}
