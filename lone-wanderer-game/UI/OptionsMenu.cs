
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LoneWandererGame.UI
{
    public class OptionsMenu
    {
        private Game1 game;

        private Vector2 origin;
        private Vector2 textOrigin;
        private float triggerButtonWidth;

        // Settings
        enum OptionButtons : int
        {
            VolumeIncrease,
            VolumeDecrease,
            ResolutionIncrease,
            ResolutionDecrease,
            FullscreenOff,
            FullscreenOn
        }
        public int VolumeOption { get; set; }
        private const int MAX_VOLUME = 10;
        private const float VOLUME_VALUE_HALF_WIDTH = 90;

        public Point ResolutionOption { get; set; }
        private const float RESOLUTION_VALUE_HALF_WIDTH = 90;
        private int resolutionIndex { get; set; }
        private List<Point> resolutions;

        public bool FullscreenOption { get; set; }

        // UI
        static readonly Vector2 ITEM_DIMENSIONS = new Vector2(590f, 110f);
        private const float ITEM_Y_SPACING = 0f;
        private const float PADDING = 25f;

        private Texture2D optionItemBg;
        private List<Button> triggerButtons;

        public OptionsMenu(Game1 game, Vector2 origin)
        {
            this.game = game;

            // Settings
            VolumeOption = game.Settings.Volume;
            ResolutionOption = new Point((int)game.Settings.Resolution.X, (int)game.Settings.Resolution.Y);
            FullscreenOption = game.Settings.Fullscreen;

            List<Point> resolutionList = new List<Point>()
            {
                new Point(1024, 576),
                new Point(1280, 720),
                new Point(1366, 768),
                new Point(1600, 900),
                new Point(1920, 1080),
                new Point(2560, 1440),
                new Point(3840, 2160)
            };

            resolutions = new List<Point>();
            for (int i = 0; i < resolutionList.Count; i++)
            {
                if (game.MonitorDimensions.X >= resolutionList[i].X)
                {
                    resolutions.Add(resolutionList[i]);
                    if (resolutionList[i].X == ResolutionOption.X)
                    {
                        resolutionIndex = i;
                    }
                }
            }

            // UI
            optionItemBg = game.Content.Load<Texture2D>("Sprites/UI/option_bg_9_scale");
            textOrigin = new Vector2(0f, game.SilkscreenRegularFont.MeasureString("0").Y / 2);

            triggerButtons = new List<Button>()
            {
                new Button(game)
                {
                    Text = "<",
                    OnClick = () => {
                        VolumeOption = Math.Max(0, VolumeOption - 1);
                        game.Settings.Volume = VolumeOption;
                        game.SettingsChanged();
                    },
                    OnPress = () => { },
                    OnRelease = () => { }
                },
                new Button(game)
                {
                    Text = ">",
                    OnClick = () => {
                        VolumeOption = Math.Min(MAX_VOLUME, VolumeOption + 1);
                        game.Settings.Volume = VolumeOption;
                        game.SettingsChanged();
                    },
                    OnPress = () => { },
                    OnRelease = () => { }
                },
                new Button(game)
                {
                    Text = "<",
                    OnClick = () => {
                        resolutionIndex = Math.Max(0, resolutionIndex - 1);
                        ResolutionOption = resolutions[resolutionIndex];
                        game.Settings.Resolution = new Vector2(ResolutionOption.X, ResolutionOption.Y);
                        game.SettingsChanged();
                    },
                    OnPress = () => { },
                    OnRelease = () => { }
                },
                new Button(game)
                {
                    Text = ">",
                    OnClick = () => {
                        resolutionIndex = Math.Min(resolutions.Count - 1, resolutionIndex + 1);
                        ResolutionOption = resolutions[resolutionIndex];
                        game.Settings.Resolution = new Vector2(ResolutionOption.X, ResolutionOption.Y);
                        game.SettingsChanged();
                    },
                    OnPress = () => { },
                    OnRelease = () => { }
                },
                new Button(game)
                {
                    Text = "Off",
                    OnClick = () => { },
                    OnPress = () => {
                        FullscreenOption = false;
                        fullscreenChanged();
                        game.Settings.Fullscreen = FullscreenOption;
                        game.SettingsChanged();
                    },
                    OnRelease = () => { }
                },
                new Button(game)
                {
                    Text = "On",
                    OnClick = () => { },
                    OnPress = () => {
                        FullscreenOption = true;
                        fullscreenChanged();
                        game.Settings.Fullscreen = FullscreenOption;
                        game.SettingsChanged();
                    },
                    OnRelease = () => { }
                }
            };

            for (int i = 0; i < triggerButtons.Count; i++)
            {
                triggerButtons[i].LoadContent("Sprites/UI/small_button.sf");
            }
            triggerButtonWidth = triggerButtons[0].Rectangle.Width;
            
            centerChanged(origin);
            fullscreenChanged();
        }

        public Vector2 getMenuDimensions()
        {
            return new Vector2(0f, (ITEM_DIMENSIONS.Y + ITEM_Y_SPACING) * 3);
        }

        public void centerChanged(Vector2 origin)
        {
            this.origin = origin;

            Vector2 rowOffset = new Vector2(0, ITEM_DIMENSIONS.Y + ITEM_Y_SPACING);
            Vector2 rowRightOrigin = new Vector2(ITEM_DIMENSIONS.X, ITEM_DIMENSIONS.Y) / 2;
            // NOTE(Medo): Button has its origin set to its center position
            float triggerButtonHalfWidth = triggerButtonWidth / 2f;

            Vector2 increaseButtonOffset = rowRightOrigin - new Vector2(PADDING + triggerButtonHalfWidth, 0f);
            Vector2 decreaseButtonOffset = new Vector2(increaseButtonOffset.X - (VOLUME_VALUE_HALF_WIDTH * 2f) - triggerButtonWidth, rowRightOrigin.Y);
            Vector2 position = origin;

            triggerButtons[(int)OptionButtons.VolumeIncrease].Position = position + decreaseButtonOffset;
            triggerButtons[(int)OptionButtons.VolumeDecrease].Position = position + increaseButtonOffset;

            position += rowOffset;
            decreaseButtonOffset = new Vector2(increaseButtonOffset.X - (RESOLUTION_VALUE_HALF_WIDTH * 2f) - triggerButtonWidth, rowRightOrigin.Y);
            triggerButtons[(int)OptionButtons.ResolutionIncrease].Position = position + decreaseButtonOffset;
            triggerButtons[(int)OptionButtons.ResolutionDecrease].Position = position + increaseButtonOffset;

            position += rowOffset;
            decreaseButtonOffset = new Vector2(increaseButtonOffset.X - triggerButtonWidth, rowRightOrigin.Y);

            triggerButtons[(int)OptionButtons.FullscreenOn].Position = position + decreaseButtonOffset;
            triggerButtons[(int)OptionButtons.FullscreenOff].Position = position + increaseButtonOffset;
        }

        private void fullscreenChanged()
        {
            if (FullscreenOption)
            {
                triggerButtons[(int)OptionButtons.FullscreenOn].setEnabled(false);
                triggerButtons[(int)OptionButtons.FullscreenOn].setTextColor(Color.Goldenrod);

                triggerButtons[(int)OptionButtons.FullscreenOff].setEnabled(true);
                triggerButtons[(int)OptionButtons.FullscreenOff].setTextColor(Color.White);


                triggerButtons[(int)OptionButtons.ResolutionIncrease].setEnabled(false);
                triggerButtons[(int)OptionButtons.ResolutionIncrease].setTextColor(Color.DarkGray);
                triggerButtons[(int)OptionButtons.ResolutionDecrease].setEnabled(false);
                triggerButtons[(int)OptionButtons.ResolutionDecrease].setTextColor(Color.DarkGray);
            }
            else
            {
                triggerButtons[(int)OptionButtons.FullscreenOn].setEnabled(true);
                triggerButtons[(int)OptionButtons.FullscreenOn].setTextColor(Color.White);

                triggerButtons[(int)OptionButtons.FullscreenOff].setEnabled(false);
                triggerButtons[(int)OptionButtons.FullscreenOff].setTextColor(Color.Goldenrod);

                triggerButtons[(int)OptionButtons.ResolutionIncrease].setEnabled(true);
                triggerButtons[(int)OptionButtons.ResolutionIncrease].setTextColor(Color.White);
                triggerButtons[(int)OptionButtons.ResolutionDecrease].setEnabled(true);
                triggerButtons[(int)OptionButtons.ResolutionDecrease].setTextColor(Color.White);
            }
        }
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < triggerButtons.Count; i++)
            {
                triggerButtons[i].Update(gameTime);
            }
        }

        private void drawOptionRow(string label, Vector2 position, Vector2 labelTextPosition, float depth)
        {
            Rectangle dstRec = new Rectangle((int)position.X, (int)position.Y, (int)ITEM_DIMENSIONS.X, (int)ITEM_DIMENSIONS.Y);
            game.SpriteBatch.Draw(optionItemBg, dstRec, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            game.SpriteBatch.DrawString(game.SilkscreenRegularFont, label.ToString(), labelTextPosition, Color.White, 0, textOrigin, 1f, SpriteEffects.None, depth + 0.01f);
        }

        public void Draw(float depth)
        {
            float xOffset = ITEM_DIMENSIONS.X / 2f;
            Vector2 position = new Vector2(origin.X - xOffset, origin.Y);
            Vector2 labelTextOffset = new Vector2(PADDING, ITEM_DIMENSIONS.Y / 2f);
            Vector2 valueTextOrigin = new Vector2(ITEM_DIMENSIONS.X, ITEM_DIMENSIONS.Y / 2f);

            drawOptionRow("Volume", position, position + labelTextOffset, depth);

            string valueText = VolumeOption.ToString();
            float valueTextHalfWidth = game.SilkscreenRegularFont.MeasureString(valueText).X / 2;
            float valueTextCenter = PADDING + triggerButtonWidth + VOLUME_VALUE_HALF_WIDTH;
            Vector2 valueTextOffset = new Vector2(valueTextCenter + valueTextHalfWidth, 0f);

            game.SpriteBatch.DrawString(game.SilkscreenRegularFont, valueText, position + valueTextOrigin - valueTextOffset, Color.White, 0, textOrigin, 1f, SpriteEffects.None, depth + 0.01f);
            position.Y += ITEM_DIMENSIONS.Y + ITEM_Y_SPACING;

            drawOptionRow("Resolution", position, position + labelTextOffset, depth);

            if (FullscreenOption)
            {
                valueText = $"{game.WindowDimensions.X} x {game.WindowDimensions.Y}";
            }
            else
            {
                valueText = $"{ResolutionOption.X} x {ResolutionOption.Y}";
            }
            valueTextHalfWidth = game.SilkscreenRegularFont.MeasureString(valueText).X / 2;
            valueTextCenter = PADDING + triggerButtonWidth + RESOLUTION_VALUE_HALF_WIDTH;
            valueTextOffset = new Vector2(valueTextCenter + valueTextHalfWidth, 0f);
            game.SpriteBatch.DrawString(game.SilkscreenRegularFont, valueText, position + valueTextOrigin - valueTextOffset, Color.White, 0, textOrigin, 1f, SpriteEffects.None, depth + 0.01f);
            position.Y += ITEM_DIMENSIONS.Y + ITEM_Y_SPACING;

            drawOptionRow("Fullscreen", position, position + labelTextOffset, depth);
            position.Y += ITEM_DIMENSIONS.Y + ITEM_Y_SPACING;

            for(int i = 0; i < triggerButtons.Count; i++)
            {
                triggerButtons[i].Draw(depth + 0.02f);
            }
        }
    }
}
