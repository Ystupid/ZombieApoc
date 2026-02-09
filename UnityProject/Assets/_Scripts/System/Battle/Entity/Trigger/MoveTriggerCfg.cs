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

    public static Vector3 GetMovement(ActorEntity actor)
    {
        var moveMent = new Vector3
        {
            x = Input.GetAxis("Horizontal"),
            z = Input.GetAxis("Vertical"),
        };

        return moveMent * 2;
    }

    public static void Move(ActorEntity actor)
    {
        var moveMent = GetMovement(actor);

        actor.transform.Translate(moveMent * Time.deltaTime);

        var signedAngle = Vector2.SignedAngle(Vector2.up, moveMent);

        var localScale = actor.SpriteRenderer.transform.localScale;

        localScale.x = signedAngle >= 0 ? -1 : 1;

        actor.SpriteRenderer.transform.localScale = localScale;
    }
}
