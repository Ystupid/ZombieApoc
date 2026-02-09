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
            OnKeyEventTrigger?.Invoke(config.Info);
        }
    }

    public void OnEventTrigger3(UnityEngine.Object obj)
    {
        if (obj is KeyEventConfig config)
        {
            OnKeyEventTrigger?.Invoke(config.Info);
        }
    }

    public void OnEventTrigger4(KeyEventInfo param)
    {
    }

    public void OnEventTrigger5(int intVal, float floatVal, string strVal, GameObject objVal)
    {
    }
}
