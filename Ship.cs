using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceDefence.Collision;
using SpaceDefence.Weapons;
using System;

namespace SpaceDefence
{
    public class Ship : GameObject
    {
        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private Texture2D double_turret;

        // Collider
        private readonly RectangleCollider _rectangleCollider;

        // Movement variables (using original logic as requested previously)
        private Vector2 _velocity;
        private readonly float _acceleration = 10f;
        private float _rotation;
        
        private IWeapon _currentWeapon; 
        private readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();
        private readonly LaserWeapon _laserWeapon = new LaserWeapon();
        private readonly DoubleBarrelWeapon _doubleBarrelWeapon = new DoubleBarrelWeapon();
        
        private float _weaponBuffTimer = 0f;
        private readonly float _weaponBuffDuration = 10f;

        // Cargo State
        private bool _isCarryingCargo = false;
        public bool IsCarryingCargo => _isCarryingCargo;

        public Ship(Point Position)
        {
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            SetCollider(_rectangleCollider);
            _velocity = Vector2.Zero;
            _rotation = 0f;
            _currentWeapon = _bulletWeapon;
        }

        
        public override void Load(ContentManager content)
        {
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");

            
            try
            {
                double_turret = content.Load<Texture2D>("double_turret");
            }
            catch (ContentLoadException)
            {
                System.Diagnostics.Debug.WriteLine("Warning: Could not load 'double_turret' texture. Using 'base_turret' as fallback.");
                double_turret = base_turret; 
            }
            

            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);

            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            
            Point screenMousePosition = inputManager.CurrentMouseState.Position;
            Vector2 worldMousePosition = GameManager.GetGameManager().ScreenToWorld(screenMousePosition.ToVector2());

            
            if (inputManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_currentWeapon != null && _currentWeapon.CanFire)
                {
                    Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), worldMousePosition);
                    
                    Texture2D currentTurretTexture = GetCurrentTurretTexture();
                    Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() + aimDirection * (currentTurretTexture.Height / 2f);
                    _currentWeapon.Fire(turretExit, aimDirection, this);
                }
            }

            // Movement Input (Original Logic Style)
            KeyboardState keyState = Keyboard.GetState();
            Vector2 accelerationDirection = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W)) accelerationDirection.Y = -1;
            if (keyState.IsKeyDown(Keys.S)) accelerationDirection.Y = 1;
            if (keyState.IsKeyDown(Keys.A)) accelerationDirection.X = -1;
            if (keyState.IsKeyDown(Keys.D)) accelerationDirection.X = 1;

            if (accelerationDirection != Vector2.Zero)
            {
                accelerationDirection.Normalize();
                _velocity += accelerationDirection * _acceleration; 
                _rotation = LinePieceCollider.GetAngle(accelerationDirection); 
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            _currentWeapon?.UpdateCooldown(deltaTime);

            
            if (_weaponBuffTimer > 0)
            {
                 _weaponBuffTimer -= deltaTime;
                 if (_weaponBuffTimer <= 0)
                 {
                     
                     _currentWeapon = _bulletWeapon;
                     System.Diagnostics.Debug.WriteLine("Weapon Buff Expired. Switched back to BulletWeapon.");
                 }
            }

            
            _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
            _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

            
             _velocity *= 0.99f; 

            base.Update(gameTime);
        }

        private Texture2D GetCurrentTurretTexture()
        {
            if (_currentWeapon is LaserWeapon)
            {
                return laser_turret;
            }
            else if (_currentWeapon is DoubleBarrelWeapon)
            {
               
                return double_turret; 
            }
            else
            {
                return base_turret;
            }
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            Vector2 bodyOrigin = ship_body.Bounds.Size.ToVector2() / 2f;
            spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, _rotation, bodyOrigin, 1f, SpriteEffects.None, 0);

            
            Texture2D turretTexture = GetCurrentTurretTexture();

            
            Point currentScreenMouse = Mouse.GetState().Position;
            Vector2 currentWorldMouse = GameManager.GetGameManager().ScreenToWorld(currentScreenMouse.ToVector2());
            Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), currentWorldMouse);
            float aimAngle = LinePieceCollider.GetAngle(aimDirection);
            Vector2 turretPosition = _rectangleCollider.shape.Center.ToVector2();
            Vector2 turretOrigin = turretTexture.Bounds.Size.ToVector2() / 2f;

            
            spriteBatch.Draw(turretTexture, turretPosition, null, Color.White, aimAngle, turretOrigin, 1f, SpriteEffects.None, 0);

            base.Draw(gameTime, spriteBatch);
        }

        

        public override void OnCollision(GameObject other)
        {
            
            if (other is Planet planet)
            {
                if (planet.Type == PlanetType.Pickup && !_isCarryingCargo)
                {
                    _isCarryingCargo = true;
                    System.Diagnostics.Debug.WriteLine("Cargo PICKED UP!");
                }
                else if (planet.Type == PlanetType.Dropoff && _isCarryingCargo)
                {
                    _isCarryingCargo = false;
                    int pointsValue = 10;
                    GameManager.GetGameManager().AddScore(pointsValue);
                    System.Diagnostics.Debug.WriteLine($"Cargo DROPPED OFF! +{pointsValue} points!");
                }
            }
            
            else if (other is Supply) 
            {
                
                Random rng = GameManager.GetGameManager().RNG;

                int weaponChoice = rng.Next(2); // Generates either 0 or 1

                if (weaponChoice == 0)
                {
                    _currentWeapon = _laserWeapon;
                    System.Diagnostics.Debug.WriteLine("Ship collided with Supply, switched to LaserWeapon!");
                }
                else 
                {
                    _currentWeapon = _doubleBarrelWeapon;
                    System.Diagnostics.Debug.WriteLine("Ship collided with Supply, switched to DoubleBarrelWeapon!");
                }

                
                _weaponBuffTimer = _weaponBuffDuration;

                
            }
            
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }
}