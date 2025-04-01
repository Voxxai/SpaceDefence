// Supply.cs - Simplified after random pickup logic moved to Ship

using SpaceDefence.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceDefence
{
    // SupplyType enum REMOVED

    internal class Supply : GameObject
    {
        // Using RectangleCollider as per previous correction
        private RectangleCollider _rectangleCollider;
        private Texture2D _texture;
        private float playerClearance = 100;

        // Type property REMOVED
        // public SupplyType Type { get; private set; }

        // Constructor is now parameterless again
        public Supply()
        {
            // No type needed anymore
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            // Load the default texture (or specific one if you prefer)
            string textureName = "Crate"; // Just load the standard crate texture
            try
            {
                _texture = content.Load<Texture2D>(textureName);
            }
            catch (ContentLoadException)
            {
                 // Fallback if needed
                 System.Diagnostics.Debug.WriteLine($"Warning: Could not load texture '{textureName}' for Supply. Loading default 'Alien'.");
                 _texture = content.Load<Texture2D>("Alien");
            }

            // Create RectangleCollider
            if (_texture != null)
            {
                 _rectangleCollider = new RectangleCollider(_texture.Bounds);
                 SetCollider(_rectangleCollider);
                 System.Diagnostics.Debug.WriteLine($"Supply loaded. Using RectangleCollider.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Error: Could not load texture for Supply. Collider not set.");
            }
        }

        public override void OnCollision(GameObject other)
        {
            // Handles removing/respawning itself when collected by Ship
            if (other is Ship)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
                // OR RandomMove(); // if you want it to respawn elsewhere
            }
             else if (other is Bullet) // Optional: Destroy supply with bullets
             {
                 GameManager.GetGameManager().RemoveGameObject(this);
             }
            base.OnCollision(other);
        }

        // RandomMove using RectangleCollider
        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            Vector2 randomPos;

            if (_rectangleCollider == null || gm.Player == null) {
                System.Diagnostics.Debug.WriteLine("Warning: RandomMove called before collider/player ready.");
                return; // Exit if collider or player isn't ready
            }


            randomPos = gm.RandomScreenLocation() - _rectangleCollider.shape.Size.ToVector2() / 2;
            _rectangleCollider.shape.Location = randomPos.ToPoint();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            Vector2 myCenter = _rectangleCollider.shape.Center.ToVector2();

            int attempts = 0;
            // Add null check for Player just in case
            while (gm.Player != null && (myCenter - centerOfPlayer).Length() < playerClearance && attempts < 100)
            {
                 randomPos = gm.RandomScreenLocation() - _rectangleCollider.shape.Size.ToVector2() / 2;
                 _rectangleCollider.shape.Location = randomPos.ToPoint();
                 myCenter = _rectangleCollider.shape.Center.ToVector2();
                 attempts++;
            }
             if(attempts >= 100) System.Diagnostics.Debug.WriteLine("Warning: RandomMove failed to find clear spot for supply.");
        }

        // Draw using RectangleCollider
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null && _rectangleCollider != null)
            {
                 spriteBatch.Draw(_texture, _rectangleCollider.shape, Color.White);
            }
            base.Draw(gameTime, spriteBatch);
        }
    }
}