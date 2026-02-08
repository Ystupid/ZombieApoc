using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StateMachine<TKey> where TKey : struct, IConvertible, IComparable
{
    /// <summary>  状态机的当前状态 </summary>
    public IState<TKey> CurrentState { get; protected set; }

    /// <summary>  状态机的上次状态 </summary>
    public IState<TKey> LastState { get; protected set; }

    /// <summary>  状态机的当前状态类型 </summary>
    public TKey CurrentStateType { get; protected set; }

    /// <summary>  状态机的上次状态类型 </summary>
    public TKey LastStateType { get; protected set; }

    protected Dictionary<TKey, IState<TKey>> _stateMap;

    public StateMachine()
    {
        _stateMap = new Dictionary<TKey, IState<TKey>>();
    }

    /// <summary>
    /// 添加一个状态到状态机
    /// </summary>
    public void AddState<W>(TKey stateType, W state) where W : IState<TKey>
    {
        if (!_stateMap.TryGetValue(stateType, out _))
        {
            state.StateType = stateType;
            _stateMap.Add(stateType, state);
        }
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    public void RemoveState(TKey stateType)
    {
        if (_stateMap.TryGetValue(stateType, out _))
        {
            _stateMap.Remove(stateType);
        }
    }

    /// <summary>
    /// 查询状态
    /// </summary>
    /// <param name="stateType"></param>
    /// <returns></returns>
    public bool HasState(TKey stateType)
    {
        return _stateMap.ContainsKey(stateType);
    }

    /// <summary>
    /// 对状态机的初始化操作
    /// </summary>
    public void Init(TKey stateType, ITuple param = null)
    {
        foreach (var pair in _stateMap)
        {
            pair.Value.Machine = this;
            pair.Value.OnInit();
        }

        CurrentState = GetState(stateType);
        LastState = CurrentState;
        CurrentState?.OnEnter(param);
    }

    /// <summary>
    /// 转换状态
    /// </summary>
    public virtual void ChangeState(TKey stateType, ITuple param = null)
    {
        if (!_stateMap.TryGetValue(stateType, out IState<TKey> state))
        {
            Debug.LogError($"状态机查找不到状态:{state}");
            return;
        }

        LastState = CurrentState;
        LastStateType = CurrentState.StateType;
        LastState.OnLeave();

        CurrentState = state;
        CurrentStateType = state.StateType;
        CurrentState.OnEnter(param);
    }

    public virtual void Update()
    {
        CurrentState?.OnUpdate();
    }

    /// <summary>
    /// 获取状态机某个状态
    /// </summary>
    public IState<TKey> GetState(TKey stateType)
    {
        if (!_stateMap.TryGetValue(stateType, out IState<TKey> state))
        {
            Debug.LogError($"状态:{stateType}不存在");
            return default;
        }

        return state;
    }

    public bool CheckIsState(bool current, TKey stateType)
    {
        return current ? Equals(CurrentState.StateType, stateType) : Equals(LastState.StateType, stateType);
    }

    public virtual void Release()
    {
        _stateMap?.Clear();
        _stateMap = null;
        CurrentState = null;
        LastState = null;
    }
}