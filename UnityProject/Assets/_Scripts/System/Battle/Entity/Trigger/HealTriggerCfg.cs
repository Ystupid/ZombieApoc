using System;
using System.Collections.Generic;
using UnityEngine;

public static class HealTriggerCfg
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

        actor.ChangeState(EActorState.Heal);

        return true;
    }

    public static bool HasInput(ActorEntity actor)
    {
        return Input.GetKeyDown(KeyCode.L);
    }

    public static bool HasState(ActorEntity actor)
    {
        if (!actor.Animator.HasState(0, AnimHash.Heal))
        {
            return false;
        }

        return actor.StateMachine.HasState(EActorState.Heal);
    }

    public static bool NeedCutout(ActorEntity actor)
    {
        var nextInfo = actor.Animator.GetNextAnimatorStateInfo(0);
        var stateInfo = actor.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == AnimHash.Heal)
        {
            if (stateInfo.normalizedTime >= 0.95f)
            {
                return true;
            }
        }
        else if (nextInfo.shortNameHash == AnimHash.Heal)
        {
            if (nextInfo.normalizedTime >= 0.95f)
            {
                return true;
            }
        }

        return false;
    }
}
