using LoneWandererGame.Progression;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;

namespace LoneWandererGame.GameScreens
{
    public class NewPlayerScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        private string recordedString = "";
        public NewPlayerScreen(Game1 game): base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
            Game.KeyboardListener.KeyPressed += KeyboardListener_KeyPressed;
        }

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                if (recordedString.Length > 3)
                {
                    PlayerScore.Name = recordedString;
                    PlayerScore.WriteToFile();
                    Game.LoadMenuScreen();
                    return;
                }
            }
            else if(e.Key == Microsoft.Xna.Framework.Input.Keys.Back)
            {
                if(recordedString.Length > 0)
                {
                    recordedString = recordedString.Substring(0, recordedString.Length - 1);
                }
            }
            if (recordedString.Length > 10)
                return;
            switch (e.Key)
            {
                // Letters
                case Microsoft.Xna.Framework.Input.Keys.A: recordedString += "A"; break;
                case Microsoft.Xna.Framework.Input.Keys.B: recordedString += "B"; break;
                case Microsoft.Xna.Framework.Input.Keys.C: recordedString += "C"; break;
                case Microsoft.Xna.Framework.Input.Keys.D: recordedString += "D"; break;
                case Microsoft.Xna.Framework.Input.Keys.E: recordedString += "E"; break;
                case Microsoft.Xna.Framework.Input.Keys.F: recordedString += "F"; break;
                case Microsoft.Xna.Framework.Input.Keys.G: recordedString += "G"; break;
                case Microsoft.Xna.Framework.Input.Keys.H: recordedString += "H"; break;
                case Microsoft.Xna.Framework.Input.Keys.I: recordedString += "I"; break;
                case Microsoft.Xna.Framework.Input.Keys.J: recordedString += "J"; break;
                case Microsoft.Xna.Framework.Input.Keys.K: recordedString += "K"; break;
                case Microsoft.Xna.Framework.Input.Keys.L: recordedString += "L"; break;
                case Microsoft.Xna.Framework.Input.Keys.M: recordedString += "M"; break;
                case Microsoft.Xna.Framework.Input.Keys.N: recordedString += "N"; break;
                case Microsoft.Xna.Framework.Input.Keys.O: recordedString += "O"; break;
                case Microsoft.Xna.Framework.Input.Keys.P: recordedString += "P"; break;
                case Microsoft.Xna.Framework.Input.Keys.Q: recordedString += "Q"; break;
                case Microsoft.Xna.Framework.Input.Keys.R: recordedString += "R"; break;
                case Microsoft.Xna.Framework.Input.Keys.S: recordedString += "S"; break;
                case Microsoft.Xna.Framework.Input.Keys.T: recordedString += "T"; break;
                case Microsoft.Xna.Framework.Input.Keys.U: recordedString += "U"; break;
                case Microsoft.Xna.Framework.Input.Keys.V: recordedString += "V"; break;
                case Microsoft.Xna.Framework.Input.Keys.W: recordedString += "W"; break;
                case Microsoft.Xna.Framework.Input.Keys.X: recordedString += "X"; break;
                case Microsoft.Xna.Framework.Input.Keys.Y: recordedString += "Y"; break;
                case Microsoft.Xna.Framework.Input.Keys.Z: recordedString += "Z"; break;

                // Numbers
                case Microsoft.Xna.Framework.Input.Keys.D0: recordedString += "0"; break;
                case Microsoft.Xna.Framework.Input.Keys.D1: recordedString += "1"; break;
                case Microsoft.Xna.Framework.Input.Keys.D2: recordedString += "2"; break;
                case Microsoft.Xna.Framework.Input.Keys.D3: recordedString += "3"; break;
                case Microsoft.Xna.Framework.Input.Keys.D4: recordedString += "4"; break;
                case Microsoft.Xna.Framework.Input.Keys.D5: recordedString += "5"; break;
                case Microsoft.Xna.Framework.Input.Keys.D6: recordedString += "6"; break;
                case Microsoft.Xna.Framework.Input.Keys.D7: recordedString += "7"; break;
                case Microsoft.Xna.Framework.Input.Keys.D8: recordedString += "8"; break;
                case Microsoft.Xna.Framework.Input.Keys.D9: recordedString += "9"; break;

                // Space
                case Microsoft.Xna.Framework.Input.Keys.Space: recordedString += " "; break;

                // Handle other keys as needed

                default:
                    // Handle keys not explicitly listed above
                    break;
            }

        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            Game.KeyboardListener.KeyPressed -= KeyboardListener_KeyPressed;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.DrawString(Game.RegularFont, "Enter name: " + recordedString, new Vector2(10f, 10f), Color.White);
            Game.SpriteBatch.End();
        }
    }
}
