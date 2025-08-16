using Godot;


namespace UI.MainMenu
{
    public partial class CreditsPage : MultiPage.Page
    {
        private void OnBackButtonPressed()
        {
            if(IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), _backPageName);
            }
        }
    }
}

