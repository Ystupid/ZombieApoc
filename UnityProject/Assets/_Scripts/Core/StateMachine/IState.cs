using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public interface IState<TKey> where TKey : struct, IConvertible, IComparable
{
    TKey StateType { get; set; }

    StateMachine<TKey> Machine { get; set; }

    /// <summary>
    /// 初始化
    /// </summary>
    void OnInit();

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="param"></param>
    void OnEnter(ITuple param);

    /// <summary>
    /// 更新状态
    /// </summary>
    void OnUpdate();

    /// <summary>
    /// 离开状态
    /// </summary>
    void OnLeave();
}
