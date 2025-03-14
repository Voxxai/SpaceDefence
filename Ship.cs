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

    // Movement variables
    private Vector2 _velocity;
    private readonly float _acceleration = 10f;
    private float _rotation;
    private Vector2 _currentDirection = Vector2.Zero;

    /// <summary>
    ///     The player character
    /// </summary>
    /// <param name="Position">The ship's starting position</param>
    public Ship(Point Position)
    {
        _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
        SetCollider(_rectangleCollider);
        _velocity = Vector2.Zero; // Initialize velocity
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
        target = inputManager.CurrentMouseState.Position;
        if (inputManager.LeftMousePress())
        {
            Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center, target);
            Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() + aimDirection * base_turret.Height / 2f;
            if (buffTimer <= 0)
            {
                GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
            }
            else
            {
                GameManager.GetGameManager()
                    .AddGameObject(new Laser(new LinePieceCollider(turretExit, target.ToVector2()), 400));
            }
        }

        KeyboardState keyState = Keyboard.GetState();
        Vector2 accelerationDirection = Vector2.Zero;

        // Get input direction
        if (keyState.IsKeyDown(Keys.W))
        {
            accelerationDirection.Y = -1;
        }

        if (keyState.IsKeyDown(Keys.S))
        {
            accelerationDirection.Y = 1;
        }

        if (keyState.IsKeyDown(Keys.A))
        {
            accelerationDirection.X = -1;
        }

        if (keyState.IsKeyDown(Keys.D))
        {
            accelerationDirection.X = 1;
        }

        // Normalize the direction for consistent speed
        if (accelerationDirection != Vector2.Zero)
        {
            accelerationDirection.Normalize();
            _currentDirection = accelerationDirection;
            _rotation = LinePieceCollider.GetAngle(accelerationDirection);
        }

        // Apply acceleration
        _velocity += accelerationDirection * _acceleration;
    }

    public override void Update(GameTime gameTime)
    {
        // Update the Buff timer
        if (buffTimer > 0) buffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Apply velocity to position
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
        _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

        // Screen wrapping
        if (_rectangleCollider.shape.X > GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width)
            _rectangleCollider.shape.X = -_rectangleCollider.shape.Width;
        if (_rectangleCollider.shape.X + _rectangleCollider.shape.Width < 0)
            _rectangleCollider.shape.X = GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width;
        if (_rectangleCollider.shape.Y > GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height)
            _rectangleCollider.shape.Y = -_rectangleCollider.shape.Height;
        if (_rectangleCollider.shape.Y + _rectangleCollider.shape.Height < 0)
            _rectangleCollider.shape.Y = GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height;

        // Apply drag to velocity (for demonstration)
        _velocity *= 0.99f; // Adjust this value as needed

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var origin = new Vector2(ship_body.Width / 2f, ship_body.Height / 2f);
        spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, _rotation, origin,
            1f, SpriteEffects.None, 0);

        var aimAngle = LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(GetPosition().Center, target));
        if (buffTimer <= 0)
        {
            var turretLocation = base_turret.Bounds;
            turretLocation.Location = _rectangleCollider.shape.Center;
            spriteBatch.Draw(base_turret, turretLocation, null, Color.White, aimAngle,
                turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
        }
        else
        {
            var turretLocation = laser_turret.Bounds;
            turretLocation.Location = _rectangleCollider.shape.Center;
            spriteBatch.Draw(laser_turret, turretLocation, null, Color.White, aimAngle,
                turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
        }

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