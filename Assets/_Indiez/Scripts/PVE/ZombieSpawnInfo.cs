using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ZombieSpawnInfo
{
    [ShowInInspector] public string PoolKey => ZombiePrefab.GetComponent<EnemyBase>().GetType().Name;
    [SerializeField] public EnemyBase ZombiePrefab;
    [Range(0f, 1f)] public float SpawnChance = 1f;
}
