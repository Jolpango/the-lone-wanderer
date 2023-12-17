using Microsoft.Xna.Framework;
using MonoGame.Jolpango.Graphics;
using System;
using System.Collections.Generic;

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
        public ParticleEmitter ParticleEmitter { get; set; }
        public int ParticleAmount { get; set; }
        public Color? LightColor { get; set; }
        public Vector2? LightSize { get; set; }
        public float? LightIntensity { get; set; }
        public List<SpellLevelDefinition> LevelDefinitions { get; set; }
    }
}
