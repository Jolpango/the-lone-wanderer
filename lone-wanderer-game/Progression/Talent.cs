using LoneWandererGame.Entity;
using LoneWandererGame.Spells;
using LoneWandererGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;



namespace LoneWandererGame.Progression
{
    public class Talent
    {
        private Player _player;
        public SpellBook spellBook;

        private Dictionary<string, Action> talentSelections;
        private List<Button> talentButtons;
        private int talentPoints = 0;
        float talentXPos;
        float talentYPos;
        bool talentWasZero;

        //talents affects
        float healthIncrease;
        int speedIncrease;
        float cooldownReduction;


        public Talent(Game1 game, Player player, SpellBook SpellBook)
        {
            _player = player;
            spellBook = SpellBook;
            talentSelections = new Dictionary<string, Action>();
            talentButtons = new List<Button>();
            talentWasZero = false;

            healthIncrease = 1.1f;
            speedIncrease = 20;
            cooldownReduction = 0.98f;

            //What each talent do     
            talentSelections.Add("HP", () => { _player.IncreaseMaxHealthMultiply(healthIncrease); talentPoints--; });
            talentSelections.Add("Speed", () => { _player.SpeedUp(speedIncrease); talentPoints--; });
            talentSelections.Add("Cooldown", () => { spellBook.cooldwonReductionMultiply(cooldownReduction); talentPoints--; });



            talentXPos = game.WindowDimensions.X / (talentSelections.Count + 5);
            talentYPos = game.WindowDimensions.Y / 8 * 7;
            int i = 0;
            foreach (var talent in talentSelections)
            {
                i++;
                Vector2 talentPosition = new Vector2(talentXPos * (i + 2), talentYPos);
                talentButtons.Add(
                new Button(game)
                {
                    Text = talent.Key,
                    OnClick = talent.Value,
                    OnPress = () => { },
                    OnRelease = () => { },
                    Position = talentPosition,
                    Scale = new Vector2(0.2f, 0.5f)
                });
            }
        }
        public void LoadContent()
        {
            foreach (var talentbutton in talentButtons)
            {
                talentbutton.LoadContent("Sprites/UI/button.sf");
            }

        }
        public void chooseTalentOnLevelUp(Game1 game, GameTime gameTime)
        {
            MouseStateExtended mouseState = game.CustomCursor.MouseState;
            bool hasCollide = false;
            foreach (var talent in talentButtons)
            {
                if (talent.Rectangle.Contains(game.CustomCursor.Rectangle))
                {
                    game.CustomCursor.CursorState = CursorState.select;
                    hasCollide = true;
                }
            }

            if (!hasCollide)
            {
                game.CustomCursor.CursorState = CursorState.pointer;
            }

            // if talents was zero change color back.
            if (talentWasZero)
            {
                talentWasZero = false;
                foreach (var talentbutton in talentButtons)
                {
                    talentbutton.setSpriteColor(Color.White);
                }
            }

            // change color if we have zero talents left
            if (talentPoints <= 0)
            {
                talentWasZero = true;
                foreach (var talentbutton in talentButtons)
                {
                    talentbutton.setSpriteColor(new Color(30, 30, 30));
                }
                return;
            } //return if no talents
;


            foreach (var talentbutton in talentButtons)
            {
                talentbutton.Update(gameTime);
            }

        }



        public void OnLevelUp()
        {
            float talentTooAdd = PlayerScore.Level / 3; //talent each level, how many
            if (talentTooAdd < 1)
                talentTooAdd = 1;
            talentPoints += (int)talentTooAdd;
        }
        public void DrawUI(Game1 game, GameTime gameTime)
        {
            int xOffset = 3;
            // HEALTH
            string healthText = $"HP: {_player.Health} -> {_player.Health * healthIncrease}";
            float thirdTextSize = game.RegularFont.MeasureString(healthText).X / 3;

            game.SpriteBatch.DrawString(game.RegularFont, healthText, new Vector2(talentXPos * xOffset - thirdTextSize,
                talentYPos + 30), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);



            string maxHealthText = $"Max: {_player.MAX_HEALTH} -> {_player.MAX_HEALTH * healthIncrease}";
            thirdTextSize = game.RegularFont.MeasureString(maxHealthText).X / 3;

            game.SpriteBatch.DrawString(game.RegularFont, maxHealthText, new Vector2(talentXPos * xOffset - thirdTextSize,
                talentYPos + 50), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);


            xOffset++;
            //SPEED
            string speedText = $"Speed: {_player.getAcceleration()} -> {_player.getAccelerationNextLevel(speedIncrease)}";
            thirdTextSize = game.RegularFont.MeasureString(speedText).X / 3;

            game.SpriteBatch.DrawString(game.RegularFont, speedText, new Vector2(talentXPos * xOffset - thirdTextSize,
                 talentYPos + 30), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);


            xOffset++;
            //COOLDOWN
            string cooldownText = $"Cooldown Reduction: {spellBook.getCooldwonReduction()} -> {spellBook.getCooldwonReductionNextLevel(cooldownReduction)}";
            float fourthTextSize = game.RegularFont.MeasureString(cooldownText).X / 4;

            game.SpriteBatch.DrawString(game.RegularFont, cooldownText, new Vector2(talentXPos * xOffset - fourthTextSize,
                 talentYPos + 30), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);



            foreach (var talentButton in talentButtons)
            {
                talentButton.Draw();
            }
            string talentPointsString = "Talent Points To Spend: ";
            Vector2 size = game.SilkscreenRegularFont.MeasureString(talentPointsString);
            talentPointsString += talentPoints;

            Vector2 pos = new Vector2(game.WindowDimensions.X / 2 - size.X / 2, talentYPos - 50);
            game.SpriteBatch.DrawString(game.SilkscreenRegularFont, talentPointsString, pos, Color.White);

        }
    }
}
