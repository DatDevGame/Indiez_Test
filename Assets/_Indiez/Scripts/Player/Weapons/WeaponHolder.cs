using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class WeaponHolder : MonoBehaviour
{
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

        float durationIK = 0.05f;
        m_RevolverTranform.DOLocalMove(currentWeapon.WeaponSO.RevolverLocalPosition, durationIK);
        m_RevolverTranform.DOLocalRotate(currentWeapon.WeaponSO.RevolverLocalRotation, durationIK);
        m_LeftHandHint.DOLocalMove(currentWeapon.WeaponSO.LeftHandIkHintLocalPosion, durationIK);
        m_LeftHandHint.DOLocalRotate(currentWeapon.WeaponSO.LeftHandIkHintLocalRotation, durationIK);

        IKRighHandPos = currentWeapon.RightHandIK;
        IKLeftHandPos = currentWeapon.LeftHandIK;
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
