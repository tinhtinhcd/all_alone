using Godot;
using Weapons;
using PowerUps;


namespace ZombieHoardGame
{
    public partial class LevelServices : RefCounted
    {
        public BulletSpawner BulletSpawner;
        public PowerUpSpawner PowerUpSpawner;
        public PointsAwarder PointsAwarder;
        public PlayerCharacter.Player Player;
        public Stopwatch Stopwatch;
        public RoundCounter RoundCounter;

        private LevelServices() {}
        private static LevelServices instance = null;
        public static LevelServices Instance {
        get {
            if (instance == null) {
                instance = new LevelServices();
            }
                return instance;
            }
        }
    }
}

