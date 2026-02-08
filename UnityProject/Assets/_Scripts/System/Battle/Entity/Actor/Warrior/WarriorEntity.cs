using System;
using System.Collections.Generic;

public class WarriorEntity : ActorEntity
{
    protected override void InitState()
    {
        base.InitState();

        StateMachine.AddState(EActorState.Idle, new ActorIdleState(this));
        StateMachine.AddState(EActorState.Move, new ActorMoveState(this));
        StateMachine.AddState(EActorState.Attack, new ActorAttackState(this));
        StateMachine.AddState(EActorState.Guard, new ActorGuardState(this));
        StateMachine.Init(EActorState.Idle);
    }
}
