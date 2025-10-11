using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public WeaponSO WeaponSO => m_WeaponSO;

    [SerializeField, BoxGroup("Referrence")] private Image m_Avatar;
    [SerializeField, BoxGroup("Referrence")] private Image SelectImage;
    [SerializeField, BoxGroup("Referrence")] private Button m_Button;

    private WeaponSO m_WeaponSO;
    private bool m_IsSelected = false;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        m_Button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (m_IsSelected) return;
        GameEventHandler.Invoke(PlayerEventCode.EquipWeapon, m_WeaponSO);
        Select();
    }
    public void Select()
    {
        m_IsSelected = true;
        SelectImage.enabled = m_IsSelected;
    }

    public void Deselect()
    {
        m_IsSelected = false;
        SelectImage.enabled = m_IsSelected;
    }

    public void InitData(WeaponSO weaponSO)
    {
        if (m_Button == null)
            m_Button = gameObject.GetComponent<Button>();

        m_WeaponSO = weaponSO;
        if (m_WeaponSO.TryGetModule<ImageItemModule>(out var module))
        {
            if (m_Avatar != null && module.thumbnailImage != null)
                m_Avatar.sprite = module.thumbnailImage;
        }
    }
}
