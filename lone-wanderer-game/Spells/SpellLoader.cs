using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

namespace LoneWandererGame.Spells
{
    public class SpellLoader
    {
        public static List<SpellDefinition> LoadSpells()
        {
            var spells = new List<SpellDefinition>();
            var files = Directory.GetFiles("Content/Spells");
            foreach (var file in files)
            {
                JObject o = JObject.Parse(File.ReadAllText(file));
                SpellDefinition spell = new SpellDefinition()
                {
                    Name = (string)o.Root["name"],
                    Asset = (string)o.Root["asset"],
                    Icon = (string)o.Root["icon"],
                    Speed = (int)o.Root["speed"],
                    SpellType = Type.GetType($"LoneWandererGame.Spells.{(string)o.Root["type"]}"),
                    LevelDefinitions = o.Root["levelDefinitions"].Select(level => new SpellLevelDefinition()
                    {
                        Cooldown = (float)level["cooldown"],
                        Description = (string)level["description"],
                        Damage = (int)level["damage"],
                        SpecialMultiplier = (int)level["specialMultiplier"]
                    }).ToList()
                };
                spells.Add(spell);
            }
            return spells;
        }
    }
}
