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
        private SpriteFont _font;
        private Rectangle _startButton;
        private Rectangle _quitButton;

        public StartScreen()
        {
            // Define the positions and sizes of the buttons
            _startButton = new Rectangle(500, 300, 200, 50);
            _quitButton = new Rectangle(500, 400, 200, 50);
        }

        public void LoadContent(ContentManager content)
        {
            _backgroundTexture= content.Load<Texture2D>("StartBackground");
            _font = content.Load<SpriteFont>("Arial");
        }

        public GameState Update(InputManager inputManager)
        {
            // Check for mouse clicks on the buttons
            if (inputManager.LeftMousePress())
            {
                Point mousePosition = inputManager.CurrentMouseState.Position;

                if (_startButton.Contains(mousePosition))
                {
                    // Start the game
                    return GameState.Playing;
                }
                else if (_quitButton.Contains(mousePosition))
                {
                    // Quit the game
                    return GameState.Quit;
                }
            }

            // Stay on the start screen
            return GameState.StartScreen;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background, title, and buttons
            if (_backgroundTexture != null)
            {
                spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            }
            else
            {
                Console.WriteLine("No background texture found");
            }

            // Draw the title
            if (_font != null)
            {
                string title = "Space Defence";
                Vector2 titleSize = _font.MeasureString(title);
                Vector2 titlePosition = new Vector2(640 - titleSize.X / 2, 100);
                spriteBatch.DrawString(_font, title, titlePosition, Color.White);

                // Draw the buttons
                spriteBatch.DrawRectangle(_startButton, Color.White);
                spriteBatch.DrawRectangle(_quitButton, Color.White);

                // Draw the button text
                string startText = "Start";
                Vector2 startTextSize = _font.MeasureString(startText);
                Vector2 startTextPosition = new Vector2(_startButton.X + _startButton.Width / 2 - startTextSize.X / 2, _startButton.Y + _startButton.Height / 2 - startTextSize.Y / 2);
                spriteBatch.DrawString(_font, startText, startTextPosition, Color.Black);

                string quitText = "Quit";
                Vector2 quitTextSize = _font.MeasureString(quitText);
                Vector2 quitTextPosition = new Vector2(_quitButton.X + _quitButton.Width / 2 - quitTextSize.X / 2, _quitButton.Y + _quitButton.Height / 2 - quitTextSize.Y / 2);
                spriteBatch.DrawString(_font, quitText, quitTextPosition, Color.Black);
            }
            else
            {
                Console.WriteLine("No font found");
            }
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