using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoneWandererGame.Spells
{
    public class SpellBook
    {
        public List<SpellDefinition> Spells { get; private set; }
        public Game1 Game { get; private set; }
        public SpellBook(Game1 game)
        {
            Spells = new List<SpellDefinition>();
            Game = game;
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
            return null;
        }
        public List<Spell> ConstructProjectileSpell(SpellDefinition spellDefinition)
        {
            Vector2 dir = new Vector2(1, 1);
            dir.Normalize();
            List<Spell> spells = new List<Spell>();
            for (int i = 0; i < spellDefinition.LevelDefinitions[spellDefinition.CurrentLevel].SpecialMultiplier; i++)
            {
                var spell = new ProjectileSpell(spellDefinition.Name, spellDefinition.Icon, spellDefinition.Asset, new Vector2(200, 200), dir, spellDefinition.Speed);
                spell.LoadContent(Game.Content);
                spells.Add(spell);
            }
            return spells;
        }
    }
}
