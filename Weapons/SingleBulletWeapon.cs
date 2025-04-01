using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons
{
    public class SingleBulletWeapon : BaseWeapon
    {
        private readonly float _bulletSpeed = 400f; 

        
        public override float FireRate => 0.2f; // Set the desired fire rate (5 shots/sec)
        
        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            
            Bullet bullet = new Bullet(position, direction, _bulletSpeed);

            GameManager.GetGameManager().AddGameObject(bullet);

            
            ResetCooldown();
        }
    }
}