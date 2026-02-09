using System;
using System.Collections.Generic;

public enum EActorState
{
    /// <summary>
    /// 静止
    /// </summary>
    Idle,

    /// <summary>
    /// 移动
    /// </summary>
    Move,

    /// <summary>
    /// 攻击
    /// </summary>
    Attack,

    /// <summary>
    /// 守护
    /// </summary>
    Guard,

    /// <summary>
    /// 受击
    /// </summary>
    Hurt,

    /// <summary>
    /// 恢复
    /// </summary>
    Heal,

    /// <summary>
    /// 死亡
    /// </summary>
    Death,
}
