using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WorldGenPrototype
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        WorldManager worldManager;

        Matrix cameraView;
        private int previousWheelValue;
        private float scaleFactor;
        private MouseState oldState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            Window.AllowUserResizing = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            worldManager = new WorldManager(spriteBatch, this.Content);
            worldManager.CreateWorld(new Vector2(400, 200));

            previousWheelValue = 0;
            scaleFactor = 0;

            cameraView = new Matrix();
            cameraView.Translation = new Vector3(0, 0, 0);
            cameraView.M11 = 1;
            cameraView.M22 = 1;
            cameraView.M33 = 1;
            cameraView.M44 = 1;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            MouseState newState = Mouse.GetState();

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                Vector2 mousePos = new Vector2((Mouse.GetState().X - cameraView.M41) / (worldManager.tileSize * cameraView.M11),
                                               (Mouse.GetState().Y - cameraView.M42) / (worldManager.tileSize * cameraView.M11));

                Vector2 adjMousePos = new Vector2((int)mousePos.X, (int)mousePos.Y);
                Console.WriteLine("Tile Location = X: {0} || Y: {1}", (int)mousePos.X, (int)mousePos.Y);
                Console.WriteLine("Clicked Tile Height is: {0}", worldManager.ReturnHeight(adjMousePos));
            }

            oldState = newState;

            if (Mouse.GetState().ScrollWheelValue > previousWheelValue)
            {
                cameraView = Matrix.CreateScale(2) * cameraView;
                previousWheelValue = Mouse.GetState().ScrollWheelValue;
            }

            if (Mouse.GetState().ScrollWheelValue < previousWheelValue)
            {
                cameraView = Matrix.CreateScale(.5f) * cameraView;
                previousWheelValue = Mouse.GetState().ScrollWheelValue;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                cameraView.Translation += new Vector3(0, 5, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                cameraView.Translation += new Vector3(0, -5, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraView.Translation += new Vector3(5, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraView.Translation += new Vector3(-5, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))

            base.Update(gameTime);
        }

        static void MouseClicked(object sender, WorldClickEventArgs e)
        {
            Console.WriteLine("Mouse Clicked At: {0}", e.clickPos);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cameraView);
            worldManager.Draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
