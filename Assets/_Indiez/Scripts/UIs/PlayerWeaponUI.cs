using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUI : MonoBehaviour
{
    [SerializeField, BoxGroup("Reference")] private WeaponSlotUI m_WeaponSlotUIPrefab;
    [SerializeField, BoxGroup("Reference")] private Transform m_WeaponSlotHolder;
    [SerializeField, BoxGroup("Reference")] private Button m_Button;
    [SerializeField, BoxGroup("Reference")] private EZAnimVector2 m_OpenAndCloseEZAim;
    [SerializeField, BoxGroup("Data")] private WeaponManagerSO m_WeaponManagerSO;

    private List<WeaponSlotUI> m_WeaponSlotUIs;
    private bool m_IsShowing = false;
    private void Awake()
    {
        GameEventHandler.AddActionEvent(PlayerEventCode.EquipWeapon, OnEquipWeapon);
        m_Button.onClick.AddListener(OnClickButton);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PlayerEventCode.EquipWeapon, OnEquipWeapon);
        m_Button.onClick.RemoveListener(OnClickButton);
    }

    private void Start()
    {
        Init();
    }

    private void OnClickButton()
    {
        if (!m_IsShowing)
        {
            m_OpenAndCloseEZAim.Play();
            m_IsShowing = true;
        }
        else
        {
            m_OpenAndCloseEZAim.InversePlay();
            m_IsShowing = false;
        }

    }

    private void OnEquipWeapon(params object[] parameters)
    {
        if (parameters[0] == null || parameters.Length <= 0)
            return;

        WeaponSO weaponSOEquip = parameters[0] as WeaponSO;
        for (int i = 0; i < m_WeaponSlotUIs.Count; i++)
        {
            if (m_WeaponSlotUIs[i].WeaponSO != weaponSOEquip)
                m_WeaponSlotUIs[i].Deselect();
        }
    }

    private void Init()
    {
        m_WeaponSlotUIs = new List<WeaponSlotUI>();
        for (int i = 0; i < m_WeaponManagerSO.initialValue.Count; i++)
        {
            WeaponSlotUI weaponSlotUI = Instantiate(m_WeaponSlotUIPrefab, m_WeaponSlotHolder);
            m_WeaponSlotUIs.Add(weaponSlotUI);
            if (m_WeaponManagerSO.initialValue[i] is WeaponSO weaponSO)
            {
                weaponSlotUI.InitData(weaponSO);
                if (m_WeaponManagerSO.currentItemInUse == weaponSO)
                    weaponSlotUI.Select();
            }
        }
    }
}
