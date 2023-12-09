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
            return null;
        }

        private List<Spell> ConstructMeleeSpell(SpellDefinition spellDefinition)
        {
            List<Spell> spells = new List<Spell>();
            MeleeSpell meleeSpell = new MeleeSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player.Direction);
            meleeSpell.LoadContent(Game.Content);
            meleeSpell.Timer = spellDefinition.TimeToLive;
            spells.Add(meleeSpell);
            return spells;
        }

        public List<Spell> ConstructProjectileSpell(SpellDefinition spellDefinition)
        {
            List<Spell> spells = new List<Spell>();
            for (int i = 0; i < spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].SpecialMultiplier; i++)
            {
                var spell = new ProjectileSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, Player.Position, Player.Direction, spellDefinition.Speed);
                spell.Timer = spellDefinition.TimeToLive;
                spell.LoadContent(Game.Content);
                spells.Add(spell);
            }
            return spells;
        }
    }
}
