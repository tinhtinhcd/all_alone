using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public partial class Dead : ZombieState
    {
        [Export]
        private int _despawnDelay = 5;

        public override async void Enter()
        {
            base.Enter();
            await ToSignal(GetTree().CreateTimer(_despawnDelay), "timeout");
            VisibleOnScreenNotifier3D vn = _blackboard.VisibleOnScreenNotifier3D;
            if (vn.IsOnScreen())
            {
                _blackboard.VisibleOnScreenNotifier3D.Connect("screen_exited", new Callable(this, nameof(OnVisibilityNotifierScreenExited)));
            }
            else
            {
                _blackboard.Character.QueueFree();
            }
        }

        protected void OnVisibilityNotifierScreenExited()
        {
            _blackboard.Character.QueueFree();
        }
    }
}

