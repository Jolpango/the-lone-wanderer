using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;

namespace LoneWandererGame.GameScreens
{
    public class TitleScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        public TitleScreen(Game1 game): base(game) { }

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
            Game.GraphicsDevice.Clear(Color.Gray);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Title Screen", Vector2.Zero, Color.White);
            Game.SpriteBatch.End();
        }
    }
}
