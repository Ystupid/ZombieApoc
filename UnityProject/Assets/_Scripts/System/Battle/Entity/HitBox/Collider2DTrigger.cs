using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collider2DTrigger : MonoBehaviour
{
    public UnityEvent<Collider2D> OnEnter;
    public UnityEvent<Collider2D> OnLeave;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnter?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnLeave?.Invoke(collision);
    }
}
