﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence;

public class Button
{
    public Rectangle Rectangle { get; private set; }
    public string Text { get; private set; }
    private SpriteFont _font;
    private bool _isHovering;
    public event EventHandler Clicked;

    public Button(Rectangle rectangle, string text, SpriteFont font)
    {
        Rectangle = rectangle;
        Text = text;
        _font = font;
        _isHovering = false;
    }

    public void Update(MouseState mouseState)
    {
        if (Rectangle.Contains(mouseState.X, mouseState.Y))
        {
            _isHovering = true;
        }
        else
        {
            _isHovering = false;
        }

        if (mouseState.LeftButton == ButtonState.Pressed && _isHovering)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color buttonColor = Color.Gray;
        if (_isHovering)
        {
            buttonColor = new Color(64, 64, 64); // Slightly darker gray
        }
        spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, Rectangle, buttonColor);
        Vector2 textSize = _font.MeasureString(Text);
        Vector2 textPosition = new Vector2(Rectangle.X + (Rectangle.Width - textSize.X) / 2, Rectangle.Y + (Rectangle.Height - textSize.Y) / 2);
        spriteBatch.DrawString(_font, Text, textPosition, Color.White);
    }
}