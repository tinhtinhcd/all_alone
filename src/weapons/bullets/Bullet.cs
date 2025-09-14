using Godot;
using HealthSystem;
using System.Collections.Generic;


namespace Weapons
{
    public partial class Bullet : Node3D
    {
        public Vector3 Velocity { set; get; }
        public int Damage { set; get; }
        public Vector3 SpawnVector { set; get; }
        public float MaxRange { set; get; }
        public Vector3 DistanceVector { set; get; }
        public Node Origin {set; get; }


        private Vector3 _frameMoveVector;

        private float _maxRangeSquared;
        private RayCast3D _rayCast;
        private int _penetrationPointsInitial = 100;
        private int _penetrationPoints;
        private HashSet<CollisionObject3D> _bulletsCanPierceSet = new HashSet<CollisionObject3D>();


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            _maxRangeSquared = Mathf.Pow(MaxRange, 2);
            _penetrationPoints = _penetrationPointsInitial;

            _rayCast = GetNode<RayCast3D>("RayCast3D");

            GlobalPosition = SpawnVector;
        }

        public override void _PhysicsProcess(double delta)
        {
            _frameMoveVector = Velocity * (float)(float)delta;
            _rayCast.ClearExceptions();
            _rayCast.TargetPosition = _frameMoveVector;
            _rayCast.ForceRaycastUpdate();

            while (_rayCast.IsColliding())
            {
                CollisionObject3D collider = _rayCast.GetCollider() as CollisionObject3D;
                if (collider == null || !_bulletsCanPierceSet.Contains(collider))
                    break;
                else {
                    _rayCast.AddException(collider);
                    _rayCast.ForceRaycastUpdate();
                }
            }
            
            if (_penetrationPoints > 0)
            {
                Move((float)delta);
            }
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void Move(float delta)
        {
            GlobalPosition += _frameMoveVector;
            DistanceVector += _frameMoveVector;
            if (DistanceVector.LengthSquared() > _maxRangeSquared)
            {
                QueueFree();
            }
        }

        private void CollideWithHurtBox(HurtBox box, float delta)
        {
            float penetrationMultiplier = (float)_penetrationPoints / (float)_penetrationPointsInitial;
            int effectiveDamage = Mathf.RoundToInt(Damage * penetrationMultiplier);
            box.ApplyDamage(effectiveDamage, Origin);
            _penetrationPoints -= box.PenetrationResistance;
            if (_penetrationPoints <= 0)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            GlobalPosition = _rayCast.GetCollisionPoint();
            _rayCast.Enabled = false;
            SetPhysicsProcess(false);
            QueueFree();
        }
    }
}

