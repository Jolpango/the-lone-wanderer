using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System.Diagnostics;
using LoneWandererGame.TileEngines;
using System.Collections.Generic;

namespace LoneWandererGame.Entity
{
    public class Player
    {
        private Game1 game;
        private TileEngine tileEngine;
        private AnimatedSprite sprite;
        private Color colorTint = Color.White;


        public enum AnimationState
        {
            none,
            idle_up_down,
            idle_left_right,
            run_up_down,
            run_left_right,
            death
        };
        private AnimationState lastAnimation;
        private SpriteEffects lastSpriteEffect;


        private Vector2 direction = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;

        public float acceleration = 1.6f;
        public float decceleration = 0.75f;
        public Vector2 Direction { get { return direction; } }
        public Vector2 Position = Vector2.Zero;
        public float Rotation = 0;
        public Vector2 Scale = new Vector2(1, 1);

        public enum State {
            Alive,
            Dead
        };
        private State state;
        private float damageTimer = 0f;
        private const float INVINCIBILITY_TIME = 0.3f;
        public int Health { get; private set; }
        public const int MAX_HEALTH = 100;

        public void Damage(int amount)
        {
            if (state == State.Alive && damageTimer == 0f)
            {
                damageTimer = INVINCIBILITY_TIME;
                Health -= amount;
                colorTint = Color.Red;

                Debug.WriteLine("Player damaged, health: " + Health.ToString());

                if (Health < 0)
                {
                    Health = 0;
                }
            }
        }
        

        public Player(Game1 game, TileEngine tileEngine, Vector2 spawnPosition) {
            this.game = game;
            lastSpriteEffect = SpriteEffects.None;
            lastAnimation = AnimationState.idle_up_down;
            
            this.tileEngine = tileEngine;
            Position = spawnPosition;
            Health = MAX_HEALTH;
            state = State.Alive;
        }
        public Rectangle getSpriteRectangle()
        {
            return (Rectangle)sprite.GetBoundingRectangle(Position, Rotation, Scale);
        }

        public void LoadContent()
        {
            var spriteSheet = game.Content.Load<SpriteSheet>("Sprites/player_animations.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
            sprite.Origin = new Vector2(16, 16);
            sprite.Depth = 0.25f;
            sprite.Play(lastAnimation.ToString());
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = KeyboardExtended.GetState();

            // This makes sure we keep the sprites facing direction while idle
            AnimationState animation = AnimationState.idle_up_down;
            SpriteEffects spriteEffect = lastSpriteEffect;

            if (keyboardState.IsKeyDown(Keys.T) && state == State.Dead)
            {
                Health = MAX_HEALTH;
                state = State.Alive;
                Debug.WriteLine("Player Revived!");
            }

            if (Health > 0)
            {
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    Damage(20);
                }
                

                switch (lastAnimation)
                {
                    case AnimationState.idle_left_right:
                    case AnimationState.run_left_right:
                        animation = AnimationState.idle_left_right;
                        break;
                }

                if (damageTimer > 0f)
                {
                    damageTimer -= dt;
                    if (damageTimer < 0f)
                    {
                        damageTimer = 0;
                        colorTint = Color.White;
                    }
                }

                // Movement Input
                Vector2 movementDirection = Vector2.Zero;
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    animation = AnimationState.run_up_down;
                    spriteEffect = SpriteEffects.None;
                    movementDirection.Y = -1;
                }
                else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    animation = AnimationState.run_up_down;
                    spriteEffect = SpriteEffects.None;
                    movementDirection.Y = 1;
                }

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    animation = AnimationState.run_left_right;
                    spriteEffect = SpriteEffects.None;
                    movementDirection.X = -1;
                }
                else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    animation = AnimationState.run_left_right;
                    spriteEffect = SpriteEffects.FlipHorizontally;
                    movementDirection.X = 1;
                }

                // Velocity and Direction
                if (movementDirection != Vector2.Zero)
                {
                    movementDirection.Normalize();
                    direction = movementDirection;
                    velocity += direction * acceleration;
                }

                RectangleF playerRect = sprite.GetBoundingRectangle(Position, Rotation, Scale); // This might break if we scale/rotate
                List<Rectangle> collisions = tileEngine.GetCollisions(playerRect, velocity);
                if (collisions.Count > 0)
                {
                    TileEngine.CollisionResolution resolution = tileEngine
                        .ResolveCollisions(collisions, playerRect, Position, sprite.Origin, velocity);

                    Position = resolution.position;
                    velocity = resolution.velocity;
                }

                sprite.Play(animation.ToString());
            }
            else
            {
                if (state == State.Alive)
                {
                    state = State.Dead;
                    colorTint = Color.White;
                    velocity = Vector2.Zero;
                    sprite.Play(AnimationState.death.ToString());
                    Debug.WriteLine("Player Died!");
                }
            }

            Position += velocity;
            velocity *= decceleration;

            // Sprite
            sprite.Color = colorTint;
            sprite.Effect = spriteEffect;
            sprite.Update(dt);

            lastAnimation = animation;
            lastSpriteEffect = spriteEffect;
        }

        public void Draw()
        {
            game.SpriteBatch.Draw(sprite, Position, Rotation, Scale);
        }
    }
}
