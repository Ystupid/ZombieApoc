using System;
using System.Collections.Generic;
using UnityEngine;

public static class GuardTriggerCfg
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

        actor.ChangeState(EActorState.Guard);

        return true;
    }

    public static bool HasInput(ActorEntity actor)
    {
        return Input.GetKeyDown(KeyCode.K) || Input.GetKey(KeyCode.K);
    }

    public static bool HasState(ActorEntity actor)
    {
        if (!actor.Animator.HasState(0, AnimHash.Guard))
        {
            return false;
        }

        return actor.StateMachine.HasState(EActorState.Guard);
    }
}
