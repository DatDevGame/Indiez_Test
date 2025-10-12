using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ZombieWaveData
{
    public int ZombieCount = 10;
    public float SpawnInterval = 1.5f;
    public float StartDelay = 2f;
    public List<ZombieSpawnInfo> ZombieTypes;

    public ZombieSpawnInfo GetRandomZombieByPercent()
    {
        float total = ZombieTypes.Sum(z => z.SpawnPercent);
        if (total <= 0)
        {
            return ZombieTypes[0];
        }

        float randomValue = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var zombie in ZombieTypes)
        {
            cumulative += zombie.SpawnPercent;
            if (randomValue <= cumulative)
                return zombie;
        }
        return ZombieTypes.Last();
    }

    [Button]
    public void Test()
    {
        Debug.Log($"Key Pro HEHE ->  {GetRandomZombieByPercent().ZombiePrefab.name}");
    }
}
