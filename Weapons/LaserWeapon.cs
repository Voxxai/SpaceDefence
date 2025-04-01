using Microsoft.Xna.Framework;
using SpaceDefence.Collision;

namespace SpaceDefence.Weapons 
{
    public class LaserWeapon : BaseWeapon 
    {
        private readonly float _laserLength = 800f; 

        
        
        public override float FireRate => 0.15f;

        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            
            LinePieceCollider laserCollider = new LinePieceCollider(position, direction, _laserLength);

            
            Laser laser = new Laser(laserCollider);
            

            
            GameManager.GetGameManager().AddGameObject(laser);

            
            ResetCooldown();
        }
    }
}