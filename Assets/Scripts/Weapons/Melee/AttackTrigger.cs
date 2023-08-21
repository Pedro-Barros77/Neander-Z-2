using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public delegate void OnTagTriggeredCallBack(Collider2D collision);
    public OnTagTriggeredCallBack OnTagTriggered;

    [SerializeField]
    public List<string> TriggerTags = new List<string>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TriggerTags.Contains(collision.gameObject.tag))
            OnTagTriggered?.Invoke(collision);
    }

    private void OnDestroy()
    {
        OnTagTriggered = null;
    }
}
