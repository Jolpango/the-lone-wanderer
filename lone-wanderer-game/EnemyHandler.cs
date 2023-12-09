using LoneWandererGame.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoneWandererGame.Entity;

namespace LoneWandererGame.Enemy
{
    public class EnemyHandler
    {
       private List<BaseEnemy> enemyList = new List<BaseEnemy>();
        public Game1 Game { get; private set; }


        // temp to just see if it works
        public EnemyHandler(Game1 game) {Game=game; enemyList.Add(new BaseEnemy(10.0f,10.0f, new Vector2(0.0f,0.0f), game)); }

        public void LoadContent()
        {
            for (int i = 0; i < enemyList.Count; i++)
                enemyList[i].LoadContent();
        }

        public void Update(GameTime gameTime, Player _player)
        {
            for (int i = 0; i < enemyList.Count; i++)
                enemyList[i].Update(gameTime, _player);
        }
        public void Draw(GameTime gameTime)
        {
            for(int i = 0;i < enemyList.Count;i++)
                enemyList[(int)i].Draw(gameTime);
        }

        public BaseEnemy GetEnemy(int index) { return enemyList[index]; }

        public List<BaseEnemy> GetEnemyAll() { return enemyList; }

        public void addEnemy(Vector2 position, float health = 10.0f, float moveSpeed = 10.0f)
        {
            enemyList.Add(new BaseEnemy(health, moveSpeed, position, Game));
            
        }
    }
}
