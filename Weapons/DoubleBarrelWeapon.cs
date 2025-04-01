// DoubleBarrelWeapon.cs
using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons // Use the same namespace
{
    public class DoubleBarrelWeapon : BaseWeapon // Inherit from BaseWeapon
    {
        private readonly float _bulletSpeed = 400f; // Speed of the bullets
        private readonly float _barrelSpread = 10.0f; // How far apart the barrels are (pixels)

        // Implement FireRate - make it faster (half the time) than SingleBulletWeapon
        // If SingleBulletWeapon.FireRate is 0.2f, this is 0.1f
        public override float FireRate => 0.1f;

        // Implement the Fire method
        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            // Calculate a vector perpendicular to the direction for barrel offset
            // If direction = (dx, dy), perpendicular = (-dy, dx)
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
            // Ensure it's normalized if direction wasn't guaranteed to be (it should be)
            // perpendicular.Normalize(); // Not strictly needed if direction is normalized

            // Calculate the offset amount based on the spread
            Vector2 offset = perpendicular * _barrelSpread / 2f; // Divide by 2 for half spread each side

            // Calculate the position for each barrel
            Vector2 leftBarrelPos = position + offset;
            Vector2 rightBarrelPos = position - offset;

            // Create two bullets, one from each position, same direction and speed
            Bullet bullet1 = new Bullet(leftBarrelPos, direction, _bulletSpeed);
            Bullet bullet2 = new Bullet(rightBarrelPos, direction, _bulletSpeed);

            // Add both bullets to the game world
            GameManager gameManager = GameManager.GetGameManager();
            gameManager.AddGameObject(bullet1);
            gameManager.AddGameObject(bullet2);

            // Reset the cooldown timer
            ResetCooldown();
        }
    }
}