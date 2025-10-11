using System.Collections;
using System.Collections.Generic;
using Premium.PoolManagement;
using UnityEngine;

public class GrenadeSoldier : BombBase
{
    private BaseSoldier m_Owner;
    public virtual void OnInit(BaseSoldier Owner)
    {
        m_Owner = Owner;
        m_IsFused = false;
        m_HasExploded = false;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Explode()
    {
        base.Explode();
        PoolManager.Release(m_Owner.GrenadeSoldierPrefab, this);
        gameObject.SetActive(false);
    }

    public void ThrowToTargetByDistance(Vector3 origin, Vector3 target, float speedMultiplier = 1f, float arcFactor = 0.2f)
    {
        transform.position = origin;
        if (m_Rb == null) m_Rb = GetComponent<Rigidbody>();

        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float x = toTargetXZ.magnitude;
        float baseSpeed = Mathf.Clamp(x * 1.2f * speedMultiplier, 5f, 30f);

        bool useHighArc = arcFactor > 0.15f;
        float arcBias = Mathf.Clamp01(arcFactor) * 0.8f;
        ThrowFromPosition_Angled(origin, target, baseSpeed, useHighArc, arcBias);
    }

    private void ThrowFromPosition_Angled(Vector3 origin, Vector3 target, float speed, bool useHighArc, float arcBias)
    {
        transform.position = origin;
        if (m_Rb == null) m_Rb = GetComponent<Rigidbody>();

        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float y = toTarget.y;
        float x = toTargetXZ.magnitude;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float speedSq = speed * speed;

        float discriminant = speedSq * speedSq - gravity * (gravity * x * x + 2 * y * speedSq);
        if (discriminant < 0)
        {
            Vector3 dir = (toTargetXZ.normalized + Vector3.up * 0.5f).normalized;
            m_Rb.velocity = dir * speed;
            return;
        }

        float discSqrt = Mathf.Sqrt(discriminant);

        float angle = useHighArc
            ? Mathf.Atan2(speedSq + discSqrt, gravity * x)
            : Mathf.Atan2(speedSq - discSqrt, gravity * x);

        float extra = arcBias * (Mathf.PI / 6f);
        if (useHighArc) angle += extra;
        else angle += extra * 0.35f;

        Vector3 velocity = (toTargetXZ.normalized * Mathf.Cos(angle) + Vector3.up * Mathf.Sin(angle)) * speed;
        m_Rb.velocity = velocity;
    }
}
