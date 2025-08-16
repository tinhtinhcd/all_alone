using Godot;


namespace UI.MainMenu
{
    public partial class BackgroundCamera : Camera3D
    {
        [Export]
        private FastNoiseLite _noise = null;
        [Export]
        private Vector3 _maxRotationDeg = new Vector3(10, 10, 2);
        [Export]
        private float _shakeSpeed = 1.5f;


        private Vector3 _initialRotationDeg;


        public override void _Ready()
        {
            _initialRotationDeg = RotationDegrees;
        }

        public override void _PhysicsProcess(double delta)
        {
            Vector3 newRotationDeg = _initialRotationDeg;
            newRotationDeg.X += _maxRotationDeg.X * RandNoiseValue(1);
            newRotationDeg.Y += _maxRotationDeg.Y * RandNoiseValue(2);
            newRotationDeg.Z += _maxRotationDeg.Z * RandNoiseValue(3);
            
            RotationDegrees = newRotationDeg;
        }

        private float RandNoiseValue(int seed)
        {
            _noise.Seed = seed;
            return _noise.GetNoise1D(Time.GetTicksMsec() * 0.001f * _shakeSpeed);
        }
    }
}

