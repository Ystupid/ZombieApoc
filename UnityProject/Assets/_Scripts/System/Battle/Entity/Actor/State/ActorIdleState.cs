using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActorIdleState : EntityStateBase<EActorState, ActorEntity>
{
    public ActorIdleState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        Owner.Animator.CrossFade(AnimHash.Idle, 0.1f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (AttackTriggerCfg.TryEnterState(Owner))
        {
            return;
        }

        if (GuardTriggerCfg.TryEnterState(Owner))
        {
            return;
        }

        if (HealTriggerCfg.TryEnterState(Owner))
        {
            return;
        }

        if (MoveTriggerCfg.TryEnterState(Owner))
        {
            return;
        }
    }
}
