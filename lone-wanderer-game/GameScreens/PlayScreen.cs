﻿using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using LoneWandererGame.Entity;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using LoneWandererGame.Enemy;
using LoneWandererGame.TileEngines;
using LoneWandererGame.Spells;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using static LoneWandererGame.Entity.Player;
using System;
using System.Diagnostics;
using MonoGame.Extended.TextureAtlases;

namespace LoneWandererGame.GameScreens
{
    public class PlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        private Player _player;
        private OrthographicCamera _camera;
        private Texture2D _groundTexture;
        private TileEngine tileEngine;

        private EnemyHandler enemyHandler;
        public SpellBook SpellBook;
        public List<Spell> ActiveSpells;
        public List<SpellDefinition> SpellDefinitions { get; private set; }
        public SpellCollisionHandler SpellCollisionHandler { get; private set; }
        public FloatingTextHandler FloatingTextHandler { get; private set; }

        private FillableBar playerHealthBar;
        private FillableBar xpBar;
        private bool levelUpInProgres;

        private Song backgroundMusic;

        public PlayScreen(Game1 game) : base(game)
        {
            tileEngine = new TileEngine(Game);
            _player = new Player(Game, tileEngine, new Vector2(440f, 300f));
            enemyHandler = new EnemyHandler(Game, tileEngine);
            ActiveSpells = new List<Spell>();
            SpellBook = new SpellBook(Game, _player, ActiveSpells);
            SpellDefinitions = new List<SpellDefinition>();
            FloatingTextHandler = new FloatingTextHandler(Game);
            SpellCollisionHandler = new SpellCollisionHandler(Game, tileEngine, enemyHandler, ActiveSpells, FloatingTextHandler);
            levelUpInProgres = false;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            Game.KeyboardListener.KeyPressed += menuactions;
            Vector2 windowDimensions = Game.WindowDimensions;
            _player.LoadContent();

            var viewportAdapter = new BoxingViewportAdapter(Game.Window, Game.GraphicsDevice, (int)windowDimensions.X, (int)windowDimensions.Y);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.ZoomIn(0.5f);
            _groundTexture = Game.Content.Load<Texture2D>("Sprites/checkerboard");

            enemyHandler.LoadContent();
            tileEngine.LoadContent();
            SpellDefinitions = SpellLoader.LoadSpells();
            foreach(var spell in SpellDefinitions)
            {
                if (spell.Name == "Whip")
                    SpellBook.AddSpell(spell);
            }
            int padding = 0;
            playerHealthBar = new FillableBar()
            {
                BarWidth = (int)Game.WindowDimensions.X - padding,
                BarHeight = 20,
                MaxValue = Player.MAX_HEALTH,
                Game = Game,
                CurrentValue = _player.Health,
                Position = new Vector2((int)(Game.WindowDimensions.X - padding) / 2, padding)
            };
            playerHealthBar.CreateTexture();
            xpBar = new FillableBar()
            {
                BarWidth = (int)Game.WindowDimensions.X - padding,
                BarHeight = 20,
                MaxValue = PlayerScore.RequiredXP,
                Game = Game,
                BarBackgroundColor = Color.LightBlue,
                BarForegroundColor = Color.Blue,
                CurrentValue = PlayerScore.XP,
                Position = new Vector2((int)(Game.WindowDimensions.X - padding) / 2, padding + 20)
            };
            xpBar.CreateTexture();
            PlayerScore.OnGainXp = GainXpFloatingText;
            PlayerScore.OnLevelUp = OnLevelUp;
            backgroundMusic = Game.Content.Load<Song>("Sounds/stage1");
            MediaPlayer.Play(backgroundMusic);
        }

        private void menuactions(object sender, KeyboardEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                Game.LoadDeathScreen();
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Game.KeyboardListener.KeyPressed -= menuactions;
        }

        public override void Update(GameTime gameTime)
        {
            if (levelUpInProgres)
            {
                chooseSpellOnLevelUp();    
            }
            else
            {

                //else
                var keyboardState = KeyboardExtended.GetState();

                _player.Update(gameTime);

                var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _camera.LookAt(_player.Position);
                enemyHandler.Update(gameTime, _player);

                UpdateSpells(gameTime);



                FloatingTextHandler.Update(gameTime);

                playerHealthBar.CurrentValue = _player.Health;
                xpBar.CurrentValue = PlayerScore.XP;
                xpBar.MaxValue = PlayerScore.RequiredXP;
            }
        }

        private void UpdateSpells(GameTime gameTime)
        {
            SpellCollisionHandler.Update();
            List<Spell> spellsToRemove = new List<Spell>();
            foreach (var spell in ActiveSpells)
            {
                spell.Update(gameTime);
                if (spell.Timer < 0)
                {
                    spellsToRemove.Add(spell);
                }
            }
            foreach (var spell in spellsToRemove)
            {
                ActiveSpells.Remove(spell);
            }

            if (_player.Health != 0)
            {
                SpellBook.Update(gameTime);
            }
        }

        public void GainXpFloatingText(int xp)
        {
            FloatingTextHandler.AddText(xp.ToString(), _player.Position, Color.White);
        }

        public void OnLevelUp()
        {
            FloatingTextHandler.AddText("Level up", _player.Position, Color.Green);
            levelUpInProgres = true;
        }

        private void chooseSpellOnLevelUp()
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                float screenWidth = Game.WindowDimensions.X / (SpellDefinitions.Count + 1);
                int spellIndex = (int)((mouseState.X - screenWidth) / screenWidth);

                if (SpellBook.IsSpellInSpellBook(SpellDefinitions[spellIndex]) >= 0)
                    SpellBook.LevelUpSpell(SpellDefinitions[spellIndex].Name);
                else
                    SpellBook.AddSpell(SpellDefinitions[spellIndex]);

                levelUpInProgres = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(new Color(new Vector3(0.23f, 0.42f, 0.12f)));

            // World
            var transformMatrix = _camera.GetViewMatrix();
            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: transformMatrix);

            Game.SpriteBatch.Draw(_groundTexture, Vector2.Zero, Color.White);
            _player.Draw();
            tileEngine.Draw();
            enemyHandler.Draw(gameTime);
            foreach(var spell in ActiveSpells)
            {
                spell.Draw(Game.SpriteBatch, Game);
            }
            FloatingTextHandler.Draw(Game.SpriteBatch);

            Game.SpriteBatch.End();

            // UI
            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack);

            Game.SpriteBatch.DrawString(Game.RegularFont, $"Level: {PlayerScore.Level}", new Vector2(10f, 40f), Color.White);
            Game.SpriteBatch.DrawString(Game.RegularFont, $"Score: {PlayerScore.Score}", new Vector2(10f, 80f), Color.White);
            playerHealthBar.Draw();
            xpBar.Draw();

            // FPS Counter
            {
                int frameRate = (int)((1 / gameTime.ElapsedGameTime.TotalSeconds) + 0.01);
                string fpsString = "FPS: " + frameRate.ToString();
                float screenWidth = Game.WindowDimensions.X;
                Vector2 size = Game.SilkscreenRegularFont.MeasureString(fpsString);
                
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, fpsString, new Vector2(screenWidth - size.X - 10f, 40f), Color.White);
            }

            //Level up
            if (levelUpInProgres)
            {
                MouseState mouseState = Mouse.GetState();
                float screenWidthPart = Game.WindowDimensions.X / (SpellDefinitions.Count + 1);
                float screenHeightPart = Game.WindowDimensions.Y / 4;

                float index = (mouseState.X - screenWidthPart) / screenWidthPart;
                index = (int)index;

                //Name
                for (int i = 0; i < SpellDefinitions.Count; i++)
                {
                    Color color = Color.DarkGoldenrod;
                    if (index == i)
                    {
                        color = Color.White;
                    }

                    SpellDefinition spell = SpellDefinitions[i];
                    Vector2 size = Game.SilkscreenRegularFont.MeasureString(spell.Name);
                    Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, spell.Name, new Vector2(screenWidthPart * i + screenWidthPart, screenHeightPart), color);
                    
                    // icons
                    Texture2D spellIcon = Game.Content.Load<Texture2D>($"Sprites/SpellIcons/{spell.Icon}");
                    float scale = .4f;
                    float iconHeight = screenHeightPart - (spellIcon.Height * scale);
                    Game.SpriteBatch.Draw(spellIcon, new Vector2(screenWidthPart * i + screenWidthPart, iconHeight),null, Color.White,0,Vector2.Zero,scale, SpriteEffects.None,0);


                    //level
                    string spellLevel;
                    int locateSpell = SpellBook.IsSpellInSpellBook(SpellDefinitions[i]);
                    if (locateSpell >= 0)
                        spellLevel = (SpellBook.Spells[locateSpell].CurrentLevel+1).ToString();
                    else
                        spellLevel = "0";
                    Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, spellLevel, new Vector2(screenWidthPart * i + screenWidthPart, screenHeightPart+20), color);

                }
            }
            Game.SpriteBatch.End();
        }
    }
}
