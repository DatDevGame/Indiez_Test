using System.Collections;
using System.Collections.Generic;
using Premium;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    [SerializeField, BoxGroup("Config")] protected float m_LifeTime = 3f;
    [SerializeField, BoxGroup("Config")] protected float m_Speed = 20f;
    [SerializeField, BoxGroup("Config")] protected float m_radius = 0.05f;
    [SerializeField, BoxGroup("Data")] protected BulletImpactDataSO m_BulletImpactDataSO;
    protected BaseWeapon m_Gun;
    protected Coroutine m_ShootCoroutine;

    protected bool m_IsActive = false;
    public virtual void OnInit(BaseWeapon baseWeapon)
    {
        m_Gun = baseWeapon;
    }
    public virtual void Shoot()
    {
        gameObject.SetActive(true);
        if (m_ShootCoroutine != null)
            StopCoroutine(m_ShootCoroutine);
        m_ShootCoroutine = StartCoroutine(ShootCoroutine());
    }

    protected virtual IEnumerator ShootCoroutine()
    {
        float timer = 0f;
        Vector3 velocity = Vector3.zero;
        float sphereRadius = 0.05f;
        Vector3 lastPos = transform.position;

        while (timer < m_LifeTime)
        {
            Vector3 targetPos = transform.position + transform.forward * m_Speed * Time.deltaTime;
            Vector3 direction = (targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPos);

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, direction, distance);
            if (hits.Length > 0)
            {
                RaycastHit closestHit = hits[0];
                float closestDist = float.MaxValue;

                foreach (var h in hits)
                {
                    float d = Vector3.Distance(transform.position, h.point);
                    if (d < closestDist)
                    {
                        closestHit = h;
                        closestDist = d;
                    }
                }

                transform.position = closestHit.point - direction * sphereRadius;
                OnHit(closestHit);
                yield break;
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref velocity,
                0.02f,
                Mathf.Infinity,
                Time.deltaTime
            );

            lastPos = transform.position;
            timer += Time.deltaTime;
            yield return null;
        }

        Despawn();
    }

    protected virtual void OnHit(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out IDamageable damageable) && m_Gun != null)
        {
            if (m_Gun.WeaponSO.TryGetModule<WeaponInfoModule>(out var module))
            {
                damageable.TakeDamage(module.Damage, hit.point);
            }
        }
        else
        {
            #region Pool BulletImpact
            ParticleSystem bulletImpactPrefab = m_BulletImpactDataSO.GetBulletImpact(hit.collider.gameObject.layer);
            if (bulletImpactPrefab != null)
            {
                // --- Bullet Impact VFX ---
                var bulletImpactPool = PoolManager.GetOrCreatePool<ParticleSystem>(
                    objectPrefab: bulletImpactPrefab,
                    initialCapacity: 1
                );
                ParticleSystem bulletImpact = bulletImpactPool.Get();

                bulletImpact.transform.SetParent(hit.collider.transform, false);
                bulletImpact.transform.position = hit.point;

                bulletImpact.gameObject.SetActive(true);
                bulletImpact.Play();
                bulletImpact.Release(bulletImpactPrefab, 1);
            }
            #endregion
        }
        Despawn();
    }

    protected virtual void Despawn()
    {
        if (m_ShootCoroutine != null)
        {
            StopCoroutine(m_ShootCoroutine);
        }
        gameObject.SetActive(false);
        PoolManager.Release(m_Gun.BulletPrefab, this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Vector3 dir = transform.forward;
        float distance = m_Speed * Time.deltaTime;
        float sphereRadius = m_radius; 

        Gizmos.DrawLine(transform.position, transform.position + dir * distance);

        Gizmos.color = new Color(1, 0.5f, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, sphereRadius);

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position + dir * distance, sphereRadius);
    }
#endif

}
