using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace LoneWandererGame.Entity
{
    class Player
    {
        private Game1 game;

        private Vector2 position = new Vector2(0, 0);
        private float rotation = 0;
        private Vector2 scale = new Vector2(1, 1);
        private AnimatedSprite sprite;

        public Player(Game1 game) { this.game = game; }

        public void LoadContent()
        {
            var spriteSheet = game.Content.Load<SpriteSheet>("Sprites/player_animations.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);

            sprite.Play("idle_up_down");
            position = new Vector2(100, 100);
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = dt * 128;
            var keyboardState = KeyboardExtended.GetState();
            var animation = "idle_up_down";
            SpriteEffects flip = SpriteEffects.None;
            Color colorTint = new Color(1, 1, 1, 1);

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                animation = "run_up_down";
                position.Y -= walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                animation = "run_up_down";
                position.Y += walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                animation = "run_left_right";
                position.X -= walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                animation = "run_left_right";
                position.X += walkSpeed;
                flip = SpriteEffects.FlipHorizontally;
            }

            sprite.Effect = flip;
            sprite.Play(animation);
            sprite.Update(dt);
        }

        public void Draw()
        {
            game.SpriteBatch.Draw(sprite, position);
        }
    }
}
