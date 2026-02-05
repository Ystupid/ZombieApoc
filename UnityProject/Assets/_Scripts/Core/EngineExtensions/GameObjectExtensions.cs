using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetComponentSafe<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }
}
