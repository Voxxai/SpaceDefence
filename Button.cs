using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence;

public class Button
{
    private Rectangle _rectangle;
    private readonly string _text;
    private readonly SpriteFont _font;
    private readonly Color _textColor = Color.White;

    public event EventHandler Clicked;

    public Button(Rectangle rectangle, string text, SpriteFont font)
    {
        _rectangle = rectangle;
        _text = text;
        _font = font;
    }

    public void Update(MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && _rectangle.Contains(mouseState.X, mouseState.Y))
            Clicked?.Invoke(this, EventArgs.Empty);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, _rectangle,
            Color.Gray); // Use a dummy texture for the button background
        var textSize = _font.MeasureString(_text);
        var textPosition = new Vector2(
            _rectangle.X + (_rectangle.Width - textSize.X) / 2,
            _rectangle.Y + (_rectangle.Height - textSize.Y) / 2);
        spriteBatch.DrawString(_font, _text, textPosition, _textColor);
    }
}