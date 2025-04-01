using Microsoft.Xna.Framework;

namespace SpaceDefence.Weapons
{
    public class DoubleBarrelWeapon : BaseWeapon
    {
        private readonly float _bulletSpeed = 400f; 
        private readonly float _barrelSpread = 10.0f; 
        
        public override float FireRate => 0.1f;
        
        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
          

            
            Vector2 offset = perpendicular * _barrelSpread / 2f; 
            
            Vector2 leftBarrelPos = position + offset;
            Vector2 rightBarrelPos = position - offset;

            
            Bullet bullet1 = new Bullet(leftBarrelPos, direction, _bulletSpeed);
            Bullet bullet2 = new Bullet(rightBarrelPos, direction, _bulletSpeed);

            
            GameManager gameManager = GameManager.GetGameManager();
            gameManager.AddGameObject(bullet1);
            gameManager.AddGameObject(bullet2);

            
            ResetCooldown();
        }
    }
}