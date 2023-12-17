using LoneWandererGame.GameScreens;
using LoneWandererGame.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public Effect SpriteEffect { get; private set; }
        public Effect LightEffect { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public LightHandler LightHandler { get; private set; }

        public SpriteFont RegularFont { get; private set; }
        public SpriteFont BoldFont { get; private set; }
        public SpriteFont SilkscreenBoldFont { get; private set; }
        public SpriteFont SilkscreenRegularFont { get; private set; }

        public MouseListener MouseListener { get { return _mouseListener; } }
        public KeyboardListener KeyboardListener { get { return _keyboardListener; } }

        public CustomCursor CustomCursor { get; private set; }

        public Vector2 WindowDimensions
        {
            get { return new Vector2((float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height); }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            graphics.ApplyChanges();

            LightHandler = new LightHandler(this);

            _screenManager = new ScreenManager();
            _keyboardListener = new KeyboardListener();
            _mouseListener = new MouseListener();
            Components.Add(_screenManager);
            Components.Add(new InputListenerComponent(this, _keyboardListener, _mouseListener));

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            CustomCursor = new CustomCursor(this);

            SpriteEffect = Content.Load<Effect>("Effects/SpriteShader");
            LightEffect = Content.Load<Effect>("Effects/LightShader");
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
        public void LoadDeathScreen(SpellBook spellBook)
        {
            _screenManager.LoadScreen(new DeathScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
        }


        protected override void Initialize()
        {
            base.Initialize();
            LoadPlayScreen();// LoadTitleScreen();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RegularFont = Content.Load<SpriteFont>("Fonts/regular");
            BoldFont = Content.Load<SpriteFont>("Fonts/bold");
            SilkscreenBoldFont = Content.Load<SpriteFont>("Fonts/silkscreen-bold");
            SilkscreenRegularFont = Content.Load<SpriteFont>("Fonts/silkscreen-regular");
            CustomCursor.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            CustomCursor.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            base.Draw(gameTime);
        }
    }
}