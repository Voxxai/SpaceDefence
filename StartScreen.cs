using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class StartScreen
    {
        private Texture2D _backgroundTexture;
        private SpriteFont _titleFont;
        private SpriteFont _buttonFont;
        private Rectangle _startButton;
        private Rectangle _quitButton;

        public StartScreen()
        {
            // Define the sizes of the buttons
            _startButton = new Rectangle(0, 0, 200, 50);
            _quitButton = new Rectangle(0, 0, 200, 50);
        }

        public void LoadContent(ContentManager content)
        {
            _backgroundTexture = content.Load<Texture2D>("StartBackground");
            _titleFont = content.Load<SpriteFont>("TitleFont");
            _buttonFont = content.Load<SpriteFont>("ButtonFont");
        }

        public GameState Update(InputManager inputManager)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePosition = new Point(mouseState.X, mouseState.Y);

            // Check for mouse clicks on the buttons
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (_startButton.Contains(mousePosition))
                {
                    // Start the game
                    Console.WriteLine("Starting game...");
                    return GameState.Playing;
                }
                else if (_quitButton.Contains(mousePosition))
                {
                    // Quit the game
                    Console.WriteLine("Quitting game...");
                    return GameState.Quit;
                }
            }

            // Stay on the start screen
            return GameState.StartScreen;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Ensure content is loaded
            if (_backgroundTexture == null || _titleFont == null || _buttonFont == null)
            {
                throw new InvalidOperationException("Content is not loaded. Call LoadContent before Draw.");
            }

            // Get screen dimensions
            int screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            int screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;

            // Draw the background
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            // Draw the title without scaling
            string title = "Space Defence";
            Vector2 titleSize = _titleFont.MeasureString(title);
            Vector2 titlePosition = new Vector2((screenWidth - titleSize.X) / 2, 100);
            spriteBatch.DrawString(_titleFont, title, titlePosition, Color.White);

            // Center the buttons
            _startButton.X = (screenWidth - _startButton.Width) / 2;
            _startButton.Y = 300;
            _quitButton.X = (screenWidth - _quitButton.Width) / 2;
            _quitButton.Y = 400;

            // Determine button colors based on hover state
            Point mousePosition = Mouse.GetState().Position;
            Color startButtonColor = _startButton.Contains(mousePosition) ? Color.Gray : Color.White;
            Color quitButtonColor = _quitButton.Contains(mousePosition) ? Color.Gray : Color.White;

            // Draw the buttons
            spriteBatch.DrawRectangle(_startButton, startButtonColor);
            spriteBatch.DrawRectangle(_quitButton, quitButtonColor);

            // Draw the button text
            string startText = "Start";
            Vector2 startTextSize = _buttonFont.MeasureString(startText);
            Vector2 startTextPosition = new Vector2(_startButton.X + (_startButton.Width - startTextSize.X) / 2, _startButton.Y + (_startButton.Height - startTextSize.Y) / 2);
            spriteBatch.DrawString(_buttonFont, startText, startTextPosition, Color.Black);

            string quitText = "Quit";
            Vector2 quitTextSize = _buttonFont.MeasureString(quitText);
            Vector2 quitTextPosition = new Vector2(_quitButton.X + (_quitButton.Width - quitTextSize.X) / 2, _quitButton.Y + (_quitButton.Height - quitTextSize.Y) / 2);
            spriteBatch.DrawString(_buttonFont, quitText, quitTextPosition, Color.Black);
        }
    }

    // Add a helper function to draw a rectangle (this can also be placed elsewhere in your project)
    public static class SpriteBatchExtensions
    {
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[] { Color.White };
            pixel.SetData<Color>(colorData);
            spriteBatch.Draw(pixel, rectangle, color);
        }
    }
}