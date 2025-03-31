using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision; // Assuming your colliders are here
using System;

namespace SpaceDefence
{
    internal class Asteroid : GameObject
    {
        private Texture2D _texture;
        private CircleCollider _circleCollider; // Or RectangleCollider if preferred

        // Constructor takes the initial position for the asteroid
        public Asteroid(Vector2 position)
        {
            // Collider will be created and positioned in LoadContent
            // Store the initial position if needed, or pass directly to collider
            // For simplicity, we'll use the position directly in Load
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            // *** Make sure you have an "Asteroid.png" (or similar) in your Content project ***
            _texture = content.Load<Texture2D>("Asteroid"); // Load the asteroid texture

            // Create a collider based on the texture size
            // Using CircleCollider centered on the texture
            float radius = Math.Max(_texture.Width, _texture.Height) / 2f;
            // The position passed to the constructor should be the intended center
            Vector2 initialPosition = _circleCollider != null ? _circleCollider.Center : Vector2.Zero; // A bit clunky, better to pass position to Load
            _circleCollider = new CircleCollider(initialPosition, radius);


            // Alternative: Use RectangleCollider if preferred
            // _rectangleCollider = new RectangleCollider(new Rectangle(position.ToPoint(), _texture.Bounds.Size));
            // _rectangleCollider.shape.Location -= new Point(_texture.Width / 2, _texture.Height / 2); // Center it

            SetCollider(_circleCollider); // Set the chosen collider

            // Position the collider - This assumes the constructor's position wasn't stored.
            // It's cleaner to pass the initial position to Load or store it.
            // Let's assume we modify the constructor or pass position differently.
            // For now, this example places it, but needs refinement on how position is passed.
             if (_circleCollider != null)
             {
                 // We need the intended position here. Let's modify the design slightly.
                 // We'll set the position *after* Load in GameManager.
             }
        }

        // Asteroids are stationary, so the Update method is empty
        public override void Update(GameTime gameTime)
        {
            // No movement logic needed
            base.Update(gameTime);
        }

        // Handle collisions
        // Inside Asteroid.cs

        public override void OnCollision(GameObject other)
        {
            
            if (other is Ship)
            {
                GameManager.GetGameManager().Game.Exit();
            }
            else if (other is Alien)
            {
                GameManager.GetGameManager().RemoveGameObject(other);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null && _circleCollider != null)
            {
                // Draw the asteroid centered on its collider position
                Vector2 origin = _texture.Bounds.Size.ToVector2() / 2f;
                spriteBatch.Draw(_texture, _circleCollider.Center, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
        }

        // Helper method to set the position AFTER the collider is created
        public void SetPosition(Vector2 position)
        {
            if (_circleCollider != null)
            {
                _circleCollider.Center = position;
            }
            // else if (_rectangleCollider != null) // If using RectangleCollider
            // {
            //     _rectangleCollider.shape.Location = (position - _rectangleCollider.shape.Size.ToVector2() / 2f).ToPoint();
            // }
        }
    }
}