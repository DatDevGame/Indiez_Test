using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HCore.Events;
using Premium;
using TMPro;
using Unity.VisualScripting;
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
    private Soldier_1 m_Soldier;
    bool isActive = true;

    private Camera m_Cam;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(PVEEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVEEventCode.OnLevelEnd, OnLevelEnd);

        m_Cam = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVEEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.RemoveActionEvent(PVEEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnLevelStart()
    {
        if (m_Player == null)
            m_Player = FindObjectOfType<BaseSoldier>();
        m_Player.OnDead += OnDead;
        m_Soldier = m_Player as Soldier_1;
        SetActive(true);
    }

    private void OnLevelEnd()
    {
        SetActive(false);
    }

    private void Update()
    {
        if (m_Soldier == null)
            return;
        if (!m_Soldier.IsAvailable())
            return;
        m_Horizontal = m_Joystick.Horizontal;
        m_Vertical = m_Joystick.Vertical;

        Vector3 inputDir = new Vector3(m_Horizontal, 0, m_Vertical);
        float inputStrength = Mathf.Clamp01(inputDir.magnitude);
        if (inputStrength > 0.01f)
        {
            Vector3 camForward = m_Cam.transform.forward;
            Vector3 camRight = m_Cam.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 moveDir = (camForward * m_Vertical + camRight * m_Horizontal).normalized;

            moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized;
            float speed = m_Player.SoldierStats.MoveSpeed * inputStrength;
            m_Player.CharacterController.Move(moveDir * m_Player.SoldierStats.MoveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);

            m_Soldier.Visual.transform.rotation = Quaternion.Slerp(
                m_Soldier.Visual.transform.rotation,
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
