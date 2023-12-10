using Microsoft.Xna.Framework;
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
using System;
using System.Linq;
using LoneWandererGame.Powerups;

namespace LoneWandererGame.GameScreens
{
    public class PlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        private Player _player;
        private OrthographicCamera _camera;
        private Texture2D _groundTexture;
        private TileEngine tileEngine;
        private PowerupHandler powerupHandler;
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
        private int spellChoices = 2;

        private Random rnd;
        List<SpellDefinition> randomSpells;
        List<SpellSelection> spellSelections;

        public PlayScreen(Game1 game) : base(game)
        {
            tileEngine = new TileEngine(Game);
            // Assumed map width and height is 512
            float tilemapCenter = (512 * 32) / 2f;
            _player = new Player(Game, tileEngine, new Vector2(tilemapCenter, tilemapCenter));
            powerupHandler = new PowerupHandler(Game, _player);
            enemyHandler = new EnemyHandler(Game, tileEngine);
            ActiveSpells = new List<Spell>();
            SpellBook = new SpellBook(Game, _player, ActiveSpells);
            SpellDefinitions = new List<SpellDefinition>();
            FloatingTextHandler = new FloatingTextHandler(Game);
            SpellCollisionHandler = new SpellCollisionHandler(Game, tileEngine, enemyHandler, ActiveSpells, FloatingTextHandler);
            levelUpInProgres = false;
            rnd = new Random();
            spellSelections = new List<SpellSelection>();
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
            powerupHandler.LoadContent();
            SpellDefinitions = SpellLoader.LoadSpells();
            foreach(var spell in SpellDefinitions)
            {
                if (spell.Name == "Gravity Axe")
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
            //MediaPlayer.Play(backgroundMusic);
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
            powerupHandler.Update(gameTime);

            playerHealthBar.CurrentValue = _player.Health;
            xpBar.CurrentValue = PlayerScore.XP;
            xpBar.MaxValue = PlayerScore.RequiredXP;
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
            randomSpells = new List<SpellDefinition>();
            randomSpells.AddRange(SpellDefinitions.Where(x => !SpellBook.IsInSpellBookMax(x.Name)).OrderBy(x => rnd.Next()).Take(spellChoices));
            if (randomSpells.Count <= 0)
            {
                levelUpInProgres = false;
            }
            int i = 0;
            spellSelections = new List<SpellSelection>();
            foreach (var spell in randomSpells)
            {
                SpellDefinition existingSpell = SpellBook.Spells.Where(s => s.Name == spell.Name).FirstOrDefault();
                if(existingSpell != null)
                {
                    float splitter = 300;
                    Vector2 middle = Game.WindowDimensions / 2;
                    Vector2 startOffset = new Vector2(splitter - (splitter / 4), 256);
                    Vector2 offset = new Vector2(i * splitter, 0);
                    Vector2 pos = middle - startOffset + offset;
                    spellSelections.Add(new SpellSelection()
                    {
                        Position = pos,
                        SpellDefinition = existingSpell,
                        HasSpell = true,
                    });
                    spellSelections[i].LoadContent(Game.Content);
                }
                else
                {
                    float splitter = 300;
                    Vector2 middle = Game.WindowDimensions / 2;
                    Vector2 startOffset = new Vector2(splitter - (splitter / 4), 256);
                    Vector2 offset = new Vector2(i * splitter, 0);
                    Vector2 pos = middle - startOffset + offset;
                    spellSelections.Add(new SpellSelection()
                    {
                        Position = pos,
                        SpellDefinition = spell,
                    });
                    spellSelections[i].LoadContent(Game.Content);
                }
                i++;
            }
        }

        private void chooseSpellOnLevelUp()
        {
            MouseStateExtended mouseState = MouseExtended.GetState();
            bool hasCollide = false;
            foreach(var spell in spellSelections)
            {
                if (spell.Rectangle.Contains(Game.CustomCursor.Rectangle))
                {
                    Game.CustomCursor.CursorState = CursorState.select;
                    hasCollide = true;
                    spell.Color = Color.Gold;
                }
                else
                {
                    spell.Color = Color.White;
                }
            }
            if (!hasCollide)
            {
                Game.CustomCursor.CursorState = CursorState.pointer;
            }
            if (mouseState.WasButtonJustDown(MouseButton.Left))
            {
                foreach (var spell in spellSelections)
                {
                    if (spell.Rectangle.Contains(Game.CustomCursor.Rectangle))
                    {
                        if (SpellBook.IsSpellInSpellBook(spell.SpellDefinition) >= 0)
                            SpellBook.LevelUpSpell(spell.SpellDefinition.Name);
                        else
                            SpellBook.AddSpell(spell.SpellDefinition);
                        levelUpInProgres = false;
                    }
                }
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
            tileEngine.Draw(_camera.BoundingRectangle);
            powerupHandler.Draw();
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
            float screenWidth = Game.WindowDimensions.X;
            if (_player.godMode)
            {
                string godModeString = "God Mode ON!";
                Vector2 godModeStrSize = Game.RegularFont.MeasureString(godModeString);
                Game.SpriteBatch.DrawString(Game.RegularFont, godModeString, new Vector2(screenWidth - godModeStrSize.X - 10f, 100f), Color.Red);
            }
            playerHealthBar.Draw();
            xpBar.Draw();

            // FPS Counter
            {
                int frameRate = (int)((1 / gameTime.ElapsedGameTime.TotalSeconds) + 0.01);
                string fpsString = "FPS: " + frameRate.ToString();
                Vector2 size = Game.SilkscreenRegularFont.MeasureString(fpsString);
                
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, fpsString, new Vector2(screenWidth - size.X - 10f, 40f), Color.White);
            }

            //Level up
            if (levelUpInProgres)
            {
                foreach(var spellSelection in spellSelections)
                {
                    spellSelection.Draw(Game.SpriteBatch, Game.SilkscreenRegularFont);
                }
            }

            Game.CustomCursor.Draw();
            Game.SpriteBatch.End();
        }
    }
}
