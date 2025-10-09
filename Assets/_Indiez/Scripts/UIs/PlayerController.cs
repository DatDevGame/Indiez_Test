using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HCore.Events;
using Premium;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick m_Joystick;
    [SerializeField] private CanvasGroupVisibility m_CanvasGroupVisibility;

    [SerializeField] private BaseSoldier m_Player;
    private float m_Horizontal;
    private float m_Vertical;
    private Vector3 m_JoyStickdir;

    bool isActive = true;

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
    private void OnLevelStart()
    {
        if (m_Player == null)
            m_Player = FindObjectOfType<BaseSoldier>();
        m_Player.OnDead += OnDead;
        SetActive(true);
    }

    private void OnLevelEnd()
    {
        SetActive(false);
    }

    private void Update()
    {
        if (!isActive || m_Player == null) return;

        m_Horizontal = m_Joystick.Horizontal;
        m_Vertical = m_Joystick.Vertical;

        Vector3 inputDir = new Vector3(m_Horizontal, 0, m_Vertical);
        float inputStrength = Mathf.Clamp01(inputDir.magnitude);

        if (inputStrength > 0.1f)
        {
            Vector3 moveDir = Camera.main.transform.TransformDirection(inputDir);
            moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized;

            float speed = m_Player.SoldierStats.MoveSpeed * inputStrength;
            m_Player.CharacterController.Move(moveDir * m_Player.SoldierStats.MoveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            m_Player.transform.rotation = Quaternion.Slerp(
                m_Player.transform.rotation,
                targetRotation,
                Time.deltaTime * 3f
            );
        }
    }
    private void OnDead()
    {
        SetActive(false);
    }
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        if (isActive)
        {
            m_CanvasGroupVisibility.Show();
        }
        else
        {
            m_CanvasGroupVisibility.Hide();
        }
    }
}
