using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using MonoGame.Jolpango.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using Microsoft.Xna.Framework;
using MonoGame.Jolpango.Utilities;

namespace LoneWandererGame.Spells
{
    public class SpellLoader
    {
        public static List<SpellDefinition> LoadSpells(Game1 game)
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
                    TimeToLive = (float)o.Root["timer"],
                    Sound = (string)o.Root["sound"],
                    SpellType = Type.GetType($"LoneWandererGame.Spells.{(string)o.Root["type"]}"),
                    LevelDefinitions = o.Root["levelDefinitions"].Select(level => new SpellLevelDefinition()
                    {
                        Cooldown = (float)level["cooldown"],
                        Description = (string)level["description"],
                        Damage = (int)level["damage"],
                        SpecialMultiplier = (int)level["specialMultiplier"]
                    }).ToList()
                };

                if (o.Root["light"] is not null)
                {
                    spell.LightSize = new Vector2((int)o.Root["light"]["size"]["x"], (int)o.Root["light"]["size"]["y"]);
                    spell.LightColor = new Color((int)o.Root["light"]["r"], (int)o.Root["light"]["g"], (int)o.Root["light"]["b"]);
                    spell.LightIntensity = (float)o.Root["light"]["intensity"];
                }
                if (o.Root["particle"] is not null)
                {

                    Texture2D tempTexture = new Texture2D(game.GraphicsDevice, 2, 2);
                    Color[] data = new Color[2 * 2];
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                    tempTexture.SetData(data);
                    spell.ParticleEmitter = new ParticleEmitter(tempTexture);
                    spell.ParticleAmount = (int)o.Root["particle"]["amount"];
                    spell.ParticleEmitter.StartColor = new Color((int)o.Root["particle"]["startcolor"]["r"], (int)o.Root["particle"]["startcolor"]["g"], (int)o.Root["particle"]["startcolor"]["b"]);
                    spell.ParticleEmitter.EndColor = new Color((int)o.Root["particle"]["endcolor"]["r"], (int)o.Root["particle"]["endcolor"]["g"], (int)o.Root["particle"]["endcolor"]["b"]);
                    //spell.ParticleEmitter.StartColor = Color.Orange;
                    //spell.ParticleEmitter.EndColor = Color.Gray;
                    spell.ParticleEmitter.MinRadius = (int)o.Root["particle"]["radius"]["min"];
                    spell.ParticleEmitter.MaxRadius = (int)o.Root["particle"]["radius"]["max"];
                    spell.ParticleEmitter.MinSpeed = (float)o.Root["particle"]["speed"]["min"];
                    spell.ParticleEmitter.MaxSpeed = (float)o.Root["particle"]["speed"]["max"];
                    spell.ParticleEmitter.MinScale = (float)o.Root["particle"]["scale"]["min"];
                    spell.ParticleEmitter.MaxScale = (float)o.Root["particle"]["scale"]["max"];
                    spell.ParticleEmitter.TimeToLive = (float)o.Root["particle"]["timetolive"];
                    spell.ParticleEmitter.MinAlpha = (float)o.Root["particle"]["alpha"]["min"];
                    spell.ParticleEmitter.MaxAlpha = (float)o.Root["particle"]["alpha"]["max"];
                    spell.ParticleEmitter.Direction = new Vector2((float)o.Root["particle"]["direction"]["x"], (float)o.Root["particle"]["direction"]["y"]);
                    spell.ParticleEmitter.DirectionFreedom = (float)o.Root["particle"]["direction"]["freedom"];
                    spell.ParticleEmitter.LayerDepth = 0.25f;
                    spell.ParticleEmitter.Easing = EasingFunction.EaseInOutQuad;
                }

                spells.Add(spell);
            }
            return spells;
        }
    }
}
