using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using LoneWandererGame.Entity;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using LoneWandererGame.TileEngines;
using Microsoft.Xna.Framework.Audio;

namespace LoneWandererGame.Enemy
{
    public class BaseEnemy
    { 

        private float moveSpeed;
        private Vector2 position= new Vector2(0.0f,0.0f);
        private float attackCooldown = 0.0f;
        private TileEngine tileEngine;
        private SoundEffect hitSound;
        private int damage;
        //animation
        private float distanceToPlayerStop = 30.0f; 
        

        //Animation
        private AnimatedSprite sprite;
        public enum AnimationState
        {
            none,
            idle_Up,
            idle_Left,
            idle_Down,
            idle_Right,
            walk_Up,
            walk_Left,
            walk_Down,
            walk_Right
        };
        public float rotation = 0;
        public Vector2 scale;
        private AnimationState lastAnimation;
        private SpriteEffects lastSpriteEffect;
        private Vector2 direction = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;
        private FillableBar healthBar;

        public Game1 Game { get; private set; }
        public float health { get; private set; }
        String spriteName;

        public RectangleF CollisionRectangle
        {
            get
            {
                return sprite.GetBoundingRectangle(position, rotation, scale);
            }
        }

        public BaseEnemy(float health, float moveSpeed, Vector2 position, Game1 game, TileEngine tileEngine, 
            string spriteName, float scaleX=1, float scaleY =1, int damage = 5)
        {
            this.health = health;
            this.moveSpeed = moveSpeed;
            this.position = position;
            Game = game;
            this.spriteName = spriteName;
            this.tileEngine = tileEngine;
            this.scale = new Vector2(scaleX, scaleY);
            this.damage = damage;
        }
        public void LoadContent()
        {
            var spriteSheet = Game.Content.Load<SpriteSheet>(spriteName, new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
            sprite.Origin = new Vector2(24, 32); // hardcoded, should be calculated but I'm too lazy
            sprite.Depth = 0.25f;

            lastSpriteEffect = SpriteEffects.None;
            lastAnimation = AnimationState.idle_Down;
            sprite.Play(lastAnimation.ToString());
            healthBar = new FillableBar()
            {
                MaxValue = (int)health,
                CurrentValue = (int)health,
                BarHeight = 4,
                BarWidth = 40,
                Game = Game,
                Position = position
            };
            healthBar.CreateTexture();
            hitSound = Game.Content.Load<SoundEffect>("Sounds/hit");
        }

        public void Update(GameTime gameTime, Player _player)
        {
            //Movement
            Vector2 direction = _player.Position - position - new Vector2(20.0f,20.0f); // this makes it in middle of player
            Vector2 distanceToPlayer = direction;
            if (direction != Vector2.Zero)
                direction.Normalize();

            velocity = Vector2.Zero;
            if (distanceToPlayer.LengthSquared() > distanceToPlayerStop)
            {
                velocity = direction * moveSpeed * gameTime.GetElapsedSeconds();
                List<Rectangle> collisions = tileEngine.GetCollisions(CollisionRectangle, velocity);
                {
                    if (collisions.Count > 0)
                    {
                        TileEngine.CollisionResolution resolution = tileEngine
                            .ResolveCollisions(collisions, CollisionRectangle, position, sprite.Origin, velocity);

                        position = resolution.position;
                        velocity = resolution.velocity;
                    }
                }
            }

            position += velocity;

            healthBar.CurrentValue = (int)health;
            healthBar.Position = new Vector2(position.X, position.Y - sprite.Origin.Y);

            //Animation
            animation(gameTime, direction, distanceToPlayer);


            if (attackCooldown <= 0.0f)
            {
                RectangleF playerbox = _player.getSpriteRectangle();
                RectangleF enemyBox = sprite.GetBoundingRectangle(position, rotation, scale);

                if (playerbox.Intersects(enemyBox))
                {
                    // TODO take damage on player here
                    _player.Damage(this.damage);
                    attackCooldown = 1.0f;
                }
                
            }    
            attackCooldown-= gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
            //RectangleF playerRect = CollisionRectangle;
            //Texture2D tempTexture = new Texture2D(Game.GraphicsDevice, (int)playerRect.Width, (int)playerRect.Height);
            //Color[] data = new Color[(int)playerRect.Width * (int)playerRect.Height];
            //for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;
            //    tempTexture.SetData(data);
            //Game.SpriteBatch.Draw(tempTexture, new Vector2(playerRect.X, playerRect.Y), null, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.76f);
            Game.SpriteBatch.Draw(sprite, position, rotation, scale);
            healthBar.Draw();
        }
        public bool TakeDamage(float damage)
        {
            health -= damage;
            return health > 0;
        }
        public bool IsDead()
        {   
            return health <= 0;
        }
        public Vector2 getPos() { return position; }
        private void animation(GameTime gameTime, Vector2 direction, Vector2 distanceToPlayer)
        {
            AnimationState animation = AnimationState.idle_Up; // idk
            switch (lastAnimation)
            {
                case AnimationState.walk_Up:
                    animation = AnimationState.idle_Up;
                    break;
                case AnimationState.walk_Left:
                    animation = AnimationState.idle_Left;
                    break;
                case AnimationState.walk_Down:
                    animation = AnimationState.idle_Down;
                    break;
                case AnimationState.walk_Right:
                    animation = AnimationState.idle_Right;
                    break;

            }
            SpriteEffects spriteEffect = lastSpriteEffect;
            Color colorTint = Color.White;
            if (direction.X >= 0.5f) { animation = AnimationState.walk_Right; }
            else if (direction.X <= -0.5f) { animation = AnimationState.walk_Left; }
            else if (direction.Y <= -0.5f) { animation = AnimationState.walk_Up; }
            else if (direction.Y >= 0.5f) { animation = AnimationState.walk_Down; }

            // Sprite
            sprite.Color = colorTint;
            sprite.Effect = spriteEffect;
            sprite.Play(animation.ToString());
            if (distanceToPlayer.LengthSquared() > distanceToPlayerStop)
                sprite.Update(gameTime.GetElapsedSeconds());

            lastAnimation = animation;
            lastSpriteEffect = spriteEffect;
        }
   
    }
}
