using Godot;


namespace UI.PauseMenu
{
    public partial class PauseMenu : MultiPage.Manager
    {
        [Signal]
        public delegate void QuitRequestedEventHandler();
        [Signal]
        public delegate void RestartRequestedEventHandler();

        private AnimationPlayer _animPlayer = null;

        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            base._Ready();
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            Home homePage = (Home)_pages["Home"];
            homePage.Connect(nameof(Home.ResumeRequested), new Callable(this, nameof(OnResumeRequested)));
            homePage.Connect(nameof(Home.RestartRequested), new Callable(this, nameof(OnRestartRequested)));
            homePage.Connect(nameof(Home.QuitRequested), new Callable(this, nameof(OnQuitRequested)));
        }
        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void Popup()
        {
            Show();
            Input.MouseMode = Input.MouseModeEnum.Confined;
            _animPlayer.Play("popup");
            GetTree().Paused = true;
        }

        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private async void OnResumeRequested()
        {
            if (CanTakeUserInput())
            {
                _animPlayer.PlayBackwards("popup");
                await ToSignal(_animPlayer, "animation_finished");
                GetTree().Paused = false;
                Input.MouseMode = Input.MouseModeEnum.Captured;
                Hide();
            }
        }

        private void OnRestartRequested()
        {
            if (CanTakeUserInput())
            {
                GetTree().Paused = false;
                EmitSignal(nameof(RestartRequested));
            }
        }

        private void OnQuitRequested()
        {
            if (CanTakeUserInput())
            {
                GetTree().Paused = false;
                Input.MouseMode = Input.MouseModeEnum.Confined;
                EmitSignal(nameof(QuitRequested));
            }
        }

        private bool CanTakeUserInput()
        {
            return Visible && !_animPlayer.IsPlaying();
        }
    }
}

