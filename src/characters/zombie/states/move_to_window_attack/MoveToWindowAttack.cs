using Godot;
using System;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public partial class MoveToWindowAttack : MoveState
    {

        public override void PhysicsUpdate(float delta)
        {
            MoveTowardsNavTarget(delta);
        }


        public override void Enter()
        {
            base.Enter();
            NavTarget = _blackboard.TargetBoardedWindow.ZombieAttackPointTransform.Origin;
            _blackboard.IsInPlayerArea = false;
            _blackboard.TargetBoardedWindow.ZombieReserveAttackPoint(_blackboard.Character);
            _blackboard.AnimStateMachine.Travel("walk");
        }

        protected override void OnNavAgentNavigationFinished()
        {
            _blackboard.TargetBoardedWindow.ReachedAttackPosition(_blackboard.Character);
            if (_blackboard.TargetBoardedWindow.IsClear)
            {
                EmitSignal(nameof(ChangeStateRequest), "ClimbThroughWindow");
            }
            else
            {
                EmitSignal(nameof(ChangeStateRequest), "AttackWindow");
            }
        }

    }
}

