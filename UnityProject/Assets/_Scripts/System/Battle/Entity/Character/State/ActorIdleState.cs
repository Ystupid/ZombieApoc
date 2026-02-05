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

        var moveMent = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical"),
        };

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Owner.ChangeState(EActorState.Attack);
            return;
        }

        if (moveMent.sqrMagnitude > 0)
        {
            Owner.ChangeState(EActorState.Move);
            return;
        }
    }
}
