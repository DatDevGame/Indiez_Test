using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "StatsSO", menuName = "ZombieWar/Stats/StatsSO", order = 0)]
public class StatsSO : SerializableScriptableObject
{
    [SerializeField, BoxGroup("General")] public float LookAtDuration = 0.2F;
    [SerializeField, BoxGroup("Combat Stats")] public int MaxHealth = 100;
    [SerializeField, BoxGroup("Combat Stats")] public float DetectionRange = 100f;
    [SerializeField, BoxGroup("Combat Stats")] public float BlockChance = 0.2f;
    [SerializeField, BoxGroup("Combat Stats")] public float CriticalChance = 0.1f;
    [SerializeField, BoxGroup("Combat Stats")] public float CriticalMultiplier = 2f;
    [SerializeField, BoxGroup("Combat Stats")] public LayerMask TeamLayerMask = -1;

    [Title("Look State Config", "", TitleAlignments.Centered)]
    [Range(0.1f, 5f)]
    public float LookAtRange = 4f;

    [Title("Move State Config", "", TitleAlignments.Centered)]
    [Range(0.1f, 5f)]
    public float MoveSpeed = 3.5f;

    [Title("Attack State Config", "", TitleAlignments.Centered)]
    public float AttackDamage = 15f;
    public float AttackRange = 0.5f;
    public float AttackCoolDown = 1f;
}
