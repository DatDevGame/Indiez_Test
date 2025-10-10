using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using HCore.Events;
using FIMSpace.FProceduralAnimation;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class Soldier_1 : BaseSoldier, INavigationPoint, IDamageable
{
    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_HitDamgePelvisImpulse; 
    [SerializeField, BoxGroup("Referrence")] protected Transform m_FakePointfire;
    [SerializeField, BoxGroup("Resource")] protected HealthBarSO m_HealthBarSO;

#if UNITY_EDITOR
    [BoxGroup("Editor")] public PPrefItemSOVariable m_CurrentWeapons;
#endif

    protected float m_TriggerTimer;
    protected float m_ForwardDistance = 0.8f;
    protected bool m_IsLooking = false;
    protected bool m_IsAiming = false;

    protected IDamageable m_TargetDamagable;
    protected INavigationPoint m_TargetNavigationPoint;
    protected Vector3 m_DefaultLocalPos;
    protected virtual void Awake()
    {
        InitWeapons();
    }
    protected virtual void Start()
    {
        Init();
    }
    protected override void InitWeapons()
    {
        base.InitWeapons();
        m_WeaponHolder.CurrentWeapon.SetFakePointFire(m_FakePointfire);
        m_WeaponHolder.CurrentWeapon.SetOwner(this);
    }
    public virtual void Init()
    {
        m_SoldierStats = new SodierStats();
        m_SoldierStats.LoadStats(m_SoldierStatsSO);

        m_FakePointfire.transform.localPosition = m_WeaponHolder.CurrentWeapon.WeaponSO.PointFirePos;
        m_FakePointfire.transform.localEulerAngles = m_WeaponHolder.CurrentWeapon.WeaponSO.PointFireEur;

        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();
        m_HealthBarMesh.material = new Material(m_HealthBarSO.PlayerHealthBarMaterial);
        RangeIntValue range = new RangeIntValue(0, m_SoldierStats.Health);
        var progress = new RangeProgress<int>(range, 100);
        m_HealthBar.Init(progress);
        m_IsActive = true;
    }
    protected virtual void Update()
    {
        if (!m_IsActive) return;
        DetectEnemy();
        LookAtTarget();
        OnUpdateAttack();

    }

    protected virtual void DetectEnemy()
    {
        List<INavigationPoint> navigations = FindTargetsInRange();
        if (navigations.Count > 0)
        {
            INavigationPoint nearestTarget = navigations
                .Where(v => v != null)
                .OrderBy(v => Vector3.Distance(transform.position, v.GetSelfPoint()))
                .FirstOrDefault();

            m_TargetNavigationPoint = nearestTarget;
        }
    }

    protected virtual void LookAtTarget()
    {
        if (m_TargetNavigationPoint == null)
            return;

        float lookatRange = m_WeaponHolder.CurrentWeapon.WeaponStats.Range * 1.05f;
        float distance = Vector3.Distance(transform.position, GetTargetPoint());
        bool canLook = distance < lookatRange && m_TargetNavigationPoint.IsAvailable();

        if (canLook)
        {
            m_IsLooking = true;
            transform.DOLookAt(GetTargetPoint(), 0.05f);

            if (!m_IsAiming)
            {
                m_IsAiming = true;

                string aimState = m_WeaponHolder.CurrentWeapon.WeaponSO.AimAnimationKey;
                m_Animator.ResetTrigger(m_WeaponHolder.CurrentWeapon.WeaponSO.IdleAnimationKey);
                m_Animator.SetTrigger(aimState);

                m_WeaponHolder.AimIK();
            }
        }
        else
        {
            m_IsLooking = false;

            if (m_IsAiming)
            {
                m_IsAiming = false;

                string idleState = m_WeaponHolder.CurrentWeapon.WeaponSO.IdleAnimationKey;
                m_Animator.ResetTrigger(m_WeaponHolder.CurrentWeapon.WeaponSO.AimAnimationKey);
                m_Animator.SetTrigger(idleState);

                m_WeaponHolder.IdleIK();
            }
        }
    }

    protected void OnUpdateAttack()
    {
        if (m_TargetNavigationPoint == null || !m_IsLooking)
            return;
        m_TriggerTimer -= Time.deltaTime;
        float distanceAttack = Vector3.Distance(transform.position, m_TargetNavigationPoint.GetSelfPoint());
        if (distanceAttack > m_WeaponHolder.CurrentWeapon.WeaponStats.Range)
            return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward * m_ForwardDistance;
        float attackRange = m_WeaponHolder.CurrentWeapon.WeaponStats.Range;

#if UNITY_EDITOR
        Debug.DrawLine(origin, origin + direction * attackRange, Color.cyan, 1.0f);
#endif
        LayerMask targetLayer = m_SoldierStats.TargetLayerMask;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, targetLayer))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null && hit.collider.gameObject.layer != gameObject.layer)
                m_TargetDamagable = target;
        }

        if (m_TriggerTimer <= 0 && m_TargetNavigationPoint.IsAvailable())
        {
            m_TriggerTimer = m_WeaponHolder.CurrentWeapon.WeaponStats.FireRate;
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (m_WeaponHolder == null) return;

        Vector3 targetPoint = GetTargetPoint();
        Vector3 aimDir = (targetPoint - m_WeaponHolder.CurrentWeapon.PointFire.position);
        aimDir.y = 0f;
        aimDir.Normalize();

        if (aimDir.sqrMagnitude > 0.001f)
        {
            float targetYaw = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }

        m_WeaponHolder.FireCurrent();
    }


    protected List<INavigationPoint> FindTargetsInRange()
    {
        var targets = new List<INavigationPoint>();
        var colliders = Physics.OverlapSphere(transform.position, m_WeaponHolder.CurrentWeapon.WeaponStats.Range, m_SoldierStats.TargetLayerMask);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == gameObject.layer)
                continue;

            var navPoint = collider.GetComponent<INavigationPoint>();
            if (navPoint != null && navPoint.IsAvailable())
            {
                targets.Add(navPoint);
            }
        }
        return targets;
    }

    protected void Dead()
    {
        OnDead?.Invoke();
    }

    public void TakeDamage(float amount, Vector3 hitPos)
    {
        if (m_SoldierStats.Health > 0)
        {
            m_SoldierStats.Health -= (int)amount;
            m_LegsAnimator.User_AddImpulse(m_HitDamgePelvisImpulse);
            GameEventHandler.Invoke(PlayerEventCode.TakeDamage, amount);
        }

        if (m_SoldierStats.Health <= 0)
        {
            Dead();
            m_SoldierStats.Health = 0;
        }
        m_HealthBar.SetValue(m_SoldierStats.Health + (int)amount, m_SoldierStats.Health, 0.2f);
    }

    public PointType GetPointType()
    {
        throw new System.NotImplementedException();
    }

    public bool IsAvailable()
    {
        return m_IsAlive;
    }

    public Vector3 GetSelfPoint()
    {
        return transform.position;
    }

    public Vector3 GetTargetPoint()
    {
        return m_TargetNavigationPoint.GetSelfPoint();
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_SoldierStatsSO == null) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        Vector3 center = transform.position + Vector3.up * 0.01f;

        float attackRange = m_CurrentWeapons.value.GetModule<WeaponInfoModule>().Range;

        // Detection Range
        DrawCircleXZ(center, m_SoldierStatsSO.DetectionRange, 64, Color.magenta);
        Vector3 detectionLabelPos = center + new Vector3(0, 0.01f, -m_SoldierStatsSO.DetectionRange + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos, $"Detection Range: {m_SoldierStatsSO.DetectionRange}", style);

        // LookAt Range = AttackRange * 1.5f
        float lookAtRange = attackRange + (attackRange * 0.05f);
        DrawCircleXZ(center, lookAtRange, 64, Color.yellow);
        Vector3 detectionLabelPos2 = center + new Vector3(0, 0.01f, -(m_SoldierStatsSO.AttackRange * 1.5f) + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos2, $"LookAt Range: {lookAtRange}", style);

        // Attack Range
        DrawCircleXZ(center, attackRange, 64, Color.red);
        Vector3 detectionLabelPos3 = center + new Vector3(0, 0.01f, -attackRange + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos3, $"Attack Range: {attackRange}", style);
    }

    private void DrawCircleXZ(Vector3 center, float radius, int segments, Color color)
    {
        Gizmos.color = color;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * angleStep * i;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
#endif


}
