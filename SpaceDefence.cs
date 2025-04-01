using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private bool _wasEscapeKeyPressed = false; // Added this variable

        public SpaceDefence()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;

            // Set the size of the screen
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Initialize the GameManager
            _gameManager = GameManager.GetGameManager();
            base.Initialize();

            // Place the player at the center of the screen
            Ship player = new Ship(new Point(GraphicsDevice.Viewport.Width/2,GraphicsDevice.Viewport.Height/2));
            
            _gameManager.Initialize(Content,this, player);

            // Add the starting objects to the GameManager
            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Supply());
            
            // Define the positions of the asteroids
            Vector2 asteroidPos1 = new Vector2(1000, 800);
            Vector2 asteroidPos2 = new Vector2(-200, 200);
            Vector2 asteroidPos3 = new Vector2(500, -400);
            
            // Create asteroids
            Asteroid asteroid1 = new Asteroid(asteroidPos1);
            Asteroid asteroid2 = new Asteroid(asteroidPos2);
            Asteroid asteroid3 = new Asteroid(asteroidPos3);
            
            // Add asteroids to the GameManager
            _gameManager.AddGameObject(asteroid1);
            _gameManager.AddGameObject(asteroid2);
            _gameManager.AddGameObject(asteroid3);
            
            // Load the content for the asteroids
            asteroid1.Load(Content);
            asteroid2.Load(Content);
            asteroid3.Load(Content);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameManager.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // Handle single press of Escape key for pausing/unpausing
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !_wasEscapeKeyPressed)
            {
                if (_gameManager.CurrentGameState == GameState.Playing)
                {
                    _gameManager.SetGameState(GameState.Paused);
                }
                else if (_gameManager.CurrentGameState == GameState.Paused)
                {
                    _gameManager.SetGameState(GameState.Playing);
                }
                else if (_gameManager.CurrentGameState == GameState.StartScreen)
                {
                    Exit();
                }
            }

            _wasEscapeKeyPressed = currentKeyboardState.IsKeyDown(Keys.Escape);
            _gameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _gameManager.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}