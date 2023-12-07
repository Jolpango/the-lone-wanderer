using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;

namespace LoneWandererGame.GameScreens
{
    public class MenuScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        public MenuScreen(Game1 game): base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Game.LoadPlayScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Gray);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Menu Screen", Vector2.Zero, Color.White);
            Game.SpriteBatch.End();
        }
    }
}
