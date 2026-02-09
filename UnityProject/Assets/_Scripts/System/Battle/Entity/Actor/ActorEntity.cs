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
    protected SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    [Path("Actor")]
    protected KeyEventTrigger _keyEventTrigger;
    public KeyEventTrigger KeyEventTrigger => _keyEventTrigger;

    protected PlayerInput _input;
    public PlayerInput Input => _input;

    protected GamePlayer _player;

    protected override void InitState()
    {
        base.InitState();

        _input = new PlayerInput();
    }

    protected override void OnEntityUpdate()
    {
        if (_player == null)
        {
            _player = GetComponent<GamePlayer>();

            if (_player != null)
            {
                _input.ReadFromPlayer(_player);
            }
        }

        base.OnEntityUpdate();

        _input.Clear();
    }

    public void AddKeyEvent(UnityAction<KeyEventInfo> callback)
    {
        _keyEventTrigger.OnKeyEventTrigger.AddListener(callback);
    }

    public void RemoveKeyEvent(UnityAction<KeyEventInfo> callback)
    {
        _keyEventTrigger.OnKeyEventTrigger.RemoveListener(callback);
    }
}
