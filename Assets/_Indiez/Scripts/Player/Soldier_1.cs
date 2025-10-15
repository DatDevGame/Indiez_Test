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
    public Transform Visual => m_Visual;
    public bool IsAiming => m_IsAiming;
    public INavigationPoint TargetNavigationPoint => m_TargetNavigationPoint;

    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_HitDamgePelvisImpulse;
    [SerializeField, BoxGroup("Config Throw Bomb")] protected float m_SpeedsMulty = 2;
    [SerializeField, BoxGroup("Config Throw Bomb")] protected float m_ArcFactor = 0.07f;
    [SerializeField, BoxGroup("Config Delay Attack Then Look")] protected float m_LookThemTime = 1.5f;
    [SerializeField, BoxGroup("References")] protected RagdollController m_RagdollController;
    [SerializeField, BoxGroup("Referrence")] protected Transform m_Visual;
    [SerializeField, BoxGroup("Referrence")] protected Transform m_FakePointfire;
    [SerializeField, BoxGroup("Referrence")] protected Transform m_GrenadePoint;
    [SerializeField, BoxGroup("Referrence")] protected Transform m_CenterPoint;
    [SerializeField, BoxGroup("Resource")] protected HealthBarSO m_HealthBarSO;

#if UNITY_EDITOR
    [BoxGroup("Editor")] public PPrefItemSOVariable m_CurrentWeapons;
#endif

    protected float m_TriggerTimer;
    protected float m_ChangeWeaponTimer;
    protected float m_ThenAimTimer;
    protected float m_ForwardDistance = 0.8f;
    protected bool m_IsLooking = false;
    protected bool m_IsAiming = false;
    protected bool m_IsFacingTarget;


    protected float m_TargetSwitchDelay = 0.5f;
    protected float m_TargetSwitchTimer = 0f;


    protected IDamageable m_TargetDamagable;
    protected INavigationPoint m_TargetNavigationPoint;


    protected virtual void Awake()
    {
        GameEventHandler.AddActionEvent(PlayerEventCode.EquipWeapon, OnEquipWeaponEvent);
        GameEventHandler.AddActionEvent(PlayerEventCode.ThrowGrenadeTrigger, ThrowGrenadeTrigger);
        InitWeapons();
    }

    protected virtual void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PlayerEventCode.EquipWeapon, OnEquipWeaponEvent);
        GameEventHandler.RemoveActionEvent(PlayerEventCode.ThrowGrenadeTrigger, ThrowGrenadeTrigger);
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
        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();
        m_HealthBarMesh.material = new Material(m_HealthBarSO.PlayerHealthBarMaterial);
        RangeIntValue range = new RangeIntValue(0, m_SoldierStats.Health);
        var progress = new RangeProgress<int>(range, m_SoldierStats.Health);

        m_HealthBar.Init(progress);
        m_IsActive = true;
    }
    protected virtual void ThrowGrenadeTrigger() => ThrowGrenade();
    protected virtual void Update()
    {
        if (!m_IsAlive) return;
        DetectEnemy();
        LookAtTarget();
        OnUpdateAttack();
    }

    protected void OnEquipWeaponEvent(params object[] parameters)
    {
        if (parameters[0] == null || parameters.Length <= 0)
            return;

        WeaponSO weaponSOEquip = parameters[0] as WeaponSO;
        EquipWeapon(weaponSOEquip);
    }

    protected virtual void EquipWeapon(WeaponSO weaponSO)
    {
        m_WeaponHolder.EquipWeapon(
                weaponSO
                .GetModule<ModelPrefabItemModule>()
                .modelPrefabAsGameObject
                .GetComponent<BaseWeapon>()
        );
        m_WeaponHolder.CurrentWeapon.SetFakePointFire(m_FakePointfire);
        m_WeaponHolder.CurrentWeapon.SetOwner(this);
        m_ChangeWeaponTimer = 1f;
    }

    protected virtual void DetectEnemy()
    {
        m_TargetSwitchTimer -= Time.deltaTime;
        List<INavigationPoint> navigations = FindTargetsInRange();
        if (navigations.Count == 0)
        {
            m_TargetNavigationPoint = null;
            return;
        }

        if (m_TargetNavigationPoint != null && m_TargetNavigationPoint.IsAvailable())
        {
            float distanceToCurrent = Vector3.Distance(transform.position, m_TargetNavigationPoint.GetSelfPoint());
            if (distanceToCurrent <= m_WeaponHolder.CurrentWeapon.WeaponStats.Range * 1.5f)
                return;
        }

        if (m_TargetSwitchTimer > 0f)
            return;

        INavigationPoint nearestTarget = navigations
            .Where(v => v != null)
            .Where(v => IsVisible(v.GetSelfPoint()))
            .OrderBy(v => Vector3.Distance(transform.position, v.GetSelfPoint()))
            .FirstOrDefault();

        if (nearestTarget != null && nearestTarget != m_TargetNavigationPoint)
        {
            m_TargetNavigationPoint = nearestTarget;
            m_TargetSwitchTimer = m_TargetSwitchDelay;
        }
    }


    private bool IsVisible(Vector3 targetPoint)
    {
        Vector3 origin = transform.position;
        Vector3 direction = targetPoint - origin;
        float distance = direction.magnitude;

        int obstacleMask = LayerMask.GetMask("Wall", "Ground");
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, obstacleMask))
            return false;

        return true;
    }


    protected virtual void LookAtTarget()
    {
        // if (!m_CharacterController.isGrounded)
        // {
        //     m_Visual.eulerAngles = Vector3.zero;
        //     m_IsLooking = false;
        //     m_IsFacingTarget = false;
        //     m_IsAiming = false;
        //     return;
        // }

        if (m_TargetNavigationPoint == null)
            return;
        m_ChangeWeaponTimer -= Time.deltaTime;

        float lookatRange = m_WeaponHolder.CurrentWeapon.WeaponStats.Range * 1.5f;
        float distance = Vector3.Distance(transform.position, GetTargetPoint());
        bool canLook = distance < lookatRange && m_TargetNavigationPoint.IsAvailable();

        if (canLook && m_ChangeWeaponTimer <= 0)
        {
            m_IsLooking = true;
            Vector3 targetPoint = GetTargetPoint();
            Vector3 dirToTarget = targetPoint - m_Visual.position;

            Vector3 flatDir = dirToTarget;
            flatDir.y = 0f;

            if (flatDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(flatDir);
                float rotateSpeed = 360f;
                m_Visual.rotation = Quaternion.RotateTowards(m_Visual.rotation, targetRot, rotateSpeed * Time.deltaTime);
                float angle = Quaternion.Angle(m_Visual.rotation, targetRot);
                m_IsFacingTarget = angle < 5f;
            }
            else
            {
                m_IsFacingTarget = false;
            }

            Vector3 muzzleDir = (targetPoint - m_FakePointfire.position).normalized;
            Quaternion aimRot = Quaternion.LookRotation(muzzleDir, m_Visual.up);
            m_FakePointfire.rotation = aimRot;

            if (!m_IsFacingTarget)
            {
                m_Visual.DOLookAt(targetPoint, 0.2f, AxisConstraint.Y);
                return;
            }


            if (!m_IsAiming)
            {
                m_IsAiming = true;
                string aimState = m_WeaponHolder.CurrentWeapon.WeaponSO.AimAnimationKey;
                m_Animator.SetBool(m_WeaponHolder.CurrentWeapon.WeaponSO.IdleAnimationKey, false);
                m_Animator.SetBool(aimState, true);

                m_WeaponHolder.AimIK();
            }
            else
            {
                m_ThenAimTimer -= Time.deltaTime;
            }
        }
        else
        {
            m_IsLooking = false;
            if (m_IsAiming)
            {
                m_IsAiming = false;

                string idleState = m_WeaponHolder.CurrentWeapon.WeaponSO.IdleAnimationKey;
                m_Animator.SetBool(m_WeaponHolder.CurrentWeapon.WeaponSO.AimAnimationKey, false);
                m_Animator.SetBool(idleState, true);

                m_WeaponHolder.IdleIK();
            }
        }
    }

    protected void OnUpdateAttack()
    {
        if (m_TargetNavigationPoint == null || !m_IsLooking || !m_IsAiming)
            return;
        Debug.Log($"Key Main -> 5");
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

        if (m_TriggerTimer <= 0 && m_ChangeWeaponTimer <= 0 && m_TargetNavigationPoint.IsAvailable())
        {
            m_TriggerTimer = m_WeaponHolder.CurrentWeapon.WeaponStats.FireRate;
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (m_WeaponHolder == null) return;
        m_WeaponHolder.FireCurrent();
    }
    protected virtual void ThrowGrenade()
    {
        if (m_IsAiming)
        {
            ThrowToTargetByDistance();
        }
    }
    protected virtual void ThrowToTargetByDistance()
    {
        var grenadePool = PoolManager.GetOrCreatePool<GrenadeSoldier>(
                objectPrefab: m_GrenadeSoldierPrefab,
                initialCapacity: 1
            );

        GrenadeSoldier grenadeSoldier = grenadePool.Get();
        grenadeSoldier.transform.SetPositionAndRotation(m_FakePointfire.position, m_FakePointfire.rotation);
        grenadeSoldier.gameObject.SetActive(true);
        grenadeSoldier.OnInit(this);
        grenadeSoldier.StartFuse();
        grenadeSoldier.ThrowToTargetByDistance(m_GrenadePoint.position, GetTargetPoint(), m_SpeedsMulty, m_ArcFactor);
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
        m_WeaponHolder.RightHandIK.weight = 0;
        m_WeaponHolder.LeftHandIK.weight = 0;
        m_IsAlive = false;
        m_Animator.enabled = false;
        m_LegsAnimator.enabled = m_IsAlive;
        m_HealthBarMesh.enabled = m_IsAlive;
        m_CharacterController.enabled = m_IsAlive;
        OnDead?.Invoke();
        m_RagdollController.EnableRagdoll();
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

    public Vector3 GetCenterPoint()
    {
        return m_CenterPoint == null ? transform.position : m_CenterPoint.position;
    }

    public PointType GetPointType()
    {
        return PointType.OpponentPoint;
    }

    public bool IsAvailable()
    {
        return m_IsAlive;
    }

    public Vector3 GetSelfPoint()
    {
        return m_CenterPoint == null ? transform.position : m_CenterPoint.position;
    }
    public Vector3 GetTargetPoint() => m_TargetNavigationPoint?.GetSelfPoint() ?? transform.position * 99;
#if UNITY_EDITOR

    [BoxGroup("Editor")] public float speedsMultyEditOR = 2;
    [BoxGroup("Editor")] public float arcFactoreDITOR = 0.07f;
    [Button]

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
