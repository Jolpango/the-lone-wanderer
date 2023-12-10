using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        private Song backgroundMusic;
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

        private SoundEffect menuNavigateSound;
        private SoundEffect menuClickSound;
        private Texture2D background;

        public override void LoadContent()
        {
            base.LoadContent();
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.IsRepeating = true;
            backgroundMusic = Game.Content.Load<Song>("Sounds/menuchip");
            MediaPlayer.Play(backgroundMusic);
            menuNavigateSound = Game.Content.Load<SoundEffect>("Sounds/swish_2");
            menuClickSound = Game.Content.Load<SoundEffect>("Sounds/swish_4");
            background = Game.Content.Load<Texture2D>("Sprites/menubackground");
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
                menuClickSound.Play();
                MenuItems[currentItem].OnPress();
            }
            if (keyboardState.WasKeyJustDown(Keys.S) || keyboardState.WasKeyJustDown(Keys.Down))
            {
                currentItem = currentItem + 1;
                menuNavigateSound.Play();
            }
            if (keyboardState.WasKeyJustDown(Keys.W) || keyboardState.WasKeyJustDown(Keys.Up))
            {
                currentItem--;
                menuNavigateSound.Play();
            }
            if (currentItem < 0)
                currentItem = 0;
            currentItem %= MenuItems.Count;
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Gray);
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            Vector2 center = Game.WindowDimensions / 2;
            Color textColor = Color.White;
            float scale = 1f;

            string menuTitle = "Main Menu";
            Vector2 size = Game.BoldFont.MeasureString(menuTitle) * scale;
            Game.SpriteBatch.DrawString(
                Game.BoldFont,
                menuTitle,
                center + new Vector2(-size.X / 2, -100f),
                textColor,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                1.0f);

            float menuItemSpacing = size.Y;

            for (int i = 0; i < MenuItems.Count; i++ )
            {
                
                if (currentItem == i)
                {
                    textColor = Color.Red;
                    scale = 1.2f;
                }
                else
                {
                    textColor = Color.White;
                    scale = 1f;
                }

                size = Game.SilkscreenRegularFont.MeasureString(MenuItems[i].Title) * scale;
                Game.SpriteBatch.DrawString(
                    Game.SilkscreenRegularFont,
                    MenuItems[i].Title,
                    center + new Vector2(-size.X / 2, menuItemSpacing * i),
                    textColor,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    1.0f);
            }
            Game.SpriteBatch.End();
        }
    }
}
