using Godot;
using System;

namespace ZombieHoardGame.PlayerCharacter.States
{
    public partial class Jumping : PlayerState
    {
        [Export]
        private float _heightTarget = 1;

        private float _impluseSpeed;

        public override void _Ready()
        {
            CalculateImpluseSpeed();
        }

        public override void PhysicsUpdate(float delta)
        {
            Vector3 newVelocity = _player.Velocity;
            newVelocity.Y += _impluseSpeed;
            _player.Velocity = newVelocity;
            _snapVector = Vector3.Zero;
            PlayerMoveAndSlide();
            UpdateState();
        }

        private void CalculateImpluseSpeed()
        {
            float timeToPeak = Mathf.Sqrt((-2.0f * _heightTarget) / -_gravityAcceleration);
            _impluseSpeed = (2 * _heightTarget) / timeToPeak;
        }
    }
}
