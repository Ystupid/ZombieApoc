using System;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    /// <summary>
    /// 根据PathAttribute为组件变量赋值
    /// </summary>
    /// <param name="component"></param>
    public static void BindComponent(this Component component)
    {
        PathAttributeExtension.BindComponent(component);
    }
}
