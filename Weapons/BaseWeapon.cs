// BaseWeapon.cs
using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons // Use the same namespace
{
    public abstract class BaseWeapon : IWeapon
    {
        // Common cooldown timer for all weapons inheriting this base
        protected float CooldownTimer = 0f;

        // --- IWeapon Implementation ---

        // FireRate must be implemented by each specific weapon subclass
        public abstract float FireRate { get; }

        // CanFire is implemented here based on the common timer
        public bool CanFire => CooldownTimer <= 0f;

        // UpdateCooldown is implemented here
        public virtual void UpdateCooldown(float deltaTime)
        {
            if (CooldownTimer > 0f)
            {
                CooldownTimer -= deltaTime;
            }
        }

        // Fire must be implemented by each specific weapon subclass
        public abstract void Fire(Vector2 position, Vector2 direction, Ship owner = null);

        // --- Helper method ---

        /// <summary>
        /// Resets the cooldown timer after firing. Called by subclasses in their Fire method.
        /// </summary>
        protected virtual void ResetCooldown()
        {
            if (FireRate > 0) // Prevent infinite cooldown if FireRate is zero
            {
                CooldownTimer = FireRate;
            }
        }
    }
}