using System;
using System.Collections.Generic;
using LoneWandererGame.Entity;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

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
        private List<string> colors;
        private Dictionary<string, SpriteSheet> spriteSheets;
        public PowerupHandler(Game1 game, Player player)
        {
            powerups = new List<Powerup>();
            spriteSheets = new Dictionary<string, SpriteSheet>();
            Game = game;
            Player = player;
            rng = new Random();
            colors = new List<string>()
            {
                "blue",
                "green",
                "grey",
                "orange",
                "pink",
                "yellow",
            };

            spawnTimer = spawnTime;
        }

        public void LoadContent()
        {
            foreach (string color in colors)
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
                powerups.Remove(powerup);

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
            Vector2 randomDirection = new Vector2(
                ((float)rng.NextDouble() * 2.0f) - 1,
                ((float)rng.NextDouble() * 2.0f) - 1
            );
            randomDirection.Normalize();
            Vector2 position = Player.Position + randomDirection * 400;

            string color = colors[rng.Next(colors.Count)];
            Powerup powerup = new Powerup(Game, position, spriteSheets[color], color, 3f);
            powerups.Add(powerup);
        }

        public void CheckPlayerPickup()
        {
            foreach (Powerup powerup in powerups)
            {
                if (!powerup.Active && Player.getSpriteRectangle().Intersects(powerup.CollisionRectangle))
                {
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
