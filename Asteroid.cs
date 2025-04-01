using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision;
using System;

namespace SpaceDefence
{
    internal class Asteroid : GameObject
    {
        private Texture2D _texture;
        private CircleCollider _circleCollider;
        private Vector2 _initialPosition;
        
        public Asteroid(Vector2 position)
        {
            _initialPosition = position;
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
                 _texture = content.Load<Texture2D>("Alien");
            }


            if (_texture != null)
            {
                
                
                float radiusMultiplier = 0.5f;
                float baseRadius = Math.Max(_texture.Width, _texture.Height) / 2f;
                float radius = baseRadius * radiusMultiplier;

                
                if (radius < 1f) radius = 1f;

                
                _circleCollider = new CircleCollider(_initialPosition, radius);

                SetCollider(_circleCollider);
                System.Diagnostics.Debug.WriteLine($"Asteroid loaded. Collider radius: {radius} at {_initialPosition}");
            }
            else
            {
                 System.Diagnostics.Debug.WriteLine($"Error: Could not load texture for Asteroid at {_initialPosition}. Collider not set.");
            }
            

        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

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
            
            if (_texture != null && collider is CircleCollider circleCollider)
            {
                Vector2 origin = _texture.Bounds.Size.ToVector2() / 2f;
                spriteBatch.Draw(_texture, circleCollider.Center, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
        }
        

    }
}