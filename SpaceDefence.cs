﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private StartScreen _startScreen;
        private GameState _currentGameState;
        private InputManager _inputManager;

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
            // Initialize the GameManager
            _gameManager = GameManager.GetGameManager();
            base.Initialize();

            // Initialize the InputManager
            _inputManager = new InputManager();

            // Place the player at the center of the screen
            Ship player = new Ship(new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

            // Add the starting objects to the GameManager
            _gameManager.Initialize(Content, this, player);
            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Supply());

            // Set the initial game state
            _currentGameState = GameState.StartScreen;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            if (_gameManager == null)
            {
                _gameManager = GameManager.GetGameManager();
                _gameManager.Initialize(Content, this, new Ship(new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2)));
            }
            _gameManager.Load(Content);

            if (_startScreen == null)
            {
                _startScreen = new StartScreen();
            }
            _startScreen.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle input and update game state
            switch (_currentGameState)
            {
                case GameState.StartScreen:
                    _currentGameState = _startScreen.Update(_inputManager);
                    break;
                case GameState.Playing:
                    _gameManager.Update(gameTime);
                    break;
                case GameState.Quit:
                    Exit();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw logic based on the current GameState
            switch (_currentGameState)
            {
                case GameState.StartScreen:
                    _startScreen.Draw(_spriteBatch);
                    break;
                case GameState.Playing:
                    _gameManager.Draw(gameTime, _spriteBatch);
                    break;
            }

            base.Draw(gameTime);
        }   
    }

    public enum GameState
    {
        StartScreen,
        Playing,
        PauseScreen,
        GameOver,
        Quit
    }
}