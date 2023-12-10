﻿using LoneWandererGame.Enemy;
using LoneWandererGame.Spells;
using LoneWandererGame.TileEngines;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LoneWandererGame
{
    public class SpellCollisionHandler
    {
        List<Spell> spells;
        EnemyHandler enemyHandler;
        Game1 game;
        TileEngine tileEngine;
        FloatingTextHandler floatingTextHandler;
        public SpellCollisionHandler(Game1 game, TileEngine tileEngine, EnemyHandler enemyHandler, List<Spell> spells, FloatingTextHandler floatingTextHandler)
        {
            this.spells = spells;
            this.enemyHandler = enemyHandler;
            this.game = game;
            this.tileEngine = tileEngine;
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
                        floatingTextHandler.AddText(spell.Damage.ToString(), new Vector2(enemy.CollisionRectangle.X, enemy.CollisionRectangle.Y), Color.Red);
                        if (spell.GetType() == typeof(ProjectileSpell))
                            spell.Timer = -1;

                        spell.HitEnemies.Add(enemy);
                    }
                }

                if (spell.GetType() == typeof(ProjectileSpell))
                    {
                        Vector2 velocity = ((ProjectileSpell)spell).GetVelocity;
                        List<Rectangle> collisions = tileEngine.GetCollisions(spell.CollisionRectangle, velocity);
                        if (collisions.Count != 0)
                            spell.Timer = -1;
                    }
            }
        }
    }
}
