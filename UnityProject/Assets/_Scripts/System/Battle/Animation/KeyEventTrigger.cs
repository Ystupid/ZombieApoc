using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Ö¡ÊÂ¼þ´¥·¢Æ÷
/// </summary>
public class KeyEventTrigger : MonoBehaviour
{
    public UnityEvent<KeyEventInfo> OnKeyEventTrigger;

    public void OnEventTrigger(EKeyEvent type)
    {
        var info = new KeyEventInfo
        {
            Type = type,
        };

        OnKeyEventTrigger?.Invoke(info);
    }

    public void OnEventTrigger2(AnimationEvent param)
    {
        if (param.objectReferenceParameter is KeyEventConfig config)
        {
            var info = new KeyEventInfo
            {
                Type = config.Type,
                IntValue = config.IntValue,
                FloatValue = config.FloatValue,
            };

            OnKeyEventTrigger?.Invoke(info);
        }
    }

    public void OnEventTrigger3(KeyEventInfo param)
    {
    }

    public void OnEventTrigger4(int intVal, float floatVal, string strVal, GameObject objVal)
    {
    }
}
