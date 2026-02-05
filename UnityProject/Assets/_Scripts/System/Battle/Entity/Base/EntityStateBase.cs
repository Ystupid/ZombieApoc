using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EntityStateBase<TKey, TOwner> : IState<TKey> where TKey : struct, IConvertible, IComparable
{
    public EntityStateBase(TOwner owner)
    {
        Owner = owner;
    }

    public TOwner Owner { get; set; }
    public TKey StateType { get; set; }
    public StateMachine<TKey> Machine { get; set; }

    public virtual void OnEnter(ITuple param) { }

    public virtual void OnInit() { }

    public virtual void OnLeave() { }

    public virtual void OnUpdate() { }
}
