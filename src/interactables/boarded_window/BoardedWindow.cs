using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZombieHoardGame.ZombieCharacter;


namespace ZombieHoardGame
{
    public partial class BoardedWindow : Area3D, IInteractable
    {
        /// Signals ///
        [Signal]
        public delegate void ActivatedEventHandler(BoardedWindow window);
        [Signal]
        public delegate void QueueFilledEventHandler(BoardedWindow window);
        [Signal]
        public delegate void QueueAvailableEventHandler(BoardedWindow window);
        [Signal]
        public delegate void NextZombieCalledEventHandler(Zombie zombie);
        [Signal]
        public delegate void BoardAddedEventHandler(BoardedWindow window);
        [Signal]
        public delegate void BoardRemovedEventHandler(BoardedWindow window);
        [Signal]
        public delegate void FullyBoardedEventHandler(BoardedWindow window);
        [Signal]
        public delegate void ClearedEventHandler(BoardedWindow window);


        /// Properties - public, protected, private ///
        public bool IsClear {
            get { return _boardCountUp == 0; }
        }

        public bool IsActive{
            private set; get;
        }

        public bool IsFullyBoarded {
            get { return _boardCountUp == _boardCountTotal; }
        }

        public Transform3D ZombieAttackPointTransform { 
            get { return _zombieAttackPoint.GlobalTransform; }
        }

        public bool IsAttackPointReserved{
            get;
            private set;
        }

        public bool IsUnderAttack{
            get { return _isAttackPointOccupied; }
        }

        /// Fields - protected or private ///
        private int _boardCountTotal;
        private int _boardCountUp;
        private AnimationPlayer _animPlayer;
        private List<Transform3D> _spawnPoints = new List<Transform3D>();
        private AudioStreamPlayer3D _audioAddBoard;
        private AudioStreamPlayer3D _audioRemoveBoard;
        private Area3D _zombieQueueArea;
        private List<Marker3D> _allQueuePoints = new List<Marker3D>();
        private Marker3D _zombieAttackPoint;
        private List<Marker3D> _availableQueuePoints = new List<Marker3D>();
        private Dictionary<Zombie, Marker3D> _queuePointReserver = new Dictionary<Zombie, Marker3D>();
        private List<Zombie> _zombieQueue = new List<Zombie>();
        private Zombie _attackingZombie;
        private bool _isAttackPointOccupied;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            GetNode<MeshInstance3D>("BuildGuide").QueueFree();

            IsActive = false;
            CacheNodeReferences();
            _zombieQueueArea.Connect("body_entered", new Callable(this, nameof(OnQueueAreaBodyEntered)));

            _isAttackPointOccupied = false;
            IsAttackPointReserved = false;
            _availableQueuePoints = _allQueuePoints;
            
            foreach(Node child in GetChildren())
            {
                if (child.IsInGroup("spawn_point"))
                {
                    Node3D newPoint = (Node3D)child;
                    _spawnPoints.Add(newPoint.GlobalTransform);
                    newPoint.QueueFree();
                }
            }
        }
    

        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public String GetInteractText()
        {
            if (_isAttackPointOccupied)
            {
                return "Zombie at window, can't build";
            }
            else if(IsFullyBoarded)
            {
                return "Fully boarded";
            }
            else
            {
                return "Add board";
            }
        }

        public void Activate()
        {
            IsActive = true;
            EmitSignal(nameof(Activated), this);
        }

        public async void PlayerAddBoard(PlayerCharacter.Player player)
        {
            if (!_animPlayer.IsPlaying() && !_isAttackPointOccupied)
            {
                int nextBoardIdx = _boardCountTotal - _boardCountUp;
                _animPlayer.Play($"add_board_{nextBoardIdx}");
                _audioAddBoard.Play();
                _boardCountUp ++;
                LevelServices.Instance.PointsAwarder.PlayerBuiltBoard(player);
                await ToSignal(_animPlayer, "animation_finished");
                EmitSignal(nameof(BoardAdded), this);
                if (_boardCountUp == _boardCountTotal)
                {
                    EmitSignal(nameof(FullyBoarded), this);
                }
            }
        }

        public void ZombieRemoveBoard()
        {
            //GD.Print("BoardRemoved");
            int nextBoardIdx = _boardCountTotal - _boardCountUp + 1;
            _animPlayer.Play($"remove_board_{nextBoardIdx}");
            _audioRemoveBoard.Play();
            _boardCountUp --;
            EmitSignal(nameof(BoardRemoved), this);
            if (_boardCountUp == 0)
            {
                EmitSignal(nameof(Cleared), this);
            }
        }

        public void ZombieReserveQueuePoint(Zombie queueingZombie, Marker3D queuePoint)
        {
            ReserveQueuePoint(queueingZombie, queuePoint);
            queueingZombie.Connect(nameof(Zombie.Died), new Callable(this, nameof(OnQueueingZombieDied)));
        }

        public void ZombieReserveAttackPoint(Zombie reserver)
        {
            UnreserveQueuePoint(reserver);
            _zombieQueue.Remove(reserver);
            _attackingZombie = reserver;

            IsAttackPointReserved = true;
            _attackingZombie.Disconnect(nameof(Zombie.Died), new Callable(this, nameof(OnQueueingZombieDied)));
            _attackingZombie.Connect(nameof(Zombie.Died), new Callable(this, nameof(OnAttackingZombieDied)));
            _attackingZombie.Connect(nameof(Zombie.EnteredPlayerArea), new Callable(this, nameof(OnAttackingZombieEnteredPlayerArea)));
        }

        public Transform3D RandomSpawnTransform()
        {
            Random rng = new Random();
            return _spawnPoints[rng.Next(0, _spawnPoints.Count)];
        }

        public Marker3D RandomAvailableQueuePoint()
        {
            Random rng = new Random();
            Marker3D randQueuePoint =  _allQueuePoints[rng.Next(0, _allQueuePoints.Count)];
            return randQueuePoint;
        }

        public void ReachedAttackPosition(Zombie zombie)
        {
            _isAttackPointOccupied = true;
        }

        public void RestoreBoards()
        {
            _audioAddBoard.Play();
            _animPlayer.Play("RESET");
            _boardCountUp = _boardCountTotal;
            EmitSignal(nameof(FullyBoarded), this);
        }

        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnQueueAreaBodyEntered(PhysicsBody3D body)
        {
            Debug.Assert(body is Zombie, "ZombieQueueArea should only detect zombies");
            if(!IsAttackPointReserved)
            {
                Zombie zombie = (Zombie)body;
                _zombieQueue.Remove(zombie);
                EmitSignal(nameof(NextZombieCalled), zombie);
            }
        }

        // CONSIDER: Clean up? Unused params
        private void OnQueueingZombieDied(Zombie zombie, Node inflictor, Vector3 position, bool isCriticalKill)
        {
            UnreserveQueuePoint(zombie);
        }

        private void OnAttackingZombieDied(Zombie zombie, Node inflictor, Vector3 position, bool isCriticalKill)
        {
            _isAttackPointOccupied = false;
            IsAttackPointReserved = false;
            CallNextZombieInQueue();
        }

        private void OnAttackingZombieEnteredPlayerArea(Zombie zombie)
        {
            _isAttackPointOccupied = false;
            IsAttackPointReserved = false;
            CallNextZombieInQueue();
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void CacheNodeReferences()
        {
            _boardCountTotal = GetNode<Node3D>("Boards").GetChildCount();
            _boardCountUp = _boardCountTotal;
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _zombieAttackPoint = GetNode<Marker3D>("ZombieAttackPoint");
            _audioAddBoard = GetNode<AudioStreamPlayer3D>("AudioStreamAddBoard");
            _audioRemoveBoard = GetNode<AudioStreamPlayer3D>("AudioStreamRemoveBoard");
            _zombieQueueArea = GetNode<Area3D>("ZombieQueueArea");
            _allQueuePoints.Add(GetNode<Marker3D>("ZombieQueuePoint1"));
            _allQueuePoints.Add(GetNode<Marker3D>("ZombieQueuePoint2"));
            _allQueuePoints.Add(GetNode<Marker3D>("ZombieQueuePoint3"));
            _allQueuePoints.Add(GetNode<Marker3D>("ZombieQueuePoint4"));
        }

        private void CallNextZombieInQueue()
        {
            if (_zombieQueue.Count > 0)
            {
                Zombie nextZombie = _zombieQueue[0];
                _zombieQueue.RemoveAt(0);
                EmitSignal(nameof(NextZombieCalled), nextZombie);
            }
        }

        private void ReserveQueuePoint(Zombie reserver, Marker3D queuePoint)
        {
            Debug.Assert(_availableQueuePoints.Contains(queuePoint), "Zombie attempted to reserve an unavaliable queue point");

            _availableQueuePoints.Remove(queuePoint);
            _queuePointReserver[reserver] = queuePoint;
            _zombieQueue.Add(reserver);

            if (_availableQueuePoints.Count == 0)
            {
                EmitSignal(nameof(QueueFilled), this);
            }
        }

        private void UnreserveQueuePoint(Zombie reserver)
        {
            Marker3D queuePoint = _queuePointReserver[reserver];
            _availableQueuePoints.Add(queuePoint);
            _queuePointReserver.Remove(reserver);
            _zombieQueue.Remove(reserver);

            if (_availableQueuePoints.Count == 1)
            {
                EmitSignal(nameof(QueueAvailable), this);
            }
        }
    }
}

