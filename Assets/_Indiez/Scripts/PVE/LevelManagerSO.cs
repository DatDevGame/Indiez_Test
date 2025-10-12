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

        int index = m_CurrentLevelIndex.value;

        // Clamp index trước
        if (index < 0 || index >= initialValue.Count)
        {
            Debug.LogWarning($"[LevelManagerSO] Index {index} out of range! Picking random level instead.");
            return GetRandomLevelSO();
        }

        var levelSO = initialValue
            .OfType<LevelSO>()
            .ElementAtOrDefault(index);

        if (levelSO == null)
        {
            Debug.LogWarning($"[LevelManagerSO] No valid LevelSO found at index {index}! Picking random level instead.");
            return GetRandomLevelSO();
        }

        return levelSO;
    }

    private LevelSO GetRandomLevelSO()
    {
        var validLevels = initialValue
            .OfType<LevelSO>()
            .Where(l => l != null)
            .ToList();

        if (validLevels.Count == 0)
        {
            Debug.LogWarning("[LevelManagerSO] No valid LevelSO available for random selection!");
            return null;
        }

        int randomIndex = Random.Range(0, validLevels.Count);
        return validLevels[randomIndex];
    }

}
