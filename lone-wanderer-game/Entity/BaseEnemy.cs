using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using MonoGame.Extended.Sprites;
using LoneWandererGame.TileEngines;
using LoneWandererGame.UI;

namespace LoneWandererGame.Entity
{
    public class BaseEnemy
    {

        private float moveSpeed;
        private Vector2 position = new Vector2(0.0f, 0.0f);
        private float attackCooldown = 0.0f;
        private TileEngine tileEngine;
        private int damage;
        //animation
        private float distanceToPlayerStop = 30.0f;


        //Animation
        private AnimatedSprite sprite;
        private Dictionary<string, AnimatedSprite> sprites;
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
        private Vector2 velocity = Vector2.Zero;
        private FillableBar healthBar;
        public bool Dormant { get; set; }

        public Game1 Game { get; private set; }
        public float health { get; private set; }

        public RectangleF CollisionRectangle
        {
            get
            {
                return sprite.GetBoundingRectangle(position, rotation, scale);
            }
        }

        public BaseEnemy(Game1 game, TileEngine tileEngine, Dictionary<string, SpriteSheet> spriteSheets)
        {
            Game = game;
            this.tileEngine = tileEngine;
            Dormant = true;
            sprites = new Dictionary<string, AnimatedSprite>();

            // Add a sprite for each spritesheet
            foreach (KeyValuePair<string, SpriteSheet> element in spriteSheets)
                sprites.Add(element.Key, new AnimatedSprite(element.Value));

            healthBar = new FillableBar()
            {
                Game = Game,
                BarHeight = 4,
                BarWidth = 40
            };
            healthBar.CreateTexture();
        }

        public void Initialize(float health, float moveSpeed, Vector2 position, string spriteSheetName, float scaleX = 1, float scaleY = 1, int damage = 5)
        {
            this.health = health;
            this.moveSpeed = moveSpeed;
            this.position = position;
            this.damage = damage;
            scale = new Vector2(scaleX, scaleY);

            sprite = sprites[spriteSheetName];
            sprite.Origin = new Vector2(24, 32); // hardcoded, should be calculated but I'm too lazy
            sprite.Depth = 0.25f;
            sprite.Color = Color.White;
            sprite.Effect = SpriteEffects.None;

            healthBar.MaxValue = (int)health;
            healthBar.CurrentValue = (int)health;
            healthBar.Position = position;

            Dormant = false;
        }

        public void Update(GameTime gameTime, Player _player)
        {
            //Movement
            Vector2 direction = _player.Position - position - new Vector2(20.0f, 20.0f); // this makes it in middle of player
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
            Animate(direction);

            if (distanceToPlayer.LengthSquared() > distanceToPlayerStop)
                sprite.Update(gameTime.GetElapsedSeconds());

            if (attackCooldown <= 0.0f)
            {
                RectangleF playerbox = _player.getSpriteRectangle();
                RectangleF enemyBox = sprite.GetBoundingRectangle(position, rotation, scale);

                if (playerbox.Intersects(enemyBox))
                {
                    // TODO take damage on player here
                    _player.Damage(damage);
                    attackCooldown = 1.0f;
                }

            }
            attackCooldown -= gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
#if DEBUG
            Game.SpriteBatch.DrawRectangle(sprite.GetBoundingRectangle(position, 0, Vector2.One * scale), Color.Red * 0.8f, 1, 0.9f);
#endif
            Game.SpriteBatch.Draw(sprite, position, rotation, scale);
            healthBar.Draw();
        }
        public void TakeDamage(float damage)
        {
            health -= damage;
        }
        public bool IsDead()
        {
            return health <= 0;
        }
        public Vector2 getPos() { return position; }
        private void Animate(Vector2 direction)
        {
            AnimationState animation = lastAnimation;
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

            if (direction != Vector2.Zero)
            {
                if (direction.X >= 0.5f) animation = AnimationState.walk_Right;
                else if (direction.X <= -0.5f) animation = AnimationState.walk_Left;
                else if (direction.Y <= -0.5f) animation = AnimationState.walk_Up;
                else if (direction.Y >= 0.5f) animation = AnimationState.walk_Down;
            }

            lastAnimation = animation;
            sprite.Play(animation.ToString());
        }
    }
}
