using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LoneWandererGame.Entity;
using LoneWandererGame.Enemy;
using Autofac.Core.Activators.Reflection;
using System.Security.Cryptography.X509Certificates;
using MonoGame.Jolpango.Graphics;

namespace LoneWandererGame.Spells
{
    public delegate void OnTimerEnd();
    public class ProjectileSpawner
    {
        public float Timer;
        public OnTimerEnd TimerEnd;
    }
    public class SpellBook
    {
        public List<SpellDefinition> Spells { get; private set; }
        public Game1 Game { get; private set; }
        public Player Player { get; private set; }
        public List<ProjectileSpawner> ProjectileSpawners { get; private set; }
        public List<Spell> ActiveSpells { get; private set; }
        public EnemyHandler EnemyHandler;
        public float cooldownReduction { get; private set; }
        public SpellBook(Game1 game, Player player, List<Spell> activeSpells, EnemyHandler enemyhandler)
        {
            Spells = new List<SpellDefinition>();
            Game = game;
            Player = player;
            ProjectileSpawners = new List<ProjectileSpawner>();
            ActiveSpells = activeSpells;
            EnemyHandler = enemyhandler;
            cooldownReduction = 1.0f;
        }
        public void AddSpell(SpellDefinition spell)
        {
            Spells.Add(spell);
        }
        public void RemoveSpell(SpellDefinition spell)
        {
            Spells.Remove(spell);
        }
        public void LevelUpSpell(string name)
        {
            var spell = Spells.Where(x => name == x.Name).FirstOrDefault();
            if (spell is null) return;
            spell.CurrentLevel = Math.Min(spell.CurrentLevel + 1, spell.LevelDefinitions.Count - 1);
        }

        public bool IsInSpellBookMax(string name)
        {
            var spell = Spells.Where(x => x.Name == name).FirstOrDefault();
            if (spell is null) return false;
            return spell.LevelDefinitions.Count - 1 == spell.CurrentLevel;
        }

        public int IsSpellInSpellBook(SpellDefinition spell)
        {
            for (int i = 0; i < Spells.Count; i++)
            {
                if (Spells[i].Name == spell.Name)
                    return i;
            }
            return -1;
        }

        public void Update(GameTime gameTime)
        {
            for(int i = 0; i < ProjectileSpawners.Count; i++)
            {
                ProjectileSpawners[i].Timer -= gameTime.GetElapsedSeconds();
                if (ProjectileSpawners[i].Timer <= 0)
                {
                    ProjectileSpawners[i].TimerEnd();
                    ProjectileSpawners.RemoveAt(i);
                    i--;
                }
            }
            foreach (var spell in Spells)
            {
                spell.Timer -= gameTime.GetElapsedSeconds();
                if(spell.ParticleEmitter is not null)
                    spell.ParticleEmitter.Update(gameTime);
                if (spell.Timer < 0)
                {
                    spell.Timer = spell.LevelDefinitions[spell.CurrentLevel].Cooldown * cooldownReduction;
                    ConstructSpell(spell);
                }
            }
        }
        public void ConstructSpell(SpellDefinition spellDefinition)
        {
            if (spellDefinition.SpellType == typeof(ProjectileSpell))
            {
                ConstructProjectileSpell(spellDefinition);
            }
            else if (spellDefinition.SpellType == typeof(MeleeSpell))
            {
                ConstructMeleeSpell(spellDefinition);
            }
            else if (spellDefinition.SpellType == typeof(AoESpell))
            {
                ConstructAoESpell(spellDefinition);
            }
            else if (spellDefinition.SpellType == typeof(GravitySpell))
            {
                ConstructGravitySpell(spellDefinition);
            }
            else if (spellDefinition.SpellType == typeof(PiercingSpell))
            {
                ConstructPiercingSpell(spellDefinition);
            }
        }
        private void ConstructAoESpell(SpellDefinition spellDefinition)
        {
            AoESpell spell = new AoESpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position);
            spell.Sound = spellDefinition.Sound;
            spell.LoadContent(Game.Content);
            spell.Timer = spellDefinition.TimeToLive;
            spell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            ActiveSpells.Add(spell);
        }
        private void ConstructMeleeSpell(SpellDefinition spellDefinition)
        {
            MeleeSpell meleeSpell = new MeleeSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player.Direction, Player);
            meleeSpell.Sound = spellDefinition.Sound;
            meleeSpell.LightColor = spellDefinition.LightColor;
            meleeSpell.LightSize = spellDefinition.LightSize;
            meleeSpell.LightIntensity = spellDefinition.LightIntensity;
            meleeSpell.LoadContent(Game.Content);
            meleeSpell.Timer = spellDefinition.TimeToLive;
            meleeSpell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            ActiveSpells.Add(meleeSpell);
        }

        private void ConstructGravitySpell(SpellDefinition spellDefinition)
        {
            for(int i = 0; i < spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].SpecialMultiplier; i++)
            {
                float multi = 1 + (i * 0.1f);
                ProjectileSpawners.Add(new ProjectileSpawner()
                {
                    Timer = i * 0.2f,
                    TimerEnd = () => { GravitySpellCallback(spellDefinition, multi); }
                });
            }
        }

        private void GravitySpellCallback(SpellDefinition spellDefinition, float forceMulti)
        {
            GravitySpell spell = new GravitySpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player, spellDefinition.Speed, forceMulti);
            spell.Sound = spellDefinition.Sound;
            spell.LightColor = spellDefinition.LightColor;
            spell.LightSize = spellDefinition.LightSize;
            spell.LightIntensity = spellDefinition.LightIntensity;
            spell.LoadContent(Game.Content);
            spell.Timer = spellDefinition.TimeToLive;
            spell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            ActiveSpells.Add(spell);
        }

        public void ConstructProjectileSpell(SpellDefinition spellDefinition)
        {
            for (int i = 0; i < spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].SpecialMultiplier; i++)
            {
                ProjectileSpawners.Add(new ProjectileSpawner()
                {
                    Timer = i * 0.2f,
                    TimerEnd = () => { ProjectileTimerCallback(spellDefinition); }
                });
            }
        }
        public void ConstructPiercingSpell(SpellDefinition spellDefinition)
        {
            for (int i = 0; i < spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].SpecialMultiplier; i++)
            {
                ProjectileSpawners.Add(new ProjectileSpawner()
                {
                    Timer = i * 0.2f,
                    TimerEnd = () => { PiercingTimerCallback(spellDefinition); }
                });
            }
        }

        public void ProjectileTimerCallback(SpellDefinition spellDefinition)
        {
            var spell = new ProjectileSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player.Direction, spellDefinition.Speed);
            spell.Timer = spellDefinition.TimeToLive;
            spell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            spell.Sound = spellDefinition.Sound;
            spell.LightColor = spellDefinition.LightColor;
            spell.LightSize = spellDefinition.LightSize;
            spell.LightIntensity = spellDefinition.LightIntensity;
            if (spellDefinition.ParticleEmitter is not null)
                spell.ParticleEmitter = CopyEmitter(spellDefinition.ParticleEmitter);
            spell.ParticleAmount = spellDefinition.ParticleAmount;
            spell.LoadContent(Game.Content);
            ActiveSpells.Add(spell);
        }
        public void PiercingTimerCallback(SpellDefinition spellDefinition)
        {
            Vector2 closestEnemy = ClosestEnemy();
            if (closestEnemy.Length() < 200.0f)
            {
                closestEnemy.Normalize();
                var spell = new PiercingSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, closestEnemy, spellDefinition.Speed)
                {
                    Timer = spellDefinition.TimeToLive,
                    Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage,
                    Sound = spellDefinition.Sound,
                    LightColor = spellDefinition.LightColor,
                    LightSize = spellDefinition.LightSize,
                    LightIntensity = spellDefinition.LightIntensity
                };
                spell.LoadContent(Game.Content);
                ActiveSpells.Add(spell);
            }
        }
        public Vector2 ClosestEnemy()
        {
            Vector2 playerPos = Player.Position;
            float range = 200f;
            float halfRange = range / 2;
            RectangleF rangeRect = new RectangleF(
                playerPos.X - halfRange,
                playerPos.Y - halfRange,
                range,
                range
            );
            List<BaseEnemy> enemies = EnemyHandler.GetNearbyEnemies(rangeRect);
            foreach (BaseEnemy enemy in enemies)
            {
                Vector2 distance = enemy.getPos() - playerPos;
                if (distance.Length() < range) // distance to close* enemy
                    return distance;
            }

            return new Vector2(1000000.0f,1000000.0f); // big number to be far away
        }

        public void Draw()
        {
            foreach(SpellDefinition spell in Spells)
            {
                if(spell.ParticleEmitter is not null)
                    spell.ParticleEmitter.Draw(Game.SpriteBatch);
            }
        }

        public ParticleEmitter CopyEmitter(ParticleEmitter emitter)
        {
            ParticleEmitter newEmitter = new ParticleEmitter(emitter.Texture)
            {
                Color = emitter.Color,
                MaxAlpha = emitter.MaxAlpha,
                MinAlpha = emitter.MinAlpha,
                MinRadius = emitter.MinRadius,
                MaxRadius = emitter.MaxRadius,
                MinScale = emitter.MinScale,
                MaxScale = emitter.MaxScale,
                StartColor = emitter.StartColor,
                EndColor = emitter.EndColor,
                LayerDepth = emitter.LayerDepth,
                TimeToLive = emitter.TimeToLive,
                MinSpeed = emitter.MinSpeed,
                MaxSpeed = emitter.MaxSpeed,
                DirectionFreedom = emitter.DirectionFreedom,
                Easing = emitter.Easing
            };
            return newEmitter;
        }

        public void cooldwonReductionMultiply(float reduce)
        {
            cooldownReduction *= reduce;
        }
        public float getCooldwonReductionNextLevel(float reduce)
        { 
            float tempReduction = (int)(cooldownReduction * reduce * 100);
            return tempReduction/100;
        }
        public float getCooldwonReduction()
        {
            float tempReduction = (int)(cooldownReduction * 100);
            return tempReduction / 100;
        }
    }
}
