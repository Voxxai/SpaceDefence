using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class GameManager
    {
        private static GameManager gameManager;

        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private ContentManager _content;
        private Texture2D _backgroundTexture;
        private SpriteFont _titleFont;
        private SpriteFont _buttonFont;
        private Button _startButton;
        private Button _quitButton;
        private Button _continueButton; // Added Continue button
        private Button _pauseQuitButton; // Now will be "Quit"
        private Button _pauseReturnToStartButton; // New button
        // private Texture2D _pauseBackgroundTexture; // Removed _pauseBackgroundTexture variable

        public Random RNG { get; private set; }
        public Ship Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }
        public Texture2D DummyTexture { get; private set; }
        public GameState CurrentGameState { get; private set; }

        public static GameManager GetGameManager()
        {
            if (gameManager == null)
                gameManager = new GameManager();
            return gameManager;
        }
        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            InputManager = new InputManager();
            RNG = new Random();
            CurrentGameState = GameState.StartScreen;
        }

        public void Initialize(ContentManager content, Game game, Ship player)
        {
            Game = game;
            _content = content;
            Player = player;

            DummyTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            DummyTexture.SetData(new Color[]{ Color.White });

            int buttonWidth = 200;
            int buttonHeight = 50;
            int centerX = Game.GraphicsDevice.Viewport.Width / 2;
            int centerY = Game.GraphicsDevice.Viewport.Height / 2;
            int spacing = 20;

            // Start Screen Buttons
            _startButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, centerY - buttonHeight - spacing / 2, buttonWidth, buttonHeight),
                "Start",
                _buttonFont);
            _startButton.Clicked += StartButton_Clicked;

            _quitButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, centerY + spacing / 2, buttonWidth, buttonHeight),
                "Quit",
                _buttonFont);
            _quitButton.Clicked += QuitButton_Clicked;

            // Pause Screen Buttons
            _continueButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY - buttonHeight - spacing * 1.5f), buttonWidth, buttonHeight),
                "Continue",
                _buttonFont);
            _continueButton.Clicked += ContinueButton_Clicked;

            _pauseQuitButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY + spacing * 0.5f), buttonWidth, buttonHeight),
                "Quit",
                _buttonFont);
            _pauseQuitButton.Clicked += PauseQuitButton_Clicked; // Now closes the game
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            CurrentGameState = GameState.Playing;
        }

        private void ContinueButton_Clicked(object sender, EventArgs e)
        {
            CurrentGameState = GameState.Playing;
        }

        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            Game.Exit(); // Directly exit the game from start screen
        }

        private void PauseQuitButton_Clicked(object sender, EventArgs e)
        {
            Game.Exit(); // Directly exit the game from pause screen
        }

        public void SetGameState(GameState newState) // Added this method
        {
            CurrentGameState = newState;
        }

        public void Load(ContentManager content)
        {
            _backgroundTexture = content.Load<Texture2D>("StartBackground"); // Replace "StartBackground" with the actual asset name
            _titleFont = content.Load<SpriteFont>("TitleFont"); // Replace "TitleFont" with the actual font asset name
            _buttonFont = content.Load<SpriteFont>("ButtonFont"); // Replace "ButtonFont" with the actual font asset name
            // _pauseBackgroundTexture = content.Load<Texture2D>("PauseBackground"); // Removed loading of PauseBackground
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.HandleInput(this.InputManager);
            }
        }

        public void CheckCollision()
        {
            // Checks once for every pair of 2 GameObjects if the collide.
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                for (int j = i + 1; j < _gameObjects.Count; j++)
                {
                    if (_gameObjects[i].CheckCollision(_gameObjects[j]))
                    {
                        _gameObjects[i].OnCollision(_gameObjects[j]);
                        _gameObjects[j].OnCollision(_gameObjects[i]);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            if (CurrentGameState == GameState.StartScreen)
            {
                _startButton.Update(mouseState);
                _quitButton.Update(mouseState);
                return;
            }

            if (CurrentGameState == GameState.Paused)
            {
                _continueButton.Update(mouseState);
                _pauseQuitButton.Update(mouseState);
                return;
            }

            if (CurrentGameState == GameState.Playing)
            {
                // Handle input
                HandleInput(InputManager);

                // Update
                foreach (GameObject gameObject in _gameObjects)
                {
                    gameObject.Update(gameTime);
                }

                // Check Collision
                CheckCollision();

                foreach (GameObject gameObject in _toBeAdded)
                {
                    gameObject.Load(_content);
                    _gameObjects.Add(gameObject);
                }
                _toBeAdded.Clear();

                foreach (GameObject gameObject in _toBeRemoved)
                {
                    gameObject.Destroy();
                    _gameObjects.Remove(gameObject);
                }
                _toBeRemoved.Clear();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (CurrentGameState == GameState.StartScreen)
            {
                spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

                string titleText = "Space Defence";
                Vector2 titleSize = _titleFont.MeasureString(titleText);
                Vector2 titlePosition = new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2,
                    100); // Adjust the Y position as needed
                spriteBatch.DrawString(_titleFont, titleText, titlePosition, Color.White);

                _startButton.Draw(spriteBatch);
                _quitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.Paused)
            {
                // Draw the game in the background
                foreach (GameObject gameObject in _gameObjects)
                {
                    gameObject.Draw(gameTime, spriteBatch);
                }

                // Draw a semi-transparent black overlay for the pause screen
                spriteBatch.Draw(DummyTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 128)); // Adjust transparency as needed

                string pauseText = "Game Paused";
                Vector2 pauseTextSize = _titleFont.MeasureString(pauseText);
                Vector2 pauseTextPosition = new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2 - pauseTextSize.X / 2,
                    150); // Adjust Y position

                spriteBatch.DrawString(_titleFont, pauseText, pauseTextPosition, Color.White);

                _continueButton.Draw(spriteBatch);
                _pauseQuitButton.Draw(spriteBatch);
            }
            else
            {
                foreach (GameObject gameObject in _gameObjects)
                {
                    gameObject.Draw(gameTime, spriteBatch);
                }
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Add a new GameObject to the GameManager.
        /// The GameObject will be added at the start of the next Update step.
        /// Once it is added, the GameManager will ensure all steps of the game loop will be called on the object automatically.
        /// </summary>
        /// <param name="gameObject"> The GameObject to add. </param>
        public void AddGameObject(GameObject gameObject)
        {
            _toBeAdded.Add(gameObject);
        }

        /// <summary>
        /// Remove GameObject from the GameManager.
        /// The GameObject will be removed at the start of the next Update step and its Destroy() mehtod will be called.
        /// After that the object will no longer receive any updates.
        /// </summary>
        /// <param name="gameObject"> The GameObject to Remove. </param>
        public void RemoveGameObject(GameObject gameObject)
        {
            _toBeRemoved.Add(gameObject);
        }

        /// <summary>
        /// Get a random location on the screen.
        /// </summary>
        public Vector2 RandomScreenLocation()
        {
            return new Vector2(
                RNG.Next(0, Game.GraphicsDevice.Viewport.Width),
                RNG.Next(0, Game.GraphicsDevice.Viewport.Height));
        }
    }
}