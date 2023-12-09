using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoneWandererGame.Entity;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;


namespace LoneWandererGame.Enemy
{
    public class BaseEnemy
    {
      
        private float moveSpeed;
        private Texture2D enemySprite;
        private Vector2 position = new Vector2(0.0f, 0.0f);
        private float attackCooldown = 0.0f;
       
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
        public Vector2 scale = new Vector2(1, 1);
        private AnimationState lastAnimation;
        private SpriteEffects lastSpriteEffect;
        private Vector2 direction = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;

        public Game1 Game { get; private set; }
        public float health { get; private set; }

        public Rectangle CollisionRectangle
        {
            get
            {
                return (Rectangle)sprite.GetBoundingRectangle(position, rotation, scale);
            }
        }

        public BaseEnemy(float health, float moveSpeed, Vector2 position, Game1 game)
        {
            this.health = health;
            this.moveSpeed = moveSpeed;
            this.position = position;
            Game = game;
        }
        public void LoadContent()
        {
            var spriteSheet = Game.Content.Load<SpriteSheet>("Sprites/Enemies/dark_soldier.sf", new JsonContentLoader());
            sprite = new AnimatedSprite(spriteSheet);
            sprite.Origin = new Vector2(16, 16);
            sprite.Depth = 0.25f;

            lastSpriteEffect = SpriteEffects.None;
            lastAnimation = AnimationState.idle_Down;
            sprite.Play(lastAnimation.ToString());

            //enemySprite = Game.Content.Load<Texture2D>("Sprites/guy");
        }

        public void Update(GameTime gameTime, Player _player)
        {
          

            //Movement
            Vector2 playerPos = new Vector2(_player.Position.X, _player.Position.Y);
            Vector2 direction = (playerPos - this.position) - new Vector2(20.0f,20.0f); // this makes it in middle of player
            Vector2 distanceToPlayer = direction;
            if (direction != new Vector2(0.0f,0.0f))
            { 
                direction.Normalize(); 
            }
            if (distanceToPlayer.LengthSquared() > distanceToPlayerStop)
                position += (direction * moveSpeed * gameTime.GetElapsedSeconds());


            //Animation
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
            if(distanceToPlayer.LengthSquared() > distanceToPlayerStop)
                sprite.Update(gameTime.GetElapsedSeconds());

            lastAnimation = animation;
            lastSpriteEffect = spriteEffect;



          
            if (attackCooldown < 0.0f)
            {
                Rectangle playerbox = _player.getSpriteRectangle();
                Rectangle enemyBox = (Rectangle)sprite.GetBoundingRectangle(position, rotation, scale);

                if (playerbox.Intersects(enemyBox))
                {
                   // TODO take damage on player here
                }
                attackCooldown = 1.0f;
            }    
            attackCooldown-= gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
              Game.SpriteBatch.Draw(sprite, position, rotation, scale);
        }
        public bool TakeDamage(float damage)
        {
            this.health -= damage;
            if (this.health <= 0)
                return false;
            else
                return true;
        }
        public bool IsDead()
        {
            if (this.health <= 0)
                return true;
            else
                return false;
        }


        







    }

}
