using LoneWandererGame.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace LoneWandererGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private readonly ScreenManager _screenManager;
        private readonly KeyboardListener _keyboardListener;
        private readonly MouseListener _mouseListener;

        public SpriteBatch SpriteBatch { get; private set; }

        public SpriteFont RegularFont { get; private set; }
        public SpriteFont BoldFont { get; private set; }
        public SpriteFont SilkscreenBoldFont { get; private set; }
        public SpriteFont SilkscreenRegularFont { get; private set; }

        public MouseListener MouseListener { get { return _mouseListener; } }
        public KeyboardListener KeyboardListener { get { return _keyboardListener; } }

        public Vector2 WindowDimensions
        {
            get { return new Vector2((float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height); }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            _screenManager = new ScreenManager();
            _keyboardListener = new KeyboardListener();
            _mouseListener = new MouseListener();
            Components.Add(_screenManager);
            Components.Add(new InputListenerComponent(this, _keyboardListener, _mouseListener));

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void LoadTitleScreen()
        {
            _screenManager.LoadScreen(new TitleScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
        }

        public void LoadPlayScreen()
        {
            _screenManager.LoadScreen(new PlayScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
        }

        public void LoadMenuScreen()
        {
            _screenManager.LoadScreen(new MenuScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
        }
        public void LoadDeathScreen()
        {
            _screenManager.LoadScreen(new DeathScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
        }


        protected override void Initialize()
        {
            base.Initialize();
            LoadMenuScreen();// LoadTitleScreen();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RegularFont = Content.Load<SpriteFont>("Fonts/regular");
            BoldFont = Content.Load<SpriteFont>("Fonts/bold");
            SilkscreenBoldFont = Content.Load<SpriteFont>("Fonts/silkscreen-bold");
            SilkscreenRegularFont = Content.Load<SpriteFont>("Fonts/silkscreen-regular");
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            base.Draw(gameTime);
        }
    }
}