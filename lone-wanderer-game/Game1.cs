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
        public MouseListener MouseListener { get { return _mouseListener; } }
        public KeyboardListener KeyboardListener { get { return _keyboardListener; } }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            LoadTitleScreen();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RegularFont = Content.Load<SpriteFont>("Fonts/regular");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            base.Draw(gameTime);
        }
    }
}