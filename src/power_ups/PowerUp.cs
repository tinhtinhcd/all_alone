using Godot;


namespace PowerUps
{
    public partial class PowerUp : Area3D
    {
        public enum Type {
            MaxAmmo, DoublePoints, DoubleTap
        }
        
        [Signal]
        public delegate void CollectedByPlayerEventHandler(PowerUp powerUp);
        [Signal]
        public delegate void TimedoutEventHandler(PowerUp powerUp);
        
        public PowerUpData ResourceData{
            set{
                _data = value;
                _meshInst.Mesh = _data.Mesh;
            }
        }

        public Type PowerupType{
            get { return _data.Type; }
        }

        public Texture2D Icon{
            get {return _data.Icon; }
        }

        private MeshInstance3D _meshInst;
        private Timer _timerDespawnWarning;
        private Timer _timerDespawn;
        private AnimationPlayer _animPlayer;
        private PowerUpData _data;
        private float _restY;
        private float _sinTime = 0;
        private float _sinBobAmplitude = 0.075f;

        public override void _Ready()
        {
            _meshInst = GetNode<MeshInstance3D>("MeshInstance3D");
            _timerDespawnWarning = GetNode<Timer>("TimerDespawnWarning");
            _timerDespawn = GetNode<Timer>("TimerDespawn");
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
            _timerDespawnWarning.Connect("timeout", new Callable(this, nameof(OnTimerDespawnWarningTimeout)));
            _timerDespawn.Connect("timeout", new Callable(this, nameof(OnTimerDespawnTimeout)));

            _restY = _meshInst.Position.y;
        }

        public override void _PhysicsProcess(double delta)
        {
            _sinTime += delta;
            Vector3 newTranslation = _meshInst.Position;
            newTranslation.y = _restY + (_sinBobAmplitude * Mathf.Sin(_sinTime * 0.6f));
            _meshInst.Position = newTranslation;
            _meshInst.RotateY(Mathf.DegToRad(16 * delta));
        }

        public async void Despawn()
        {
            _animPlayer.Play("collected");
            await ToSignal(_animPlayer, "animation_finished");
            QueueFree();
        }

        private void OnBodyEntered(PhysicsBody3D body)
        {
            // Collision mask should be set so only colliding wth the player
            _timerDespawn.Stop();
            _timerDespawnWarning.Stop();
            EmitSignal(nameof(CollectedByPlayer), this);
        }

        private void OnTimerDespawnWarningTimeout()
        {
            _animPlayer.Play("despawn_warning");
        }

        private void OnTimerDespawnTimeout()
        {
            EmitSignal(nameof(Timedout), this);
        }
    }
}

