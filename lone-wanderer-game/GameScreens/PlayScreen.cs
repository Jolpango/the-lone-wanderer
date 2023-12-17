using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using LoneWandererGame.Entity;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using LoneWandererGame.Enemy;
using LoneWandererGame.TileEngines;
using LoneWandererGame.Spells;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using LoneWandererGame.Powerups
using MonoGame.Jolpango.Graphics;

namespace LoneWandererGame.GameScreens
{
    public enum PlayState
    {
        Playing,
        Paused,
        LevelUp,
        GameOver
    }
    public class PlayScreen : GameScreen
    {
        private new Game1 Game => (Game1)base.Game;

        private Player _player;
        private OrthographicCamera _camera;
        private TileEngine tileEngine;
        private PowerupHandler powerupHandler;
        private EnemyHandler enemyHandler;
        public SpellBook SpellBook;
        public PlayState State { get; set; }
        public List<Spell> ActiveSpells;
        public List<SpellDefinition> SpellDefinitions { get; private set; }
        public SpellCollisionHandler SpellCollisionHandler { get; private set; }
        public FloatingTextHandler FloatingTextHandler { get; private set; }

        private FillableBar playerHealthBar;
        private FillableBar xpBar;

        private Song backgroundMusic;
        private int spellChoices = 2;
        private float uiScoreCooldown = 1.0f;
        private int savedShowExp = 0;
        private Queue<int> frameTimes = new Queue<int>();

        private Random rnd;
        List<SpellDefinition> randomSpells;
        List<SpellSelection> spellSelections;

        public PlayScreen(Game1 game) : base(game)
        {
            var viewportAdapter = new BoxingViewportAdapter(Game.Window, Game.GraphicsDevice, (int)Game.WindowDimensions.X, (int)Game.WindowDimensions.Y);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.ZoomIn(0.5f);

            tileEngine = new TileEngine(Game);
            // Assumed map width and height is 512
            float tilemapCenter = (512 * 32) / 2f;
            _player = new Player(Game, tileEngine, new Vector2(tilemapCenter, tilemapCenter));
            powerupHandler = new PowerupHandler(Game, _player);
            enemyHandler = new EnemyHandler(Game, tileEngine, _camera);
            ActiveSpells = new List<Spell>();
            SpellBook = new SpellBook(Game, _player, ActiveSpells, enemyHandler);
            SpellDefinitions = new List<SpellDefinition>();
            FloatingTextHandler = new FloatingTextHandler(Game);
            SpellCollisionHandler = new SpellCollisionHandler(Game, tileEngine, enemyHandler, ActiveSpells, FloatingTextHandler);
            rnd = new Random();
            spellSelections = new List<SpellSelection>();
            State = PlayState.Playing;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            _player.LoadContent();

            enemyHandler.LoadContent();
            tileEngine.LoadContent();
            powerupHandler.LoadContent();
            SpellDefinitions = SpellLoader.LoadSpells(Game);
            foreach(var spell in SpellDefinitions)
            {
                if (spell.Name == "Fireball")
                    SpellBook.AddSpell(spell);
            }

            Vector2 windowDimensions = Game.WindowDimensions;
            int padding = 0;
            playerHealthBar = new FillableBar()
            {
                BarWidth = (int)windowDimensions.X - padding,
                BarHeight = 20,
                MaxValue = Player.MAX_HEALTH,
                Game = Game,
                CurrentValue = _player.Health,
                Position = new Vector2((int)(windowDimensions.X - padding) / 2, padding)
            };
            playerHealthBar.CreateTexture();
            xpBar = new FillableBar()
            {
                BarWidth = (int)windowDimensions.X - padding,
                BarHeight = 20,
                MaxValue = PlayerScore.RequiredXP,
                Game = Game,
                BarBackgroundColor = Color.LightBlue,
                BarForegroundColor = Color.Blue,
                CurrentValue = PlayerScore.XP,
                Position = new Vector2((int)(windowDimensions.X - padding) / 2, padding + 20)
            };
            xpBar.CreateTexture();
            PlayerScore.OnGainXp = GainXpFloatingText;
            PlayerScore.OnLevelUp = OnLevelUp;
            backgroundMusic = Game.Content.Load<Song>("Sounds/stage1");
            MediaPlayer.Volume = 0.01f;
            MediaPlayer.Play(backgroundMusic);

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (State == PlayState.Playing)
            {
                UpdatePlaying(gameTime);
            }
            else if (State == PlayState.LevelUp)
            {
                chooseSpellOnLevelUp();
            }
            else if (State == PlayState.Paused)
            {
                UpdatePaused(gameTime);
            }
            else if (State == PlayState.GameOver)
            {
                UpdateGameOver(gameTime);
            }
        }

        private void UpdatePaused(GameTime gameTime)
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
            if (keyboardState.WasKeyJustDown(Keys.P))
            {
                State = PlayState.Playing;
            }
        }

        private void UpdateGameOver(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                Game.LoadDeathScreen(SpellBook);
            }
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
            if (keyboardState.WasKeyJustDown(Keys.P))
            {
                State = PlayState.Paused;
            }
            _player.Update(gameTime);

            ParticleEmitter.Shared.Update(gameTime);
            _camera.LookAt(_player.Position);
            enemyHandler.Update(gameTime, _player);

            UpdateSpells(gameTime);

            FloatingTextHandler.Update(gameTime);

            playerHealthBar.CurrentValue = _player.Health;
            xpBar.CurrentValue = PlayerScore.XP;
            xpBar.MaxValue = PlayerScore.RequiredXP;
            powerupHandler.Update(gameTime);

            if (_player.Health <= 0)
            {
                Game.LightHandler.clearLights();
                State = PlayState.GameOver;
            }

            if (uiScoreCooldown>0f)
                uiScoreCooldown -= gameTime.GetElapsedSeconds();
        }

        private void UpdateSpells(GameTime gameTime)
        {
            SpellCollisionHandler.Update();
            List<Spell> spellsToRemove = new List<Spell>();
            foreach (var spell in ActiveSpells)
            {
                spell.Update(gameTime);
                if (spell.Timer < 0)
                    spellsToRemove.Add(spell);
            }

            foreach (var spell in spellsToRemove)
                ActiveSpells.Remove(spell);

            if (_player.Health != 0)
                SpellBook.Update(gameTime);
        }

        public void GainXpFloatingText(int xp)
        {
            savedShowExp += xp; 
            if (uiScoreCooldown <= 0f)
            {
                FloatingTextHandler.AddText(savedShowExp.ToString(), _player.Position, Color.White);
                uiScoreCooldown = 1f;
                savedShowExp = 0;
            }
        }

        public void OnLevelUp()
        {
            FloatingTextHandler.AddText("Level up", _player.Position, Color.Green);
            randomSpells = new List<SpellDefinition>();
            randomSpells.AddRange(SpellDefinitions.Where(x => !SpellBook.IsInSpellBookMax(x.Name)).OrderBy(x => rnd.Next()).Take(spellChoices));
            if (randomSpells.Count <= 0)
            {
                return;
            }
            State = PlayState.LevelUp;
            int i = 0;
            spellSelections = new List<SpellSelection>();
            foreach (var spell in randomSpells)
            {
                SpellDefinition existingSpell = SpellBook.Spells.Where(s => s.Name == spell.Name).FirstOrDefault();
                if(existingSpell != null)
                {
                    float splitter = 350;
                    Vector2 middle = Game.WindowDimensions / 2;
                    Vector2 startOffset = new Vector2(splitter - (splitter / 4), 256);
                    Vector2 offset = new Vector2(i * splitter, 0);
                    Vector2 pos = middle - startOffset + offset;
                    spellSelections.Add(new SpellSelection()
                    {
                        Position = pos,
                        SpellDefinition = existingSpell,
                        HasSpell = true,
                    });
                    spellSelections[i].LoadContent(Game.Content);
                }
                else
                {
                    float splitter = 350;
                    Vector2 middle = Game.WindowDimensions / 2;
                    Vector2 startOffset = new Vector2(splitter - (splitter / 4), 256);
                    Vector2 offset = new Vector2(i * splitter, 0);
                    Vector2 pos = middle - startOffset + offset;
                    spellSelections.Add(new SpellSelection()
                    {
                        Position = pos,
                        SpellDefinition = spell,
                    });
                    spellSelections[i].LoadContent(Game.Content);
                }
                i++;
            }
        }

        private void chooseSpellOnLevelUp()
        {
            MouseStateExtended mouseState = Game.CustomCursor.MouseState;
            bool hasCollide = false;
            foreach(var spell in spellSelections)
            {
                if (spell.Rectangle.Contains(Game.CustomCursor.Rectangle))
                {
                    Game.CustomCursor.CursorState = CursorState.select;
                    hasCollide = true;
                    spell.Color = Color.Gold;
                    spell.ColorBox = Color.Gray;
                }
                else
                {
                    spell.Color = Color.White;
                    spell.ColorBox = Color.White;
                }
            }
            if (!hasCollide)
            {
                Game.CustomCursor.CursorState = CursorState.pointer;
            }
            if (mouseState.WasButtonJustDown(MouseButton.Left))
            {
                foreach (var spell in spellSelections)
                {
                    if (spell.Rectangle.Contains(Game.CustomCursor.Rectangle))
                    {
                        if (SpellBook.IsSpellInSpellBook(spell.SpellDefinition) >= 0)
                            SpellBook.LevelUpSpell(spell.SpellDefinition.Name);
                        else
                            SpellBook.AddSpell(spell.SpellDefinition);
                        State = PlayState.Playing;
                        Game.CustomCursor.CursorState = CursorState.pointer;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(new Color(new Vector3(0.23f, 0.42f, 0.12f)));

            var transformMatrix = _camera.GetViewMatrix();
            Game.SpriteBatch.Begin(effect: Game.SpriteEffect, sortMode: SpriteSortMode.FrontToBack, transformMatrix: transformMatrix);
            DrawWorld(gameTime);
            Game.SpriteBatch.End();

            // Lights
            Game.SpriteEffect.Parameters["ambient_intensity"].SetValue(new Vector4(1f, 1f, 1f, 0.3f));
            Game.SpriteBatch.Begin(effect: Game.LightEffect, sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.Additive, transformMatrix: transformMatrix);
            DrawLights();
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack);
            DrawUI(gameTime);

            Game.CustomCursor.Draw();
            Game.SpriteBatch.End();
        }

        private void DrawWorld(GameTime gameTime)
        {
            _player.Draw();
            tileEngine.Draw(_camera.BoundingRectangle);
            powerupHandler.Draw();
            enemyHandler.Draw(gameTime);
            ParticleEmitter.Shared.Draw(Game.SpriteBatch);
            SpellBook.Draw();
            foreach (var spell in ActiveSpells)
            {
                spell.Draw(Game.SpriteBatch, Game);
            }
            FloatingTextHandler.Draw(Game.SpriteBatch);
        }

        private void DrawLights()
        {
            List<Light> lights = Game.LightHandler.getLights();
            for (int i = 0; i < lights.Count; i++)
            {
                int lightSize = lights[i].size;
                Vector2 lightOrigin = new Vector2((float)lights[i].size / 2f, (float)lights[i].size / 2f);
                Vector2 lightPosition = lights[i].position - lightOrigin;
                Color lightColor = new Color(lights[i].color, lights[i].intensity);

                Game.SpriteBatch.Draw(Game.LightHandler._blankTexture, new Rectangle((int)lightPosition.X, (int)lightPosition.Y, lightSize, lightSize), lightColor);
            }
        }

        private void DrawUI(GameTime gameTime)
        {
            Game.SpriteBatch.DrawString(Game.RegularFont, $"Level: {PlayerScore.Level}", new Vector2(10f, 40f), Color.White);
            Game.SpriteBatch.DrawString(Game.RegularFont, $"Score: {PlayerScore.Score}", new Vector2(10f, 80f), Color.White);
            float screenWidth = Game.WindowDimensions.X;
            if (_player.godMode)
            {
                string godModeString = "God Mode ON!";
                Vector2 godModeStrSize = Game.RegularFont.MeasureString(godModeString);
                Game.SpriteBatch.DrawString(Game.RegularFont, godModeString, new Vector2(screenWidth - godModeStrSize.X - 10f, 100f), Color.Red);
            }
            playerHealthBar.Draw();
            xpBar.Draw();

            // FPS Counter
            {
                while (frameTimes.Count > 100)
                    frameTimes.Dequeue();

                frameTimes.Enqueue((int)((1 / gameTime.ElapsedGameTime.TotalSeconds) + 0.01));

                int averageFps = 0;
                foreach (int frametime in frameTimes)
                    averageFps += frametime;
                averageFps /= frameTimes.Count;

                string fpsString = $"FPS: {averageFps}";
                Vector2 size = Game.SilkscreenRegularFont.MeasureString(fpsString);

                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, fpsString, new Vector2(screenWidth - size.X - 10f, 40f), Color.White);
            }

            //Level up
            if (State == PlayState.LevelUp)
            {
                foreach (var spellSelection in spellSelections)
                {
                    spellSelection.Draw(Game.SpriteBatch, Game.SilkscreenRegularFont);
                }
            }
            else if (State == PlayState.GameOver)
            {
                string text = "Game Over";
                Vector2 origin = Game.SilkscreenRegularFont.MeasureString(text) / 2;
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, text, Game.WindowDimensions / 2, Color.White, 0, origin, 1, SpriteEffects.None, 0.99f);
                text = "Press [Enter] to continue";
                origin = Game.SilkscreenRegularFont.MeasureString(text) / 2;
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, text, Game.WindowDimensions / 2 + new Vector2(0, 50), Color.White, 0, origin, 1, SpriteEffects.None, 0.99f);
                
                Texture2D tempTexture = new Texture2D(Game.GraphicsDevice, (int)Game.WindowDimensions.X, (int)Game.WindowDimensions.Y);
                Color[] data = new Color[(int)Game.WindowDimensions.X * (int)Game.WindowDimensions.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                    tempTexture.SetData(data);
                Game.SpriteBatch.Draw(tempTexture, Vector2.Zero, null, Color.White * 0.5f, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.98f);
            }
            else if (State == PlayState.Paused)
            {
                string text = "Paused";
                Vector2 origin = Game.SilkscreenRegularFont.MeasureString(text) / 2;
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, text, Game.WindowDimensions / 2, Color.White, 0, origin, 1, SpriteEffects.None, 0.99f);
                text = "Press [P] to continue";
                origin = Game.SilkscreenRegularFont.MeasureString(text) / 2;
                Game.SpriteBatch.DrawString(Game.SilkscreenRegularFont, text, Game.WindowDimensions / 2 + new Vector2(0, 50), Color.White, 0, origin, 1, SpriteEffects.None, 0.99f);

                Texture2D tempTexture = new Texture2D(Game.GraphicsDevice, (int)Game.WindowDimensions.X, (int)Game.WindowDimensions.Y);
                Color[] data = new Color[(int)Game.WindowDimensions.X * (int)Game.WindowDimensions.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                tempTexture.SetData(data);
                Game.SpriteBatch.Draw(tempTexture, Vector2.Zero, null, Color.White * 0.5f, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.98f);
            }
        }
    }
}
