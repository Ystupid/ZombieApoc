using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class StateEntity<TState> : EntityBase where TState : struct, IConvertible, IComparable
{
    private StateMachine<TState> _stateMachine;
    public StateMachine<TState> StateMachine => _stateMachine;

    public TState CurrentStateType => _stateMachine.CurrentStateType;
    public TState LastStateType => _stateMachine.LastStateType;

    public UnityEvent<TState> OnStateChanged;

    protected override void OnEntityInit()
    {
        base.OnEntityInit();

        OnStateChanged = new UnityEvent<TState>();

        InitState();
    }

    protected override void OnEntityUpdate()
    {
        base.OnEntityUpdate();

        _stateMachine.Update();
    }

    protected virtual void InitState()
    {
        _stateMachine = new StateMachine<TState>();
    }

    public virtual void ChangeState(TState stateType, bool force = false)
    {
        if (!Equals(stateType, _stateMachine.CurrentStateType) || force)
        {
            _stateMachine.ChangeState(stateType);

            OnStateChanged?.Invoke(stateType);
        }
    }
}
