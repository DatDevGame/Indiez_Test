using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Premium.PoolManagement;
using HCore.Helpers;
using HCore.Events;
using Unity.VisualScripting;
using Sirenix.OdinInspector;
using System.Linq;

public class PVELevelController : MonoBehaviour
{
    [SerializeField, BoxGroup("Reference")] private BaseSoldier m_Sodier;
    [SerializeField, BoxGroup("Data")] private LevelManagerSO m_LevelManagerSO;
    [SerializeField, BoxGroup("Data")] private PPrefItemSOVariable m_CurrentLevelSO;
    [SerializeField, BoxGroup("Data")] private IntVariable m_CurrentWave;
    [SerializeField, BoxGroup("Data")] private PPrefIntVariable m_CurrentLevelPPref;

    private List<EnemyBase> m_EnemyBases;
    private int m_CurrentWaveIndex = 0;
    private int m_AliveZombieCount = 0;
    private bool m_IsSpawning = false;
    private LevelSO m_CurrentLevel;
    private MapBase m_Map;
    private void Start()
    {
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        m_EnemyBases = new List<EnemyBase>();
        LevelSO currentLevelSO = m_LevelManagerSO.GetCurrentLevelSO();
        m_CurrentLevel = currentLevelSO;
        if (currentLevelSO == null)
        {
            Debug.LogError($"{this.gameObject.name} - currentLevelSO is Null");
            yield break;
        }

        SpawnMap();
        m_CurrentLevelSO.value = m_LevelManagerSO.GetCurrentLevelSO();
        m_Sodier.transform.position = m_CurrentLevel.Map.GetPlayerPoint();
        m_Sodier.OnDead += HandlePlayerDead;
        GameEventHandler.Invoke(PVEEventCode.OnLevelStart, m_CurrentLevelSO.value, m_Sodier);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SpawnWave(m_CurrentLevel.ZombieWaves[m_CurrentWaveIndex]));
    }
    private void SpawnMap()
    {
        m_Map = Instantiate(m_CurrentLevel.Map);
    }
    private IEnumerator SpawnWave(ZombieWaveData wave)
    {
        m_CurrentWave.value = 0;
        m_IsSpawning = true;
        yield return new WaitForSeconds(wave.StartDelay);

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
                m_EnemyBases.Add(zombie);
                zombie.OnDead += HandleZombieDied;
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
        zombie.OnDead -= HandleZombieDied;
    }

    private void HandlePlayerDead()
    {
        m_CurrentLevelPPref.value++;
        GameEventHandler.Invoke(PVEEventCode.OnLevelEnd, false);
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(2f);
        m_CurrentWaveIndex++;
        m_CurrentWave.value = m_CurrentWaveIndex;

        if (m_CurrentWaveIndex < m_CurrentLevel.ZombieWaves.Count)
        {
            yield return StartCoroutine(SpawnWave(m_CurrentLevel.ZombieWaves[m_CurrentWaveIndex]));
        }
        else
        {
            GameEventHandler.Invoke(PVEEventCode.OnLevelEnd, true);
        }
    }

    private bool IsEndGame()
    {
        return m_EnemyBases
        .Where(v => v.gameObject.activeSelf)
        .All(x => !x.IsAlive);
    }
}
