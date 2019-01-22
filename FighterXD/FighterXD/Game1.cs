using FighterXD.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace FighterXD
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World world;
        World menuWorld;
        World optionWorld;

        enum GameState { Menu, Game, Option }

        GameState state;

        Player player1;

        Rectangle window;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1080;
            
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            window = Window.ClientBounds;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            switch (state)
            {
                case GameState.Menu:
                    if (menuWorld != null)
                        menuWorld.viewport.ratio = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
                    break;
                case GameState.Game:
                    if (world != null)
                        world.viewport.ratio = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
                    break;
                case GameState.Option:
                    if (optionWorld != null)
                        optionWorld.viewport.ratio = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
                    break;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        string savePath;
        void SaveSettings()
        {
            string s = (graphics.IsFullScreen ? "X" : "D") + (int)(volume * 100);
            
            if (!File.Exists(savePath))
            {
                File.Create(savePath);
            }

            StreamWriter writer = File.CreateText(savePath);
            writer.Write(s);
            writer.Close();
        }

        void LoadSettings()
        {
            if (File.Exists(savePath))
            {
                string s = File.ReadAllText(savePath);

                string[] str = s.Split('\n');
                if (str.Length > 0)
                {
                    if (str[0].Length > 1)
                    {
                        if (str[0][0] == 'X')
                        {
                            ToggleFullscreen();
                        }
                        int i = int.Parse(str[0].Substring(1));
                        volume = i / 100f;
                    }
                }
            }
        }
        
        protected override void LoadContent()
        {
            DateTime t = DateTime.Now;
            Console.WriteLine("Starting to load assets");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            string[] content = new string[] { "background", "Rocket_Launcher", "floor", "Bullet", "eye", "blob", "Portal"};
            savePath = Directory.GetCurrentDirectory() + @"\Save\settings";
            LoadSettings();
            foreach (string s in content)
            {
                textures.Add(s, Content.Load<Texture2D>(s));
            }

            content = new string[] { "buttonClick", "deathSound","Shoot","hit", };

            foreach (string s in content)
            {
                sounds.Add(s, Content.Load<SoundEffect>(s));
            }

            font = Content.Load<SpriteFont>("font");

            Console.WriteLine("Assets loaded, it took " + (DateTime.Now.Subtract(t).TotalMilliseconds) + " milliseconds");

            state = GameState.Menu;

            LoadMenuWorld();

            LoadGameWorld();


            LoadOptionWorld();
        }

        static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        static SpriteFont font;

        public static Texture2D GetTexture(string name)
        {
            if (textures.TryGetValue(name, out Texture2D tex))
                return tex;
            return null;
        }

        

        static float volume = 1f;

        public static void PlaySound(string name)
        {
            if (sounds.TryGetValue(name, out SoundEffect sound))
                sound.Play(volume, 0, 0);
        }

        public static SpriteFont GetFont()
        {
            return font;
        }

        void LoadMenuWorld()
        {

            DateTime t = DateTime.Now;
            Console.WriteLine("Starting to load menu");

            menuWorld = new World(null, new Vector2(1920, 1080), new Main.Viewport(Vector2.Zero, new Vector2(1920, 1080), 0.5f));
            Button b1 = new Button(textures["floor"], "Play", new Vector2(0, -200), new Vector2(200, 100), font, Color.LightGray, Color.WhiteSmoke, Color.Gray, Color.Black, () => { state = GameState.Game; });
            Button b2 = new Button(textures["floor"], "Reset", new Vector2(0, -70), new Vector2(200, 100), font, Color.LightGray, Color.WhiteSmoke, Color.Gray, Color.Black, () => { LoadGameWorld(); });
            Button b3 = new Button(textures["floor"], "Options", new Vector2(0, 70), new Vector2(200, 100), font, Color.LightGray, Color.WhiteSmoke, Color.Gray, Color.Black, () => { state = GameState.Option; });
            Button b4 = new Button(textures["floor"], "Quit", new Vector2(0, 200), new Vector2(200, 100), font, Color.LightGray, Color.WhiteSmoke, Color.Gray, Color.Black, Exit);
            menuWorld.Initialize(b1);
            menuWorld.Initialize(b2);
            menuWorld.Initialize(b3);
            menuWorld.Initialize(b4);


            Console.WriteLine("menu loaded, it took " + (DateTime.Now.Subtract(t).TotalMilliseconds) + " milliseconds");
        }

        void LoadOptionWorld()
        {
            DateTime t = DateTime.Now;
            Console.WriteLine("Starting to load options");


            optionWorld = new World(null, new Vector2(1920, 1080), new Main.Viewport(Vector2.Zero, new Vector2(1920, 1080), 0.5f));
            Button b1 = new Button(textures["floor"], "Toggle Fullscreen", new Vector2(0, -200), new Vector2(400, 100), font, Color.LightGray, Color.WhiteSmoke, Color.Gray, Color.Black, ToggleFullscreen);
            optionWorld.Initialize(b1);

            TextObject soundfxprcnt = new TextObject(font, new Vector2(600,50));
            soundfxprcnt.text = "percent";
            soundfxprcnt.color = Color.White;
            Slider r = new Slider(volume, 0, 0.5f, (float f) => { volume = f; soundfxprcnt.text = (f * 100) + "%"; }, 
                textures["floor"], textures["floor"], new Vector2(50, 50), new Vector2(800, 80), 80);
            optionWorld.Initialize(r);

            TextObject soundfx = new TextObject(font, new Vector2(-600, 50));
            soundfx.text = "sound Effects";
            soundfx.color = Color.White;
            optionWorld.Initialize(soundfx);

            TextObject musicprcnt = new TextObject(font, new Vector2(600, 50));
            musicprcnt.text = "percent";
            musicprcnt.color = Color.White;
            Slider m = new Slider(volume, 0, 0.5f, (float f) => { MediaPlayer.Volume = f; musicprcnt.text = (f * 100) + "%"; },
               textures["floor"], textures["floor"], new Vector2(50, 150), new Vector2(800, 80), 80);
            optionWorld.Initialize(m);

            TextObject music = new TextObject(font, new Vector2(-600, 150));
            music.text = "Music";
            music.color = Color.White;
            optionWorld.Initialize(music);


            Console.WriteLine("game loaded, it took " + (DateTime.Now.Subtract(t).TotalMilliseconds) + " milliseconds");
        }

        void LoadGameWorld()
        {

            DateTime time = DateTime.Now;
            Console.WriteLine("Starting to load game");


            world = new World(textures["background"], new Vector2(4000, 4000), new Main.Viewport(new Vector2(0, 0), new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height), 2));
            

            Terrain t = new Terrain(new Vector2(0, 200), new Vector2(800, 500), 5, textures["floor"]);
            world.Initialize(t);

            player1 = new Player(new PlayerInfo(Keys.A, Keys.D, Keys.W, Keys.R, Keys.S, 30000, 1000, 0.2f, 20, textures["Bullet"], 2000, 0.1f), new CircleCollider(1), textures["blob"], new Vector2(500, 0), new Vector2(100, 100)) { color = Color.Blue, depth = -1 };
            player1.Collider.SetSize();
            

            RotateTowardsMouse eye1 = new RotateTowardsMouse(textures["eye"], new Vector2(20, -5), new Vector2(20, 20), 0, new Vector2(0, 0), false)
            {
                Parent = player1
            };
            RotateTowardsMouse eye2 = new RotateTowardsMouse(textures["eye"], new Vector2(-20, -5), new Vector2(20, 20), 0, new Vector2(0, 0), false)
            {
                Parent = player1
            };

            TextObject text = new TextObject(font, new Vector2(0, -80), 0, 0.4f)
            {
                Parent = player1,
                text = "XD",
                color = Color.Black,
                depth = 2,
            };

            RotateTowardsMouse o = new RotateTowardsMouse() { Parent = player1 };

            GameObject weapon = new GameObject(new Vector2(player1.spriteSize.Y / 5, player1.spriteSize.Y / -2), -XMath.pi / 2, textures["Rocket_Launcher"], new Vector2(100, 50)) { Parent = o, depth = 1 };

            Main.Object w = new Main.Object(new Vector2(50, 0)) { Parent = weapon };

            player1.InitShootingPlace(w);

            world.Initialize(player1);


            Enemy zombie = new Enemy(textures["blob"], new CircleCollider(1), new Vector2(700, 0), new Vector2(100, 100), 500, player1, 3) { depth = -2 };
            zombie.Collider.SetSize();

            TextObject eText = new TextObject(font, new Vector2(0, -80), 0, 0.4f)
            {
                Parent = zombie,
                text = "xd",
                color = Color.Black,
                depth = 2
            };

            Enemy small = (Enemy)zombie.CreateCopy();
            small.spriteSize = new Vector2(30, 30);
            small.Collider.SetSize();
            small.color = Color.Red;
            small.children[0].Position = new Vector2(0, -40);
            Enemy big = (Enemy)zombie.CreateCopy();
            big.spriteSize = new Vector2(300, 300);
            big.Collider.SetSize();
            big.color = Color.Yellow;
            big.children[0].Position = new Vector2(0, -160);
            big.maxHealth = 5;


            EnemySpawner spawner = new EnemySpawner(textures["Portal"], new Vector2(800, 800), new Vector2(700, 0), 5, 3, new Enemy[] { zombie , small, big }, player1) { depth = -4};

            world.Initialize(spawner);

            world.SortDepth();


            Console.WriteLine("game loaded, it took " + (DateTime.Now.Subtract(time).TotalMilliseconds) + " milliseconds");
        }

        protected override void UnloadContent()
        {
            SaveSettings();
        }
        
        protected void ToggleFullscreen()
        {
            if (!graphics.IsFullScreen)
            {
                window = Window.ClientBounds;
                graphics.PreferredBackBufferHeight = 1080;
                graphics.PreferredBackBufferWidth = 1920;
            }
            else
            {
                Window.Position = window.Location;
                graphics.PreferredBackBufferHeight = window.Height;
                graphics.PreferredBackBufferWidth = window.Width;
            }
            graphics.ToggleFullScreen();
        }
        GameState laststate = GameState.Game;
       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                state = GameState.Menu;
            
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
            Song menu;
            Song inGame;
            menu = Content.Load<Song>("menu");
            inGame = Content.Load<Song>("inGame");
            switch (state)
            {
                case GameState.Menu:
                    MenuWorldUpdate(delta);
                    if(laststate != GameState.Menu)
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Play(menu);
                        laststate = GameState.Menu;
                    }
                    
                    break;
                case GameState.Game:
                    GameWorldUpdate(delta);
                    if (laststate != GameState.Game)
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Play(inGame);
                        laststate = GameState.Game;
                    }
                    break;
                case GameState.Option:

                    OptionWorldUpdate(delta);
                    break;
            }
        }

        void GameWorldUpdate(float delta)
        {
            world.Update(delta, Keyboard.GetState(), Mouse.GetState(), Window);
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) world.viewport.center = player1.LocalPosition;
            if (player1.Health <= 0 || !player1.enabled)
            {
                PlaySound("deathSound");
                state = GameState.Menu;
                LoadGameWorld();
            }
        }

        void MenuWorldUpdate(float delta)
        {
            menuWorld.Update(delta, Keyboard.GetState(), Mouse.GetState(), Window);
            Vector2 dif = -menuWorld.ViewportToWorld(Window.ClientBounds.Size.ToVector2() / 2);
            menuWorld.viewport.center += dif * delta;
        }


        void OptionWorldUpdate(float delta)
        {
            optionWorld.Update(delta, Keyboard.GetState(), Mouse.GetState(), Window);
            Vector2 dif = -optionWorld.ViewportToWorld(Window.ClientBounds.Size.ToVector2() / 2);
            optionWorld.viewport.center += dif * delta;
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);
            switch (state)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Game:
                    DrawWorld();
                    break;

                case GameState.Option:
                    DrawOptions();
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void DrawMenu()
        {
            menuWorld.Draw(spriteBatch, Window);
        }

        void DrawWorld()
        {
            world.Draw(spriteBatch, Window);
        }


        void DrawOptions()
        {
            optionWorld.Draw(spriteBatch, Window);
        }
    }
}
