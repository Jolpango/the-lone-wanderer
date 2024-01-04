using LoneWandererGame.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace LoneWandererGame.UI
{
    public class Button : Clickable
    {
        public enum ButtonState
        {
            Pressed,
            Released,
            Clicked,
            Disabled
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
        private Color spriteColor = Color.White;
        private Color textColor = Color.White;
        
        public Button(Game1 game) { Game = game; }
        public void LoadContent(string path)
        {
            SpriteSheet spriteSheet = Game.Content.Load<SpriteSheet>(path, new JsonContentLoader());
            Sprite = new AnimatedSprite(spriteSheet);
            State = ButtonState.Released;
            Sprite.Play(State.ToString().ToLower());
            //Sprite.Origin = new Vector2(Rectangle.Width / 2, Rectangle.Height / 2);
            TextSize = Game.SilkscreenRegularFont.MeasureString(Text);
        }

        public void setEnabled(bool enabled)
        {
            if (enabled)
            {
                State = ButtonState.Released;
                Sprite.Play(State.ToString().ToLower());
                Sprite.Color = spriteColor;
            }
            else
            {
                State = ButtonState.Disabled;
                Sprite.Play(ButtonState.Pressed.ToString().ToLower());
                Sprite.Color = Color.DarkGray;
            }

        }
        public void setSpriteColor(Color color)
        {
            spriteColor = color;
            Sprite.Color = spriteColor;
        }
        public void setTextColor(Color color)
        {
            textColor = color;
        }
        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);

            // Prevent stuff from happening if clicking is being done or not enabled
            if (State == ButtonState.Clicked || State == ButtonState.Disabled)
                return;
            if (Game.CustomCursor.MouseState.WasButtonJustUp(MouseButton.Left) && State != ButtonState.Clicked)
            {
                if (Game.CustomCursor.Rectangle.Intersects(Rectangle))
                {
                    State = ButtonState.Clicked;
                    Sprite.Play(State.ToString().ToLower(), () =>
                    {
                        OnClick();
                        if (State == ButtonState.Disabled)
                        {
                            return;
                        }
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
                    if (State == ButtonState.Disabled)
                    {
                        return;
                    }
                    State = ButtonState.Pressed;
                    Sprite.Play(State.ToString().ToLower());

                }
            }
            else if(Game.CustomCursor.MouseState.IsButtonUp(MouseButton.Left) && State != ButtonState.Released)
            {
                State = ButtonState.Released;
                Sprite.Play(State.ToString().ToLower());

            }
            if (State != ButtonState.Disabled && !Game.CustomCursor.Rectangle.Intersects(Rectangle))
            {
                State = ButtonState.Released;
                Sprite.Play(State.ToString().ToLower());
            }
        }

        public void Draw(float depth = 0.90f)
        {
            Sprite.Depth = depth;
            Game.SpriteBatch.Draw(Sprite, Position, Rotation, Scale);
            var offset = new Vector2(0, 2);
            var pos = Position;
            if (State == ButtonState.Pressed)
            {
                pos += offset;
            }
            Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, Text, pos, textColor, 0, TextSize / 2, 1, SpriteEffects.None, depth + 0.01f);
        }
    }
}
