using System;
using System.Collections.Generic;

public class MonkEntity : ActorEntity
{
    protected override void InitState()
    {
        base.InitState();

        StateMachine.AddState(EActorState.Idle, new ActorIdleState(this));
        StateMachine.AddState(EActorState.Move, new ActorMoveState(this));
        StateMachine.AddState(EActorState.Heal, new ActorHealState(this));
        StateMachine.Init(EActorState.Idle);
    }
}
