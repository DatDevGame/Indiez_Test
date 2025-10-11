using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ThrowGrenadeButton : MonoBehaviour
{
    [SerializeField, BoxGroup("Referrence")] private Button m_Button;
    [SerializeField, BoxGroup("Referrence")] private Image m_CooldownImage;
    [SerializeField, BoxGroup("Config")] private float m_CooldownTime = 5f;

    private bool m_IsCooldown = false;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClickButton);

        if (m_CooldownImage != null)
        {
            m_CooldownImage.type = Image.Type.Filled;
            m_CooldownImage.fillMethod = Image.FillMethod.Radial360;
            m_CooldownImage.fillOrigin = (int)Image.Origin360.Top;
            m_CooldownImage.fillClockwise = false;
            m_CooldownImage.fillAmount = 0f;
        }
    }

    private void OnDestroy()
    {
        m_Button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        if (m_IsCooldown)
            return;

        GameEventHandler.Invoke(PlayerEventCode.ThrowGrenadeTrigger);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        m_IsCooldown = true;
        float timer = 0f;

        while (timer < m_CooldownTime)
        {
            timer += Time.deltaTime;
            if (m_CooldownImage != null)
            {
                m_CooldownImage.fillAmount = 1f - (timer / m_CooldownTime);
            }
            yield return null;
        }

        if (m_CooldownImage != null)
            m_CooldownImage.fillAmount = 0f;

        m_IsCooldown = false;
    }
}
