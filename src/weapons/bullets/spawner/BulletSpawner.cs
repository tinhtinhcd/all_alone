using Godot;


namespace Weapons
{
    public partial class BulletSpawner : Node
    {
        public bool DoubleProjectileSpawn { set; get; } = false;

        private PackedScene _bulletPackedScene;

        public override void _Ready()
        {
            _bulletPackedScene = GD.Load<PackedScene>("res://src/weapons/bullets/Bullet.tscn");
        }
        
        public void SpawnGunShot(Vector3 spawnPoint, Vector3 directionForward, Vector3 directionRight, float spread, Gun originGun, Node3D origin)
        {
            
            int projectileCount = originGun.ShotProjectileCount;
            if (DoubleProjectileSpawn){
                projectileCount *= 2;
            }

            for (int i = 0; i < projectileCount; i ++)
            {
                SpawnBulletProjectile(spawnPoint, directionForward, directionRight, spread, originGun, origin);
            }
        }

        private void SpawnBulletProjectile(Vector3 spawnPoint, Vector3 directionForward, Vector3 directionRight, float spread, Gun originGun, Node3D origin)
        {
            Bullet _newBullet = _bulletPackedScene.Instantiate<Bullet>();

            float deflection = (float)(Mathf.DegToRad(spread * 0.5f) * GD.RandfRange(-1, 1));
            Vector3 bulletDirection = directionForward.Rotated(directionRight, deflection);
            bulletDirection = bulletDirection.Rotated(directionForward, (float)GD.RandfRange(0, Mathf.Tau));

            _newBullet.Velocity = bulletDirection * originGun.ProjectileSpeed;
            _newBullet.Damage = originGun.ProjectileDamage;
            _newBullet.SpawnVector = spawnPoint;
            _newBullet.MaxRange = originGun.MaxRange;
            _newBullet.Origin = origin;

            AddChild(_newBullet);
        }
    }
}
