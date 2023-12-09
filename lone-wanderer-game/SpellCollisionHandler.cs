using LoneWandererGame.Enemy;
using LoneWandererGame.Spells;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame
{
    public class SpellCollisionHandler
    {
        List<Spell> spells;
        EnemyHandler enemyHandler;
        Game1 game;
        FloatingTextHandler floatingTextHandler;
        public SpellCollisionHandler(Game1 game, EnemyHandler enemyHandler, List<Spell> spells, FloatingTextHandler floatingTextHandler)
        {
            this.spells = spells;
            this.enemyHandler = enemyHandler;
            this.game = game;
            this.floatingTextHandler = floatingTextHandler;
        }
        public void Update()
        {
            foreach (Spell spell in spells)
            {
                foreach(BaseEnemy enemy in enemyHandler.GetEnemyAll())
                {
                    if (!spell.HitEnemies.Contains(enemy) && spell.CollisionRectangle.Intersects(enemy.CollisionRectangle))
                    {
                        enemy.TakeDamage(spell.Damage);
                        floatingTextHandler.AddText(spell.Damage.ToString(), new Vector2(enemy.CollisionRectangle.X, enemy.CollisionRectangle.Y));
                        if (spell.GetType() == typeof(ProjectileSpell))
                        {
                            spell.Timer = -1;
                        }
                        spell.HitEnemies.Add(enemy);
                    }
                }
            }
        }
    }
}
