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

        // StartCoroutine(CommonCoroutine.Delay(0.01f, false, () =>
        // {
        //     Transform child = transform;

        //     Vector3 worldPos = child.position;
        //     Quaternion worldRot = child.rotation;

        //     child.SetParent(null, false);
        //     child.position = worldPos;
        //     child.rotation = worldRot;

        //     transform.SetParent(null);

        // }));

        while (timer < m_LifeTime)
        {
            Vector3 targetPos = transform.position + transform.forward * m_Speed * Time.deltaTime;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref velocity,
                0.02f,
                Mathf.Infinity,
                Time.deltaTime
            );

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_Speed * Time.deltaTime))
            {
                transform.position = hit.point;
                OnHit(hit);
                yield break;
            }

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
}
