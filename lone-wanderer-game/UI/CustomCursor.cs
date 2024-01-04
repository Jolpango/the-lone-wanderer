using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace LoneWandererGame.UI
{
    public enum CursorState
    {
        pointer,
        select
    }
    public class CustomCursor
    {
        public Game1 Game { get; set; }
        private CursorState cursorState;
        public MouseStateExtended MouseState;
        public Vector2 Position { get; set; }
        public CursorState CursorState
        {
            get
            {
                return cursorState;
            }
            set
            {
                if (cursorState != value)
                {
                    sprite.Play(value.ToString());
                }
                cursorState = value;
            }
        }

        private AnimatedSprite sprite;
        public CustomCursor(Game1 game)
        {
            Game = game;
            cursorState = CursorState.pointer;
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 1, 1);
            }
        }

        public void LoadContent()
        {
            var spriteSheet = Game.Content.Load<SpriteSheet>($"Sprites/cursor.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
            sprite.Depth = 0.99f;
            sprite.Play(cursorState.ToString());
            sprite.Origin = new Vector2(8, 0);
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            MouseState = MouseExtended.GetState();
            Position = new Vector2(MouseState.X, MouseState.Y);
        }

        public void Draw()
        {
            sprite.Draw(Game.SpriteBatch, Position, 0, Vector2.One);
        }
    }
}
