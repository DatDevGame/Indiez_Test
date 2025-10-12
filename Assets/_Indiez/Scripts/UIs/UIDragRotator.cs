using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIDragRotator : MonoBehaviour, IDragHandler
{
    [Header("Object to Rotate")]
    [SerializeField] private Transform m_TargetObject;
    [SerializeField] private float m_RotationSpeed = 0.2f;
    [SerializeField] private float m_ReturnDuration = 1f; 
    [SerializeField] private float m_IdleTime = 2.5f; 
    private Quaternion m_InitialRotation;
    private Coroutine m_ReturnCoroutine;
    private float m_LastDragTime;

    private void Start()
    {
        if (m_TargetObject != null)
            m_InitialRotation = m_TargetObject.rotation;
    }

    private void Update()
    {
        if (m_TargetObject == null) return;
        if (Time.time - m_LastDragTime >= m_IdleTime && m_ReturnCoroutine == null)
        {
            m_ReturnCoroutine = StartCoroutine(RotateBack());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_TargetObject == null) return;
        m_TargetObject.Rotate(Vector3.up, -eventData.delta.x * m_RotationSpeed);
        m_LastDragTime = Time.time;

        if (m_ReturnCoroutine != null)
        {
            StopCoroutine(m_ReturnCoroutine);
            m_ReturnCoroutine = null;
        }
    }

    private IEnumerator RotateBack()
    {
        Quaternion startRotation = m_TargetObject.rotation;
        float timer = 0f;

        while (timer < m_ReturnDuration)
        {
            timer += Time.deltaTime;
            m_TargetObject.rotation = Quaternion.Slerp(startRotation, m_InitialRotation, timer / m_ReturnDuration);
            yield return null;
        }

        m_TargetObject.rotation = m_InitialRotation;
        m_ReturnCoroutine = null;
    }
}
