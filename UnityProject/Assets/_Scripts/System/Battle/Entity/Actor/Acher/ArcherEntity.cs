using System;
using System.Collections.Generic;

public class ArcherEntity : ActorEntity
{
    protected override void InitState()
    {
        base.InitState();

        StateMachine.AddState(EActorState.Idle, new ActorIdleState(this));
        StateMachine.AddState(EActorState.Move, new ActorMoveState(this));
        StateMachine.AddState(EActorState.Attack, new ArcherAttackState(this));
        StateMachine.Init(EActorState.Idle);
    }
}
