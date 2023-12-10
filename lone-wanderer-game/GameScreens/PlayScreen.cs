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

        public PlayScreen(Game1 game) : base(game)
        {
            tileEngine = new TileEngine(Game);
            _player = new Player(Game, tileEngine, new Vector2(440f, 300f));
            enemyHandler = new EnemyHandler(Game, tileEngine);
            ActiveSpells = new List<Spell>();
            SpellBook = new SpellBook(Game, _player);
            SpellDefinitions = new List<SpellDefinition>();
            FloatingTextHandler = new FloatingTextHandler(Game);
            SpellCollisionHandler = new SpellCollisionHandler(Game, tileEngine, enemyHandler, ActiveSpells, FloatingTextHandler);
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
                if (spell.SpellType == typeof(AoESpell))
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
            var keyboardState = KeyboardExtended.GetState();

            _player.Update(gameTime);

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _camera.LookAt(_player.Position);
            enemyHandler.Update(gameTime, _player);

            UpdateSpells(gameTime);
            FloatingTextHandler.Update(gameTime);

            playerHealthBar.CurrentValue = _player.Health;
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
                var spells = SpellBook.Update(gameTime);
                if (spells is not null)
                {
                    ActiveSpells.AddRange(spells);
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

            Game.SpriteBatch.DrawString(Game.RegularFont, "Play Screen", new Vector2(10f, 10f), Color.White);

            playerHealthBar.Draw();

            // FPS Counter
            {
                int frameRate = (int)((1 / gameTime.ElapsedGameTime.TotalSeconds) + 0.01);
                string fpsString = "FPS: " + frameRate.ToString();
                float screenWidth = Game.WindowDimensions.X;
                Vector2 size = Game.SilkscreenRegularFont.MeasureString(fpsString);
                
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, fpsString, new Vector2(screenWidth - size.X - 10f, 10f), Color.White);
            }

            Game.SpriteBatch.End();
        }
    }
}
