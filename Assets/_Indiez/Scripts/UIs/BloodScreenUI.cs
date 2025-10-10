using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using HCore.Events;

[RequireComponent(typeof(Image))]
public class BloodScreenUI : MonoBehaviour
{
    [Header("Blood Effect Settings")]
    [SerializeField] private float m_FlashDuration = 0.15f;
    [SerializeField] private float m_FadeDuration = 0.5f;
    [SerializeField] private Color m_InsideColor = new Color(0.6f, 0, 0, 0.4f);
    [SerializeField] private Color m_OutsideColor = new Color(0.6f, 0, 0, 0f);
    [SerializeField] private float m_GradientBias = 0.25f;

    private Image m_Image;
    private Material m_MatInstance;
    private Tween m_CurrentTween;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(PlayerEventCode.TakeDamage, OnTakeDamge);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PlayerEventCode.TakeDamage, OnTakeDamge);
    }

    private void OnTakeDamge(params object[] parameters)
    {
        ShowBlood();
    }

    private void Start()
    {
        m_Image = GetComponent<Image>();

        m_MatInstance = Instantiate(m_Image.material);
        m_Image.material = m_MatInstance;

        m_MatInstance.SetColor("_InsideColor", new Color(0, 0, 0, 0));
        m_MatInstance.SetColor("_OutsideColor", new Color(0, 0, 0, 0));
    }

    private void ShowBlood()
    {
        CameraShake.Instance.Shake(0.1f, 0.25f);
        if (m_CurrentTween != null && m_CurrentTween.IsActive())
            m_CurrentTween.Kill();

        m_MatInstance.SetColor("_InsideColor", m_InsideColor);
        m_MatInstance.SetColor("_OutsideColor", m_OutsideColor);
        m_MatInstance.SetFloat("_GradientBias", m_GradientBias);

        m_CurrentTween = DOVirtual.Float(1f, 0f, m_FadeDuration, (t) =>
        {
            Color inside = Color.Lerp(m_InsideColor, new Color(0, 0, 0, 0), 1 - t);
            Color outside = Color.Lerp(m_OutsideColor, new Color(0, 0, 0, 0), 1 - t);
            m_MatInstance.SetColor("_InsideColor", inside);
            m_MatInstance.SetColor("_OutsideColor", outside);
        }).SetEase(Ease.OutQuad);
    }
}
