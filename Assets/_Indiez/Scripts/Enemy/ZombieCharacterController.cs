using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ZombieCharacterController : MonoBehaviour, INavigationPoint
{
    [SerializeField, BoxGroup("References")] private ZombieAIController m_ZombieAIController;
    private void Awake()
    {
        if (m_ZombieAIController == null)
            m_ZombieAIController = gameObject.GetComponent<ZombieAIController>();
    }

    public PointType GetPointType() => m_ZombieAIController.GetPointType();
    public Vector3 GetSelfPoint() => m_ZombieAIController.GetSelfPoint();
    public Vector3 GetTargetPoint() => m_ZombieAIController.GetTargetPoint();
    public bool IsAvailable() => m_ZombieAIController.IsAvailable();
}
