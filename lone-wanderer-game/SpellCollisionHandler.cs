using LoneWandererGame.Enemy;
using LoneWandererGame.Spells;
using LoneWandererGame.TileEngines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private SoundEffect hitSound;
        public SpellCollisionHandler(Game1 game, TileEngine tileEngine, EnemyHandler enemyHandler, List<Spell> spells, FloatingTextHandler floatingTextHandler)
        {
            this.spells = spells;
            this.enemyHandler = enemyHandler;
            this.game = game;
            this.tileEngine = tileEngine;
            this.floatingTextHandler = floatingTextHandler;
            hitSound = game.Content.Load<SoundEffect>("Sounds/hit");
        }

        public void Update()
        {
            int soundTriggerCount = 0;
            const int MAX_SOUND_TRIGGER_COUNT = 50;
            foreach (Spell spell in spells)
            {
                List<BaseEnemy> nearbyEnemies = enemyHandler.GetNearbyEnemies(spell.CollisionRectangle);
                foreach(BaseEnemy enemy in nearbyEnemies)
                {
                    if (enemy.Dormant) continue;

                    if (!spell.HitEnemies.Contains(enemy) && spell.CollisionRectangle.Intersects(enemy.CollisionRectangle))
                    {
                        enemy.TakeDamage(spell.Damage);
                        if (soundTriggerCount < MAX_SOUND_TRIGGER_COUNT)
                        {
                            hitSound.Play(0.05f, 1.0f, 1.0f);
                            soundTriggerCount++;
                        }
                        floatingTextHandler.AddText(spell.Damage.ToString(), new Vector2(enemy.CollisionRectangle.X, enemy.CollisionRectangle.Y), Color.Red);
                        spell.HitEnemies.Add(enemy);

                        if (spell.GetType() == typeof(ProjectileSpell))
                            spell.Timer = -1;
                    }
                }

                Vector2 velocity = spell.GetVelocity();
                if (velocity == Vector2.Zero)
                    continue;

                if (tileEngine.GetCollisions(spell.CollisionRectangle, velocity).Count != 0)
                    spell.Timer = -1;
            }
        }
    }
}
