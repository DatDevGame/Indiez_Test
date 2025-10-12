using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManagerSO", menuName = "ZombieWar/Level/LevelManagerSO")]
public class LevelManagerSO : ItemManagerSO
{
    [SerializeField] private PPrefIntVariable m_CurrentLevelIndex;

    public LevelSO GetCurrentLevelSO()
    {
        if (m_CurrentLevelIndex == null)
        {
            Debug.LogWarning("[LevelManagerSO] m_CurrentLevelIndex is null!");
            return null;
        }

        if (initialValue == null || !initialValue.Any())
        {
            Debug.LogWarning("[LevelManagerSO] initialValue list is empty!");
            return null;
        }

        int index = Mathf.Clamp(m_CurrentLevelIndex.value, 0, initialValue.Count - 1);

        var levelSO = initialValue
            .OfType<LevelSO>()
            .ElementAtOrDefault(index);

        if (levelSO == null)
            Debug.LogWarning($"[LevelManagerSO] No valid LevelSO found at index {index}!");

        return levelSO;
    }
}
