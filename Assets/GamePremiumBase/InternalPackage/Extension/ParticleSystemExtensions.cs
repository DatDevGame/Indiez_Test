using System.Collections;
using System.Collections.Generic;
using Premium;
using Premium.PoolManagement;
using UnityEngine;

public static class ParticleSystemExtensions
{
    public static void Release(this ParticleSystem particleInstance, ParticleSystem particlePrefab, float delayTime)
    {
        PoolManager.Instance.StartCoroutine(CommonCoroutine.Delay(delayTime, false, () =>
        {
            if (PoolManager.ContainsPool(particlePrefab.GetInstanceID().ToString()) || PoolManager.ContainsPool(particlePrefab.gameObject.GetInstanceID().ToString()))
            {
                PoolManager.Release(particlePrefab, particleInstance);
                particleInstance.gameObject.SetActive(false);
                particleInstance.transform.SetParent(PoolManager.Instance.transform);
            }
        }));
    }
}