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

    [SerializeField, BoxGroup("Test")] private GameObject m_PlayerWeaponUI;
    [SerializeField, BoxGroup("Test")] private CanvasGroup m_CanvasGroupTest;

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

    private void Update()
    {
        Debug.Log($"Key Pro - 1 - {m_PlayerWeaponUI.activeSelf}");
        
        Debug.Log($"Key Pro - 2 - {m_CanvasGroupTest.alpha}");
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

        m_CurrentWave.value = 0;
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
        m_IsSpawning = true;
        yield return new WaitForSeconds(wave.StartDelay);

        m_AliveZombieCount = wave.ZombieCount;

        for (int i = 0; i < wave.ZombieCount; i++)
        {
            ZombieSpawnInfo zombieSpawnInfo = wave.GetRandomZombieByPercent();
            var zombiePrefab = zombieSpawnInfo.ZombiePrefab;
            Transform spawnPoint = m_CurrentLevel.Map.GetRandomSpawnPoint();
            Debug.Log($"Key Pro HEHE 1 -> {zombiePrefab.name}");
            if (zombiePrefab != null && spawnPoint != null)
            {
                Debug.Log($"Key Pro HEHE 2 -> {zombiePrefab.name}");
                var pool = PoolManager.GetOrCreatePool<EnemyBase>($"{zombieSpawnInfo.PoolKey}", zombiePrefab, initialCapacity: 1);
                var zombie = pool.Get();
                zombie.transform.position = spawnPoint.position;
                zombie.transform.rotation = spawnPoint.rotation;
                zombie.gameObject.SetActive(true);
                zombie.Init();
                m_EnemyBases.Add(zombie);
                zombie.OnDead += HandleZombieDied;
                Debug.Log($"Key Pro HEHE 3 -> {zombiePrefab.name}");
            }
            yield return new WaitForSeconds(wave.SpawnInterval);
        }
        m_IsSpawning = false;
    }

    private void HandleZombieDied(EnemyBase zombie)
    {
        m_EnemyBases.Remove(zombie);
        if (m_EnemyBases.Count <= 0 && !m_IsSpawning)
            StartCoroutine(NextWave());
        zombie.OnDead -= HandleZombieDied;
    }

    private void HandlePlayerDead()
    {
        GameEventHandler.Invoke(PVEEventCode.OnLevelEnd, false);
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(2f);
        m_CurrentWaveIndex++;
        m_CurrentWave.value++;

        if (m_CurrentWaveIndex < m_CurrentLevel.ZombieWaves.Count)
        {
            yield return StartCoroutine(SpawnWave(m_CurrentLevel.ZombieWaves[m_CurrentWaveIndex]));
        }
        else
        {
            m_CurrentLevelPPref.value++;
            GameEventHandler.Invoke(PVEEventCode.OnLevelEnd, true);
        }
    }
}
