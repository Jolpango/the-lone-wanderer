using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using LoneWandererGame.Utilities;
using LoneWandererGame.Entity;

namespace LoneWandererGame.Powerups
{
    public class PowerupHandler
    {
        private List<Powerup> powerups;
        public Game1 Game { get; private set; }
        public Player Player { get; private set; }
        private Random rng;
        private float spawnTimer = 0f;
        private float spawnTime = 4f;
        private List<string> colorNames;
        private List<Color> colors;
        private Dictionary<string, SpriteSheet> spriteSheets;
        public PowerupHandler(Game1 game, Player player)
        {
            powerups = new List<Powerup>();
            spriteSheets = new Dictionary<string, SpriteSheet>();
            Game = game;
            Player = player;
            rng = new Random();
            colorNames = new List<string>()
            {
                "blue",
                "green",
                "grey",
                "orange",
                "pink",
                "yellow",
            };

            colors = new List<Color>()
            {
                Color.Blue,
                Color.Green,
                Color.Gray,
                Color.Orange,
                new Color(255, 55, 55), //Color.Pink,
                Color.Yellow,
            };

            spawnTimer = spawnTime;
        }

        public void LoadContent()
        {
            foreach (string color in colorNames)
            {
                string path = $"Sprites/PowerUps/crystal-small-{color}.sf";
                spriteSheets.Add(color, Game.Content.Load<SpriteSheet>(path, new JsonContentLoader()));
            }
        }

        public void Update(GameTime gameTime)
        {
            List<Powerup> powerupsToRemove = new List<Powerup>();
            foreach(Powerup powerup in powerups)
            {
                powerup.Update(gameTime);
                if (powerup.Active && powerup.ActiveTimer <= 0)
                {
                    UnaffectPlayer(powerup);
                    powerupsToRemove.Add(powerup);
                }
            }
            foreach(Powerup powerup in powerupsToRemove)
            {
                powerups.Remove(powerup);
            }

            if (spawnTimer <= 0.0f)
            {
                spawnTimer = spawnTime;
                AddPowerup();
            }

            spawnTimer -= gameTime.GetElapsedSeconds();

            CheckPlayerPickup();
        }

        public void Draw()
        {
            foreach(Powerup powerup in powerups)
                if (!powerup.Active)
                    powerup.Draw(Game.SpriteBatch, Game);
        }

        public void AddPowerup()
        {
            Vector2 position = Player.Position + LWGMath.GetRandomDirection(rng) * 400;

            int colorIndex = rng.Next(colors.Count);
            string colorName = colorNames[colorIndex];

            Color color = colors[colorIndex];
            int lightIndex = -1;
            if (Game.LightHandler.hasEmptyLightSlots())
            {
                lightIndex = Game.LightHandler.AddLight(position, new Vector2(100), color, 0.6f);
            }
            Powerup powerup = new Powerup(Game, position, spriteSheets[colorName], colorName, 3f, lightIndex);
            powerups.Add(powerup);
        }

        public void CheckPlayerPickup()
        {
            foreach (Powerup powerup in powerups)
            {
                if (!powerup.Active && Player.getSpriteRectangle().Intersects(powerup.CollisionRectangle))
                {
                    if (powerup.LightIndex != -1)
                    {
                        Game.LightHandler.RemoveLight(powerup.LightIndex);
                    }
                    powerup.Active = true;
                    AffectPlayer(powerup);
                }
            }
        }

        public void AffectPlayer(Powerup powerup)
        {
            switch (powerup.Color)
            {
                case "blue":
                    break;
                case "green":
                    powerup.EffectAmount = rng.Next(10, 50);
                    Player.SpeedUp(powerup.EffectAmount);
                    break;
                case "grey":
                    powerup.EffectAmount = rng.Next(2, 6);
                    Player.MakeInvincible(powerup.EffectAmount);
                    break;
                case "orange":
                    powerup.EffectAmount = rng.Next(0, 10);
                    Player.Heal(powerup.EffectAmount);
                    break;
                case "pink":
                    break;
            }
        }

        public void UnaffectPlayer(Powerup powerup)
        {
            switch (powerup.Color)
            {
                case "blue":
                    break;
                case "green":
                    Player.SlowDown(powerup.EffectAmount);
                    break;
                case "grey":
                    break;
                case "orange":
                    break;
                case "pink":
                    break;
            }
        }
    }
}
