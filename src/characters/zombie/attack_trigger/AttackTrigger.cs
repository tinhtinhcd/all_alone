using Godot;
using System.Diagnostics;
using ZombieHoardGame.PlayerCharacter;


namespace ZombieHoardGame.ZombieCharacter
{
    public partial class AttackTrigger : Area3D
    {
        [Signal]
        public delegate void PlayerEnteredEventHandler(Player player);

        public bool IsPlayerInside{
            private set;
            get;
        }

        public override void _Ready()
        {
            Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
            Connect("body_exited", new Callable(this, nameof(OnBodyExited)));
        }
        
        private void OnBodyEntered(PhysicsBody3D body)
        {
            Debug.Assert(body is Player, "AttackTrigger must only detect Player characters");
            IsPlayerInside = true;
            EmitSignal(nameof(PlayerEntered), (Player)body);
        }

        private void OnBodyExited(PhysicsBody3D body)
        {
            Debug.Assert(body is Player, "AttackTrigger must only detect Player characters");
            IsPlayerInside = false;
        }
    }
}

