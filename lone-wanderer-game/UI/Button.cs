using LoneWandererGame.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.UI
{
    public class Button : Clickable
    {
        public enum ButtonState
        {
            Pressed,
            Released,
            Clicked
        }
        public ButtonState State { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public Vector2 Position { get; set; }
        public Game1 Game { get; set; }
        public float Rotation { get; set; } = 0.0f;
        public Vector2 Scale { get; set; } = new Vector2(1f);
        public string Text { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                return (Rectangle)Sprite.GetBoundingRectangle(Position, Rotation, Scale);
            }
        }
        public Vector2 TextSize { get; set; }
        
        public Button(Game1 game) { Game = game; }
        public void LoadContent(string path)
        {
            SpriteSheet spriteSheet = Game.Content.Load<SpriteSheet>(path, new JsonContentLoader());
            Sprite = new AnimatedSprite(spriteSheet);
            State = ButtonState.Released;
            Sprite.Play(State.ToString().ToLower());
            Sprite.Depth = 0.90f;
            //Sprite.Origin = new Vector2(Rectangle.Width / 2, Rectangle.Height / 2);
            TextSize = Game.SilkscreenRegularFont.MeasureString(Text);
        }

        public void changeColour (Color color)
        {
            Sprite.Color = color;
        }
        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            // Prevent stuff from happening if clicking is being done
            if (State == ButtonState.Clicked)
                return;
            if (Game.CustomCursor.MouseState.WasButtonJustUp(MouseButton.Left) && State != ButtonState.Clicked)
            {
                if (Game.CustomCursor.Rectangle.Intersects(Rectangle))
                {
                    State = ButtonState.Clicked;
                    Sprite.Play(State.ToString().ToLower(), () =>
                    {
                        OnClick();
                        State = ButtonState.Released;
                        Sprite.Play(State.ToString().ToLower());
                    });
                }
            }
            else if(Game.CustomCursor.MouseState.IsButtonDown(MouseButton.Left) && State != ButtonState.Pressed)
            {
                if (Game.CustomCursor.Rectangle.Intersects(Rectangle))
                {
                    OnPress();
                    State = ButtonState.Pressed;
                    Sprite.Play(State.ToString().ToLower());

                }
            }
            else if(Game.CustomCursor.MouseState.IsButtonUp(MouseButton.Left) && State != ButtonState.Released)
            {
                State = ButtonState.Released;
                Sprite.Play(State.ToString().ToLower());

            }
            if (!Game.CustomCursor.Rectangle.Intersects(Rectangle))
            {
                State = ButtonState.Released;
                Sprite.Play(State.ToString().ToLower());
            }
        }

        public void Draw()
        {
            Game.SpriteBatch.Draw(Sprite, Position, Rotation, Scale);
            var offset = new Vector2(0, 2);
            var pos = Position;
            if (State == ButtonState.Pressed)
            {
                pos += offset;
            }
            Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, Text, pos, Color.White, 0, TextSize / 2, 1, SpriteEffects.None, 0.91f);
        }
    }
}
