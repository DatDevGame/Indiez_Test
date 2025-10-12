using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "ZombieWar/Level/LevelSO")]
public class LevelSO : ItemSO
{
    [Header("Level Info")]
    public MapBase Map;
    [TextArea] public string Description;

    public List<ZombieWaveData> ZombieWaves;
}
