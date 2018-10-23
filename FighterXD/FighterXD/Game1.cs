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
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
        }
        
        protected override void Initialize()
        {

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            world = new World(Content.Load<Texture2D>("background"),new Vector2(4000,4000),new Rectangle(0,0,Window.ClientBounds.Width ,Window.ClientBounds.Height));
            world.ViewportZoom = 1.01f;
            PhysicalObject p = new PhysicalObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), Content.Load<Texture2D>("Rocket_Launcher"), new Vector2(500, 500), new Vector2(200, 200));
            world.Initialize(p);
            ((RectangleCollider)p.Collider).SetSize();
<<<<<<< HEAD
            RigidObject p2 = new RigidObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), Content.Load<Texture2D>("Rocket_Launcher"), new Vector2(500, 600), new Vector2(200, 200));
=======
            RigidObject p2 = new RigidObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), Content.Load<Texture2D>("Rocket_Launcher"), new Vector2(500, 0), new Vector2(200, 200));
>>>>>>> f3f1cb5fccf761524529284cacae61ad08ac7499
            world.Initialize(p2);
            ((RectangleCollider)p2.Collider).SetSize();
            // player1 = new Player(Content.Load<Texture2D>("blob"), 380, 640, 4.5f, 4.5f, Content.Load<Texture2D>("eye"));
            // player2 = new Player(Content.Load<Texture2D>("blob"), 380, 640, 4.5f, 4.5f, Content.Load<Texture2D>("eye"));
            // background = new Background(Content.Load<Texture2D>("jail"), Window);
            // weapon = new Weapon(Content.Load<Texture2D>("Rocket_launcher"));

        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            world.Update((float)gameTime.ElapsedGameTime.Milliseconds/1000, Keyboard.GetState());
            base.Update(gameTime);
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
