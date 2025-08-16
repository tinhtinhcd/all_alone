using Godot;


namespace ZombieHoardGame
{
    public partial class RoundCounter : Node
    {
        [Signal]
        public delegate void IncrementedEventHandler(int newValue);
        
        public int RoundsStarted{
            private set;
            get;
        } = 0;

        public void Increment()
        {
            RoundsStarted++;
            EmitSignal(nameof(Incremented), RoundsStarted);
        }
    }
}

