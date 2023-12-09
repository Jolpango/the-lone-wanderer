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

        private AnimatedSprite sprite;

        public Vector2 position = new Vector2(0, 0);
        public float rotation = 0;
        public Vector2 scale = new Vector2(1, 1);

        public Player(Game1 game, Vector2 spawnPosition) {
            this.game = game;
            position = spawnPosition;
        }

        public void LoadContent()
        {
            var spriteSheet = game.Content.Load<SpriteSheet>("Sprites/player_animations.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);

            sprite.Play("idle_up_down");
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = dt * 128;
            var keyboardState = KeyboardExtended.GetState();
            var animation = "idle_up_down";
            SpriteEffects flip = SpriteEffects.None;
            Color colorTint = Color.White;

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

            sprite.Color = colorTint;
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
