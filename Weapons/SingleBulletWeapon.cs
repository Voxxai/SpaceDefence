// SingleBulletWeapon.cs
using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons // Use the same namespace
{
    public class SingleBulletWeapon : BaseWeapon // Inherit from BaseWeapon
    {
        private readonly float _bulletSpeed = 400f; // Speed of the bullet projectile

        // Implement the abstract FireRate property from BaseWeapon
        public override float FireRate => 0.2f; // Set the desired fire rate (5 shots/sec)

        // Implement the abstract Fire method from BaseWeapon
        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            // Create a new bullet instance
            Bullet bullet = new Bullet(position, direction, _bulletSpeed);

            // Add the bullet to the game world
            GameManager.GetGameManager().AddGameObject(bullet);

            // Reset the cooldown timer (using the method from BaseWeapon)
            ResetCooldown();
        }
    }
}