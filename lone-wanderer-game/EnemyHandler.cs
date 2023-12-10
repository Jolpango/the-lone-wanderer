using LoneWandererGame.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoneWandererGame.Entity;
using MonoGame.Extended;
using System.Security.Cryptography;
using Microsoft.Xna.Framework.Input;
using LoneWandererGame.TileEngines;

namespace LoneWandererGame.Enemy
{
    public class EnemyHandler
    {
        private float bossSpawn = 25.0f;
        private int bossNr = 1;
        //Just to disable enemies (1/2)
        private bool disableEnemies = false;

        private List<BaseEnemy> enemyList = new List<BaseEnemy>();
        private float spawnTimer = 0;
        private TileEngine tileEngine;
        Random randomNumber;
        


        public Game1 Game { get; private set; }


        // temp to just see if it works
        public EnemyHandler(Game1 game, TileEngine tileEngine)
        {
            Game = game;
            this.tileEngine = tileEngine;
            randomNumber = new Random();

        }

        public void LoadContent()
        {
            for (int i = 0; i < enemyList.Count; i++)
                enemyList[i].LoadContent();
        }

        public void Update(GameTime gameTime, Player _player)
        {
            //Just to disable enemies (2/2)
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.IsKeyDown(Keys.P))
            {
                disableEnemies = true;
            }
            if (keyboardState.IsKeyDown(Keys.O))
            {
                disableEnemies = false;
            }
            if (disableEnemies) { return; }




            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Update(gameTime, _player);
                if (enemyList[i].IsDead())
                {
                    PlayerScore.GainXP(1);
                    PlayerScore.Score++;
                    enemyList.RemoveAt(i);
                    i--;
                }
            }

            if (spawnTimer <= 0.0f)
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

                    Vector2 spawnPos = _player.Position + randomPos * 700;
                    if (i % 5 == 0)
                        addEnemyBlue(10.0f, 100.0f, spawnPos);
                    else if (i % 5 == 1)
                        addEnemyBlack(10.0f, 100.0f, spawnPos);
                    else if (i % 5 == 2)
                        addEnemyCommander(10.0f, 100.0f, spawnPos);
                    else if (i % 5 == 3)
                        addEnemyDragonRider(10.0f, 100.0f, spawnPos);
                    else if (i % 5 == 4)
                        addEnemyLord(10.0f, 100.0f, spawnPos);

                    if ((int)gameTime.TotalGameTime.TotalSeconds >= bossSpawn)
                    {
                              
                        addEnemyOverLord(50.0f*bossNr, 50.0f, spawnPos, 5+(5*bossNr));
                        bossNr++;
                        bossSpawn = (float)bossNr * 25;
                    }
                }
            }
            spawnTimer -= gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
            for(int i = 0;i < enemyList.Count;i++)
                enemyList[i].Draw(gameTime);
        }

        public BaseEnemy GetEnemy(int index) { return enemyList[index]; }

        public List<BaseEnemy> GetEnemyAll() { return enemyList; }

        public void addEnemyBlue(float health, float moveSpeed, Vector2 position)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game, tileEngine, "Sprites/Enemies/dark_soldier.sf"));
            enemyList.Last().LoadContent();
            
        }
        public void addEnemyBlack(float health, float moveSpeed, Vector2 position)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game, tileEngine, "Sprites/Enemies/dark_soldier-black.sf"));
            enemyList.Last().LoadContent();

        }
        public void addEnemyCommander(float health, float moveSpeed, Vector2 position)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game, tileEngine, "Sprites/Enemies/dark_soldier-commander.sf"));
            enemyList.Last().LoadContent();

        }
        public void addEnemyDragonRider(float health, float moveSpeed, Vector2 position)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game, tileEngine, "Sprites/Enemies/dark_soldier-dragonrider.sf"));
            enemyList.Last().LoadContent();

        }
        public void addEnemyLord(float health, float moveSpeed, Vector2 position)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game, tileEngine, "Sprites/Enemies/dark_soldier-lord.sf"));
            enemyList.Last().LoadContent();

        }
        public void addEnemyOverLord(float health, float moveSpeed, Vector2 position, int damage)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game, tileEngine, "Sprites/Enemies/dark_soldier-overlord.sf",4.0f,4.0f));
            enemyList.Last().LoadContent();

        }
    }
}
