using FighterXD.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            world = new World(Content.Load<Texture2D>("background"),new Vector2(1920,1080),new Rectangle(100,100,Window.ClientBounds.Width ,Window.ClientBounds.Height));
            world.ViewportZoom = 1.01f;
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
            world.Update(0.1f, Keyboard.GetState());
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
