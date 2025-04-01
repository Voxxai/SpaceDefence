// LaserWeapon.cs
using Microsoft.Xna.Framework;
using SpaceDefence.Collision;

namespace SpaceDefence.Weapons // Use the same namespace
{
    public class LaserWeapon : BaseWeapon // Inherit from BaseWeapon
    {
        private readonly float _laserLength = 800f; // Length of the laser beam

        // Implement the abstract FireRate property
        // Let's make the laser fire a bit faster than the bullet, adjust as needed
        public override float FireRate => 0.15f;

        // Implement the abstract Fire method
        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            // Create the LinePieceCollider for the laser, using start, direction, and length
            // See LinePieceCollider.cs [cite: uploaded:SpaceDefence/Collision/LinePieceCollider.cs]
            LinePieceCollider laserCollider = new LinePieceCollider(position, direction, _laserLength);

            // Create a new Laser instance, passing the collider and length (optional constructor overload)
            // See Laser.cs [cite: uploaded:SpaceDefence/Laser.cs]
            Laser laser = new Laser(laserCollider); // Constructor might just need the collider
            // Or if using the constructor overload: Laser laser = new Laser(laserCollider, _laserLength);

            // Add the laser to the game world
            GameManager.GetGameManager().AddGameObject(laser);

            // Reset the cooldown timer
            ResetCooldown();
        }
    }
}