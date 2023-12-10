using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LoneWandererGame.Entity;

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
        public SpellBook(Game1 game, Player player, List<Spell> activeSpells)
        {
            Spells = new List<SpellDefinition>();
            Game = game;
            Player = player;
            ProjectileSpawners = new List<ProjectileSpawner>();
            ActiveSpells = activeSpells;
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
                if (spell.Timer < 0)
                {
                    spell.Timer = spell.LevelDefinitions[spell.CurrentLevel].Cooldown;
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

        public void ProjectileTimerCallback(SpellDefinition spellDefinition)
        {
            var spell = new ProjectileSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player.Direction, spellDefinition.Speed);
            spell.Timer = spellDefinition.TimeToLive;
            spell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            spell.Sound = spellDefinition.Sound;
            spell.LoadContent(Game.Content);
            ActiveSpells.Add(spell);
        }
    }
}
