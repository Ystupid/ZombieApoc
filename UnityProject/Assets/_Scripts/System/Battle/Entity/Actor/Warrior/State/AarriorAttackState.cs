using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AarriorAttackState : ActorAttackState
{
    private int _nextStep;
    private bool _hasInput;

    public AarriorAttackState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        _hasInput = false;

        Owner.AddKeyEvent(OnKeyEvent);
    }

    public override void OnUpdate()
    {
        if (AttackTriggerCfg.HasInput(Owner))
        {
            _hasInput = true;
        }

        if (AttackTriggerCfg.NeedCutout(Owner))
        {
            Owner.ChangeState(EActorState.Idle);
            return;
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();

        Owner.RemoveKeyEvent(OnKeyEvent);
    }

    private void OnKeyEvent(KeyEventInfo info)
    {
        if (info.Type == EKeyEvent.SetNextStep)
        {
            ChangeStep(info.IntValue);
        }
    }

    private void ChangeStep(int step)
    {
    }
}
