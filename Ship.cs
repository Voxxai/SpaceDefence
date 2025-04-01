// Ship.cs - Integrated DoubleBarrelWeapon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceDefence.Collision;
using SpaceDefence.Weapons; // Using statement for weapon classes
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
        private readonly float _acceleration = 10f; // Adjust as needed (original value)
        private float _rotation;

        // --- Weapon Fields ---
        private IWeapon _currentWeapon; // Holds the currently equipped weapon
        // Instances of available weapons
        private readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();
        private readonly LaserWeapon _laserWeapon = new LaserWeapon(); // Still exists, just not activated by Supply currently
        private readonly DoubleBarrelWeapon _doubleBarrelWeapon = new DoubleBarrelWeapon(); // Instance of the new weapon

        // --- Weapon Buff Timer ---
        // Times how long the DoubleBarrelWeapon (or other picked-up weapon) lasts
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
            // Start with the basic bullet weapon
            _currentWeapon = _bulletWeapon;
        }

        // Inside Ship.cs -> Load method
        public override void Load(ContentManager content)
        {
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");

            // --- Load the double barrel turret texture ---
            try
            {
                double_turret = content.Load<Texture2D>("double_turret"); // <<< ADD THIS LINE
            }
            catch (ContentLoadException)
            {
                System.Diagnostics.Debug.WriteLine("Warning: Could not load 'double_turret' texture. Using 'base_turret' as fallback.");
                double_turret = base_turret; // Use base turret if specific one is missing
            }
            // --- End loading double barrel texture ---

            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);

            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            // Aiming
            Point screenMousePosition = inputManager.CurrentMouseState.Position;
            Vector2 worldMousePosition = GameManager.GetGameManager().ScreenToWorld(screenMousePosition.ToVector2());

            // Firing - Delegate to the current weapon
            if (inputManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_currentWeapon != null && _currentWeapon.CanFire)
                {
                    Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), worldMousePosition);
                    // Use the currently displayed turret texture's height for offset calculation, or stick to base?
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
                _velocity += accelerationDirection * _acceleration; // No deltaTime here
                _rotation = LinePieceCollider.GetAngle(accelerationDirection); // Set visual rotation
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the CURRENT weapon's cooldown
            _currentWeapon?.UpdateCooldown(deltaTime);

            // Update Weapon Buff Timer - switches back to BULLET weapon
            if (_weaponBuffTimer > 0)
            {
                 _weaponBuffTimer -= deltaTime;
                 if (_weaponBuffTimer <= 0)
                 {
                     // Always revert to the basic bullet weapon after buff expires
                     _currentWeapon = _bulletWeapon;
                     System.Diagnostics.Debug.WriteLine("Weapon Buff Expired. Switched back to BulletWeapon.");
                 }
            }

            // Apply Velocity to Position (Original Logic Style)
            _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
            _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

            // Apply Drag (Original Logic Style)
             _velocity *= 0.99f; // Original drag factor

            base.Update(gameTime);
        }

        // Helper method to get the correct turret texture based on the weapon
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
            // Draw Ship Body - Use _rotation calculated in HandleInput
            Vector2 bodyOrigin = ship_body.Bounds.Size.ToVector2() / 2f;
            spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, _rotation, bodyOrigin, 1f, SpriteEffects.None, 0);

            // Get the correct turret texture
            Texture2D turretTexture = GetCurrentTurretTexture();

            // Aiming logic
            Point currentScreenMouse = Mouse.GetState().Position;
            Vector2 currentWorldMouse = GameManager.GetGameManager().ScreenToWorld(currentScreenMouse.ToVector2());
            Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), currentWorldMouse);
            float aimAngle = LinePieceCollider.GetAngle(aimDirection);
            Vector2 turretPosition = _rectangleCollider.shape.Center.ToVector2();
            Vector2 turretOrigin = turretTexture.Bounds.Size.ToVector2() / 2f;

            // Draw the selected turret texture
            spriteBatch.Draw(turretTexture, turretPosition, null, Color.White, aimAngle, turretOrigin, 1f, SpriteEffects.None, 0);

            base.Draw(gameTime, spriteBatch);
        }

        // Inside Ship.cs

        public override void OnCollision(GameObject other)
        {
            // Planet collision logic (remains the same)
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
            // --- Supply Collision - Random Weapon ---
            else if (other is Supply) // No need to cast to check type here
            {
                // 1. Get the Random Number Generator from GameManager
                Random rng = GameManager.GetGameManager().RNG;

                // 2. Choose randomly between the two weapons (0 = Laser, 1 = Double Barrel)
                int weaponChoice = rng.Next(2); // Generates either 0 or 1

                // 3. Assign the chosen weapon
                if (weaponChoice == 0)
                {
                    _currentWeapon = _laserWeapon;
                    System.Diagnostics.Debug.WriteLine("Ship collided with Supply, switched to LaserWeapon!");
                }
                else // weaponChoice == 1
                {
                    _currentWeapon = _doubleBarrelWeapon;
                    System.Diagnostics.Debug.WriteLine("Ship collided with Supply, switched to DoubleBarrelWeapon!");
                }

                // 4. Start the buff timer regardless of which weapon was chosen
                _weaponBuffTimer = _weaponBuffDuration;

                // 5. Let the Supply remove itself (handled in Supply.OnCollision)
            }
            // Handle other collisions if needed (e.g., else if (other is Alien) ...)
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }
}