using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HCore.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, BoxGroup("Referenes")] private CinemachineVirtualCamera m_FollowingPlayerCamera;

    private BaseSoldier m_Soldier;
    private void Awake()
    {
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnLevelStart(object[] parrams)
    {
        if (parrams == null || parrams.Length <= 0)
            return;
        m_Soldier = (BaseSoldier)parrams[1];

        if (m_Soldier != null)
            m_FollowingPlayerCamera.m_Follow = m_Soldier.transform;
    }

    private void OnLevelEnd()
    {

    }

    public void SetFollowing(Transform target)
    {
        m_FollowingPlayerCamera.m_Follow = target;
    }
}
