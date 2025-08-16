using Godot;
using System;
using GameGeneral;


namespace UI.MainMenu
{
    public partial class MainMenu : MultiPage.Manager
    {
        public override void _Ready()
        {
            base._Ready();

            Home homePage = (Home)_pages["Home"];
            homePage.Connect(nameof(Home.PlayLevelRequested), new Callable(this, nameof(OnHomePlayLevelRequested)));
        }


        private void OnHomePlayLevelRequested(String levelName)
        {
            Main.Instantiate.TransitionToLevel(levelName);
        }
    }
}

