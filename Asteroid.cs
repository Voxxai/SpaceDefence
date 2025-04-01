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
        private CircleCollider _circleCollider;
        private Vector2 _initialPosition; // <<< ADDED: To store the position

        // Constructor takes the initial position for the asteroid
        public Asteroid(Vector2 position)
        {
            _initialPosition = position; // <<< ADDED: Store the position
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            // Load the texture
            try
            {
                _texture = content.Load<Texture2D>("Asteroid");
            }
            catch (ContentLoadException)
            {
                 System.Diagnostics.Debug.WriteLine("Warning: Could not load 'Asteroid' texture. Loading 'Alien' fallback.");
                 _texture = content.Load<Texture2D>("Alien"); // Fallback
            }


            if (_texture != null)
            {
                // --- Create Collider with smaller hitbox ---
                // Adjust this multiplier (0.1f to 1.0f) to change hitbox size relative to visual size
                float radiusMultiplier = 0.5f; // <<< ADDED: 50% size hitbox (Adjust as needed)
                float baseRadius = Math.Max(_texture.Width, _texture.Height) / 2f;
                float radius = baseRadius * radiusMultiplier;

                // Ensure radius is at least 1 pixel
                if (radius < 1f) radius = 1f;

                // Create collider using the STORED initial position and calculated radius
                _circleCollider = new CircleCollider(_initialPosition, radius); // <<< MODIFIED: Use _initialPosition

                SetCollider(_circleCollider); // Set it for the GameObject
                System.Diagnostics.Debug.WriteLine($"Asteroid loaded. Collider radius: {radius} at {_initialPosition}");
            }
            else
            {
                 System.Diagnostics.Debug.WriteLine($"Error: Could not load texture for Asteroid at {_initialPosition}. Collider not set.");
            }

            // --- Placeholder code block REMOVED ---
            // if (_circleCollider != null) { ... } // REMOVED

        } // End Load


        public override void Update(GameTime gameTime)
        {
            // Stationary
            base.Update(gameTime);
        }

        // OnCollision handles Ship and Alien
        public override void OnCollision(GameObject other)
        {
            if (other is Ship)
            {
                System.Diagnostics.Debug.WriteLine("Asteroid collided with Ship!"); // Added Debug message
                GameManager.GetGameManager().Game.Exit(); // Exit game
            }
            else if (other is Alien)
            {
                 System.Diagnostics.Debug.WriteLine("Asteroid collided with Alien.");
                 GameManager.GetGameManager().RemoveGameObject(other); // Remove Alien
            }
        }

        // Draw method remains the same
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Use the base class collider field for checking
            if (_texture != null && collider is CircleCollider circleCollider)
            {
                Vector2 origin = _texture.Bounds.Size.ToVector2() / 2f;
                spriteBatch.Draw(_texture, circleCollider.Center, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
        }

        // SetPosition method REMOVED (not needed with _initialPosition storage)
        // public void SetPosition(Vector2 position) { ... }

    } // End Class
}