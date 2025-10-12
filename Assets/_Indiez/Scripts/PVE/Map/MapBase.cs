using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Premium.PoolManagement;

public class MapBase : MonoBehaviour
{
    [Header("Player Points")]
    [SerializeField] private Transform m_PlayerPoint;
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> m_SpawnPoints = new List<Transform>();

    [Header("Spawn Settings")]
    [SerializeField] private bool m_ShowGizmos = true;

    public Vector3 GetPlayerPoint() => m_PlayerPoint.transform.position;

    public Transform GetRandomSpawnPoint()
    {
        if (m_SpawnPoints.Count == 0)
            return null;
        return m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!m_ShowGizmos) return;
        Gizmos.color = Color.green;

        foreach (var point in m_SpawnPoints)
        {
            if (point == null) continue;
            Gizmos.DrawSphere(point.position, 3f);
            Gizmos.DrawWireSphere(point.position, 5f);
        }
    }
#endif
}
