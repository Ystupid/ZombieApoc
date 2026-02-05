using System;
using System.Collections.Generic;

public enum EViewLoadState
{
    /// <summary>
    /// 默认状态
    /// </summary>
    None,

    /// <summary>
    /// 加载中
    /// </summary>
    Loading,

    /// <summary>
    /// 加载完成
    /// </summary>
    Finish,

    /// <summary>
    /// 加载失败
    /// </summary>
    Failure,
}
