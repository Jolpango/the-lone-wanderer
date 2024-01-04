using LoneWandererGame.Entity;
using LoneWandererGame.TileEngines;
using LoneWandererGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Jolpango.Graphics;
using System.Collections.Generic;

namespace LoneWandererGame.Spells
{
    public class SpellCollisionHandler
    {
        List<Spell> spells;
        EnemyHandler enemyHandler;
        Game1 game;
        TileEngine tileEngine;
        FloatingTextHandler floatingTextHandler;
        ParticleEmitter bloodEmitter;
        ParticleEmitter bloodEmitterDeath;
        private SoundEffect hitSound;
        public SpellCollisionHandler(Game1 game, TileEngine tileEngine, EnemyHandler enemyHandler, List<Spell> spells, FloatingTextHandler floatingTextHandler)
        {
            this.spells = spells;
            this.enemyHandler = enemyHandler;
            this.game = game;
            this.tileEngine = tileEngine;
            this.floatingTextHandler = floatingTextHandler;
            hitSound = game.Content.Load<SoundEffect>("Sounds/hit");
            Texture2D tempTexture = new Texture2D(game.GraphicsDevice, 2, 2);
            Color[] data = new Color[2 * 2];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            tempTexture.SetData(data);
            bloodEmitter = new ParticleEmitter(tempTexture)
            {
                StartColor = Color.Red,
                EndColor = Color.Crimson,
                Direction = Vector2.UnitY,
                Easing = MonoGame.Jolpango.Utilities.EasingFunction.EaseOutBack,
                LayerDepth = 0.25f,
                DirectionFreedom = 50,
                MaxAlpha = 0,
                MinAlpha = 1,
                MaxScale = 1,
                MinScale = 1,
                MaxRadius = 10,
                MinRadius = 1,
                MaxSpeed = 1,
                MinSpeed = 1,
                TimeToLive = 1f
            };
            bloodEmitterDeath = new ParticleEmitter(tempTexture)
            {
                StartColor = Color.Red,
                EndColor = Color.Crimson,
                Direction = Vector2.UnitY,
                Easing = MonoGame.Jolpango.Utilities.EasingFunction.EaseOutBack,
                LayerDepth = 0.25f,
                DirectionFreedom = 360,
                MaxAlpha = 0,
                MinAlpha = 1,
                MaxScale = 2,
                MinScale = 1,
                MaxRadius = 30,
                MinRadius = 1,
                MaxSpeed = 10,
                MinSpeed = 2,
                TimeToLive = 1f
            };

        }

        public void Update(GameTime gameTime)
        {
            int soundTriggerCount = 0;
            bloodEmitter.Update(gameTime);
            bloodEmitterDeath.Update(gameTime);
            const int MAX_SOUND_TRIGGER_COUNT = 50;
            foreach (Spell spell in spells)
            {
                List<BaseEnemy> nearbyEnemies = enemyHandler.GetNearbyEnemies(spell.CollisionRectangle);
                foreach (BaseEnemy enemy in nearbyEnemies)
                {
                    if (enemy.Dormant) continue;

                    if (!spell.HitEnemies.Contains(enemy) && spell.CollisionRectangle.Intersects(enemy.CollisionRectangle))
                    {
                        enemy.TakeDamage(spell.Damage);
                        bloodEmitter.Emit(enemy.getPos(), 30);
                        if (enemy.IsDead())
                        {
                            bloodEmitterDeath.Emit(enemy.getPos(), 10);
                        }
                        if (soundTriggerCount < MAX_SOUND_TRIGGER_COUNT)
                        {
                            hitSound.Play();
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

        public void Draw(SpriteBatch spriteBatch)
        {
            bloodEmitter.Draw(spriteBatch);
            bloodEmitterDeath.Draw(spriteBatch);
        }
    }
}
