using Godot;
using System;


namespace UI.MainMenu
{
    public partial class Home : MultiPage.Page
    {
        [Signal]
        public delegate void PlayLevelRequestedEventHandler(String levelName);

        private void OnBtnPlayPressed()
        {
            if(IsActive)
            {
                EmitSignal(nameof(PlayLevelRequested), "Level1");
            }
        }

        private void OnBtnExitPressed()
        {
            if(IsActive)
            {
                GetTree().Quit();
            }
        }

        private void OnBtnSettingsPressed()
        {
            if(IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), "SettingsPage");
            }
        }

        private void OnBtnControlsPressed()
        {
            if(IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), "ControlsPage");
            }
        }

        private void OnBtnCreditsPressed()
        {
            if(IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), "CreditsPage");
            }
        }
    }
}

