using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActorHurtState : EntityStateBase<EActorState, ActorEntity>
{
    private float _durationTime;
    private Color _originalColor;

    public ActorHurtState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        _durationTime = 0.2f;

        _originalColor = Owner.SpriteRenderer.color;
        Owner.SpriteRenderer.color = Color.red;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _durationTime -= Time.deltaTime;

        if (_durationTime <= 0)
        {
            Owner.ChangeState(EActorState.Idle);
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();

        Owner.SpriteRenderer.color = _originalColor;
    }
}
