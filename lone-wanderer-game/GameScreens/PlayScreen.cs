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

namespace LoneWandererGame.GameScreens
{
    public class PlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        private Player _player;
        private OrthographicCamera _camera;

        public PlayScreen(Game1 game): base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
            Game.KeyboardListener.KeyPressed += menuactions;

            Vector2 windowDimensions = Game.WindowDimensions;
            _player = new Player(Game, Vector2.Zero);
            _player.LoadContent();
            
            var viewportAdapter = new BoxingViewportAdapter(Game.Window, Game.GraphicsDevice, (int)windowDimensions.X, (int)windowDimensions.Y);
            _camera = new OrthographicCamera(viewportAdapter);
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

            _camera.LookAt(_player.position);
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
            Game.SpriteBatch.Begin(transformMatrix: transformMatrix);
            _player.Draw();
            Game.SpriteBatch.End();
            
        }
    }
}
