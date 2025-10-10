using UnityEngine;

[System.Serializable]
public class SodierStats
{
    public LayerMask TargetLayerMask;
    public LayerMask TeamLayerMask;
    public int Health;
    public float AttackDamage;
    public float AttackRange;
    public float AttackCoolDown;
    public float MoveSpeed;
    public float CriticalChance;
    public float CriticalMultiplier;
    public float BlockChance;

    public void LoadStats(SoldierStatsSO statsSO)
    {
        TargetLayerMask = statsSO.TargetLayermask;
        TeamLayerMask = statsSO.TeamLayerMask;
        Health = statsSO.MaxHealth;
        AttackDamage = statsSO.AttackDamage;
        AttackRange = statsSO.AttackRange;
        AttackCoolDown = statsSO.AttackCoolDown;
        BlockChance = statsSO.BlockChance;
        MoveSpeed = statsSO.MoveSpeed;
        CriticalChance = statsSO.CriticalChance;
        CriticalMultiplier = statsSO.CriticalMultiplier;
    }
}
