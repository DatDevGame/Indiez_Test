using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField, BoxGroup("Refference")] private BaseSoldier m_BaseSoldier;
    [SerializeField, BoxGroup("Refference")] private Transform m_RevolverTranform;
    [SerializeField, BoxGroup("Right Hand IK")] private TwoBoneIKConstraint m_RightHandIK;
    [SerializeField, BoxGroup("Right Hand IK")] private Transform m_RightHandTarget;

    [SerializeField, BoxGroup("Left Hand IK")] private TwoBoneIKConstraint m_LeftHandIK;
    [SerializeField, BoxGroup("Left Hand IK")] private Transform m_LeftHandTarget;
    [SerializeField, BoxGroup("Left Hand IK")] private Transform m_LeftHandHint;
    [ShowInInspector] private Transform IKRighHandPos;
    [ShowInInspector] private Transform IKLeftHandPos;

    [ShowInInspector, ReadOnly] private BaseWeapon currentWeapon;

    private Dictionary<WeaponSO, BaseWeapon> m_WeaponSlot;

    private void Awake()
    {
        if (m_BaseSoldier == null)
            m_BaseSoldier = gameObject.GetComponent<BaseSoldier>();
    }

    private void Update()
    {
        if (currentWeapon != null && IKRighHandPos != null && IKLeftHandPos != null)
        {
            currentWeapon.transform.parent = m_RevolverTranform;
            currentWeapon.transform.position = m_RevolverTranform.position;
            currentWeapon.transform.rotation = m_RevolverTranform.rotation;
            m_LeftHandTarget.position = IKLeftHandPos.position;
            m_LeftHandTarget.rotation = IKLeftHandPos.rotation;
        }
    }

    public void EquipWeapon(BaseWeapon newWeapon)
    {
        if (m_WeaponSlot == null)
            m_WeaponSlot = new Dictionary<WeaponSO, BaseWeapon>();

        if (newWeapon == null)
            return;

        if (currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);

        if (m_WeaponSlot.ContainsKey(newWeapon.WeaponSO))
        {
            currentWeapon = m_WeaponSlot[newWeapon.WeaponSO];
            currentWeapon.gameObject.SetActive(true);
        }
        else
        {
            currentWeapon = Instantiate(newWeapon, m_RevolverTranform);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
            m_WeaponSlot.Add(newWeapon.WeaponSO, currentWeapon);
        }
        currentWeapon.OnEquip();

        IKHandle
        (currentWeapon.WeaponSO.IK_Idle.RevolverLocalPosition,
        currentWeapon.WeaponSO.IK_Idle.RevolverLocalRotation,
        currentWeapon.WeaponSO.IK_Idle.LeftHandIkHintLocalPosion,
        currentWeapon.WeaponSO.IK_Idle.LeftHandIkHintLocalRotation);

        IKRighHandPos = currentWeapon.RightHandIK;
        IKLeftHandPos = currentWeapon.LeftHandIK;

        if (m_BaseSoldier != null)
            m_BaseSoldier.Animator.SetTrigger(currentWeapon.WeaponSO.IdleAnimationKey);

    }

    [Button]
    public void TestAim()
    {
        IKHandle
(currentWeapon.WeaponSO.IK_Aim.RevolverLocalPosition,
currentWeapon.WeaponSO.IK_Aim.RevolverLocalRotation,
currentWeapon.WeaponSO.IK_Aim.LeftHandIkHintLocalPosion,
currentWeapon.WeaponSO.IK_Aim.LeftHandIkHintLocalRotation);

        if (m_BaseSoldier != null)
        {
            m_BaseSoldier.Animator.SetTrigger(currentWeapon.WeaponSO.AimAnimationKey);
        }

    }

    [Button]
    public void TestIdle()
    {
        IKHandle
(currentWeapon.WeaponSO.IK_Idle.RevolverLocalPosition,
currentWeapon.WeaponSO.IK_Idle.RevolverLocalRotation,
currentWeapon.WeaponSO.IK_Idle.LeftHandIkHintLocalPosion,
currentWeapon.WeaponSO.IK_Idle.LeftHandIkHintLocalRotation);

        if (m_BaseSoldier != null)
        {
            m_BaseSoldier.Animator.SetTrigger(currentWeapon.WeaponSO.IdleAnimationKey);
        }

    }

    private void IKHandle(
        Vector3 RevolverLocalPosition,
        Vector3 RevolverLocalRotation,
        Vector3 LeftHandIkHintLocalPosion,
        Vector3 LeftHandIkHintLocalRotation,
        float duration = 0.05f)
    {
        m_RevolverTranform.DOLocalMove(RevolverLocalPosition, duration);
        m_RevolverTranform.DOLocalRotate(RevolverLocalRotation, duration);
        m_LeftHandHint.DOLocalMove(LeftHandIkHintLocalPosion, duration);
        m_LeftHandHint.DOLocalRotate(LeftHandIkHintLocalRotation, duration);
    }

    public void FireCurrent()
    {
        if (currentWeapon)
            currentWeapon.Fire();
    }

    public void ReloadCurrent()
    {
        if (currentWeapon)
            currentWeapon.Reload();
    }
}
