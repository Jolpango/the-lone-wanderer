using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LoneWandererGame
{
    public class FillableBar
    {
        private float percentage;
        private int currentValue;
        public int CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                percentage = (float)currentValue / (float)MaxValue;
            }
        }
        public int MaxValue { get; set; }
        public int BarWidth { get; set; }
        public int BarHeight { get; set; }
        public Color BarForegroundColor { get; set; } = Color.Green;
        public Color BarBackgroundColor { get; set; } = Color.Red;

        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                position.X -= BarWidth / 2;
            }
        }
        public Texture2D Texture { get; set; }
        public Game1 Game { get; set; }
        public FillableBar(){ }

        public void CreateTexture()
        {
            Texture2D tempTexture = new Texture2D(Game.GraphicsDevice, BarWidth, BarHeight);
            Color[] data = new Color[BarWidth * BarHeight];
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.White;
            tempTexture.SetData(data);
            Texture = tempTexture;
        }

        public void Draw()
        {
            int width = (int)(BarWidth * percentage);
            Game.SpriteBatch.Draw(
                Texture,
                Position,
                new Rectangle(0, 0, width, BarHeight),
                BarForegroundColor,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                1f);
            Game.SpriteBatch.Draw(
                Texture,
                Position,
                new Rectangle(0, 0, BarWidth, BarHeight),
                BarBackgroundColor,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.9f);
        }
    }
}
