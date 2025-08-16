using Godot;


namespace PowerUps
{
    public partial class PowerUpData : Resource
    {
        [Export]
        public PowerUp.Type Type;
        [Export]
        public Mesh Mesh;
        [Export]
        public Texture2D Icon;
    }
}

