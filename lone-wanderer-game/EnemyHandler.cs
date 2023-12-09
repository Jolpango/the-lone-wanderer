using LoneWandererGame.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoneWandererGame.Entity;
using MonoGame.Extended;
using System.Security.Cryptography;

namespace LoneWandererGame.Enemy
{
    public class EnemyHandler
    {
        private List<BaseEnemy> enemyList = new List<BaseEnemy>();
        private float spawnTimer = 0;
        Random randomNumber;

        public Game1 Game { get; private set; }


        // temp to just see if it works
        public EnemyHandler(Game1 game) 
        {
            Game=game;
            randomNumber = new Random();

        }

        public void LoadContent()
        {
            for (int i = 0; i < enemyList.Count; i++)
                enemyList[i].LoadContent();
        }

        public void Update(GameTime gameTime, Player _player)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Update(gameTime, _player);
                if(enemyList[i].IsDead()) 
                { 
                    enemyList.RemoveAt(i); 
                    i--; 
                }             
            }

            if (spawnTimer <=0.0f)
            {
                spawnTimer = 1.0f;
                int enemiesToSpawn = (int)gameTime.TotalGameTime.TotalSeconds / 4;
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    float randomX = (float)randomNumber.NextDouble() * 2.0f;
                    float randomY = (float)randomNumber.NextDouble() * 2.0f;
                    randomX--;
                    randomY--;

                    Vector2 randomPos = new Vector2(randomX, randomY);
                    randomPos.Normalize();

                    Vector2 spawnPos =_player.Position + randomPos*700;
                    addEnemy(10.0f, 100.0f, spawnPos);
                }
            }
            spawnTimer-=gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
            for(int i = 0;i < enemyList.Count;i++)
                enemyList[i].Draw(gameTime);
        }

        public BaseEnemy GetEnemy(int index) { return enemyList[index]; }

        public List<BaseEnemy> GetEnemyAll() { return enemyList; }

        public void addEnemy(float health, float moveSpeed, Vector2 position)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game));
            enemyList.Last().LoadContent();
            
        }
    }
}
