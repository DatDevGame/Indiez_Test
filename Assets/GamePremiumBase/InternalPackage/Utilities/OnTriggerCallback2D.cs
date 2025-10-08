using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class OnTriggerCallback2D : MonoBehaviour
{
    public event Action<Collider2D> onTriggerEnter = delegate { };
    public event Action<Collider2D> onTriggerStay = delegate { };
    public event Action<Collider2D> onTriggerExit = delegate { };

    public bool isFilterByTag = true;
    [TagSelector, ShowIf("isFilterByTag")]
    public List<string> tagFilter = new List<string>() { "Untagged" };

    [SerializeField]
    private UnityEvent onTriggerEnterEvent;
    [SerializeField]
    private UnityEvent onTriggerStayEvent;
    [SerializeField]
    private UnityEvent onTriggerExitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFilterByTag || tagFilter.Any(item => other.CompareTag(item)))
        {
            onTriggerEnter?.Invoke(other);
            onTriggerEnterEvent?.Invoke();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isFilterByTag || tagFilter.Any(item => other.CompareTag(item)))
        {
            onTriggerStay?.Invoke(other);
            onTriggerStayEvent?.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isFilterByTag || tagFilter.Any(item => other.CompareTag(item)))
        {
            onTriggerExit?.Invoke(other);
            onTriggerExitEvent?.Invoke();
        }
    }
}