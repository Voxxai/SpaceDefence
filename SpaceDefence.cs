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

            // Add the starting objects to the GameManager
            _gameManager.Initialize(Content, this, player);
            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Supply());
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