using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceDefence.Collision;

namespace SpaceDefence;

public class Ship : GameObject
{
    private Texture2D ship_body;
    private Texture2D base_turret;
    private Texture2D laser_turret;
    private float buffTimer = 10;
    private readonly float buffDuration = 10f;
    private readonly RectangleCollider _rectangleCollider;
    private Point target;
    
    private Vector2 _velocity;
    private readonly float _acceleration = 10f;
    private float _rotation;
    private Vector2 _currentDirection = Vector2.Zero;
    
    private float _fireRate = 0.2f;
    private float _fireCooldownTimer = 0f;

    /// <summary>
    ///     The player character
    /// </summary>
    /// <param name="Position">The ship's starting position</param>
    public Ship(Point Position)
    {
        _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
        SetCollider(_rectangleCollider);
        _velocity = Vector2.Zero;
    }

    public override void Load(ContentManager content)
    {
        // Ship sprites from: https://zintoki.itch.io/space-breaker
        ship_body = content.Load<Texture2D>("ship_body");
        base_turret = content.Load<Texture2D>("base_turret");
        laser_turret = content.Load<Texture2D>("laser_turret");
        _rectangleCollider.shape.Size = ship_body.Bounds.Size;
        _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);
        base.Load(content);
    }

    public override void HandleInput(InputManager inputManager)
    {
        base.HandleInput(inputManager);
        
        Point screenMousePosition = inputManager.CurrentMouseState.Position;

        // Convert screen mouse position to WORLD coordinates using the GameManager helper
        Vector2 worldMousePosition =
            GameManager.GetGameManager().ScreenToWorld(screenMousePosition.ToVector2());

        // Use the WORLD mouse position for aiming calculations
        if (inputManager.LeftMousePress())
        {
            if (_fireCooldownTimer <= 0)
            {
                // Calculate direction from ship center to WORLD mouse position
                Vector2 aimDirection =
                    LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(),
                        worldMousePosition);

                // Calculate the turret exit point based on the ship's center and aim direction
                Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() + aimDirection * (base_turret.Height / 2f);

                if (buffTimer <= 0) // Standard bullet
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else // Laser (buff active)
                {
                    GameManager.GetGameManager()
                        .AddGameObject(new Laser(new LinePieceCollider(turretExit, worldMousePosition),800)); 
                }
                _fireCooldownTimer = _fireRate;
            }
            
        }
            
        KeyboardState keyState = Keyboard.GetState();
        Vector2 accelerationDirection = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.W)) accelerationDirection.Y = -1;
        if (keyState.IsKeyDown(Keys.S)) accelerationDirection.Y = 1;
        if (keyState.IsKeyDown(Keys.A)) accelerationDirection.X = -1;
        if (keyState.IsKeyDown(Keys.D)) accelerationDirection.X = 1;

        if (accelerationDirection != Vector2.Zero)
        {
            accelerationDirection.Normalize();
            _currentDirection = accelerationDirection;
            _rotation = LinePieceCollider.GetAngle(accelerationDirection);
        }

        _velocity += accelerationDirection * _acceleration;


    }

    public override void Update(GameTime gameTime)
    {
        if (_fireCooldownTimer > 0)
        {
            _fireCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        // Update the Buff timer
        if (buffTimer > 0) buffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Apply velocity to position
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
        _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

        // Screen wrapping
        // if (_rectangleCollider.shape.X > GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width)
        //     _rectangleCollider.shape.X = -_rectangleCollider.shape.Width;
        // if (_rectangleCollider.shape.X + _rectangleCollider.shape.Width < 0)
        //     _rectangleCollider.shape.X = GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width;
        // if (_rectangleCollider.shape.Y > GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height)
        //     _rectangleCollider.shape.Y = -_rectangleCollider.shape.Height;
        // if (_rectangleCollider.shape.Y + _rectangleCollider.shape.Height < 0)
        //     _rectangleCollider.shape.Y = GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height;

        _velocity *= 0.99f;

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Ship body drawing remains the same...
        var origin = new Vector2(ship_body.Width / 2f, ship_body.Height / 2f);
        spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, _rotation, origin,
            1f, SpriteEffects.None, 0);

        
        Point currentScreenMouse = Mouse.GetState().Position;
        
        // Convert to world coords for aiming
        Vector2 currentWorldMouse = GameManager.GetGameManager().ScreenToWorld(currentScreenMouse.ToVector2());

        // Calculate the aim angle based on ship center and WORLD mouse position
        Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), currentWorldMouse);
        float aimAngle = LinePieceCollider.GetAngle(aimDirection); 

        // Select the correct turret texture
        Texture2D turretTexture = (buffTimer <= 0) ? base_turret : laser_turret;

        // Calculate turret draw position (ship's center) and origin (center of the turret texture)
        Vector2 turretPosition = _rectangleCollider.shape.Center.ToVector2();
        Vector2 turretOrigin = turretTexture.Bounds.Size.ToVector2() / 2f;

        // Draw the turret pointing towards the world mouse position
        spriteBatch.Draw(turretTexture, turretPosition, null, Color.White, aimAngle, turretOrigin, 1f,
            SpriteEffects.None, 0);

        base.Draw(gameTime, spriteBatch);
    }

    public void Buff()
    {
        buffTimer = buffDuration;
    }

    public Rectangle GetPosition()
    {
        return _rectangleCollider.shape;
    }
}