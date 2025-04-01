
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceDefence
{
    

    internal class Supply : GameObject
    {
        
        private RectangleCollider _rectangleCollider;
        private Texture2D _texture;
        private float playerClearance = 100;

        
        

        
        public Supply()
        {
             
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            
            string textureName = "Crate"; 
            try
            {
                _texture = content.Load<Texture2D>(textureName);
            }
            catch (ContentLoadException)
            {
                 
                 System.Diagnostics.Debug.WriteLine($"Warning: Could not load texture '{textureName}' for Supply. Loading default 'Alien'.");
                 _texture = content.Load<Texture2D>("Alien");
            }

            
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
            
            if (other is Ship)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
                
            }
             else if (other is Bullet) 
             {
                 GameManager.GetGameManager().RemoveGameObject(this);
             }
            base.OnCollision(other);
        }

        
        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            Vector2 randomPos;

            if (_rectangleCollider == null || gm.Player == null) {
                System.Diagnostics.Debug.WriteLine("Warning: RandomMove called before collider/player ready.");
                return; 
            }


            randomPos = gm.RandomScreenLocation() - _rectangleCollider.shape.Size.ToVector2() / 2;
            _rectangleCollider.shape.Location = randomPos.ToPoint();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            Vector2 myCenter = _rectangleCollider.shape.Center.ToVector2();

            int attempts = 0;
            
            while (gm.Player != null && (myCenter - centerOfPlayer).Length() < playerClearance && attempts < 100)
            {
                 randomPos = gm.RandomScreenLocation() - _rectangleCollider.shape.Size.ToVector2() / 2;
                 _rectangleCollider.shape.Location = randomPos.ToPoint();
                 myCenter = _rectangleCollider.shape.Center.ToVector2();
                 attempts++;
            }
             if(attempts >= 100) System.Diagnostics.Debug.WriteLine("Warning: RandomMove failed to find clear spot for supply.");
        }

        
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