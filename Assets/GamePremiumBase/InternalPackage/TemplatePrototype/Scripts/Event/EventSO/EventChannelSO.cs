using UnityEngine;
using UnityEngine.Events;
using HCore.Events;

[CreateAssetMenu(fileName = "EventChannelSO", menuName = "Hcore/EventChannelSO")]
public class EventChannelSO : ScriptableObject
{
    public event UnityAction onEventTriggered
    {
        add
        {
            if (value == null)
                return;
            m_OnEventTriggered.AddListener(value);
        }
        remove
        {
            if (value == null)
                return;
            m_OnEventTriggered.RemoveListener(value);
        }
    }
    public event UnityAction<object[]> onEventTriggeredWithParams
    {
        add
        {
            if (value == null)
                return;
            m_OnEventTriggeredWithParams.AddListener(value);
        }
        remove
        {
            if (value == null)
                return;
            m_OnEventTriggeredWithParams.RemoveListener(value);
        }
    }

    [TextArea]
    [SerializeField]
    private string m_Description;
    [SerializeField]
    private EventCode m_EventCode;
    [SerializeField]
    private UnityEvent m_OnEventTriggered = new UnityEvent();
    [SerializeField]
    private UnityEvent<object[]> m_OnEventTriggeredWithParams = new UnityEvent<object[]>();

    private void OnEnable()
    {
        if (m_EventCode == null || m_EventCode.eventCode == null)
            return;
        GameEventHandler.AddActionEvent(m_EventCode, Notify);
        GameEventHandler.AddActionEvent(m_EventCode, NotifyWithParams);
    }

    private void OnDisable()
    {
        if (m_EventCode == null || m_EventCode.eventCode == null)
            return;
        GameEventHandler.RemoveActionEvent(m_EventCode, Notify);
        GameEventHandler.RemoveActionEvent(m_EventCode, NotifyWithParams);
    }

    public void Notify()
    {
        m_OnEventTriggered.Invoke();
    }

    public void NotifyWithParams(params object[] parameters)
    {
        m_OnEventTriggeredWithParams.Invoke(parameters);
    }
}