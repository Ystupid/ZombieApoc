using System;
using System.Collections.Generic;
using UnityEngine;

public static class AttackTriggerCfg
{
    public static bool TryEnterState(ActorEntity actor)
    {
        if (!HasInput(actor))
        {
            return false;
        }

        if (!HasState(actor))
        {
            return false;
        }

        actor.ChangeState(EActorState.Attack);

        return true;
    }

    public static bool HasInput(ActorEntity actor)
    {
        return Input.GetKeyDown(KeyCode.J);
    }

    public static bool HasState(ActorEntity actor)
    {
        if (!actor.Animator.HasState(0, AnimHash.Attack))
        {
            return false;
        }

        return actor.StateMachine.HasState(EActorState.Attack);
    }

    public static bool NeedCutout(ActorEntity actor, int animHash)
    {
        var nextInfo = actor.Animator.GetNextAnimatorStateInfo(0);
        var stateInfo = actor.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == animHash)
        {
            if (stateInfo.normalizedTime >= 0.95f)
            {
                return true;
            }
        }
        else if (nextInfo.shortNameHash == animHash)
        {
            if (nextInfo.normalizedTime >= 0.95f)
            {
                return true;
            }
        }

        return false;
    }
}
