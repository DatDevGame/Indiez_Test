using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ZombieSpawnInfo
{
    [ShowInInspector]
    public string PoolKey
    {
        get
        {
            if (ZombiePrefab == null)
                return "None";

            var enemy = ZombiePrefab.GetComponents<MonoBehaviour>()
                .FirstOrDefault(c => c is EnemyBase);

            return enemy != null ? enemy.GetType().Name : "Unknown";
        }
    }


    [SerializeField] public EnemyBase ZombiePrefab;
    [Range(0, 100)] public float SpawnPercent;
}
