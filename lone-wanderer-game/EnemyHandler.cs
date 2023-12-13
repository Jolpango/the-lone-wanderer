using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using LoneWandererGame.Entity;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Serialization;
using LoneWandererGame.TileEngines;
using LoneWandererGame.Utilities;
using System.Linq;

namespace LoneWandererGame.Enemy
{
    public class EnemyHandler
    {
        private float bossSpawn = 25.0f;
        private int bossNr = 1;
        //Just to disable enemies (1/2)
        private bool disableEnemies = false;
        private List<BaseEnemy> enemies;
        private List<string> enemyNames;
        private Queue<int> freeList = new Queue<int>();
        private Dictionary<string, SpriteSheet> spriteSheets = new Dictionary<string, SpriteSheet>();
        private float spawnTimer = 0;
        private TileEngine tileEngine;
        Random randomNumber;
        public Game1 Game { get; private set; }

        public EnemyHandler(Game1 game, TileEngine tileEngine)
        {
            Game = game;
            this.tileEngine = tileEngine;
            randomNumber = new Random();
        }

        public void LoadContent()
        {
            enemyNames = new List<string>()
            {
                "dark_soldier",
                "dark_soldier-black",
                "dark_soldier-commander",
                "dark_soldier-dragonrider",
                "dark_soldier-lord",
                "dark_soldier-overlord"
            };
            foreach (string name in enemyNames)
            {
                string path = $"Sprites/Enemies/{name}.sf";
                spriteSheets.Add(name, Game.Content.Load<SpriteSheet>(path, new JsonContentLoader()));
            }

            int enemyCapacity = 2000;
            enemies = new List<BaseEnemy>(enemyCapacity);
            for (int i = 0; i < enemyCapacity; i++)
            {
                enemies.Add(new BaseEnemy(Game, tileEngine, spriteSheets));
                freeList.Enqueue(i);
            }
        }

        public void Update(GameTime gameTime, Player _player)
        {
            bool OutOfBounds(Vector2 position, Vector2 tileMapCullSize, Vector2 edgeSpawnCullDistance)
            {
                return position.X > tileMapCullSize.X
                    || position.Y > tileMapCullSize.Y
                    || position.X < edgeSpawnCullDistance.X
                    || position.Y < edgeSpawnCullDistance.Y;
            };

            //Just to disable enemies (2/2)
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.P))
                disableEnemies = true;
            if (keyboardState.IsKeyDown(Keys.O))
                disableEnemies = false;
            if (disableEnemies) return;

            for (int i = 0; i < enemies.Count; i++)
            {
                BaseEnemy enemy = enemies[i];
                if (enemy.Dormant) continue;

                enemy.Update(gameTime, _player);
                if (enemy.IsDead())
                {
                    PlayerScore.GainXP(1);
                    PlayerScore.Score++;
                    enemy.Dormant = true;
                    freeList.Enqueue(i);
                }
            }

            if (spawnTimer <= 0f)
            {
                spawnTimer = 1f;
                int enemiesToSpawn = (int)gameTime.TotalGameTime.TotalSeconds / 4;
                Vector2 edgeSpawnCullDistance = new Vector2(2f, 2f);
                Vector2 tileMapCullSize = tileEngine.GetMapSize() - edgeSpawnCullDistance;
                Queue<int> enemyIndices = new Queue<int>();

                // Enqueue to new queue to give boss creation precedence over enemy creation
                foreach(int index in freeList.Take(enemiesToSpawn).ToList()) // safely take enemiesToSpawn or freeList.Queue indices
                    enemyIndices.Enqueue(index);

                // Only needs to check this once instead of in every iteration
                if (gameTime.TotalGameTime.TotalSeconds >= bossSpawn && enemyIndices.Count != 0)
                {
                    Vector2 spawnPos = _player.Position + LWGMath.GetRandomDirection(randomNumber) * 700;
                    if (!OutOfBounds(spawnPos, tileMapCullSize, edgeSpawnCullDistance))
                    {
                        BaseEnemy boss = enemies[enemyIndices.Dequeue()];
                        boss.Initialize(50.0f * bossNr, 50.0f, spawnPos, "dark_soldier-overlord", 5 + (5 * bossNr));
                        bossNr++;
                        bossSpawn = bossNr * 25;
                    }
                }

                while (enemyIndices.Count != 0)
                {
                    int index = enemyIndices.Dequeue();

                    Vector2 spawnPos = _player.Position + LWGMath.GetRandomDirection(randomNumber) * 700;
                    if (OutOfBounds(spawnPos, tileMapCullSize, edgeSpawnCullDistance))
                        continue;

                    BaseEnemy enemy = enemies[freeList.Dequeue()];
                    switch (index % 5)
                    {
                        case 0:
                            enemy.Initialize(2.0f, 100.0f, spawnPos, "dark_soldier");
                            break;
                        case 1:
                            enemy.Initialize(5.0f, 100.0f, spawnPos, "dark_soldier-black");
                            break;
                        case 2:
                            enemy.Initialize(40.0f, 80.0f, spawnPos, "dark_soldier-commander");
                            break;
                        case 3:
                            enemy.Initialize(5.0f, 250.0f, spawnPos, "dark_soldier-dragonrider");
                            break;
                        case 4:
                            enemy.Initialize(20.0f, 100.0f, spawnPos, "dark_soldier-lord");
                            break;
                    }
                }
            }

            spawnTimer -= gameTime.GetElapsedSeconds();
        }
        public void Draw(GameTime gameTime)
        {
            foreach (BaseEnemy enemy in enemies)
                if (!enemy.Dormant)
                    enemy.Draw(gameTime);
        }

        public List<BaseEnemy> GetEnemies() { return enemies; }
    }
}
