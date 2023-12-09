using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using System.Collections.Generic;

namespace LoneWandererGame.GameScreens
{
    public delegate void OnPress();
    public struct MenuItem
    {
        public string Title { get; set; }
        public OnPress OnPress { get; set; }
    }
    public class MenuScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        private int currentItem = 0;
        public MenuScreen(Game1 game): base(game)
        {
            MenuItems = new List<MenuItem>()
            {
                new MenuItem
                {
                    Title = "New Game",
                    OnPress = () => Game.LoadPlayScreen()
                },
                new MenuItem
                {
                    Title = "Quit",
                    OnPress = () => Game.Exit()
                }
            };
        }

        public List<MenuItem> MenuItems { get; set; }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.WasKeyJustDown(Keys.Escape))
            {
                Game.Exit();
            }
            if (keyboardState.WasKeyJustDown(Keys.Enter))
            {
                MenuItems[currentItem].OnPress();
            }
            if (keyboardState.WasKeyJustDown(Keys.S) || keyboardState.WasKeyJustDown(Keys.Down))
            {
                currentItem = currentItem + 1;
            }
            if (keyboardState.WasKeyJustDown(Keys.W) || keyboardState.WasKeyJustDown(Keys.Up))
            {
                currentItem--;
            }
            if (currentItem < 0)
                currentItem = 0;
            currentItem %= MenuItems.Count;
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Gray);
            Game.SpriteBatch.Begin();
            Vector2 center = Game.WindowDimensions / 4;
            for(int i = 0; i < MenuItems.Count; i++ )
            {
                if (currentItem == i)
                {
                    Vector2 size = Game.RegularFont.MeasureString(MenuItems[i].Title);
                    Game.SpriteBatch.DrawString(
                        Game.RegularFont,
                        MenuItems[i].Title,
                        center + new Vector2(size.X / 2, size.Y * i),
                        Color.Red,
                        0,
                        Vector2.Zero,
                        1.0f,
                        SpriteEffects.None,
                        1.0f);

                }
                else
                {
                    Vector2 size = Game.RegularFont.MeasureString(MenuItems[i].Title);
                    Game.SpriteBatch.DrawString(
                        Game.RegularFont,
                        MenuItems[i].Title,
                        center + new Vector2(size.X / 2, size.Y * i),
                        Color.White,
                        0,
                        Vector2.Zero,
                        1.0f,
                        SpriteEffects.None,
                        1.0f);
                }
            }
            Game.SpriteBatch.End();
        }
    }
}
