using System;
using Premium;
using Premium.PoolManagement;
using UnityEngine;

public static class PoolSystemExtensions
{
    public static void Release<T>(this T instance, T prefab, float delayTime) where T : Component
    {
        PoolManager.Instance.StartCoroutine(CommonCoroutine.Delay(delayTime, false, () =>
        {
            string key = prefab.GetInstanceID().ToString();
            if (PoolManager.ContainsPool(key) || PoolManager.ContainsPool(prefab.gameObject.GetInstanceID().ToString()))
            {
                PoolManager.Release(prefab, instance);
                instance.gameObject.SetActive(false);
            }
        }));
    }
}
