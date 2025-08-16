using Godot;
using System;
using System.Collections.Generic;
using ZombieHoardGame.ZombieCharacter;
using ZombieHoardGame.PlayerCharacter;


namespace ZombieHoardGame
{
    public partial class HoardDirector : Node
    {
        /// Signals ///
        [Signal]
        public delegate void AllZombiesKilledEventHandler();

        /// Exported Fields ///
        [Export]
        private int _initialZombieCount = 5 ;
        [Export]
        private int _newZombiesPerRound = 1;
        [Export]
        private float _zombieHealthGainPerRound = 0.005f;
        [Export]
        private float _zombiesConcurrentMax = 30;
        [Export]
        private float _zombieSpawnTime = 2;
        [Export]
        private PackedScene _zombiePackedScene = null;
        [Export]
        private float _powerUpDropChance = 0.4f;

        /// Properties - public, protected, private ///
        public int TotalZombiesKilled{ private set; get;} = 0;
        public int TotalZombiesKilledHeadshot{ private set; get; } = 0; 

        /// Fields - protected or private ///
        private Player _player;
        private List<Zombie> _zombies = new List<Zombie>();
        private List<BoardedWindow> _activeBoardedWindows = new List<BoardedWindow>();
        private List<BoardedWindow> _availableBoardedWindows = new List<BoardedWindow>();
        private Timer _spawnTimer;
        private int _zombieSpawnsAllowed;
        private float _zombieHealthMultiplier;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            _spawnTimer = GetNode<Timer>("SpawnTimer");
            _spawnTimer.WaitTime = _zombieSpawnTime;
            _spawnTimer.Connect("timeout", new Callable(this, nameof(OnSpawnTimerTimeout)));
        }
        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void SetPlayerReference(Player player)
        {
            _player = player;
        }

        public void AllBoardedWindows(List<BoardedWindow> allWindows)
        {
            foreach(BoardedWindow window in allWindows)
            {
                if (window.IsActive)
                {
                    _activeBoardedWindows.Add(window);
                    _availableBoardedWindows.Add(window);
                    window.Connect(nameof(BoardedWindow.QueueFilled), new Callable(this, nameof(OnBoardedWindowQueueFilled)));

                }
                else
                {
                    window.Connect(nameof(BoardedWindow.Activated), new Callable(this, nameof(OnBoardedWindowActivated)));
                }
            }
        }

        public void StartSpawningNewRoundZombies(int roundNumber)
        {
            _zombieSpawnsAllowed = _initialZombieCount + (_newZombiesPerRound * (roundNumber - 1));
            _zombieHealthMultiplier = 1 + (_zombieHealthGainPerRound * (roundNumber - 1));

            _spawnTimer.Start();
        }

        public void Disable()
        {
            _spawnTimer.Stop();
            foreach(Zombie zombie in _zombies)
            {
                zombie.QueueFree();
            }
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////

        private void OnZombiePlayerPositionUpdateRequested(Zombie zombie)
        {
            zombie.UpdatePlayerPosition(_player.GlobalPosition);
        }

        private void OnZombiedDied(Zombie zombie, Node killer, Vector3 position, bool isCriticalKill)
        {
            
            _zombies.Remove(zombie);

            if (zombie.IsInPlayerArea)
            {
                RollPowerupSpawn(position);
            }

            if (killer is Player)
            {
                LevelServices.Instantiate.PointsAwarder.PlayerKilledZombie(
                    (Player)killer, isCriticalKill, zombie.IsInPlayerArea
                );
            }

            if (_zombies.Count < _zombiesConcurrentMax && _zombieSpawnsAllowed > 0 && _spawnTimer.IsStopped())
            {
                _spawnTimer.Start();
            }
            else if (_zombieSpawnsAllowed == 0 && _zombies.Count == 0)
            {
                EmitSignal(nameof(AllZombiesKilled));
            }

            TotalZombiesKilled++;
            if (isCriticalKill)
            {
                TotalZombiesKilledHeadshot++;
            }
        }

        private void OnSpawnTimerTimeout()
        {
            if (_availableBoardedWindows.Count > 0)
            {
                SpawnZombie();
            }
            else
            {
                //GD.Print($"Zombies waiting to spawn {_zombieSpawnsAllowed}");
            }
            
            if (_zombies.Count < _zombiesConcurrentMax && _zombieSpawnsAllowed > 0)
            {
                _spawnTimer.Start();
            }
        }

        private void OnBoardedWindowActivated(BoardedWindow window)
        {
            _activeBoardedWindows.Add(window);
            _availableBoardedWindows.Add(window);
            window.Connect(nameof(BoardedWindow.QueueFilled), new Callable(this, nameof(OnBoardedWindowQueueFilled)));
        }

        private void OnBoardedWindowQueueFilled(BoardedWindow window)
        {
            _availableBoardedWindows.Remove(window);
            window.Disconnect(nameof(BoardedWindow.QueueFilled), new Callable(this, nameof(OnBoardedWindowQueueFilled)));
            window.Connect(nameof(BoardedWindow.QueueAvailable), new Callable(this, nameof(OnBoardedWindowQueueAvailable)));
        }

        private void OnBoardedWindowQueueAvailable(BoardedWindow window)
        {
            _availableBoardedWindows.Add(window);
            window.Disconnect(nameof(BoardedWindow.QueueAvailable), new Callable(this, nameof(OnBoardedWindowQueueAvailable)));
            window.Connect(nameof(BoardedWindow.QueueFilled), new Callable(this, nameof(OnBoardedWindowQueueFilled)));
        }

        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private BoardedWindow RandomAvailableWindow()
        {
            Random rng = new Random();
            return _availableBoardedWindows[rng.Next(0, _availableBoardedWindows.Count)];
        }

        private void SpawnZombie()
        {
            Zombie newZombie = _zombiePackedScene.Instantiate<Zombie>();
            newZombie.HealthMultiplier = _zombieHealthMultiplier;

            BoardedWindow destinationWindow = RandomAvailableWindow();
            newZombie.SetTargetWindow(destinationWindow);
            
            AddChild(newZombie);
            _zombies.Add(newZombie);
            newZombie.Connect(nameof(Zombie.Died), new Callable(this, nameof(OnZombiedDied)));
            newZombie.Connect(nameof(Zombie.PlayerPositionUpdateRequest), new Callable(this, nameof(OnZombiePlayerPositionUpdateRequested)));
            newZombie.GlobalTransform = destinationWindow.RandomSpawnTransform();

            _zombieSpawnsAllowed--;
        }

        private void RollPowerupSpawn(Vector3 position)
        {
            Random r = new Random();
            double roll = r.NextDouble();
            if (roll <= _powerUpDropChance)
            {
                LevelServices.Instantiate.PowerUpSpawner.SpawnRandom(position);
            }
        }
    }
}

