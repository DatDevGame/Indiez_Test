using System;
using System.Collections.Generic;
using System.Data;
using FIMSpace.FProceduralAnimation;
using HCore.Events;
using HCore.Helpers;
using Premium;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSoldier : MonoBehaviour
{
    public Action OnDead = delegate { };
    public CharacterController CharacterController => m_CharacterController;
    public Animator Animator => m_Animator;
    public SodierStats SoldierStats => m_SoldierStats;
    public SoldierStatsSO StatsSOData => m_SoldierStatsSO;
    public SoldierAnimationSO AnimationKeySO => m_SoldierAnimationSO;
    public GrenadeSoldier GrenadeSoldierPrefab => m_GrenadeSoldierPrefab;
    public bool IsAlive => m_IsAlive;
    public bool IsActive => m_IsActive;
    public bool IsLocal => m_IsLocal;

    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_BlockHit;
    [SerializeField, BoxGroup("References")] protected WeaponHolder m_WeaponHolder;
    [SerializeField, BoxGroup("References")] protected CharacterController m_CharacterController;
    [SerializeField, BoxGroup("References")] protected HealthBar m_HealthBar;
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected LegsAnimator m_LegsAnimator;
    [SerializeField, BoxGroup("References")] protected MeshRenderer m_HealthBarMesh;
    [SerializeField, BoxGroup("Resource")] protected GrenadeSoldier m_GrenadeSoldierPrefab;
    [SerializeField, BoxGroup("Data")] protected SoldierAnimationSO m_SoldierAnimationSO;
    [SerializeField, BoxGroup("Data")] protected WeaponManagerSO m_WeaponManagerSO;
    [SerializeField, BoxGroup("Data")] protected SoldierStatsSO m_SoldierStatsSO;
    [ShowInInspector, ReadOnly] protected SodierStats m_SoldierStats;
    protected bool m_IsAlive = true;
    protected bool m_IsActive = false;
    protected bool m_IsLocal = false;

    protected virtual void InitWeapons()
    {
        m_WeaponHolder.EquipWeapon(
            m_WeaponManagerSO.currentItemInUse
                .Cast<WeaponSO>()
                .GetModule<ModelPrefabItemModule>()
                .modelPrefabAsGameObject
                .GetComponent<BaseWeapon>()
        );
    }
}
