using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Indiez;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Soldier_1 : BaseSoldier, Indiez.Interface.INavigationPoint
{
    [SerializeField, BoxGroup("Resource")] protected StatsSO m_PlayerStatsSO;

    protected float m_TriggerTimer;
    protected float m_ForwardDistance = 0.8f;
    protected bool m_IsLooking = false;

    //protected PlayerBoxerAnimationEventReceiver m_PlayerBoxerAnimationEventReceiver;
    protected IDamageable m_TargetDamagable;
    protected INavigationPoint m_TargetNavigationPoint;

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
    }
    public virtual void Init()
    {
        m_StatsSO = m_PlayerStatsSO;
        m_SoldierStats = new SodierStats();
        m_SoldierStats.LoadStats(m_StatsSO);
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
        if (m_TargetNavigationPoint != null)
        {
            float attackRange = Vector3.Distance(transform.position, m_TargetNavigationPoint.GetSelfPoint());
            if (attackRange < m_PlayerStatsSO.AttackRange * m_PlayerStatsSO.LookAtRange && m_TargetNavigationPoint.IsAvailable())
            {
                m_IsLooking = true;
                transform.DOLookAt(m_TargetNavigationPoint.GetSelfPoint(), m_PlayerStatsSO.LookAtDuration);
            }
            else
            {
                m_IsLooking = false;
            }

        }
    }

    protected void OnUpdateAttack()
    {
        if (m_TargetNavigationPoint == null || !m_IsLooking)
            return;
        m_TriggerTimer -= Time.deltaTime;
        float distanceAttack = Vector3.Distance(transform.position, m_TargetNavigationPoint.GetSelfPoint());
        if (distanceAttack > m_SoldierStats.AttackRange)
            return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward * m_ForwardDistance;
        float attackRange = m_ForwardDistance;

#if UNITY_EDITOR
        Debug.DrawLine(origin, origin + direction * attackRange, Color.cyan, 1.0f);
#endif
        LayerMask targetLayer = m_SoldierStats.TeamLayerMask;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, targetLayer))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null && hit.collider.gameObject.layer != gameObject.layer)
                m_TargetDamagable = target;
        }

        if (m_TriggerTimer <= 0 && m_TargetNavigationPoint.IsAvailable())
            PerformAttack();
    }
    private void PerformAttack()
    {
        string keyAttackType = UnityEngine.Random.Range(0, 1) <= 0 ? m_AnimationKeySO.HeadAttack : m_AnimationKeySO.BodyAttack;
        m_Animator.SetTrigger(keyAttackType);
        float animationLength = 0f;
        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        animationLength = stateInfo.length / m_AnimationKeySO.DivineAnimSpeedAttack;
        m_TriggerTimer = m_SoldierStats.AttackCoolDown + animationLength;
    }

    //Call In Animation
    public void HandleAttackHit()
    {
        if (m_TargetDamagable != null && m_TargetNavigationPoint != null)
        {
            float distanceAttack = Vector3.Distance(transform.position, m_TargetNavigationPoint.GetSelfPoint());
            if (distanceAttack <= m_SoldierStats.AttackRange)
            {
                HapticManager.Instance.PlayFlashHaptic();
                CameraShake.Instance.Shake(0.05f, 0.2f);
                m_TargetDamagable.TakeDamage(m_SoldierStats.AttackDamage);
                //SoundManager.Instance.PlayLoopSFX(GetRandomPunchSound(), volumn: 0.5f);
            }
        }
    }
    protected List<INavigationPoint> FindTargetsInRange()
    {
        var targets = new List<INavigationPoint>();
        var colliders = Physics.OverlapSphere(transform.position, m_PlayerStatsSO.DetectionRange, m_PlayerStatsSO.TeamLayerMask);
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


    public BaseSoldier GetSoldier()
    {
        throw new System.NotImplementedException();
    }

    public Indiez.Interface.PointType GetPointType()
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
        if (m_PlayerStatsSO == null) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        Vector3 center = transform.position + Vector3.up * 0.01f;

        // Detection Range
        DrawCircleXZ(center, m_PlayerStatsSO.DetectionRange, 64, Color.magenta);
        Vector3 detectionLabelPos = center + new Vector3(0, 0.01f, -m_PlayerStatsSO.DetectionRange + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos, $"Detection Range: {m_PlayerStatsSO.DetectionRange}", style);

        // LookAt Range = AttackRange * 1.5f
        float lookAtRange = m_PlayerStatsSO.AttackRange * m_PlayerStatsSO.LookAtRange;
        DrawCircleXZ(center, lookAtRange, 64, Color.yellow);
        Vector3 detectionLabelPos2 = center + new Vector3(0, 0.01f, -(m_PlayerStatsSO.AttackRange * 1.5f) + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos2, $"LookAt Range: {m_PlayerStatsSO.AttackRange * 1.5f}", style);

        // Attack Range
        DrawCircleXZ(center, m_PlayerStatsSO.AttackRange, 64, Color.red);
        Vector3 detectionLabelPos3 = center + new Vector3(0, 0.01f, -m_PlayerStatsSO.AttackRange + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos3, $"Attack Range: {m_PlayerStatsSO.AttackRange}", style);
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
