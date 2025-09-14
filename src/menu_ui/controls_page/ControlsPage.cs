using Godot;
using System;


namespace UI
{
    public partial class ControlsPage : UI.MultiPage.Page
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

