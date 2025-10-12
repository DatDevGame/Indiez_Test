using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InfoGamepPlayUI : MonoBehaviour
{
    [SerializeField, BoxGroup("Reference")] private TMP_Text m_WaveText;
    [SerializeField, BoxGroup("Reference")] private CanvasGroupVisibility m_CanvasGroupVisibility;
    [SerializeField, BoxGroup("Data")] private IntVariable m_CurrentWave;
    [SerializeField, BoxGroup("Data")] private LevelManagerSO m_LevelManagerSO;


    private void Awake()
    {
        GameEventHandler.AddActionEvent(PVEEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVEEventCode.OnLevelEnd, OnLevelEnd);
        m_CurrentWave.onValueChanged += OnChangedWave;
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVEEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.RemoveActionEvent(PVEEventCode.OnLevelEnd, OnLevelEnd);
        m_CurrentWave.onValueChanged -= OnChangedWave;
    }

    private void OnLevelStart()
    {
        UpdateUI();
        m_CanvasGroupVisibility.Show();
    }

    private void OnLevelEnd(params object[] parameters)
    {
        m_CanvasGroupVisibility.Hide();
    }

    private void UpdateUI()
    {
        LevelSO levelSO = m_LevelManagerSO.currentItemInUse.Cast<LevelSO>();
        if (levelSO != null)
            m_WaveText.SetText($"Wave: {m_CurrentWave.value + 1}/{levelSO.ZombieWaves.Count}");

    }
    private void OnChangedWave(ValueDataChanged<int> data)
    {
        LevelSO levelSO = m_LevelManagerSO.currentItemInUse.Cast<LevelSO>();
        if (levelSO != null)
            m_WaveText.SetText($"Wave: {data.newValue + 1}/{levelSO.ZombieWaves.Count}");
    }
}
