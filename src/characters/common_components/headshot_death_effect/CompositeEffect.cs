using Godot;


namespace GameGeneral
{
    public partial class CompositeEffect : Node3D
    {
        public void Play()
        {
            foreach (Node effect in GetChildren())
            {
                if (effect is GpuParticles3D)
                {
                    ((GpuParticles3D)effect).Emitting = true;
                }
                else if (effect is AudioStreamPlayer3D)
                {
                    ((AudioStreamPlayer3D)effect).Play();
                }
            }
        }
    }
}

