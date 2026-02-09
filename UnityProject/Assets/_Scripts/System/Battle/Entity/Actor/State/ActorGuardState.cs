using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActorGuardState : EntityStateBase<EActorState, ActorEntity>
{
    public ActorGuardState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        Owner.Animator.CrossFade(AnimHash.Guard, 0.1f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!GuardTriggerCfg.HasInput(Owner))
        {
            Owner.ChangeState(EActorState.Idle);
        }
    }
}