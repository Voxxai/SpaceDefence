using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Alien : GameObject
    {
        private CircleCollider _circleCollider;
        private Texture2D _texture;
        private float playerClearance = 100;
        private float _speed = 60f; // Initial speed
        private bool _isGameOver = false;
        private float _speedIncrement = 5f;

        public Alien()
        {

        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Alien");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            if (_isGameOver)
            {
                return; // Stop updating if game over
            }

            // Get player position
            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();

            // Calculate direction to player
            Vector2 direction = playerPosition - _circleCollider.Center;
            direction.Normalize();

            // Move towards player
            _circleCollider.Center += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check distance to player (Game Over)
            float distanceToPlayer = Vector2.Distance(_circleCollider.Center, playerPosition);
            if (distanceToPlayer < 40) // Adjust this threshold as needed
            {
                _isGameOver = true;
                GameManager.GetGameManager().Game.Exit(); // Close the game
                // You can implement a proper game over screen/logic here
            }

            base.Update(gameTime);
        }
        public override void OnCollision(GameObject other)
        {
            if (other is Bullet)
            {
                _speed += _speedIncrement;
                RandomMove();
            }
            base.OnCollision(other);
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }
    }
}