using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.Spells
{
    public class SpellDefinition
    {
        public Type SpellType { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Asset { get; set; }
        public int CurrentLevel { get; set; } = 0;
        public float Timer { get; set; }
        public float TimeToLive { get; set; }
        public float Speed { get; set; }
        public string Sound { get; set; }
        public List<SpellLevelDefinition> LevelDefinitions { get; set; }
    }
}
