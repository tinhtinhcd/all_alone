using Godot;
using System;
using System.Collections.Generic;


namespace ZombieHoardGame
{
    public partial class Zone : Node3D
    {
        [Signal]
        public delegate void ActivatedEventHandler();

        /// Exported Fields ///
        [Export]
        private Godot.Collections.Array<NodePath> _accessObstructionNodePaths = new Godot.Collections.Array<NodePath>();
        [Export]
        private bool _activateOnReady = false;

        /// Properties - public, protected, private ///
        public List<BoardedWindow> ZoneWindows{
            get { return _windows; }
        }

        /// Fields - protected or private ///
        private List<BoardedWindow> _windows = new List<BoardedWindow>();
        private bool _isActivated = false;


        public override void _Ready()
        {
            GetNode<MeshInstance3D>("EditorMarker").QueueFree();

            ZoneEffectFortify zoneEffectFortify = null;
            foreach (Node child in GetChildren())
            {
                if (child is BoardedWindow)
                {
                    _windows.Add((BoardedWindow)child);
                }
                else if (child is ZoneEffectFortify)
                {
                    zoneEffectFortify = (ZoneEffectFortify)child;
                }
            }

            if (zoneEffectFortify != null)
            {
                zoneEffectFortify.SetZoneWindows(_windows);
            }

            if (_activateOnReady)
            {
                Activate();
            }
            else
            {
                foreach(NodePath path in _accessObstructionNodePaths)
                {
                    Obstruction obstruction = GetNode<Obstruction>(path);
                    obstruction.Connect(nameof(Obstruction.Cleared), new Callable(this, nameof(OnObstructionCleared)));
                }
            }
        }
        
        private void OnObstructionCleared()
        {
            if (!_isActivated)
            {
                Activate();
            }
        }

        private void Activate()
        {
            foreach(BoardedWindow window in _windows)
            {
                window.Activate();
            }
            _isActivated = true;
            EmitSignal(nameof(Activated));
        }
    }
}

