using LoneWandererGame.MongoDBManagers;
using LoneWandererGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using MonoGame.Jolpango.Graphics;
using System;
using System.Collections.Generic;

namespace LoneWandererGame.GameScreens
{
    public class MenuScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;
        private Song backgroundMusic;
        public MenuScreen(Game1 game): base(game)
        {
            Vector2 center = Game.WindowDimensions / 2;
            center.Y -= 92;
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
                    OnClick = async () => {
                        List<string> highscores = await MongoDBManager.Instance.GetHighScores();
                        foreach (var highscore in highscores)
                            Console.WriteLine(highscore.ToString());
                    },
                    OnPress = () => {},
                    OnRelease = () => {},
                    Position = center + new Vector2(0, 80)
                },
                new Button(Game)
                {
                    Text = "Exit",
                    OnClick = Game.Exit,
                    OnPress = () => {},
                    OnRelease = () => {},
                    Position = center + new Vector2(0, 160)
                }
            };
            foreach (var item in MenuItems)
            {
                item.LoadContent("Sprites/UI/button.sf");
            }
        }

        public List<Button> MenuItems { get; set; }

        private Texture2D background;

        public override void LoadContent()
        {
            base.LoadContent();
            MediaPlayer.Volume = 0.01f;
            MediaPlayer.IsRepeating = true;
            backgroundMusic = Game.Content.Load<Song>("Sounds/menuchip");
            MediaPlayer.Play(backgroundMusic);
            background = Game.Content.Load<Texture2D>("Sprites/menubackground");
            ParticleEmitter.Shared.LayerDepth = 0.98f;
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var item in MenuItems)
            {
                item.Update(gameTime);
            }
            ParticleEmitter.Shared.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Gray);
            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack);
            Game.CustomCursor.Draw();
            Game.SpriteBatch.Draw(background, new Rectangle(0, 0, (int)Game.WindowDimensions.X, (int)Game.WindowDimensions.Y), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            foreach(var button in MenuItems)
            {
                button.Draw();
            }
            ParticleEmitter.Shared.Draw(Game.SpriteBatch);
            Game.SpriteBatch.End();
        }
    }
}
