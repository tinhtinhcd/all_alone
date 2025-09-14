using Godot;
using System;


namespace ZombieHoardGame.PlayerCharacter.States
{
    public partial class Sprinting : MoveState
    {
        private AudioStreamPlayer _audioBreathing;
        private Timer _maxDurationTimer;
        private Timer _cooldownTimer;
        private float _cooldownTimeMax = 0;


        public override void _Ready()
        {
            base._Ready();
            _audioBreathing = GetNode<AudioStreamPlayer>("AudioBreathing");
            _maxDurationTimer = GetNode<Timer>("MaxDurationTimer");
            _cooldownTimer = GetNode<Timer>("CooldownTimer");
            _cooldownTimeMax = (float)_cooldownTimer.WaitTime;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (!_cooldownTimer.IsStopped())
            {
                // Reduce volume of breathing effect
                float effectStrength = (float)_cooldownTimer.TimeLeft / _cooldownTimeMax;
                SetBreathingAudioVolume(effectStrength);
            }
            else if (!_maxDurationTimer.IsStopped())
            {
                // Increase volume of breathing effect
                float effectStrength = 1 - ((float)_maxDurationTimer.TimeLeft / (float)_maxDurationTimer.WaitTime);
                SetBreathingAudioVolume(effectStrength);
            }
        }


        public override void Enter()
        {
            base.Enter();
            if (_cooldownTimer.IsStopped())  // TODO: Work cooldown feature into base state machine
            {
                _maxDurationTimer.Start();
                _maxDurationTimer.Connect("timeout", new Callable(this, nameof(OnMaxDurationTimerTimeout)));
            }
            else
            {
                EmitSignal(nameof(ChangeStateRequest), "Walking");
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (_maxDurationTimer.IsConnected("timeout", new Callable(this, nameof(OnMaxDurationTimerTimeout))))
            {
                float cooldownProportion = ((float)_maxDurationTimer.WaitTime - (float)_maxDurationTimer.TimeLeft) / (float)_maxDurationTimer.WaitTime;
                _cooldownTimer.Start(_cooldownTimeMax * cooldownProportion);
                _maxDurationTimer.Stop();
                _maxDurationTimer.Disconnect("timeout", new Callable(this, nameof(OnMaxDurationTimerTimeout)));
            }
        }

        private void OnMaxDurationTimerTimeout()
        {
            EmitSignal(nameof(ChangeStateRequest), "Walking");
        }

        private void SetBreathingAudioVolume(float volLinear)
        {
            _audioBreathing.VolumeDb = Mathf.LinearToDb(volLinear);
            if (volLinear == 0)
            {
                _audioBreathing.Stop();
            }
            else if (!_audioBreathing.Playing)
            {
                _audioBreathing.Play();
            }
        }

    }
}

