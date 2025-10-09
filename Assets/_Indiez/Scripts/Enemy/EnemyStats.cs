using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public LayerMask TargetLayermask;
    public LayerMask TeamLayerMask;
    public int Health;
    public float AttackDamage;
    public float AttackRange;
    public float AttackCoolDown;
    public float MoveSpeed;
    public float DetectionRange;
    public float CriticalChance;
    public float CriticalMultiplier;

    public void LoadStats(EnemyStatsSO statsSO)
    {
        TargetLayermask = statsSO.TargetLayermask;
        TeamLayerMask = statsSO.TeamLayerMask;
        Health = statsSO.MaxHealth;
        AttackDamage = statsSO.AttackDamage;
        AttackRange = statsSO.AttackRange;
        AttackCoolDown = statsSO.AttackCoolDown;
        MoveSpeed = statsSO.MoveSpeed;
        DetectionRange = statsSO.DetectionRange;
        CriticalChance = statsSO.CriticalChance;
        CriticalMultiplier = statsSO.CriticalMultiplier;
    }
}
