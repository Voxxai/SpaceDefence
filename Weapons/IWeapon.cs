// IWeapon.cs
using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons
{
    public interface IWeapon
    {
        /// <summary>
        /// Gets the time delay (in seconds) required between shots for this weapon.
        /// </summary>
        float FireRate { get; }

        /// <summary>
        /// Gets whether the weapon is currently ready to fire (cooldown finished).
        /// </summary>
        bool CanFire { get; }

        /// <summary>
        /// Updates the weapon's internal cooldown timer.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update frame.</param>
        void UpdateCooldown(float deltaTime);

        /// <summary>
        /// Executes the firing logic for this weapon.
        /// </summary>
        /// <param name="position">The world position where the projectile should originate.</param>
        /// <param name="direction">The normalized direction vector the projectile should travel.</param>
        /// <param name="owner">Reference to the Ship firing the weapon (optional, if needed).</param>
        void Fire(Vector2 position, Vector2 direction, Ship owner = null); // Made owner optional
    }
}