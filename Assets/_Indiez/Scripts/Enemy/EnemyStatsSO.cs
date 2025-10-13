using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "ZombieWar/Stats/EnemyStatsSO", order = 0)]
public class EnemyStatsSO : StatsSO
{
    [SerializeField, BoxGroup("Combat Stats")] public LayerMask TargetLayermask = -1;
    [SerializeField, BoxGroup("Combat Stats")] public float PercentSpeedUpThenScream = 0.5f;
    [SerializeField, BoxGroup("Combat Stats")] public float DurationSpeedUp = 1;
}
