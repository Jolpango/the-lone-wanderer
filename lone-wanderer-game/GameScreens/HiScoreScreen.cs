using LoneWandererGame.MongoDBManagers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using System.Diagnostics;
using System.Threading;

namespace LoneWandererGame.GameScreens
{
    public class HiScoreScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        private string[] scores;
        public HiScoreScreen(Game1 game) : base(game)
        {
            Thread fetchThread = new Thread(fetchHiScores);
            fetchThread.Start();
        }
        private void fetchHiScores()
        {
            MongoDBManager.Instance.GetHighScores().ContinueWith((task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    scores = task.Result.ToArray();
                }
            });
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardStateExtended state = KeyboardExtended.GetState();
            if (state.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Game.LoadMenuScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin();
            if (scores != null)
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, scores[i], new Vector2(10f, 30f * i), Color.White);
                }
            }
            else
            {
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, "Loading...", new Vector2(10f, 0f), Color.White);
            }
            Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, "Press [Enter] to go back", new Vector2(400f, 50f), Color.White);
            Game.SpriteBatch.End();
        }
    }
}
