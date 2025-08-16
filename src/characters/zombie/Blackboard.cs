using Godot;
using HealthSystem;


namespace ZombieHoardGame.ZombieCharacter
{
    public partial class Blackboard : RefCounted
    {
        public Zombie Character;
        public NavigationAgent3D NavAgent;
        public AttackTrigger AttackTrigger;
        public Vector3 PlayerPosition; // Not updated every frame
        public Health Health;
        public HitBox AttackHitBox;
        public AudioStreamPlayer3D AudioStreamPlayer;
        public BoardedWindow TargetBoardedWindow;
        public bool IsInPlayerArea;
        public AnimationTree AnimTree;
        public AnimationNodeStateMachinePlayback AnimStateMachine;
        public VisibleOnScreenNotifier3D VisibleOnScreenNotifier3D;
        public GameGeneral.LOD.Switcher LODSwitcher;
    }
}

