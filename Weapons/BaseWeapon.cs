using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons
{
    public abstract class BaseWeapon : IWeapon
    {
        protected float CooldownTimer = 0f;
        
        public abstract float FireRate { get; }
        
        public bool CanFire => CooldownTimer <= 0f;
        
        public virtual void UpdateCooldown(float deltaTime)
        {
            if (CooldownTimer > 0f)
            {
                CooldownTimer -= deltaTime;
            }
        }
        
        public abstract void Fire(Vector2 position, Vector2 direction, Ship owner = null);
        
        protected virtual void ResetCooldown()
        {
            if (FireRate > 0)
            {
                CooldownTimer = FireRate;
            }
        }
    }
}