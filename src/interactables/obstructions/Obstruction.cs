using Godot;
using System;


namespace ZombieHoardGame
{
    public partial class Obstruction : StaticBody3D, IInteractable
    {
        [Signal]
        public delegate void ClearedEventHandler();
        
        [Export]
        private int _buyCost = 750;

        public int BuyCost { get{ return _buyCost; } }

        private GpuParticles3D _clearEffect;
        private AudioStreamPlayer3D _audioClear;
        private NavigationRegion3D _navMeshInst;

        public override void _Ready()
        {
            _clearEffect = GetNode<GpuParticles3D>("ClearEffect");
            _audioClear = GetNode<AudioStreamPlayer3D>("AudioClear");
            _navMeshInst = GetNode<NavigationRegion3D>("NavMeshInst");

            _navMeshInst.Enabled = false;
        }

        public String GetInteractText()
        {
            return $"[BUY - {_buyCost}]";
        }

        public void Clear(PlayerCharacter.Player player)
        {
            _clearEffect.Emitting = true;
            _audioClear.Play();
            foreach (Node3D child in GetChildren())
            {
                if (child is CollisionShape3D)
                {
                    ((CollisionShape3D)child).Disabled = true;
                }
                else if (child is MeshInstance3D)
                {
                    child.Hide();
                }
            }
            player.IncrementPoints(-BuyCost);
            EmitSignal(nameof(Cleared));
            _navMeshInst.Enabled = true;
        }
    }
}

