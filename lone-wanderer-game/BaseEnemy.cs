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
        private float health;
        private float moveSpeed;
        private Texture2D guySprite;
        private Vector2 position = new Vector2(0.0f, 0.0f);
        public Game1 Game { get; private set; }


        public BaseEnemy(float health, float moveSpeed, Vector2 position, Game1 game)
        {
            this.health = health;
            this.moveSpeed = moveSpeed;
            this.position = position;
            Game = game;
        }
        public void LoadContent()
        {
            guySprite = Game.Content.Load<Texture2D>("Sprites/guy");
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
        }
        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(guySprite, position, null, Color.White);
        }






    }

}
