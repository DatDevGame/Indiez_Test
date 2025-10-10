using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCore.SerializedDataStructure;

[CreateAssetMenu(fileName = "BulletImpactDataSO", menuName = "ZombieWar/Weapon/BulletImpactDataSO")]
public class BulletImpactDataSO : SerializableScriptableObject
{
    public SerializedDictionary<LayerMask, ParticleSystem> m_BulletImpactDictionary;

    public ParticleSystem GetBulletImpact(int layer)
    {
        int mask = 1 << layer;
        return m_BulletImpactDictionary.TryGetValue(mask, out var vfx) ? vfx : null;
    }

}
