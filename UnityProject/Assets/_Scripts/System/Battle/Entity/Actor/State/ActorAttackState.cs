using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ActorAttackState : EntityStateBase<EActorState, ActorEntity>
{
    public ActorAttackState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        Owner.Animator.CrossFade(AnimHash.Attack, 0.1f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (AttackTriggerCfg.NeedCutout(Owner))
        {
            Owner.ChangeState(EActorState.Idle);
            return;
        }
    }
}
