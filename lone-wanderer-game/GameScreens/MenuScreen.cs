using LoneWandererGame.MongoDBManagers;
using LoneWandererGame.Progression;
using LoneWandererGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using MonoGame.Jolpango.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LoneWandererGame.GameScreens
{
    public class MenuScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        private Song backgroundMusic;
        private const float BUTTON_DISTANCE = 80;

        public MenuScreen(Game1 game) : base(game)
        {
            Vector2 center = Game.WindowDimensions / 2;
            center.Y -= 92;
            Vector2 buttonPosition = center;

            MenuItems = new List<Button>()
            {
                new Button(Game)
                {
                    Text = "New Game",
                    OnClick = Game.LoadPlayScreen,
                    OnPress = () => {},
                    OnRelease = () => {},
                    Position = center
                },
                new Button(Game)
                {
                    Text = "HiScores",
                    OnClick = () => {
                        Game.LoadHiscoreScreen();
                    },
                    OnPress = () => {},
                    OnRelease = () => {},
                    Position = buttonPosition += new Vector2(0, BUTTON_DISTANCE)
                },
                new Button(Game)
                {
                    Text = "Options",
                    OnClick = () => {
                        showOptions = true;
                    },
                    OnPress = () => {},
                    OnRelease = () => {},
                    Position = buttonPosition += new Vector2(0, BUTTON_DISTANCE)
                },
                new Button(Game)
                {
                    Text = "Exit",
                    OnClick = Game.Exit,
                    OnPress = () => {},
                    OnRelease = () => {},
                    Position = buttonPosition += new Vector2(0, BUTTON_DISTANCE)
                }
            };
            foreach (var item in MenuItems)
            {
                item.LoadContent("Sprites/UI/button.sf");
            }
            showOptions = false;
            Vector2 buttonOffset = new Vector2(0, -64 / 2);
            Vector2 optionsMenuOffset = new Vector2(0, -30f);
            Vector2 optionsMenuOrigin = center + buttonOffset + optionsMenuOffset;
            optionsMenu = new OptionsMenu(Game, optionsMenuOrigin);

            optionsBackPressed = false;
            optionsBackButton = new Button(game)
            {
                Text = "Back",
                OnClick = () => { optionsBackPressed = true; },
                OnPress = () => { },
                OnRelease = () => { },
                Position = optionsMenuOrigin + new Vector2(0f, optionsMenu.getMenuDimensions().Y + BACK_BUTTON_Y_OFFSET)
            };
            optionsBackButton.LoadContent("Sprites/UI/button.sf");
        }

        public List<Button> MenuItems { get; set; }

        private Texture2D background;

        private bool showOptions;
        private OptionsMenu optionsMenu;

        private Button optionsBackButton;
        private bool optionsBackPressed;
        private const float BACK_BUTTON_Y_OFFSET = (64f / 2f) + 10f;

        public override void LoadContent()
        {
            base.LoadContent();
            MediaPlayer.IsRepeating = true;
            backgroundMusic = Game.Content.Load<Song>("Sounds/menuchip");
            MediaPlayer.Play(backgroundMusic);
            background = Game.Content.Load<Texture2D>("Sprites/menubackground");
            ParticleEmitter.Shared.LayerDepth = 0.98f;
        }

        public void windowSizeChanged()
        {
            Vector2 center = Game.WindowDimensions / 2;
            center.Y -= 92;
            Vector2 buttonPosition = center;
            foreach (Button item in MenuItems)
            {
                item.Position = buttonPosition += new Vector2(0, BUTTON_DISTANCE);
            }

            Vector2 buttonOffset = new Vector2(0, -64 / 2);
            Vector2 optionsMenuOffset = new Vector2(0, -30f);
            Vector2 optionsMenuOrigin = center + buttonOffset + optionsMenuOffset;
            optionsMenu.centerChanged(optionsMenuOrigin);

            optionsBackButton.Position = optionsMenuOrigin + new Vector2(0f, optionsMenu.getMenuDimensions().Y + BACK_BUTTON_Y_OFFSET);
        }

        public override void Update(GameTime gameTime)
        {
            if (Game.SettingsUpdated)
            {
                windowSizeChanged();
                Game.SettingsUpdated = false;
            }

            if (!showOptions)
            {
                foreach(var item in MenuItems)
                {
                    item.Update(gameTime);
                }
            }
            else
            {
                optionsMenu.Update(gameTime);
                optionsBackButton.Update(gameTime);

                if (optionsBackPressed)
                {
                    showOptions = false;
                    optionsBackPressed = false;
                }
            }
            ParticleEmitter.Shared.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Gray);
            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack);
            Game.CustomCursor.Draw();
            Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, "Player: " + PlayerScore.Name, new Vector2(10, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            Game.SpriteBatch.Draw(background, new Rectangle(0, 0, (int)Game.WindowDimensions.X, (int)Game.WindowDimensions.Y), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            if (!showOptions)
            {
                foreach (var button in MenuItems)
                {
                    button.Draw();
                }
            }
            else
            {
                optionsMenu.Draw(0.91f);
                optionsBackButton.Draw(0.93f);
            }

            ParticleEmitter.Shared.Draw(Game.SpriteBatch);
            Game.SpriteBatch.End();
        }
    }
}
