using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActorHealState : EntityStateBase<EActorState, ActorEntity>
{
    public ActorHealState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        Owner.Animator.CrossFade(AnimHash.Heal, 0.1f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (HealTriggerCfg.NeedCutout(Owner))
        {
            Owner.ChangeState(EActorState.Idle);
        }
    }
}
