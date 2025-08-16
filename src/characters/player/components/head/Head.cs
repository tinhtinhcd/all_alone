using Godot;

namespace ZombieHoardGame.PlayerCharacter
{
    public partial class Head : Node3D
    {

        public Basis CameraBasis
        {
            get { return _camera.GlobalTransform.Basis; }
        }

        public float CameraFOV { get { return _camera.Fov; }}

        private Camera3D _camera;
        private Marker3D _weaponAnchorPoint;

        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;

            _camera = GetNode<Camera3D>("Camera3D");
            _weaponAnchorPoint = GetNode<Marker3D>("Guns");
        }

    }
}

