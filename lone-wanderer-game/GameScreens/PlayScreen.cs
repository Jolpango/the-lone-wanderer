using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using LoneWandererGame.Entity;

namespace LoneWandererGame.GameScreens
{
    public class PlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        private Player player;

        public PlayScreen(Game1 game): base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
            Game.KeyboardListener.KeyPressed += menuactions;
            player = new Player(Game);
            player.LoadContent();
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

            player.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Green);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Play Screen", Vector2.Zero, Color.White);
            player.Draw();
            Game.SpriteBatch.End();
        }
    }
}
