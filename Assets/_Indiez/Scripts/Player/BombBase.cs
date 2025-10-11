using System;
using System.Collections;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
public class BombBase : MonoBehaviour
{
    [SerializeField, BoxGroup("Core Config")] protected float m_FuseTime = 3.0f;
    [SerializeField, BoxGroup("Core Config")] protected float m_ExplosionRadius = 5.0f;
    [SerializeField, BoxGroup("Core Config")] protected int m_Damage = 100;

    [SerializeField, BoxGroup("Physics Config")] protected bool m_ApplyExplosionForce = true;
    [SerializeField, BoxGroup("Physics Config")] protected float m_ExplosionForce = 700f;
    [SerializeField, BoxGroup("Physics Config")] protected float m_ExplosionUpwardModifier = 1f;

    [SerializeField, BoxGroup("Resource")] protected ParticleSystem m_ExplosionVFX;

    protected Action m_OnExplosion = delegate { };

    protected Rigidbody m_Rb;
    protected bool m_IsFused = false;
    protected bool m_HasExploded = false;

    protected virtual void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
    }

    public virtual void Throw(Vector3 velocity)
    {
        if (m_Rb != null)
        {
            m_Rb.velocity = velocity;
        }
    }

    [Button]
    public virtual void StartFuse()
    {
        if (m_IsFused) return;
        m_IsFused = true;
        StartCoroutine(FuseCoroutine());
    }

    public virtual void CancelFuse()
    {
        if (!m_IsFused) return;
        m_IsFused = false;
        StopAllCoroutines();
    }

    protected virtual IEnumerator FuseCoroutine()
    {
        float t = 0f;
        while (t < m_FuseTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
        Explode();
    }

    protected virtual void Explode()
    {
        if (m_HasExploded) return;
        m_HasExploded = true;

        // VFX
        if (m_ExplosionVFX != null)
        {
            var explosionVFXPool = PoolManager.GetOrCreatePool<ParticleSystem>(
                objectPrefab: m_ExplosionVFX,
                initialCapacity: 1
            );
            ParticleSystem explosionVFX = explosionVFXPool.Get();
            explosionVFX.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            explosionVFX.gameObject.SetActive(true);
            explosionVFX.Play();
            explosionVFX.Release(m_ExplosionVFX, 3f);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        foreach (var col in hits)
        {
            IDamageable dmg = col.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(m_Damage, transform.position);
            }

            if (m_ApplyExplosionForce)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius, m_ExplosionUpwardModifier, ForceMode.Impulse);
                }
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, m_ExplosionRadius);
    }
}
