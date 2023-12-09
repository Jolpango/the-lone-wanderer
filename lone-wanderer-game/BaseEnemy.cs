using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoneWandererGame.Entity;

namespace LoneWandererGame.Enemy
{
    public class BaseEnemy
    {
      
        private float moveSpeed;
        private Texture2D enemySprite;
        private Vector2 position = new Vector2(0.0f, 0.0f);
        private float attackCooldown = 0.0f;

        public Game1 Game { get; private set; }
        public float health { get; private set; }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, enemySprite.Width, enemySprite.Height);
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
            enemySprite = Game.Content.Load<Texture2D>("Sprites/guy");
        }

        public void Update(GameTime gameTime, Player _player)
        {

            Vector2 playerPos = new Vector2(_player.Position.X, _player.Position.Y);
            Vector2 direction = (playerPos - this.position);
            if (direction != new Vector2(0.0f,0.0f))
            { 
                direction.Normalize(); 
            }
       
            position+= (direction * moveSpeed * gameTime.GetElapsedSeconds());


            //TODO add colision with player and enemy
            if (attackCooldown < 0.0f)
            {
                //player should take dmg TODO
                attackCooldown = 1.0f;
            }    
            attackCooldown-= gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(enemySprite, position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.15f);
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
