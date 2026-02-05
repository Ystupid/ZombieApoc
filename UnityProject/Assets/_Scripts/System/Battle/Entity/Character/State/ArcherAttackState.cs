using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class ArcherAttackState : EntityStateBase<EActorState, ActorEntity>
{
    public ArcherAttackState(ActorEntity owner) : base(owner) { }


    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        Owner.Animator.CrossFade(AnimHash.Attack, 0.1f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        var nextInfo = Owner.Animator.GetNextAnimatorStateInfo(0);
        var stateInfo = Owner.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == AnimHash.Attack)
        {
            if (stateInfo.normalizedTime >= 0.95f)
            {
                Owner.ChangeState(EActorState.Idle);
                return;
            }
        }
        else if (nextInfo.shortNameHash == AnimHash.Attack)
        {
            if (nextInfo.normalizedTime >= 0.95f)
            {
                Owner.ChangeState(EActorState.Idle);
                return;
            }
        }
    }
}
