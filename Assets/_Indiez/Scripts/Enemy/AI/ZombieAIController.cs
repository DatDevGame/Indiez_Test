using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class ZombieAIController : AIBotController, INavigationPoint
{
    public bool IsRunning => m_IsRunning;
    public EnemyBase EnemyBase => m_EnemyBase;
    public CharacterController CharacterController => m_CharacterController;
    public Animator Animator => m_Animator;
    public ZombieAIProfile ZombieAIProfile => m_ZombieAIProfile;
    public ZombieAnimationSO AnimationKeySO => m_AnimationKeySO;

    [SerializeField, BoxGroup("Reference")] protected EnemyBase m_EnemyBase;
    [SerializeField, BoxGroup("Reference")] protected CharacterController m_CharacterController;

    private ZombieAIProfile m_ZombieAIProfile;
    private Animator m_Animator;
    private ZombieAnimationSO m_AnimationKeySO;

    protected override void Awake()
    {
        base.Awake();
        if (m_AIProfile is ZombieAIProfile zombieAIProfile)
            m_ZombieAIProfile = zombieAIProfile;
        if (m_Animator == null)
            m_Animator = m_EnemyBase.Animator;
        if (m_AnimationKeySO == null)
            m_AnimationKeySO = m_EnemyBase.ZombieAnimationSO;
        m_EnemyBase.OnDead += OnDead;
    }
    public override void InitializeStateMachine()
    {
        base.InitializeStateMachine();
    }
    public override List<INavigationPoint> FindTargetsInRange()
    {
        var targets = new List<INavigationPoint>();
        if (m_AIProfile == null || m_EnemyBase.EnemyStats == null)
            return targets;

        var colliders = Physics.OverlapSphere(m_BotTransform.position, m_EnemyBase.EnemyStats.DetectionRange, m_EnemyBase.EnemyStats.TargetLayermask);
        foreach (var collider in colliders)
        {
            var navPoint = collider.GetComponent<INavigationPoint>();
            if (navPoint != null && navPoint.IsAvailable())
            {
                targets.Add(navPoint);
            }
        }
        return targets;
    }
    public bool IsAvailable()
    {
        return m_IsRunning;
    }
    public PointType GetPointType()
    {
        return PointType.OpponentPoint;
    }
    public Vector3 GetSelfPoint()
    {
        return transform.position;
    }
    public Vector3 GetTargetPoint()
    {
        return m_Target == null ? Vector3.zero : m_Target.GetSelfPoint();
    }

    protected void OnDead()
    {
        m_NavMeshAgent.enabled = false;
        StopStateMachine();
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        StatsSO m_GizMosBoxerAIProfile = m_EnemyBase.StatsSOData;
        if (m_GizMosBoxerAIProfile == null) return;

        Vector3 center = transform.position + (Vector3.up * 0.1f);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        // === DETECTION RANGE ===
        float detectionRadius = m_GizMosBoxerAIProfile.DetectionRange;
        Gizmos.color = Color.red;
        DrawCircleXZ(center, detectionRadius, 64);

        Color detectionFill = new Color(1f, 0f, 0f, 0.1f);
        Handles.color = detectionFill;
        Handles.DrawSolidDisc(center, Vector3.up, detectionRadius);

        Vector3 detectionLabelPos = center + new Vector3(0, 0.01f, -detectionRadius + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos, $"Detection Range: {detectionRadius}", style);

        // === ATTACK RANGE ===
        float attackRadius = EnemyBase.StatsSOData.AttackRange;
        Gizmos.color = Color.yellow;
        DrawCircleXZ(center, attackRadius, 64);

        Color attackFill = new Color(1f, 1f, 0f, 0.1f);
        Handles.color = attackFill;
        Handles.DrawSolidDisc(center, Vector3.up, attackRadius);

        Vector3 attackLabelPos = center + new Vector3(0, 0.01f, -attackRadius + 0.2f);
        Handles.color = Color.yellow;
        Handles.Label(attackLabelPos, $"Attack Range: {attackRadius}", style);
    }
    private void DrawCircleXZ(Vector3 center, float radius, int segments)
    {
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
