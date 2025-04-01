using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons
{
    public interface IWeapon
    {

        float FireRate { get; }


        bool CanFire { get; }


        void UpdateCooldown(float deltaTime);


        void Fire(Vector2 position, Vector2 direction, Ship owner = null); // Made owner optional
    }
}