using Sirenix.Utilities;
using UnityEngine;

namespace Premium.GameManagement
{
    [GlobalConfig("Assets/Resources")]
    [WindowMenuItem("GlobalConfig", "GameConfig", "Assets/Resources")]
    public class GlobalGameConfigSO : GlobalConfig<GlobalGameConfigSO>
    {
        [SerializeField]
        private bool m_IsDevModeEnabled;
        [SerializeField]
        private int m_TargetFPS = 60;

        public bool isDevModeEnabled => m_IsDevModeEnabled;
        public int targetFPS => m_TargetFPS;
    }
}