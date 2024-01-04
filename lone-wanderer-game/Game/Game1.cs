using LoneWandererGame.GameScreens;
using LoneWandererGame.Spells;
using LoneWandererGame.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Jolpango.Graphics;
using LoneWandererGame.UI;
using LoneWandererGame.Entity;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace LoneWandererGame
{
    public class Game1 : Game
    {

        private GraphicsDeviceManager graphics;
        private readonly ScreenManager _screenManager;
        private readonly KeyboardListener _keyboardListener;
        private readonly MouseListener _mouseListener;

        public SettingsHandler SettingsHandler { get; private set; }
        public SettingsData Settings { get; private set; }
        public bool SettingsUpdated { get; set; }
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

        public static Game1 Game { get; set; }

        public CustomCursor CustomCursor { get; private set; }

        public Vector2 MonitorDimensions
        {
            get { return new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height); }
        }

        public Vector2 WindowDimensions
        {
            get {
                if (graphics.IsFullScreen)
                {
                    return MonitorDimensions;
                }
                else
                {
                    return new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height); 
                }
            }
        }

        public Game1()
        {
            SettingsHandler = new SettingsHandler();
            Settings = SettingsHandler.Load();
            SoundEffect.MasterVolume = Settings.Volume / 10f;
            MediaPlayer.Volume = 0.5f * Settings.Volume / 10f;
            SettingsUpdated = false;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)Settings.Resolution.X;
            graphics.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
            graphics.HardwareModeSwitch = false; // Allways borderless fullscreen because exclusive fullscreen sucks and is slower
            graphics.IsFullScreen = Settings.Fullscreen;

            // graphics.SynchronizeWithVerticalRetrace = false;
            // IsFixedTimeStep = false;
            graphics.ApplyChanges();

            // Reapply resolution if loaded resolution setting is higher then size
            if (MonitorDimensions.X < (int)Settings.Resolution.X)
            {
                Settings.Resolution = new Vector2(  GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 
                                                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                SettingsHandler.Save(Settings);
                graphics.PreferredBackBufferWidth = (int)Settings.Resolution.X;
                graphics.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
                graphics.ApplyChanges();
            }

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


            Texture2D tempTexture = new Texture2D(GraphicsDevice, 2, 2);
            Color[] data = new Color[2 * 2];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            tempTexture.SetData(data);
            ParticleEmitter.Shared = new ParticleEmitter(tempTexture);
            Game = this;
        }

        public void SettingsChanged()
        {
            SoundEffect.MasterVolume = Settings.Volume / 10f;
            MediaPlayer.Volume = 0.5f * Settings.Volume / 10f;

            graphics.PreferredBackBufferWidth = (int)Settings.Resolution.X;
            graphics.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
            graphics.IsFullScreen = Settings.Fullscreen;

            graphics.ApplyChanges();
            SettingsHandler.Save(Settings);
            SettingsUpdated = true;
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
            LoadMenuScreen();
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