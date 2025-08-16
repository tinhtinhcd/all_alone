using Godot;
using System;
using System.Collections.Generic;
using ZombieHoardGame.ZombieCharacter.FSMController;
using HealthSystem;
using GameGeneral;


namespace ZombieHoardGame.ZombieCharacter
{
    public partial class Zombie : CharacterBody3D
    {
        /// Signals ///
        [Signal]
        public delegate void DiedEventHandler(Zombie zombie, Node inflictor, Vector3 position, bool isCriticalKill);
        [Signal]
        public delegate void EnteredPlayerAreaEventHandler(Zombie zombie);
        [Signal]
        public delegate void PlayerPositionUpdateRequestEventHandler(Zombie zombie);

        /// Exported Fields ///
        [Export]
        private Godot.Collections.Array<AudioStream> _audioStreamsGroan = new Godot.Collections.Array<AudioStream>();
        [Export]
        private Godot.Collections.Array<AudioStream> _audioStreamsAttack = new Godot.Collections.Array<AudioStream>();

        /// Properties - public, protected, private ///
        public bool IsInPlayerArea{ get { return _blackboard.IsInPlayerArea; }}
        public float HealthMultiplier{ set; private get; }

        /// Fields - protected or private ///
        private Blackboard _blackboard = new Blackboard();
        private Controller _controller;
        private GameGeneral.LOD.Switcher _lodSwitcher;
        private bool _isDead = false;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            CacheNodeReferences();
            _controller.Blackboard = _blackboard;

            _blackboard.Health.Connect(nameof(Health.Hurt), new Callable(this, nameof(OnHealthHurt)));
            _blackboard.VisibleOnScreenNotifier3D.Connect("screen_entered", new Callable(this, nameof(OnVisibilityNotifierScreenEntered)));
            _blackboard.VisibleOnScreenNotifier3D.Connect("screen_exited", new Callable(this, nameof(OnVisibilityNotifierScreenExited)));

            ApplyHealthMultiplier();
        }

        public override void _Process(double delta)
        {
            _stateMachine.Update((float)delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            _stateMachine.PhysicsUpdate((float)delta);
        }
        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void UpdatePlayerPosition(Vector3 position)
        {
            _blackboard.PlayerPosition = position;
        }

        public void SetTargetWindow(BoardedWindow window)
        {
            _blackboard.TargetBoardedWindow = window;
        }

        public void PlayRandomAudioGroan()
        {
            Random r = new Random();
            AudioStream randAttackAudio = _audioStreamsGroan[r.Next(0, _audioStreamsGroan.Count - 1)];
            _blackboard.AudioStreamPlayer.Stream = randAttackAudio;
            _blackboard.AudioStreamPlayer.Play();
        }

        public void PlayRandomAudioAttack()
        {
            Random r = new Random();
            AudioStream randAttackAudio = _audioStreamsAttack[r.Next(0, _audioStreamsAttack.Count - 1)];
            _blackboard.AudioStreamPlayer.Stream = randAttackAudio;
            _blackboard.AudioStreamPlayer.Play();
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnHealthHurt(Node inflictor, int value, bool isCritical)
        {
            if (_blackboard.Health.Points <= 0 && !_isDead)
            {
                Die(inflictor, isCritical);
            }
        }

        private void OnVisibilityNotifierScreenEntered()
        {
            _lodSwitcher.Activate(true);
        }

        private void OnVisibilityNotifierScreenExited()
        {
            _lodSwitcher.Activate(false);
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////

        private void CacheNodeReferences()
        {
            _controller = GetNode<Controller>("Controller");
            _lodSwitcher = GetNode<GameGeneral.LOD.Switcher>("LODSwitcher");

            _blackboard.Character = this;
            _blackboard.NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
            _blackboard.AttackTrigger = GetNode<AttackTrigger>("AttackTrigger");
            _blackboard.Health = GetNode<Health>("Health");
            _blackboard.AttackHitBox = GetNode<HitBox>("HitBox");
            _blackboard.AudioStreamPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
            _blackboard.AnimTree = GetNode<AnimationTree>("AnimationTree");
            _blackboard.AnimStateMachine = (AnimationNodeStateMachinePlayback)_blackboard.AnimTree.Get("parameters/playback");
            _blackboard.VisibleOnScreenNotifier3D = GetNode<VisibleOnScreenNotifier3D>("VisibleOnScreenNotifier3D");
            _blackboard.LODSwitcher = _lodSwitcher;
        }

        private void ApplyHealthMultiplier()
        {
            _blackboard.Health.PointsMax = Mathf.RoundToInt(_blackboard.Health.PointsMax * HealthMultiplier);
            _blackboard.Health.RestoreToMax();
        }

        private void RemoveTargetWindowBoard() // Only to be used in the attack window animation
        {
            BoardedWindow window = _blackboard.TargetBoardedWindow;
            window.ZombieRemoveBoard();
        }

        private void Die(Node killer, bool wasKilledByCriticalHit)
        {
            _isDead = true;
            EmitSignal(nameof(Died), this, killer, GlobalPosition, wasKilledByCriticalHit);
            GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;

            if (wasKilledByCriticalHit)
            {
                // Could spawn with a higher level script similar to the BulletSpawner?
                GetNode<CompositeEffect>("Body/Armature/Skeleton3D/BoneAttachHead/HeadshotDeathEffect").Play();
                GetNode<MeshInstance3D>("Body/Armature/Skeleton3D/NeckStump").Show();
                GetNode<MeshInstance3D>("Body/Armature/Skeleton3D/Head").Hide();
            }

            _controller.Die();
        }
    }
}

