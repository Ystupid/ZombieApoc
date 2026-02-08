using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorEntity : StateEntity<EActorState>
{
    [Path("Actor")]
    protected Animator _animator;
    public Animator Animator => _animator;

    [Path("Actor")]
    protected KeyEventTrigger _keyEventTrigger;
    public KeyEventTrigger KeyEventTrigger => _keyEventTrigger;

    public void AddKeyEvent(UnityAction<EKeyEvent> callback)
    {
        _keyEventTrigger.OnKeyEventTrigger.AddListener(callback);
    }

    public void RemoveKeyEvent(UnityAction<EKeyEvent> callback)
    {
        _keyEventTrigger.OnKeyEventTrigger.RemoveListener(callback);
    }
}
