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

        public PlayScreen(Game1 game) : base(game)
        {
            enemyHandler = new EnemyHandler(Game);

        }
        public override void LoadContent()
        {
            base.LoadContent();
            Game.KeyboardListener.KeyPressed += menuactions;

            Vector2 windowDimensions = Game.WindowDimensions;
            _player = new Player(Game, Vector2.Zero);
            _player.LoadContent();
            
            var viewportAdapter = new BoxingViewportAdapter(Game.Window, Game.GraphicsDevice, (int)windowDimensions.X, (int)windowDimensions.Y);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.ZoomIn(0.5f);
            _groundTexture = Game.Content.Load<Texture2D>("Sprites/checkerboard");

            enemyHandler.LoadContent();
            tileEngine = new TileEngine(Game);
            tileEngine.LoadContent();
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
            Game.SpriteBatch.End();
        }
    }
}
