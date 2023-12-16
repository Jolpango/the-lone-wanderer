using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;

namespace LoneWandererGame.GameScreens
{
    public class DeathScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        public DeathScreen(Game1 game): base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Game.LoadMenuScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Red);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Death Screen", new Vector2(10f, 10f), Color.White);
            Game.SpriteBatch.End();
        }
    }
}
