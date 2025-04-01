// Inside Planet.cs (Modified for Single Frame from Sprite Sheet)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision;
using System;

namespace SpaceDefence
{
    public enum PlanetType { Pickup, Dropoff }

    internal class Planet : GameObject
    {
        private Texture2D _spriteSheet; // Load the whole sheet
        private CircleCollider _circleCollider;
        private Vector2 _initialPosition;

        public PlanetType Type { get; private set; }

        // --- Frame dimension variables needed, but not animation timing ---
        private int _frameCount;    // Still need this to calculate width
        private int _frameWidth;    // Width of a single frame
        private int _frameHeight;   // Height of a single frame

        // --- Animation timer and current frame variables REMOVED ---

        public Planet(Vector2 position, PlanetType type)
        {
            _initialPosition = position;
            Type = type;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            // Load the sprite sheet texture based on type
            string textureName = (Type == PlanetType.Pickup) ? "PlanetPickup" : "PlanetDropoff";
            try
            {
                _spriteSheet = content.Load<Texture2D>(textureName); // Load the strip
            }
            catch (ContentLoadException)
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Could not load texture '{textureName}'. Loading default 'Alien' texture instead.");
                _spriteSheet = content.Load<Texture2D>("Alien"); // Fallback
            }

            if (_spriteSheet != null)
            {
                // --- Calculate Frame Data (Still needed for size) ---
                // IMPORTANT: You still need to know the total number of frames.
                // Using 60 as placeholder - ADJUST IF NEEDED!
                _frameCount = 60; // <<< ADJUST THIS based on your actual sprite sheet

                if (_frameCount > 0) // Avoid division by zero
                {
                    _frameHeight = _spriteSheet.Height;
                    _frameWidth = _spriteSheet.Width / _frameCount; // Calculate width
                }
                else // Handle case of invalid frame count
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Invalid frame count ({_frameCount}) for planet {Type}. Using full texture dimensions.");
                    _frameHeight = _spriteSheet.Height;
                    _frameWidth = _spriteSheet.Width;
                    _frameCount = 1; // Treat as single frame
                }


                // --- Create Collider based on single frame size ---
                float radius = Math.Max(_frameWidth, _frameHeight) / 2f * 0.9f;
                _circleCollider = new CircleCollider(_initialPosition, radius);
                SetCollider(_circleCollider);
                System.Diagnostics.Debug.WriteLine($"Static Frame Planet {Type} loaded. Frame W: {_frameWidth}, Frame H: {_frameHeight}. Collider at {_circleCollider.Center}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Error: Could not load any texture for Planet at {_initialPosition}. Cannot set up dimensions or collider.");
            }
        }

        // Update method: No animation logic needed
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        // Draw method: Always draws the FIRST frame (index 0)
        // Inside Planet.cs (Static Frame Version)

// ... (Load, Update methods etc.) ...

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Check collider AND texture AND frame dimensions are valid
            if (_spriteSheet != null && collider is CircleCollider circleCollider && _frameWidth > 0 && _frameHeight > 0)
            {
                // --- Adjust the width slightly ---
                int adjustment = 15; // <<< How many pixels to trim from the right side. Adjust this value!
                int adjustedWidth = _frameWidth - adjustment;

                // Ensure width doesn't become negative if adjustment is too large
                if (adjustedWidth < 1) adjustedWidth = 1;

                // --- Calculate Source Rectangle for the FIRST frame with adjusted width ---
                Rectangle sourceRect = new Rectangle(
                    0,                  // X position on the sheet (start of first frame)
                    0,                  // Y position on the sheet (top)
                    adjustedWidth,      // Use the *adjusted* width
                    _frameHeight        // Height of the frame
                );

                // --- Calculate Origin based on ORIGINAL frame size for centering ---
                // Using the original width helps keep it centered visually as intended
                Vector2 origin = new Vector2(_frameWidth / 2f, _frameHeight / 2f);

                // --- Draw the Adjusted First Frame ---
                spriteBatch.Draw(
                    _spriteSheet,          // The sprite sheet texture
                    circleCollider.Center, // Destination position on screen (world coords)
                    sourceRect,            // Use the adjusted source rectangle
                    Color.White,           // Tint
                    0f,                    // Rotation
                    origin,                // Origin (center based on original frame width)
                    1f,                    // Scale (we're clipping, not scaling here)
                    SpriteEffects.None,
                    0f
                );
            }
            base.Draw(gameTime, spriteBatch);
        }
    }
}