using FighterXD.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FighterXD
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World world;

        Player player1;

        TextObject text;

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
            if (world != null)
                world.viewport.ratio = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            world = new World(Content.Load<Texture2D>("background"),new Vector2(4000,4000), new Main.Viewport(new Vector2(0, 0), new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height)));

            PhysicalObject p = new PhysicalObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), Content.Load<Texture2D>("Rocket_Launcher"), new Vector2(500, 500), new Vector2(200, 200));
            world.Initialize(p);
            ((RectangleCollider)p.Collider).SetSize();

            RigidObject p2 = new RigidObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), Content.Load<Texture2D>("Rocket_Launcher"), new Vector2(500, 0), new Vector2(200, 200));
            world.Initialize(p2);
            ((RectangleCollider)p2.Collider).SetSize();

            world = new World(Content.Load<Texture2D>("background"),new Vector2(80000,80000),new Main.Viewport(new Vector2(0, 0), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)));
            //world.ViewportZoom = 1.01f;

            InitTerrain(new Vector2(500, 400), new Vector2(500, 500), new Vector2(5, 5), Content.Load<Texture2D>("floor"), true);

            //PhysicalObject p = new PhysicalObject(new CircleCollider(1), Content.Load<Texture2D>("blob"), new Vector2(500, 2000), new Vector2(1500, 1500));
            //p.Collider.SetSize();
            //world.Initialize(p);

            player1 = new Player(new PlayerInfo(Keys.A, Keys.D, Keys.W, Keys.Space, 30000, 1000, 0.2f, 20), new CircleCollider(1), Content.Load<Texture2D>("blob"), new Vector2(500, 0), new Vector2(100, 100));
            player1.Collider.SetSize();

            Texture2D eye = Content.Load<Texture2D>("eye");

            RotateTowardsMouse eye1 = new RotateTowardsMouse(eye, new Vector2(20, -5), new Vector2(20, 20), 0, new Vector2(0, 0), false)
            {
                Parent = player1
            };
            RotateTowardsMouse eye2 = new RotateTowardsMouse(eye, new Vector2(-20, -5), new Vector2(20, 20), 0, new Vector2(0, 0), false)
            {
                Parent = player1
            };

            text = new TextObject(Content.Load<SpriteFont>("font"), new Vector2(0, -100))
            {
                Parent = player1,
                text = "XD",
                color = Color.Black
            }; 
            world.Initialize(player1);

            
            // player1 = new Player(Content.Load<Texture2D>("blob"), 380, 640, 4.5f, 4.5f, Content.Load<Texture2D>("eye"));
            // player2 = new Player(Content.Load<Texture2D>("blob"), 380, 640, 4.5f, 4.5f, Content.Load<Texture2D>("eye"));
            // background = new Background(Content.Load<Texture2D>("jail"), Window);
            // weapon = new Weapon(Content.Load<Texture2D>("Rocket_launcher")); 
        }

        void InitTerrain(Vector2 start, Vector2 size, Vector2 blockSize, Texture2D texture, bool ceiling)
        {
            Vector2 blocks = size / blockSize;
            int blocksX = 0;
            int blocksY = 0;
            if (!ceiling)
            {
                blocksX = (int)Math.Floor(blocks.X);
                blocksY = (int)Math.Floor(blocks.Y);
            }
            else
            {
                blocksX = (int)Math.Ceiling(blocks.X);
                blocksY = (int)Math.Ceiling(blocks.Y);
            }


            for (int y = 0; y <= blocksY; y++)
            {
                for (int x = 0; x <= blocksX; x++)
                {
                    Vector2 pos = start + new Vector2(x, y) * blockSize;

                    ExplodableObject p = new ExplodableObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), texture, pos, blockSize);
                    p.Collider.SetSize();
                    world.Initialize(p);
                }
            }
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F))
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

            world.Update((float)gameTime.ElapsedGameTime.TotalSeconds, Keyboard.GetState(), Mouse.GetState(), Window);
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.H)) world.Explode(player1);

            Vector2[] normals = player1.GetCollisionNormals();
            text.text = player1.Health.ToString();

            if (Keyboard.GetState().IsKeyDown(Keys.Space)) world.viewport.center = player1.position;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);
            world.Draw(spriteBatch, Window);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
