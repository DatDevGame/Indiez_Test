using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Premium.PoolManagement;
using HCore.Helpers;
using HCore.Events;
using Unity.VisualScripting;

public class PVELevelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelManagerSO m_LevelManagerSO;
    [SerializeField] private PPrefItemSOVariable m_CurrentLevelSO;

    private List<EnemyBase> m_EnemyBases;
    private int m_CurrentWaveIndex = 0;
    private int m_AliveZombieCount = 0;
    private bool m_IsSpawning = false;
    private LevelSO m_CurrentLevel;
    private MapBase m_Map;

    private void OnEnable()
    {
        //ZombieBase.OnZombieDied += HandleZombieDied;
    }

    private void OnDisable()
    {
        //ZombieBase.OnZombieDied -= HandleZombieDied;
    }

    private void Start()
    {
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        LevelSO currentLevelSO = m_LevelManagerSO.GetCurrentLevelSO();
        m_CurrentLevel = currentLevelSO;
        if (currentLevelSO == null)
        {
            Debug.LogError($"{this.gameObject.name} - currentLevelSO is Null");
            yield break;
        }

        SpawnMap();
        m_CurrentLevelSO.value = m_LevelManagerSO.GetCurrentLevelSO();
        GameEventHandler.Invoke(PVEEventCode.OnLevelStart);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SpawnWave(m_CurrentLevel.ZombieWaves[m_CurrentWaveIndex]));
    }
    private void SpawnMap()
    {
        m_Map = Instantiate(m_CurrentLevel.Map);
    }
    private IEnumerator SpawnWave(ZombieWaveData wave)
    {
        m_IsSpawning = true;
        yield return new WaitForSeconds(wave.StartDelay);

        Debug.Log($"🧟 Wave {wave.WaveIndex} started!");
        m_AliveZombieCount = wave.ZombieCount;

        for (int i = 0; i < wave.ZombieCount; i++)
        {
            ZombieSpawnInfo zombieSpawnInfo = wave.ZombieTypes.GetRandom();
            var zombiePrefab = zombieSpawnInfo.ZombiePrefab;
            Transform spawnPoint = m_CurrentLevel.Map.GetRandomSpawnPoint();

            if (zombiePrefab != null && spawnPoint != null)
            {
                var pool = PoolManager.GetOrCreatePool<EnemyBase>($"{zombieSpawnInfo.PoolKey}", zombiePrefab, initialCapacity: 1);
                var zombie = pool.Get();
                zombie.transform.position = spawnPoint.position;
                zombie.transform.rotation = spawnPoint.rotation;
                zombie.gameObject.SetActive(true);
                zombie.Init();
            }
            yield return new WaitForSeconds(wave.SpawnInterval);
        }
        m_IsSpawning = false;
    }

    private void HandleZombieDied(EnemyBase zombie)
    {
        m_AliveZombieCount--;

        if (m_AliveZombieCount <= 0 && !m_IsSpawning)
            StartCoroutine(NextWave());
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(2f);

        m_CurrentWaveIndex++;

        if (m_CurrentWaveIndex < m_CurrentLevel.ZombieWaves.Count)
        {
            yield return StartCoroutine(SpawnWave(m_CurrentLevel.ZombieWaves[m_CurrentWaveIndex]));
        }
        else
        {
            Debug.Log("🎉 All waves completed! Level clear!");
        }
    }
}
