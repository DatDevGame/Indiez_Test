using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "ZombieWar/Level/LevelSO")]
public class LevelSO : ItemSO
{
    [Header("Level Info")]
    public int LevelIndex;
    public string LevelName;
    public MapBase Map;
    [TextArea] public string Description;

    [Header("Gameplay Settings")]
    public float LevelDuration = 300f;
    public int RequiredKillCount = 50;
    public List<ZombieWaveData> ZombieWaves;
    
    [Header("Rewards")]
    public int GoldReward = 100;
    public int ExpReward = 50;
    public ItemSO[] ItemRewards;
}
