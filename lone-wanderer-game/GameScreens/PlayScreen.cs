using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;

namespace LoneWandererGame.GameScreens
{
    public class PlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        private Vector2 guyPosition = new Vector2(0, 0);
        private Texture2D guySprite;

        public PlayScreen(Game1 game): base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
            Game.KeyboardListener.KeyPressed += menuactions;
            guySprite = Game.Content.Load<Texture2D>("Sprites/guy");
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
            Game.KeyboardListener.KeyTyped -= menuactions;
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            {
                guyPosition.Y--;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                guyPosition.Y++;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                guyPosition.X--;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                guyPosition.X++;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Green);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Play Screen", Vector2.Zero, Color.White);
            Game.SpriteBatch.Draw(guySprite, guyPosition, null, Color.White);
            Game.SpriteBatch.End();
        }
    }
}
