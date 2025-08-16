using Godot;
using System;
using System.Diagnostics;

// Main scene node, program starts here.
// Manages loading of and transitions between major secnes
namespace GameGeneral
{
    public partial class Main : Node
    {
        /// Exported Fields ///
        [Export]
        private PackedScene _initialScene = null;

        private const float _POLL_TIME = 0.02f;
        private const String _levelDirectoryPath = "res://src/levels/";
        private const String _mainMenuFilePath = "res://src/menu_ui/main_menu/MainMenu.tscn";


        /// Fields - protected or private ///
        private ResourceLoader.ThreadLoadStatus _loaderStatus;
        private AnimationPlayer _animPlayer;
        private ColorRect _fadeRect;
        private TextureProgressBar _progressBar;
        private Timer _loaderPollTimer;
        private string _currentLoadingPath;


        private Main() {}
        private static Main instance;
        public static Main Instance {
            get {
                return instance;
            }
        }


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            instance = this;
            CacheNodeReferences();
            Setup();
        }

        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void TransitionToLevel(String levelName)
        {
            ChangeSceneBackground($"{_levelDirectoryPath}{levelName}.tscn");
        }

        public void TransitionToMainMenu()
        {
            ChangeSceneBackground(_mainMenuFilePath);
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnChangeSceneRequest(String sceneResPath)
        {
            CallDeferred(nameof(ChangeSceneBackground), sceneResPath);
        }

        private void OnAnimationFinished(String animName)
        {
            _fadeRect.MouseFilter = Control.MouseFilterEnum.Ignore;
        }

        private void OnLoaderPollTimerTimeout()
        {
            var status = ResourceLoader.LoadThreadedGetStatus(_currentLoadingPath);

            switch (status)
            {
                case ResourceLoader.ThreadLoadStatus.Loaded:
                    PackedScene res = (PackedScene)ResourceLoader.LoadThreadedGet(_currentLoadingPath);
                    SetCurrentScene(res.Instantiate());
                    break;
                
                case ResourceLoader.ThreadLoadStatus.InProgress:
                    var progress = new Godot.Collections.Array();
                    ResourceLoader.LoadThreadedGetStatus(_currentLoadingPath, progress);
                    if (progress.Count > 0)
                    {
                        _progressBar.Value = (float)progress[0];
                    }
                    _loaderPollTimer.Start();
                    break;

                default:
                    Debug.Assert(false, $"ResourceLoader Error: {status.ToString()}");
                    break;
            }
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void CacheNodeReferences()
        {
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _fadeRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
            _progressBar = GetNode<TextureProgressBar>("CanvasLayer/TextureProgressBar");
            _loaderPollTimer = GetNode<Timer>("LoaderPollTimer");
        }

        private void Fade(bool fadeIn)
        {
            _fadeRect.MouseFilter = Control.MouseFilterEnum.Stop;
            _progressBar.Hide();
            if (fadeIn)
            {
                _animPlayer.Play("fade");
            }
            else
            {
                _animPlayer.PlayBackwards("fade");
            }
        }

        private async void SetCurrentScene(Node newScene)
        {
            GetTree().Root.AddChild(newScene);
            GetTree().CurrentScene = newScene;

            if (_animPlayer.IsPlaying())
            {
                await ToSignal(_animPlayer, "animation_finished");
            }
            Fade(false);
        }

        private async void Setup()
        {
            _loaderPollTimer.WaitTime = _POLL_TIME;

            // Make sure root node is ready to take new child
            await ToSignal(GetTree().Root, "ready");
            SetCurrentScene(_initialScene.Instantiate());
        }

        private async void ChangeSceneBackground(String newScenePath)
        {
            Fade(true);
            await ToSignal(_animPlayer, "animation_finished");
            _progressBar.Value = 0;
            _progressBar.Show();
            
            Node currentScene = GetTree().CurrentScene;
            if (currentScene != null)
            {
                currentScene.QueueFree();
                GetTree().CurrentScene = null;
            }
            
            _currentLoadingPath = newScenePath;
            ResourceLoader.LoadThreadedRequest(newScenePath, "PackedScene");
            _loaderPollTimer.Start();
        }
    }
}
