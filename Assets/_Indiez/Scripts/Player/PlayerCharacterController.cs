using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour, INavigationPoint
{
    [SerializeField, BoxGroup("Referrence")] private Soldier_1 m_Soldier;

    private void Awake()
    {
        if (m_Soldier == null)
            m_Soldier = GetComponentInChildren<Soldier_1>();
    }

    public PointType GetPointType()
    {
        return m_Soldier.GetPointType();
    }

    public Vector3 GetSelfPoint()
    {
        return m_Soldier.GetSelfPoint();
    }

    public Vector3 GetTargetPoint()
    {
        return m_Soldier.GetTargetPoint();
    }

    public bool IsAvailable()
    {
        return m_Soldier.IsAvailable();
    }
}
