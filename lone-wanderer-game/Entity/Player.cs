﻿using Microsoft.Xna.Framework;
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

        public bool godMode = false;
        private float godModeButtonTimer = 0f;

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


        private Vector2 direction = new Vector2(0, 1);
        private Vector2 velocity = Vector2.Zero;

        public float LastXDirection { get; private set; } = 1f;

        public float acceleration = 1f;
        public float decceleration = 0.5f;
        public Vector2 Direction { get { return direction; } }
        public Vector2 Position = Vector2.Zero;
        public float Rotation = 0;
        public Vector2 Scale = new Vector2(1, 1);

        private int lightIndex;

        public enum State
        {
            Alive,
            Dying,
            Dead
        };
        private State state;
        private float damageTimer = 0f;
        private float damagedEffectTimer = 0f;
        private const float DAMAGED_EFFECT_TIME = 0.3f;
        public int Health { get; private set; }
        public int MAX_HEALTH = 100;
        public string Name { get; private set; } = "";

        public void Damage(int amount)
        {
            if (!godMode && state == State.Alive && damageTimer == 0f)
            {
                damagedEffectTimer = DAMAGED_EFFECT_TIME;
                Health -= amount;
                colorTint = Color.Red;

                Debug.WriteLine("Player damaged, health: " + Health.ToString());

                if (Health < 0)
                {
                    Health = 0;
                }
            }
        }
        public bool isDead()
        {
            return state == State.Dead;
        }

        public Player(Game1 game, TileEngine tileEngine, Vector2 spawnPosition)
        {
            this.game = game;
            lastSpriteEffect = SpriteEffects.None;
            lastAnimation = AnimationState.idle_up_down;

            this.tileEngine = tileEngine;
            Position = spawnPosition;
            Health = MAX_HEALTH;
            state = State.Alive;
            Name = "admin";

            lightIndex = game.LightHandler.AddLight(Position, new Vector2(700), Color.White, 0.6f);
        }
        public RectangleF getSpriteRectangle()
        {
            return sprite.GetBoundingRectangle(Position, Rotation, Scale);
        }

        private void OnDeathAnimationDone()
        {
            if (state == State.Dying)
            {
                state = State.Dead;
                Debug.WriteLine("Player Died!");
            }
        }

        public void LoadContent()
        {
            var spriteSheet = game.Content.Load<SpriteSheet>("Sprites/player_animations.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
            sprite.Origin = new Vector2(16, 16);
            sprite.Depth = 0.28f;
            sprite.Play(lastAnimation.ToString());
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = KeyboardExtended.GetState();

            // This makes sure we keep the sprites facing direction while idle
            AnimationState animation = AnimationState.idle_up_down;
            SpriteEffects spriteEffect = lastSpriteEffect;

            if (keyboardState.IsKeyDown(Keys.T) && (state == State.Dying || state == State.Dead))
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

                if (keyboardState.IsKeyDown(Keys.L) && godModeButtonTimer == 0f)
                {
                    godMode = !godMode;
                    godModeButtonTimer = 0.3f;
                }
                if (godModeButtonTimer > 0f)
                {
                    godModeButtonTimer -= dt;
                    if (godModeButtonTimer < 0f)
                    {
                        godModeButtonTimer = 0f;
                    }
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
                if (damagedEffectTimer > 0f)
                {
                    damagedEffectTimer -= dt;
                    if (damagedEffectTimer < 0f)
                    {
                        damagedEffectTimer = 0f;
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
                    LastXDirection = -1;
                }
                else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    animation = AnimationState.run_left_right;
                    spriteEffect = SpriteEffects.FlipHorizontally;
                    movementDirection.X = 1;
                    LastXDirection = 1;
                }

                // Velocity and Direction
                if (movementDirection != Vector2.Zero)
                {
                    movementDirection.Normalize();
                    direction = movementDirection;
                    float accel = acceleration;
                    if (godMode)
                    {
                        accel *= 3f;
                    }
                    velocity += direction * accel;
                }

                if (!godMode)
                {
                    RectangleF playerRect = sprite.GetBoundingRectangle(Position, Rotation, Scale); // This might break if we scale/rotate
                    List<Rectangle> collisions = tileEngine.GetCollisions(playerRect, velocity);
                    if (collisions.Count > 0)
                    {
                        TileEngine.CollisionResolution resolution = tileEngine
                            .ResolveCollisions(collisions, playerRect, Position, sprite.Origin, velocity);

                        Position = resolution.position;
                        velocity = resolution.velocity;
                    }
                }

                sprite.Play(animation.ToString());
            }
            else
            {
                if (state == State.Alive)
                {
                    state = State.Dying;
                    colorTint = Color.White;
                    velocity = Vector2.Zero;
                    sprite.Play(AnimationState.death.ToString(), OnDeathAnimationDone);
                    Debug.WriteLine("Player Dying!");
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
            game.LightHandler.updatePosition(lightIndex, Position);
        }

        public void Draw()
        {
#if DEBUG
            game.SpriteBatch.DrawRectangle(sprite.GetBoundingRectangle(Position, 0, Vector2.One), Color.Red * 0.8f, 1, 0.9f);
#endif
            game.SpriteBatch.Draw(sprite, Position, Rotation, Scale);
        }

        public void Heal(int amount)
        {
            Health += amount;
            if (Health >= MAX_HEALTH)
                Health = MAX_HEALTH;
        }
        public void IncreaseMaxHealthAdd(int amount)
        {
            Health += amount;
            MAX_HEALTH += amount;
        }

        public void IncreaseMaxHealthMultiply(float amount)
        {
            float tempHealth = Health * amount;
            Health = (int)(tempHealth + 0.5);

            float tempMaxHealth = MAX_HEALTH * amount;
            MAX_HEALTH = (int)(tempMaxHealth + 0.5);
        }

        public void SpeedUp(int amount)
        {
            float percentage = 1 + amount / 100f;
            acceleration *= percentage;
        }
        public float getAcceleration() { return acceleration; }

        //Gets the next acceleration that the player will get x.xx
        public float getAccelerationNextLevel(int amount)
        {
            float tempAcceleration = acceleration;
            float percentage = 1 + amount / 100f;
            tempAcceleration *= percentage;
            tempAcceleration = tempAcceleration * 100.0f / 100;
            return tempAcceleration;
        }

        public void SlowDown(int amount)
        {
            float percentage = 1 + amount / 100f;
            acceleration /= percentage;
        }

        public void MakeInvincible(int amount)
        {
            damageTimer = amount;
            colorTint = Color.Gray;
        }
    }
}
