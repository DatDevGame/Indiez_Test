using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private List<Rigidbody> m_RagdollBodies = new List<Rigidbody>();
    [SerializeField] private List<Collider> m_RagdollColliders = new List<Collider>();

    private bool m_IsRagdollActive = false;

    private void Awake()
    {
        SetRagdoll(false);
    }

#if UNITY_EDITOR
    [Button("Load Rig & Collider")]
    private void Load()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            Collider collider = rb.GetComponent<Collider>();
            collider.enabled = true;
            rb.isKinematic = true;

            m_RagdollBodies.Add(rb);
            m_RagdollColliders.Add(rb.GetComponent<Collider>());
        }
    }
#endif

    [Button]
    private void SetRagdoll(bool active)
    {
        m_IsRagdollActive = active;

        for (int i = 0; i < m_RagdollBodies.Count; i++)
        {
            var rb = m_RagdollBodies[i];
            var col = m_RagdollColliders[i];

            if (rb == null || col == null) continue;

            rb.isKinematic = !active;
            //col.enabled = active;
        }
    }

    public void EnableRagdoll()
    {
        SetRagdoll(true);
    }

    public void DisableRagdoll()
    {
        SetRagdoll(false);
    }
}
