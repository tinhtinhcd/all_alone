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

            float deflection = (float)(Mathf.DegToRad(spread * 0.5f) * GD.Randf() * (1 - -1) + -1);
            Vector3 bulletDirection = directionForward.Rotated(directionRight, deflection);
            bulletDirection = bulletDirection.Rotated(directionForward, (float)GD.Randf() * (Mathf.Tau - 0) + 0);

            _newBullet.Velocity = bulletDirection * originGun.ProjectileSpeed;
            _newBullet.Damage = originGun.ProjectileDamage;
            _newBullet.SpawnVector = spawnPoint;
            _newBullet.MaxRange = originGun.MaxRange;

            AddChild(_newBullet);
        }
    }
}
