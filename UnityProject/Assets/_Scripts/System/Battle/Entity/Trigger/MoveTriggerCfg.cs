using System;
using System.Collections.Generic;
using UnityEngine;

public static class MoveTriggerCfg
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

        actor.ChangeState(EActorState.Move);

        return true;
    }

    public static bool HasInput(ActorEntity actor)
    {
        return GetMovement(actor).sqrMagnitude > 0;
    }

    public static bool HasState(ActorEntity actor)
    {
        if (!actor.Animator.HasState(0, AnimHash.Move))
        {
            return false;
        }

        return actor.StateMachine.HasState(EActorState.Move);
    }

    public static Vector2 GetMovement(ActorEntity actor)
    {
        var moveMent = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical"),
        };

        return moveMent;
    }

    public static void Move(ActorEntity actor)
    {
        var moveMent = GetMovement(actor);

        actor.transform.Translate(moveMent * Time.deltaTime);
    }
}
