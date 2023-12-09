using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.Spells
{
    public class SpellLevelDefinition
    {
        public int Damage { get; set; }
        public float Cooldown { get; set; }
        public string Description { get; set; }
        public int SpecialMultiplier { get; set; }
    }
}
