using Godot;
using System.Collections.Generic;


namespace HealthSystem
{
    public partial class HitBox : Area3D
    {
        [Export]
        private int _damage = 20;
        [Export(PropertyHint.Flags, "player,zombie")]
        private int _damageLayers = 0;

        private List<HurtBox> _overlappedHurtBoxes = new List<HurtBox>();

        public override void _Ready()
        {
            Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
            Connect("area_exited", new Callable(this, nameof(OnAreaExited)));
        }

        public void Activate()
        {
            foreach(HurtBox box in _overlappedHurtBoxes)
            {
                box.ApplyDamage(_damage, Owner);
            }
        }

        private bool HasMutualDamageLayer(HurtBox compaireBox)
        {
            return (_damageLayers & compaireBox.DamageLayers) > 0;
        }

        private void OnAreaEntered(Area3D newArea)
        {
            if (HasMutualDamageLayer((HurtBox)newArea))
            {
                AddOverlappedHurtBox((HurtBox)newArea);
            }
        }


        private void OnAreaExited(Area3D newArea)
        {
            if (HasMutualDamageLayer((HurtBox)newArea))
            {
                _overlappedHurtBoxes.Remove((HurtBox)newArea);
            }
        }

        private void AddOverlappedHurtBox(HurtBox newHurtBox)
        {
            _overlappedHurtBoxes.Add(newHurtBox);
        }
    }
}

