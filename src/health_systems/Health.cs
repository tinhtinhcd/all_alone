using Godot;
using System;
using System.Collections.Generic;


namespace HealthSystem
{
    public partial class Health : Node3D
    {
        // CONSIDER: Emit "HealthEvent" signal which has a reference parameter containing data about the event
        [Signal]
        public delegate void HurtEventHandler(Node inflictor, int value, bool isCritical);
        [Signal]
        public delegate void IncreasedEventHandler();
        [Signal]
        public delegate void PointsChangedEventHandler();

        [Export]
        private int _pointsMax = 100;
        [Export]
        private float _regenRate = 2; // CONSIDER: Make regen a seperate component?
        [Export]
        private float _regenDelay = 3;
        [Export]
        private Godot.Collections.Array<NodePath> _hurtBoxNodePaths = new Godot.Collections.Array<NodePath>();

        public float Points {
            private set;
            get;
        }
        public int PointsMax{ 
            set; get;
        }
        public float PointsPercentage{
            get { return (float)Points / (float)PointsMax; }
        }

        private Timer _regenDelayTimer;

        public override void _Ready()
        {
            PointsMax = _pointsMax;
            Points = PointsMax;

            foreach (NodePath hurtboxNodePath in _hurtBoxNodePaths)
            {
                HurtBox hurtbox = GetNode<HurtBox>(hurtboxNodePath);
                hurtbox.Connect(nameof(HurtBox.Hurt), new Callable(this, nameof(OnHurtBoxHurt)));
            }

            _regenDelayTimer = GetNode<Timer>("RegenDelayTimer");
            _regenDelayTimer.WaitTime = _regenDelay;
        }

        public override void _Process(double delta)
        {
            if (_regenRate > 0 && Points < PointsMax && _regenDelayTimer.IsStopped())
            {
                IncrementPoints((float)(float)delta * _regenRate);
                EmitSignal(nameof(Increased));
            }
        }

        public void RestoreToMax()
        {
            Points = PointsMax;
        }
        
        private void OnHurtBoxHurt(HurtBox box, Node inflictor, int damageValue)
        {
            IncrementPoints(-damageValue);
            EmitSignal(nameof(Hurt), inflictor, damageValue, box.IsCritical);
            if (_regenRate > 0)
            {
                _regenDelayTimer.Start();
            }        
        }

        private void IncrementPoints(float increment)
        {
            Points = Mathf.Clamp(Points + increment, 0, PointsMax);
            EmitSignal(nameof(PointsChanged));
        }
    }
}

