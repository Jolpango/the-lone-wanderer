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

        public PlayScreen(Game1 game) : base(game)
        {
            _player = new Player(Game, Vector2.Zero);
            enemyHandler = new EnemyHandler(Game);
            ActiveSpells = new List<Spell>();
            SpellBook = new SpellBook(Game, _player);
            SpellDefinitions = new List<SpellDefinition>();
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
            tileEngine = new TileEngine(Game);
            tileEngine.LoadContent();
            SpellDefinitions = SpellLoader.LoadSpells();
            foreach(var spell in SpellDefinitions)
            {
                SpellBook.AddSpell(spell);
            }
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
        }

        private void UpdateSpells(GameTime gameTime)
        {
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

            var spells = SpellBook.Update(gameTime);
            if (spells is not null)
            {
                ActiveSpells.AddRange(spells);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Green);

            // UI
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Play Screen", Vector2.Zero, Color.White);
            Game.SpriteBatch.End();

            // World
            var transformMatrix = _camera.GetViewMatrix();
            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: transformMatrix);

            Game.SpriteBatch.Draw(_groundTexture, Vector2.Zero, Color.White);
            _player.Draw();
            tileEngine.Draw();
            enemyHandler.Draw(gameTime);
            foreach(var spell in ActiveSpells)
            {
                spell.Draw(Game.SpriteBatch);
            }
            Game.SpriteBatch.End();
        }
    }
}
