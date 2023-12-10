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
    public class SpellBook
    {
        public List<SpellDefinition> Spells { get; private set; }
        public Game1 Game { get; private set; }
        public Player Player { get; private set; }
        public SpellBook(Game1 game, Player player)
        {
            Spells = new List<SpellDefinition>();
            Game = game;
            Player = player;
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
        public List<Spell> Update(GameTime gameTime)
        {
            List<Spell> spells = null;
            foreach (var spell in Spells)
            {
                spell.Timer -= gameTime.GetElapsedSeconds();
                if (spell.Timer < 0)
                {
                    spell.Timer = spell.LevelDefinitions[spell.CurrentLevel].Cooldown;
                    if (spells is null)
                    {
                        spells = new List<Spell>();
                    }
                    spells.AddRange(ConstructSpell(spell));
                }
            }
            return spells;
        }
        public List<Spell> ConstructSpell(SpellDefinition spellDefinition)
        {
            if (spellDefinition.SpellType == typeof(ProjectileSpell))
            {
                return ConstructProjectileSpell(spellDefinition);
            }
            else if (spellDefinition.SpellType == typeof(MeleeSpell))
            {
                return ConstructMeleeSpell(spellDefinition);
            }
            else if (spellDefinition.SpellType == typeof(AoESpell))
            {
                return ConstructAoESpell(spellDefinition);
            }
            return null;
        }
        private List<Spell> ConstructAoESpell(SpellDefinition spellDefinition)
        {
            List<Spell> spells = new List<Spell>();
            AoESpell spell = new AoESpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position);
            spell.Sound = spellDefinition.Sound;
            spell.LoadContent(Game.Content);
            spell.Timer = spellDefinition.TimeToLive;
            spell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            spells.Add(spell);
            return spells;
        }
        private List<Spell> ConstructMeleeSpell(SpellDefinition spellDefinition)
        {
            List<Spell> spells = new List<Spell>();
            MeleeSpell meleeSpell = new MeleeSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player.Direction);
            meleeSpell.Sound = spellDefinition.Sound;
            meleeSpell.LoadContent(Game.Content);
            meleeSpell.Timer = spellDefinition.TimeToLive;
            meleeSpell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
            spells.Add(meleeSpell);
            return spells;
        }

        public List<Spell> ConstructProjectileSpell(SpellDefinition spellDefinition)
        {
            List<Spell> spells = new List<Spell>();
            for (int i = 0; i < spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].SpecialMultiplier; i++)
            {
                var spell = new ProjectileSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position + (Player.Direction * 10 * i), Player.Direction, spellDefinition.Speed);
                spell.Timer = spellDefinition.TimeToLive;
                spell.Damage = spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].Damage;
                spell.Sound = spellDefinition.Sound;
                spell.LoadContent(Game.Content);
                spells.Add(spell);
            }
            return spells;
        }
    }
}
