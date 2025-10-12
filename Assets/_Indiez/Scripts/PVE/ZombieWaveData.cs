using System.Collections.Generic;

[System.Serializable]
public class ZombieWaveData
{
    public int WaveIndex;
    public int ZombieCount = 10;
    public float SpawnInterval = 1.5f;
    public float StartDelay = 2f;
    public List<ZombieSpawnInfo> ZombieTypes;
}
