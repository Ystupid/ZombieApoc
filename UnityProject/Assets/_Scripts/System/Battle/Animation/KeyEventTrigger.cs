using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Ö¡ÊÂ¼þ´¥·¢Æ÷
/// </summary>
public class KeyEventTrigger : MonoBehaviour
{
    public UnityEvent<EKeyEvent> OnKeyEventTrigger;

    public void Trigger(EKeyEvent type)
    {
        OnKeyEventTrigger?.Invoke(type);
    }
}
